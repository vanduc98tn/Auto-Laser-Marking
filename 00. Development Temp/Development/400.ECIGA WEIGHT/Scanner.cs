using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Development
{
    class Scanner
    {
        private const int READ_TIMEOUT = 500;
        private static MyLogger logger = new MyLogger("ScannerComm");
        private SerialPort serialPort;
        private byte[] readBuf = new byte[1];
        private volatile bool isReading = false;
        private volatile List<byte> readingBuf;
        public delegate void RxDataHandler(byte rx);
        public event RxDataHandler DataReceived;
        private static bool enableReadingLog = false;
        private bool isConnect = false;
        public Scanner(string portName, int baudrate)
        {
            try
            {
                if (String.IsNullOrEmpty(portName))
                {
                    return;
                }
                this.serialPort = new SerialPort(portName, baudrate, Parity.None, 8, StopBits.One);
            }
            catch (Exception ex)
            {
                logger.Create(String.Format("ScannerComm error:" + ex.Message), LogLevel.Error);
            }

        }
        public Scanner(string portName, int baudrate, Parity parity = Parity.Even, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            try
            {
                if (String.IsNullOrEmpty(portName))
                {
                    return;
                }
                this.serialPort = new SerialPort(portName, baudrate, parity, dataBits, stopBits);
            }
            catch (Exception ex)
            {
                logger.Create(String.Format("ScannerComm error:" + ex.Message), LogLevel.Error);
            }

        }
        
        public string Open()
        {
            string rs = "";
            try
            {

                if (!this.serialPort.IsOpen)
                {
                    this.serialPort.Open();
                    //this.serialPort.BaseStream.BeginRead(this.readBuf, 0, 1,
                    //    new AsyncCallback(this.readCallback), this.serialPort);
                    rs = "\n\r Connect to Scanner Successfull \n\r";
                    isConnect = true;
                }
            }
            catch (Exception ex)
            {
                logger.Create(String.Format("ScannerComm error:" + ex.Message), LogLevel.Error);
                rs = "ScannerComm error:" + ex.Message;
                isConnect = false;
            }
            return rs;
        }
        private void readCallback(IAsyncResult iar)
        {
            try
            {
                var port = (SerialPort)iar.AsyncState;
                if (!port.IsOpen)
                {
                    logger.Create("readCallback: port is closed -> stop reading!", LogLevel.Information);
                    return;
                }
                int rxCnt = port.BaseStream.EndRead(iar);
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
                    if (this.DataReceived != null)
                    {
                        this.DataReceived(rx);
                    }
                }

                // Continue reading:            
                port.BaseStream.BeginRead(this.readBuf, 0, 1, new AsyncCallback(readCallback), port);
            }
            catch (Exception ex)
            {
                logger.Create("readCallback error:" + ex.Message, LogLevel.Error);
            }
        }
        public string Stop()
        {
            string rs = "";
            try
            {
                if (this.serialPort != null && this.serialPort.IsOpen)
                {
                    this.serialPort.Close();
                    rs = "\n\r Scanner Stoped ! \n\r";
                }
            }
            catch (Exception ex)
            {
                logger.Create(String.Format("Stop error:" + ex.Message), LogLevel.Error);
                rs = "Stop error:" + ex.Message;
            }
            return rs;
        }
        public String ReadQR()
        {
            String ret = "";
            var logger = new ScannerLogger();

            isReading = true;
            this.readingBuf = new List<byte>();
            var cmd = String.Format("ON");

            if (enableReadingLog)
            {
                logger.CreateTxLog(cmd);
            }

            this.serialPort.Write(cmd);

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
                    logger.CreateRxLog(ret);
                }
            }
            else
            {
                // Finish reading:
                this.serialPort.Write("OFF");
                if (enableReadingLog)
                {
                    isReading = true;
                    logger.CreateTxLog("OFF");
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
                        logger.CreateRxLog(ret);
                    }
                }
                else
                {
                    isReading = false;
                }

                // Log:
            }

            // Update counters:

            // Check error:
            if ((ret != null) && (ret.Contains("ERROR")))
            {
                ret = "";
            }

            return ret;
        }
        /// <summary>
        /// Lệnh scanner đọc code
        /// </summary>
        /// <returns>Chuỗi code đọc được</returns>
        public string Trigger()
        {
            string ret = "";

            try
            {
                this.serialPort.ReadTimeout = 1000;
                this.serialPort.Write("ON");
                Thread.Sleep(100);
                ret = this.serialPort.ReadLine();
                ret = ret.Trim();
            }
            catch (Exception ex)
            {
                logger.Create(String.Format("ReadQR error:" + ex.Message), LogLevel.Error);
                ret = "read Timeout";
            }
            return ret;
        }
        public bool IsOpen()
        {
            bool kq = false;
            try
            {
                if (serialPort.IsOpen)
                {
                    kq = true;
                }
                else { kq = false; }
            }
            catch (Exception ex)
            {
                logger.Create("kiem tra open port Err : " + ex.ToString(), LogLevel.Error);
            }
            return kq;
        }
    }
}
