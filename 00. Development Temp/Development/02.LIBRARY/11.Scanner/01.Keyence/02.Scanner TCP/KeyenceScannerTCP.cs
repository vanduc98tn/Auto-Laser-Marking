using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Development
{
    class KeyenceScannerTCP
    {
        public const String BANK_ID_PKG = "01";
        public const String BANK_ID_JIG = "02";
        public const String BANK_ID_PCB = "03";

        private const int CONNECT_TIMEOUT = 100;
        private const int READ_TIMEOUT = 300;

        private static MyLogger logger = new MyLogger("ScannerCommTcp");

        private static bool enableReadingLog = false;

        // private ScannerSettings settings;
        private Socket tcpClient;
        private bool IsStarted = false;
        private Thread threadMonitor;

        private byte[] readBuf = new byte[1];
        private volatile bool isReading = false;
        private volatile List<byte> readingBuf;

        public delegate void RxDataHandler(byte rx);

        public delegate void DisconnectHandler();

        public event RxDataHandler EvDataReceived;

        //public event DisconnectHandler EvDisconnect;
        private string ipAdress;
        private int portNo;

        /// 

        private const int RECONNECT_DELAY = 5000; // Wait 5 seconds before retrying
        private const int MAX_RECONNECT_ATTEMPTS = 5;


        private bool isAutoReconnecting = true;
        public Boolean IsConnected
        {
            get
            {
                try
                {
                    if (tcpClient != null)
                    {
                        var sk = tcpClient;
                        if ((!sk.Connected) || sk.Poll(100, SelectMode.SelectRead) && (sk.Available == 0))
                        {
                            return false;
                        }
                        return true;
                    }
                }
                catch { }
                return false;
            }
        }


        public int PortNo { get => portNo; set => portNo = value; }
        public string IpAdress { get => ipAdress; set => ipAdress = value; }

        public static void EnableReadingLog(bool enable)
        {
            enableReadingLog = true;
        }
        public KeyenceScannerTCP(TCPSetting TCP)
        {
            try
            {
                this.ipAdress = TCP.Ip;
                this.portNo = TCP.Port;
            }
            catch (Exception ex)
            {
                logger.Create(String.Format("ScannerCommTcp error:" + ex.Message), LogLevel.Error);
            }
        }
        public KeyenceScannerTCP(string _IpAdress, int _PortNo)
        {
            try
            {
                this.ipAdress = _IpAdress;
                this.portNo = _PortNo;
            }
            catch (Exception ex)
            {
                logger.Create(String.Format("ScannerCommTcp error:" + ex.Message), LogLevel.Error);
            }
        }
        public KeyenceScannerTCP()
        {
            try
            {
                this.ipAdress = "192.168.0.1";
                this.portNo = 1000;
            }
            catch (Exception ex)
            {
                logger.Create(String.Format("ScannerCommTcp error:" + ex.Message), LogLevel.Error);
            }
        }


        public bool Start()
        {
            try
            {
                var scannerIp = IPAddress.Parse(ipAdress);
                var scannerEp = new IPEndPoint(scannerIp, portNo);
                tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                logger.Create($"Start connecting to {scannerEp}", LogLevel.Information);

                IAsyncResult iar = tcpClient.BeginConnect(scannerEp, null, null);
                bool success = iar.AsyncWaitHandle.WaitOne(100, true);
                if (IsConnected)
                {
                    tcpClient.EndConnect(iar);
                    IsStarted = true;
                    logger.Create(" -> connected!", LogLevel.Information);

                    tcpClient.BeginReceive(readBuf, 0, 1, SocketFlags.None, new AsyncCallback(readCallback), tcpClient);

                    StartConnectionMonitor();

                }
                else
                {
                    tcpClient.Close();
                    logger.Create(" -> connect failed!", LogLevel.Warning);
                }
            }
            catch (Exception ex)
            {
                logger.Create($"Start error: {ex.Message}", LogLevel.Error);
            }
            return IsConnected;
        }
        private void StartConnectionMonitor()
        {
            threadMonitor = new Thread(() =>
            {
                while (IsStarted)
                {
                    try
                    {
                        Thread.Sleep(1000);
                        if (IsStarted && !IsConnected)
                        {
                            logger.Create("Connection lost. Attempting to reconnect...", LogLevel.Information);
                            Reconnect();
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Create($"MonitorConnection error: {ex.Message}", LogLevel.Error);
                    }
                }
            })
            {
                IsBackground = true
            };
            threadMonitor.Start();
        }
        private void Reconnect()
        {
            int attempts = 0;

            while (isAutoReconnecting && attempts < MAX_RECONNECT_ATTEMPTS && !IsConnected)
            {
                try
                {
                    Stop(); // Close current connection before reconnecting
                    Thread.Sleep(RECONNECT_DELAY); // Wait before retrying
                    logger.Create($"Reconnection attempt {attempts + 1}", LogLevel.Information);

                    Start(); // Retry the connection
                    attempts++;
                }
                catch (Exception ex)
                {
                    logger.Create($"Reconnect attempt {attempts + 1} failed: {ex.Message}", LogLevel.Information);
                }
            }

            if (IsConnected)
            {
                logger.Create("Reconnected successfully!", LogLevel.Information);
            }
            else
            {
                logger.Create("Failed to reconnect after maximum attempts.", LogLevel.Error);
            }
        }
        public void Stop()
        {
            try
            {
                IsStarted = false;
                if (tcpClient != null)
                {
                    if (threadMonitor != null)
                    {
                        threadMonitor.Abort();
                    }
                    tcpClient.Shutdown(SocketShutdown.Both);
                    tcpClient.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Create(String.Format("Stop error:" + ex.Message), LogLevel.Error);
            }
        }
        public String ReadQR() // String bankId
        {
            if (!IsConnected)
            {
                logger.Create(" -> disconnect -> discard ReadQR!", LogLevel.Error);
                return "";
            }
            String ret = "";
            //var qrLogger = new ScannerLogger();

            isReading = true;
            this.readingBuf = new List<byte>();
            //var cmd = String.Format("LON{0}\r", bankId);
            var cmd = String.Format("LON\r");

            if (enableReadingLog)
            {
                //qrLogger.CreateTxLog(cmd);
            }

            // Send command:
            tcpClient.Send(ASCIIEncoding.ASCII.GetBytes(cmd));

            // Wait for result:
            for (int i = 0; i < READ_TIMEOUT / 10; i++)
            {
                if (!isReading)
                {
                    break;
                }
                Thread.Sleep(10);
            }
            if (!isReading)
            {
                ret = ASCIIEncoding.ASCII.GetString(this.readingBuf.ToArray());

                // Log:
                if (enableReadingLog)
                {
                    //qrLogger.CreateRxLog(ret);
                }
            }
            else
            {
                // Finish reading:
                tcpClient.Send(ASCIIEncoding.ASCII.GetBytes("LOFF\r"));
                if (enableReadingLog)
                {
                    isReading = true;
                    //qrLogger.CreateTxLog("LOFF\r");
                    for (int i = 0; i < READ_TIMEOUT / 10; i++)
                    {
                        if (!isReading)
                        {
                            break;
                        }
                        Thread.Sleep(10);
                    }
                    if (!isReading)
                    {
                        ret = ASCIIEncoding.ASCII.GetString(this.readingBuf.ToArray());
                        //qrLogger.CreateRxLog(ret);
                    }
                }
                else
                {
                    isReading = false;
                }


            }

            // Update counters:
            //ScannerManager.UpdateCounters();

            // Check error:
            if ((ret != null) && (ret.Contains("ERROR")))
            {
                ret = "";
            }

            return ret;
        }
        public void Focusing()
        {
            if (!IsConnected)
            {
                logger.Create(" -> disconnect -> discard Focusing!", LogLevel.Warning);
            }
            var cmd = String.Format("FTUNE\r");
            tcpClient.Send(ASCIIEncoding.ASCII.GetBytes(cmd));
        }
        public void Tuning(String bankId)
        {
            if (!IsConnected)
            {
                logger.Create(" -> disconnect -> discard Tuning!", LogLevel.Warning);
            }
            var cmd = String.Format("TUNE{0}\r", bankId);
            tcpClient.Send(ASCIIEncoding.ASCII.GetBytes(cmd));
        }
        public void FinishTuning()
        {
            if (!IsConnected)
            {
                logger.Create(" -> disconnect -> discard FinishTuning!", LogLevel.Warning);
            }
            var cmd = String.Format("TQUIT\r");
            tcpClient.Send(ASCIIEncoding.ASCII.GetBytes(cmd));
        }
        private void readCallback(IAsyncResult iar)
        {
            try
            {
                var sk = (Socket)iar.AsyncState;
                if (!sk.Connected)
                {
                    logger.Create("readCallback: socket is disconnected -> stop reading!", LogLevel.Information);
                    return;
                }
                int rxCnt = sk.EndReceive(iar);
                if (rxCnt == 1)
                {
                    byte rx = this.readBuf[0];

                    // Update reading buffer:
                    if (isReading)
                    {
                        if (rx == 0x0d)
                        {
                            isReading = false;
                        }
                        else
                        {
                            this.readingBuf.Add(rx);
                        }
                    }

                    // Update to UI.log:
                    if (this.EvDataReceived != null)
                    {
                        this.EvDataReceived(rx);
                    }
                }

                // Continue reading:
                sk.BeginReceive(readBuf, 0, 1, SocketFlags.None, new AsyncCallback(readCallback), sk);
            }
            catch (Exception ex)
            {
                logger.Create("readCallback error:" + ex.Message, LogLevel.Error);
            }
        }
    }
}
