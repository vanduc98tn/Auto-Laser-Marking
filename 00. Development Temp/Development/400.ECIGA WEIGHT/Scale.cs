using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Development
{
    class Scales
    {
        private static MyLogger logger = new MyLogger("Scale");

        private SerialPort serialPort;
        private byte[] readBuf = new byte[1];
        private volatile bool isReading = false;
        private volatile List<byte> readingBuf = new List<byte>();
        public delegate void RxDataHandler(byte rx);
        public event RxDataHandler DataReceived;
        public Scales(string portName, int baudrate)
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
                logger.Create(String.Format("Scale error:" + ex.Message),LogLevel.Error);
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
                    rs = "\n\r Connect to Scale Successfull \n\r";
                }
            }
            catch (Exception ex)
            {
                logger.Create(String.Format("Scale error:" + ex.Message), LogLevel.Error);
                rs = "Scale error:" + ex.Message;
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
                    logger.Create("readCallback: port is closed -> stop reading!",LogLevel.Information);
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
                logger.Create("readCallback error:" + ex.Message,LogLevel.Error);
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
                    rs = "Scale Stopped !";
                }
            }
            catch (Exception ex)
            {
                logger.Create(String.Format("Stop error:" + ex.Message), LogLevel.Error);
                rs = "Stop error:" + ex.Message;
            }
            return rs;
        }
        public string Command(string cmd)
        {
            string ret = "";
            try
            {
                this.serialPort.Close();
                this.serialPort.Open();
                this.serialPort.Write(cmd);
                Thread.Sleep(1000);
                byte[] data = new byte[500];
                this.serialPort.Read(data, 0, data.Length);
                byte[] dataScale = new byte[7];
                for (int i = 0; i < 7; i++)
                {
                    dataScale[i] = data[13 + i];
                }
                ret = ASCIIEncoding.ASCII.GetString(dataScale);
            }
            catch (Exception ex)
            {
                logger.Create(String.Format("Read Scale error:" + ex.Message), LogLevel.Error);
            }
            return ret;


        }
        /// <summary>
        /// Đọc giá trị cân
        /// </summary>
        /// <returns>giá trị cân</returns>
        public string Read()
        {
            /*
             * Đọc lặp dòng dữ liệu trả về đến khi dòng chứa các ký tự đầu dòng là "NET" hoặc "G"
             * Data gồm 7 ký tự - tính từ ký tự thứ 7.                        
                                        Data digit
              specification              1 2 3 4 5     6     7 8 9 10 11 12 13       14 15 16      17 18
                          
              st row:Net weight-Data     title        space          data               unit         CR
                            
              nd row: Unit weight-data   title        space          data               unit         CR
                            
              rd row: Quantity-data      title        space          data           CR(14 15)
                            
              th row: Tare weight-data   title        space          data              weight        CR
             */
            string rs = "";
            bool readexisted = false;
            try
            {
                while (!readexisted)
                {
                    this.serialPort.ReadTimeout = 3000;
                    serialPort.DiscardInBuffer();
                    Thread.Sleep(100);
                    string result = this.serialPort.ReadLine();
                    if (result.Length > 16)
                    {
                        if (result.Substring(0, 3) == "NET" || result.Substring(0, 1) == "G")
                        {
                            double actual = Convert.ToDouble(result.Substring(7, 7));
                            rs = actual.ToString();
                            readexisted = true;
                        }
                        else
                        {
                            rs = "";
                        }

                    }
                    else
                    {
                        rs = "";
                    }

                }
            }
            catch (Exception)
            {
                rs = "Read Timeout";
            }
            return rs;
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
