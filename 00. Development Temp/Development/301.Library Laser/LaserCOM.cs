using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Development
{
   
    class LaserCOM
    {
        private MyLogger logger = new MyLogger("COMLaser");
        private SerialPort _serialPort;
        private object PLCLock = new object();
        public bool isConnect = false;

        private const int READ_TIMEOUT = 500;
        private static bool enableReadingLog = false;
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
        public bool SendBytes(byte[] data)
        {
            try
            {
                lock (PLCLock)
                {
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Write(data, 0, data.Length);
                        logger.Create01($"Sent {data.Length} bytes: {BitConverter.ToString(data)}", LogLevel.Information);
                        return true;
                    }
                    else
                    {
                        logger.Create01("Error: COM port is not open", LogLevel.Error);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Create01($"Error sending bytes: {ex.Message}", LogLevel.Error);
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
                            string hexData = BitConverter.ToString(data.ToArray()).Replace("-", " ");
                            logger.CreateLaser($"Received data: {hexData}", LogLevel.Information);
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

                           
                            string hexData = BitConverter.ToString(buffer).Replace("-", " ");
                            logger.CreateLaser($"Received data: {hexData}", LogLevel.Information);

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
        public byte[] SendDataLaser(byte[] data)
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

                string hexData = BitConverter.ToString(rec).Replace("-", " ");
                logger.CreateLaser($"Received data: {hexData}", LogLevel.Information);
            }
            else
            {
                rec = null;
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
                    _serialPort.Open();
                    isConnect = true;
                    this._serialPort.BaseStream.BeginRead(this.readBuf, 0, 1,
                        new AsyncCallback(this.readCallback), this._serialPort);

                }

            }
            catch (Exception ex)
            {
                logger.Create01($"Error Open COM {ex}", LogLevel.Error);
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
                logger.Create01($"Error Close COM {ex}", LogLevel.Error);
            }
        }

        public void Send01OutAnd02OutTester()
        {
            // STX = 0x02 , CMD = 0x32 , CMD = 0x032 , O = 0x4F , K = 0x4B , ETX = 0x03
            byte[] Data01Out02OutTester = new byte[] { 0x02, 0x32, 0x32, 0x4F, 0X4B, 0x03, 0x0D, 0X0A };
            SendBytes(Data01Out02OutTester);
        }
        public void Send01InAnd02OutTester()
        {
            // STX = 0x02 , CMD = 0x32 , CMD = 0x033 , O = 0x4F , K = 0x4B , ETX = 0x03
            byte[] Data01Out02OutTester = new byte[] { 0x02, 0x32, 0x33, 0x4F, 0X4B, 0x03, 0x0D, 0X0A };
            SendBytes(Data01Out02OutTester);
        }
        public void Send01OutAnd02InTester()
        {
            // STX = 0x02 , CMD = 0x32 , CMD = 0x034 , O = 0x4F , K = 0x4B , ETX = 0x03
            byte[] Data01Out02OutTester = new byte[] { 0x02, 0x32, 0x34, 0x4F, 0X4B, 0x03, 0x0D, 0X0A };
            SendBytes(Data01Out02OutTester);
        }
        public void SendLotin(string Lotin)
        {
            
            if (Lotin.Length < 21)
            {
                Lotin = Lotin.PadRight(21, '-');
            }
            else if (Lotin.Length > 21)
            {
                Lotin = Lotin.Substring(0, 21); 
            }

            byte[] Data01 = new byte[] { 0x02 , 0x39 ,0x39 };

            byte[] LotinBytes = System.Text.Encoding.ASCII.GetBytes(Lotin);

            byte[] DataEnd = new byte[] { 0x03, 0x0D, 0X0A };

            byte[] DataSendLotin = new byte[Data01.Length + LotinBytes.Length + DataEnd.Length];
            Array.Copy(Data01, 0, DataSendLotin, 0, Data01.Length);
            Array.Copy(LotinBytes, 0, DataSendLotin, Data01.Length, LotinBytes.Length);
            Array.Copy(DataEnd, 0, DataSendLotin, Data01.Length + LotinBytes.Length, DataEnd.Length);


            SendBytes(DataSendLotin);
        }

        public void SendCheckAgain()
        {
            // STX = 0x02 , CMD = 0x33 , CMD = 0x033 , O = 0x4F , K = 0x4B , ETX = 0x03
            byte[] DataCheckAgain = new byte[] { 0x02, 0x33, 0x33, 0x4F, 0X4B, 0x03, 0x0D, 0X0A };
            SendBytes(DataCheckAgain);
        }
        public void  SendResult()
        {
            // STX = 0x02 , CMD = 0x34 , CMD = 0x034 , O = 0x4F , K = 0x4B , ETX = 0x03
            byte[] DataResult = new byte[] { 0x02, 0x34, 0x34, 0x4F, 0X4B, 0x03, 0x0D, 0X0A };
            SendBytes(DataResult);
        }
        public void SendIRSS(bool CH1_EN, bool CH2_EN, bool CH3_EN, bool CH4_EN, bool CH5_EN, bool CH6_EN, bool CH7_EN, bool CH8_EN, bool CH9_EN, bool CH10_EN, bool CH11_EN, bool CH12_EN)

        {
            byte[] Data01 = new byte[] { 0x02, 0x31, 0x31 };
            byte[] DataCH = new byte[12];
            byte[] DataEnd = new byte[] { 0x03, 0x0D, 0X0A };

          
            bool[] channels = { CH1_EN, CH2_EN, CH3_EN, CH4_EN, CH5_EN, CH6_EN, CH7_EN, CH8_EN, CH9_EN, CH10_EN, CH11_EN, CH12_EN };

            for (int i = 0; i < DataCH.Length; i++)
            {
                DataCH[i] = (byte)(channels[i] ? 0x31 : 0x30);
            }

            
            byte[] DataSend = new byte[Data01.Length + DataCH.Length + DataEnd.Length];
            Array.Copy(Data01, 0, DataSend, 0, Data01.Length);
            Array.Copy(DataCH, 0, DataSend, Data01.Length, DataCH.Length);
            Array.Copy(DataEnd, 0, DataSend, Data01.Length + DataCH.Length, DataEnd.Length);

            SendBytes(DataSend);
        }
    }
}
