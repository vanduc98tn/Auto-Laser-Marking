using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Development
{
   
    class LaserCOM
    {
        private MyLogger logger = new MyLogger("COMLaser");
        private SerialPort _serialPort;
        private object PLCLock = new object();
        public bool isConnect = false;

        private const int READ_TIMEOUT = 1000;
        private byte[] readBuf = new byte[1];
        private volatile bool isReading = false;
        private volatile List<byte> readingBuf;

        public delegate void RxDataHandler(byte rx);

        //public event EventHandler<string> DataReceived;


        public delegate void DataReceivedHandler(object sender, List<byte> data);
        public event DataReceivedHandler DataReceived;

        public LaserCOM( COMSetting comSetting) 
        {
            
            _serialPort = new SerialPort
            {
                PortName = comSetting.portName,
                BaudRate = comSetting.baudrate,
                Parity = comSetting.parity,
                DataBits = comSetting.dataBits,
                StopBits = comSetting.stopBits,
                ReadTimeout = 50,
                WriteTimeout = 50
            };
            //_serialPort.DataReceived += _serialPort_DataReceived;
        }
        public LaserCOM()
        {

            _serialPort = new SerialPort
            {
                PortName = "COM7",// fix tam com3 nhé
                BaudRate = 9600,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                ReadTimeout = 50,
                WriteTimeout = 50
            };
            //_serialPort.DataReceived += _serialPort_DataReceived;
        }
        public bool SendBytes1(byte[] data)
        {
            try
            {
                lock (PLCLock)
                {
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Write(data, 0, data.Length);
                        logger.CreateLaser($"Sent {data.Length} bytes: {BitConverter.ToString(data)}", LogLevel.Information);
                        return true;
                    }
                    else
                    {
                        logger.CreateLaser("Error: COM port is not open", LogLevel.Error);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.CreateLaser($"Error sending bytes: {ex.Message}", LogLevel.Error);
                return false;
            }
        }
        public bool SendBytes(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                logger.CreateLaser("Error: data = 0 or is null", LogLevel.Error);
                return false;
            }
            try
            {
                lock (PLCLock)
                {
                    if (!_serialPort.IsOpen)
                    {
                        logger.CreateLaser("Error: COM port is not open", LogLevel.Error);
                        return false;
                    }

                    const int MAX_CHUNK_SIZE = 127;
                    int offset = 0;
                    _serialPort.WriteTimeout = 5000;

                    while (offset < data.Length)
                    {
                        int chunkSize = Math.Min(MAX_CHUNK_SIZE, data.Length - offset);
                        _serialPort.Write(data, offset, chunkSize);
                        logger.CreateLaser($"Sent {chunkSize} bytes: {BitConverter.ToString(data, offset, chunkSize)}", LogLevel.Information);
                        while (_serialPort.BytesToWrite > 0)
                        {
                            Thread.Sleep(1);
                        }
                        offset += chunkSize;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.CreateLaser($"Error sending bytes: {ex.Message}", LogLevel.Error);
                return false;
            }
        }

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                lock (PLCLock)
                {
                    if (_serialPort.IsOpen)
                    {
                        List<byte> data = new List<byte>();


                        while (_serialPort.BytesToRead > 0)
                        {
                            int count = _serialPort.BytesToRead;
                            byte[] buffer = new byte[count];
                            _serialPort.Read(buffer, 0, count);
                            data.AddRange(buffer);
                            System.Threading.Thread.Sleep(2);
                        }

                        if (data.Count > 0)
                        {
                            string hexData = BitConverter.ToString(data.ToArray()).Replace(" ", "-");
                            logger.CreateLaser($"Received {hexData.Length} byte: {hexData}", LogLevel.Information);
                            DataReceived?.Invoke(this, data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.CreateLaser($"Error receiving data: {ex.Message}", LogLevel.Error);
            }
        }
        private void _serialPort_DataReceived1(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                lock (PLCLock)
                {
                    if (_serialPort.IsOpen)
                    {
                      
                        int bytesToRead = _serialPort.BytesToRead;
                        if (bytesToRead > 0)
                        {
                           
                            byte[] buffer = new byte[bytesToRead];
                          
                            _serialPort.Read(buffer, 0, bytesToRead);
                            List<byte> data = new List<byte>(buffer);

                           
                            string hexData = BitConverter.ToString(buffer).Replace(" ", "-");
                            logger.CreateLaser($"Received {hexData.Length} byte: {hexData}", LogLevel.Information);

                            // Gọi sự kiện với List<byte>
                            DataReceived?.Invoke(this, data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.CreateLaser($"Error receiving data: {ex.Message}", LogLevel.Error);
            }
        }
        public byte[] SendWaitResponse(byte[] data)
        {
            byte[] rec;
            isReading = true;
            this.readingBuf = new List<byte>();

            SendBytes(data);

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
                rec = this.readingBuf.ToArray();
                string hexData = BitConverter.ToString(rec).Replace(" ", "-");
                logger.CreateLaser($"Received {rec.Length} byte: {hexData}", LogLevel.Information);
            }
            else
            {
                rec = null;
                logger.CreateLaser($"Received 0 byte: <null>", LogLevel.Information);
            }
            

            return rec;
        }
        private void readCallback(IAsyncResult iar)
        {
            try
            {
                var port = (SerialPort)iar.AsyncState;
                if (!port.IsOpen)
                {
                    logger.Create("readCallback: port is closed -> stop reading!", LogLevel.Warning);
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
                    //if (this.DataReceived != null)
                    //{
                    //    this.DataReceived(rx);
                    //}
                }

                // Continue reading:            
                port.BaseStream.BeginRead(this.readBuf, 0, 1, new AsyncCallback(readCallback), port);
            }
            catch (Exception ex)
            {
                logger.Create("readCallback error:" + ex.Message, LogLevel.Error);
            }
        }

        public bool isOpen()
        {
            return isConnect;
        }
        public void Open()
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.WriteBufferSize = 8192;
                    _serialPort.Open();
                    isConnect = true;
                    this._serialPort.BaseStream.BeginRead(this.readBuf, 0, 1,
                        new AsyncCallback(this.readCallback), this._serialPort);

                }

            }
            catch (Exception ex)
            {
                logger.CreateLaser($"Error Open COM {ex}", LogLevel.Error);
                isConnect = false;
            }
        }
        public void Close()
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                    isConnect = false;

                }
            }
            catch (Exception ex)
            {
                logger.CreateLaser($"Error Close COM {ex}", LogLevel.Error);
            }
        }

        public string SendSwitchPrg(int prg)
        {
            string repStr = $"GA,0"; //Response send GA ok
            string cmd = $"GA,{prg}";
            
            cmd += "\r";

            byte[] bycmd = Encoding.ASCII.GetBytes(cmd);
            byte[] rec = SendWaitResponse(bycmd);

            string recStr = "";
            if (rec != null) recStr = Encoding.ASCII.GetString(rec);

            if (recStr == repStr && recStr != string.Empty)
            //if (rec != null)
            {
                return recStr;
            }
            else return "NG";
        }
        public string SendBlockOn(int prg, params int[] block)
        {
            string repStr = $"D6,0"; //Response send D6 ok
            string cmd = $"D6,{prg},1";

            if (block != null && block.Length > 0)
            {
                cmd += "," + string.Join(",", block);
            }
            cmd += "\r";

            byte[] bycmd = Encoding.ASCII.GetBytes(cmd);
            byte[] rec = SendWaitResponse(bycmd);

            string recStr = "";
            if (rec != null) recStr = Encoding.ASCII.GetString(rec);

            if (recStr == repStr && recStr != string.Empty)
            //if (rec != null)
            {
                return recStr;
            }
            else return "NG";
        }

    }
}
