using ITM_Semiconductor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Development
{
    class FX_RS232_MODBUS
    {

        #region Properties
        private SerialPort Comport = new SerialPort();
        private object PLCLock = new object();
        #endregion

        private object ComportLock = new object();
        private volatile byte[] rxBuf = new byte[1];
        private volatile bool isReading = false;
        private volatile List<byte> readingBuf;
        private byte plcMbAddr;
        private static MyLogger logger = new MyLogger("PlcComm");


        #region Modbus Constants
        private const Byte FC_READ_COIL_STATUS = 0x01;
        private const Byte FC_READ_HOLDING_REGISTERS = 0x03;
        private const Byte FC_WRITE_SINGLE_COIL = 0x05;
        private const Byte FC_WRITE_SINGLE_REGISTER = 0x06;
        private const Byte FC_WRITE_MULTIPLE_REGISTERS = 0x10;
        private const Int32 MB_RESPONSE_TIMEOUT = 200;
        private const Int32 READ_RETRY_TIMEOUT = 100;
        private const Int32 WRITE_RETRY_TIMEOUT = 200;
        #endregion

        public FX_RS232_MODBUS(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One, byte plcAddr = 1)
        {
            this.Comport = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            this.Comport.ReadTimeout = 1000;
            this.Comport.WriteTimeout = 1000;
            this.Comport.ReadBufferSize = 1024;
            this.Comport.WriteBufferSize = 1024;
            this.plcMbAddr = plcAddr;
        }

        public bool Open(string port)
        {
            try
            {
                this.Comport.PortName = port;
                this.Comport.BaudRate = 9600;
                this.Comport.Parity = Parity.None;
                this.Comport.DataBits = 8;
                this.Comport.StopBits = StopBits.One;
                this.Comport.ReadTimeout = 1000;
                this.Comport.WriteTimeout = 1000;
                this.Comport.ReadBufferSize = 1024;
                this.Comport.WriteBufferSize = 1024;
                if (this.Comport.IsOpen)
                    this.Comport.Close();
                this.Comport.Open();
                this.Comport.BaseStream.BeginRead(this.rxBuf, 0, 1, new AsyncCallback(this.readCallback), this.Comport);
                return this.Comport.IsOpen;
            }
            catch (Exception ex)
            {
                logger.Create("Open error: " + ex.Message, LogLevel.Error);
                return false;
            }
        }

        public void Close()
        {
            try
            {
                if (this.Comport.IsOpen)
                    this.Comport.Close();
            }
            catch (Exception ex)
            {
                logger.Create("Close error: " + ex.Message, LogLevel.Error);
            }
        }

        public bool IsOpen()
        {
            return this.Comport.IsOpen;
        }

        public bool WriteBit(DeviceCode device, int address, bool value)
        {
            lock (PLCLock)
            {
                if (this.Comport == null || !this.Comport.IsOpen)
                    throw new Exception("Please check the status of the serial port!");

                try
                {
                    switch (device)
                    {
                        case DeviceCode.M:
                            for (int i = 0; i < 2; i++)
                            {
                                if (WriteSingleCoil((UInt16)address, (byte)(value ? 1 : 0)))
                                    return true;
                                Thread.Sleep(WRITE_RETRY_TIMEOUT);
                            }
                            break;
                        default:
                            throw new Exception("Unsupported device type");
                    }
                }
                catch (Exception ex)
                {
                    logger.Create($"WriteBit error: {ex.Message}", LogLevel.Error);
                  
                }
                return false;
            }
        }

        public bool ReadBit(DeviceCode device, int address, out bool value)
        {
            value = false;
            lock (PLCLock)
            {
                if (this.Comport == null || !this.Comport.IsOpen)
                    return false;

                try
                {
                    switch (device)
                    {
                        case DeviceCode.M:
                            for (int i = 0; i < 2; i++)
                            {
                                byte[] bits = ReadCoilStatus((UInt16)address, 1);
                                if (bits != null && bits.Length == 1)
                                {
                                    value = bits[0] == 1;
                                    return true;
                                }
                                Thread.Sleep(READ_RETRY_TIMEOUT);
                            }
                            break;
                        default:
                            throw new Exception("Unsupported device type");
                    }
                }
                catch (Exception ex)
                {
                    logger.Create($"ReadBit error: {ex.Message}", LogLevel.Error);
                    
                }
                return false;
            }
        }

        public bool WriteWord(DeviceCode device, int address, int value)
        {
            lock (PLCLock)
            {
                if (this.Comport == null || !this.Comport.IsOpen)
                    throw new Exception("Please check the status of the serial port!");

                try
                {
                    switch (device)
                    {
                        case DeviceCode.D:
                            for (int i = 0; i < 2; i++)
                            {
                                if (WriteSingleRegister((UInt16)address, (UInt16)value))
                                    return true;
                                Thread.Sleep(WRITE_RETRY_TIMEOUT);
                            }
                            break;
                        default:
                            throw new Exception("Unsupported device type");
                    }
                }
                catch (Exception ex)
                {
                    logger.Create($"WriteWord error: {ex.Message}", LogLevel.Error);
                }
                return false;
            }
        }

        public bool ReadWord(DeviceCode device, int address, out short value)
        {
            value = 0;
            lock (PLCLock)
            {
                if (this.Comport == null || !this.Comport.IsOpen)
                    return false;

                try
                {
                    switch (device)
                    {
                        case DeviceCode.D:
                            for (int i = 0; i < 2; i++)
                            {
                                var regs = ReadHoldingRegisters((UInt16)address, 1);
                                if (regs != null && regs.Length == 1)
                                {
                                    value = (short)regs[0];
                                    return true;
                                }
                                Thread.Sleep(READ_RETRY_TIMEOUT);
                            }
                            break;
                        default:
                            throw new Exception("Unsupported device type");
                    }
                }
                catch (Exception ex)
                {
                    logger.Create($"ReadWord error: {ex.Message}", LogLevel.Error);
                }
                return false;
            }
        }

        public bool WriteDWord(DeviceCode device, int address, int value)
        {
            lock (PLCLock)
            {
                if (this.Comport == null || !this.Comport.IsOpen)
                    throw new Exception("Please check the status of the serial port!");

                try
                {
                    switch (device)
                    {
                        case DeviceCode.D:
                            UInt16[] regs = new UInt16[]
                            {
                            (UInt16)(value & 0xffff),
                            (UInt16)(value >> 16)
                            };
                            for (int i = 0; i < 2; i++)
                            {
                                if (WriteMultipleRegisters((UInt16)address, regs))
                                    return true;
                                Thread.Sleep(WRITE_RETRY_TIMEOUT);
                            }
                            break;
                        default:
                            throw new Exception("Unsupported device type");
                    }
                }
                catch (Exception ex)
                {
                    logger.Create($"WriteDWord error: {ex.Message}", LogLevel.Error);
                }
                return false;
            }
        }

        public bool ReadDWord(DeviceCode device, int address, out int value)
        {
            value = 0;
            lock (PLCLock)
            {
                if (this.Comport == null || !this.Comport.IsOpen)
                    throw new Exception("Please check the status of the serial port!");

                try
                {
                    switch (device)
                    {
                        case DeviceCode.D:
                            for (int i = 0; i < 2; i++)
                            {
                                var regs = ReadHoldingRegisters((UInt16)address, 2);
                                if (regs != null && regs.Length == 2)
                                {
                                    value = (regs[1] << 16) | regs[0];
                                    return true;
                                }
                                Thread.Sleep(READ_RETRY_TIMEOUT);
                            }
                            break;
                        default:
                            throw new Exception("Unsupported device type");
                    }
                }
                catch (Exception ex)
                {
                    logger.Create($"ReadDWord error: {ex.Message}", LogLevel.Error);
                }
                return false;
            }
        }

        public bool ReadMultiWord(DeviceCode device, int address, int number, out List<short> values)
        {
            values = new List<short>();
            lock (PLCLock)
            {
                if (this.Comport == null || !this.Comport.IsOpen)
                    return false;

                try
                {
                    switch (device)
                    {
                        case DeviceCode.D:
                            for (int i = 0; i < number; i++)
                            {
                                for (int retry = 0; retry < 2; retry++)
                                {
                                    var regs = ReadHoldingRegisters((UInt16)(address + i), 1);
                                    if (regs != null && regs.Length == 1)
                                    {
                                        values.Add((short)regs[0]);
                                        break;
                                    }
                                    Thread.Sleep(READ_RETRY_TIMEOUT);
                                }
                            }
                            return values.Count == number;
                        default:
                            throw new Exception("Unsupported device type");
                    }
                }
                catch (Exception ex)
                {
                    logger.Create($"ReadMultiWord error: {ex.Message}", LogLevel.Error);
                }
                return false;
            }
        }

        public bool ReadMultiDWord(DeviceCode device, int address, int number, out List<int> values)
        {
            values = new List<int>();
            lock (PLCLock)
            {
                if (this.Comport == null || !this.Comport.IsOpen)
                    throw new Exception("Please check the status of the serial port!");

                try
                {
                    switch (device)
                    {
                        case DeviceCode.D:
                            for (int i = 0; i < number; i++)
                            {
                                for (int retry = 0; retry < 2; retry++)
                                {
                                    var regs = ReadHoldingRegisters((UInt16)(address + i * 2), 2);
                                    if (regs != null && regs.Length == 2)
                                    {
                                        values.Add((regs[1] << 16) | regs[0]);
                                        break;
                                    }
                                    Thread.Sleep(READ_RETRY_TIMEOUT);
                                }
                            }
                            return values.Count == number;
                        default:
                            throw new Exception("Unsupported device type");
                    }
                }
                catch (Exception ex)
                {
                    logger.Create($"ReadMultiDWord error: {ex.Message}",LogLevel.Error);
                }
                return false;
            }
        }

        private void readCallback(IAsyncResult iar)
        {
            try
            {
                var port = (SerialPort)iar.AsyncState;
                if (!port.IsOpen)
                {
                    logger.Create("readCallback: port is closed -> stop reading!",LogLevel.Warning);
                    return;
                }
                int rxCnt = port.BaseStream.EndRead(iar);
                if (rxCnt == 1)
                {
                    byte rx = this.rxBuf[0];
                    if (isReading)
                    {
                        this.readingBuf.Add(rx);
                    }
                }
                port.BaseStream.BeginRead(this.rxBuf, 0, 1, new AsyncCallback(readCallback), port);
            }
            catch (Exception ex)
            {
                logger.Create("readCallback error: " + ex.Message, LogLevel.Error);
            }
        }

        private Boolean waitForResponse(Int32 rxExpectedCnt, Int32 tout)
        {
            while (tout > 0)
            {
                if (readingBuf.Count >= rxExpectedCnt)
                {
                    isReading = false;
                    return true;
                }
                Thread.Sleep(10);
                tout -= 10;
            }
            isReading = false;
            logger.Create("waitForResponse failed: readingBuf.Cnt=" + readingBuf.Count.ToString(), LogLevel.Error);
            return false;
        }

        private static UInt16 CRC16(byte[] buf, Int32 len)
        {
            UInt16 crc16 = 0xFFFF;
            Int32 i, j, tmp8;

            for (i = 0; i < len; ++i)
            {
                crc16 ^= (byte)buf[i];
                for (j = 8; j > 0; --j)
                {
                    tmp8 = crc16 & 0x0001;
                    crc16 >>= 1;
                    if (tmp8 == 1)
                    {
                        crc16 ^= 0xA001;
                    }
                }
            }
            return crc16;
        }

        private static Boolean VerifyChecksum(byte[] rxBuf)
        {
            if (rxBuf == null || rxBuf.Length <= 2)
                return false;
            var rxLen = rxBuf.Length;
            UInt16 crcCal = CRC16(rxBuf, rxLen - 2);
            UInt16 crcRx = (UInt16)(rxBuf[rxLen - 2] + (rxBuf[rxLen - 1] << 8));
            if (crcCal != crcRx)
            {
                logger.Create(String.Format("Checksum error:RX={0:X4}, CAL={1:X4}", crcRx, crcCal),LogLevel.Error);
                return false;
            }
            return true;
        }

        private Byte[] ReadCoilStatus(UInt16 coilIndex, UInt16 coinCnt)
        {
            byte[] txBuf = new byte[32];
            Int32 idx = 0;
            Byte functionCode = FC_READ_COIL_STATUS;

            txBuf[idx++] = plcMbAddr;
            txBuf[idx++] = functionCode;
            txBuf[idx++] = (Byte)(coilIndex >> 8);
            txBuf[idx++] = (Byte)(coilIndex & 0xff);
            txBuf[idx++] = (Byte)(coinCnt >> 8);
            txBuf[idx++] = (Byte)(coinCnt & 0xff);
            UInt16 crc = CRC16(txBuf, idx);
            txBuf[idx++] = (Byte)(crc & 0xff);
            txBuf[idx++] = (Byte)(crc >> 8);

            lock (ComportLock)
            {
                isReading = true;
                readingBuf = new List<byte>();
                this.Comport.Write(txBuf, 0, idx);

                Int32 rxByteCnt = (coinCnt + 7) / 8;
                Int32 rxExpectedCnt = 3 + rxByteCnt + 2;
                if (!waitForResponse(rxExpectedCnt, MB_RESPONSE_TIMEOUT))
                {
                    logger.Create(String.Format(" -> ReadCoilStatus {0}/cnt={1}: no response!", coilIndex, coinCnt), LogLevel.Warning);
                    return null;
                }
                byte[] rxBuf = new byte[rxExpectedCnt];
                Array.Copy(readingBuf.ToArray(), rxBuf, rxExpectedCnt);

                if ((rxBuf[0] != plcMbAddr) || (rxBuf[1] != functionCode))
                {
                    logger.Create(String.Format(" -> ReadCoilStatus {0}/cnt={1}: invalid DeviceAddress or FunctionCode!", coilIndex, coinCnt),LogLevel.Warning);
                    return null;
                }
                if (!VerifyChecksum(rxBuf))
                {
                    logger.Create(String.Format(" -> ReadCoilStatus {0}/cnt={1}: invalid checksum!", coilIndex, coinCnt),LogLevel.Warning);
                    return null;
                }

                byte[] ret = new byte[coinCnt];
                for (int i = 0; i < coinCnt; i++)
                {
                    Int32 byteIdx = 3 + i / 8;
                    if ((rxBuf[byteIdx] & (1 << i)) != 0)
                        ret[i] = 1;
                    else
                        ret[i] = 0;
                }
                return ret;
            }
        }

        private UInt16[] ReadHoldingRegisters(UInt16 startAddr, UInt16 regCnt)
        {
            byte[] txBuf = new byte[32];
            Int32 idx = 0;
            Byte functionCode = FC_READ_HOLDING_REGISTERS;

            if (startAddr >= 40000)
                startAddr -= 40000;

            txBuf[idx++] = plcMbAddr;
            txBuf[idx++] = functionCode;
            txBuf[idx++] = (Byte)(startAddr >> 8);
            txBuf[idx++] = (Byte)(startAddr & 0xff);
            txBuf[idx++] = (Byte)(regCnt >> 8);
            txBuf[idx++] = (Byte)(regCnt & 0xff);
            UInt16 crc = CRC16(txBuf, idx);
            txBuf[idx++] = (Byte)(crc & 0xff);
            txBuf[idx++] = (Byte)(crc >> 8);

            lock (ComportLock)
            {
                isReading = true;
                readingBuf = new List<byte>();
                this.Comport.Write(txBuf, 0, idx);

                Int32 rxByteCnt = regCnt * 2;
                Int32 rxExpectedCnt = 3 + rxByteCnt + 2;
                if (!waitForResponse(rxExpectedCnt, MB_RESPONSE_TIMEOUT))
                {
                    logger.Create(String.Format(" -> ReadHoldingRegisters {0:X4}/cnt={1}: no response!", startAddr, regCnt), LogLevel.Warning);
                    return null;
                }
                byte[] rxBuf = new byte[rxExpectedCnt];
                Array.Copy(readingBuf.ToArray(), rxBuf, rxExpectedCnt);

                if ((rxBuf[0] != plcMbAddr) || (rxBuf[1] != functionCode))
                {
                    logger.Create(String.Format(" -> ReadHoldingRegisters {0:X4}/cnt={1}: invalid DeviceAddress or FunctionCode!", startAddr, regCnt), LogLevel.Warning);
                    return null;
                }
                if (!VerifyChecksum(rxBuf))
                {
                    logger.Create(String.Format(" -> ReadHoldingRegisters {0:X4}/cnt={1}: invalid checksum!", startAddr, regCnt), LogLevel.Warning);
                    return null;
                }

                UInt16[] ret = new UInt16[regCnt];
                for (int i = 0; i < regCnt; i++)
                {
                    ret[i] = (UInt16)((rxBuf[3 + i * 2] << 8) | rxBuf[3 + i * 2 + 1]);
                }
                return ret;
            }
        }

        private Boolean WriteSingleCoil(UInt16 coilIndex, Byte value)
        {
            byte[] txBuf = new byte[32];
            Int32 idx = 0;
            Byte functionCode = FC_WRITE_SINGLE_COIL;
            UInt16 coilValue = value != 0 ? (UInt16)0xff00 : (UInt16)0x0000;

            txBuf[idx++] = plcMbAddr;
            txBuf[idx++] = functionCode;
            txBuf[idx++] = (Byte)(coilIndex >> 8);
            txBuf[idx++] = (Byte)(coilIndex & 0xff);
            txBuf[idx++] = (Byte)(coilValue >> 8);
            txBuf[idx++] = (Byte)(coilValue & 0xff);
            UInt16 crc = CRC16(txBuf, idx);
            txBuf[idx++] = (Byte)(crc & 0xff);
            txBuf[idx++] = (Byte)(crc >> 8);

            lock (ComportLock)
            {
                isReading = true;
                readingBuf = new List<byte>();
                this.Comport.Write(txBuf, 0, idx);

                Int32 rxExpectedCnt = 8;
                if (!waitForResponse(rxExpectedCnt, MB_RESPONSE_TIMEOUT))
                {
                    logger.Create(String.Format(" -> WriteSingleCoil {0}/{1:X2}: no response!", coilIndex, value), LogLevel.Error);
                    return false;
                }
                byte[] rxBuf = new byte[rxExpectedCnt];
                Array.Copy(readingBuf.ToArray(), rxBuf, rxExpectedCnt);

                if ((rxBuf[0] != plcMbAddr) || (rxBuf[1] != functionCode))
                {
                    logger.Create(String.Format(" -> WriteSingleCoil {0}/{1:X2}: invalid DeviceAddress or FunctionCode!", coilIndex, value), LogLevel.Error);
                    return false;
                }
                if (!VerifyChecksum(rxBuf))
                {
                    logger.Create(String.Format(" -> WriteSingleCoil {0}/{1:X2}: invalid checksum!", coilIndex, value), LogLevel.Error);
                    return false;
                }
                if (((rxBuf[2] << 8) + rxBuf[3]) != coilIndex)
                {
                    logger.Create(String.Format(" -> WriteSingleCoil {0}/{1:X2}: invalid Rsp.CoilIndex!", coilIndex, value), LogLevel.Error);
                    return false;
                }
                if (((rxBuf[4] << 8) + rxBuf[5]) != coilValue)
                {
                    logger.Create(String.Format(" -> WriteSingleCoil {0}/{1:X2}: invalid Rsp.CoilValue!", coilIndex, value), LogLevel.Error);
                    return false;
                }
                return true;
            }
        }

        private Boolean WriteSingleRegister(UInt16 regAddr, UInt16 regValue)
        {
            byte[] txBuf = new byte[32];
            Int32 idx = 0;
            Byte functionCode = FC_WRITE_SINGLE_REGISTER;

            txBuf[idx++] = plcMbAddr;
            txBuf[idx++] = functionCode;
            txBuf[idx++] = (Byte)(regAddr >> 8);
            txBuf[idx++] = (Byte)(regAddr & 0xff);
            txBuf[idx++] = (Byte)(regValue >> 8);
            txBuf[idx++] = (Byte)(regValue & 0xff);
            UInt16 crc = CRC16(txBuf, idx);
            txBuf[idx++] = (Byte)(crc & 0xff);
            txBuf[idx++] = (Byte)(crc >> 8);

            lock (ComportLock)
            {
                isReading = true;
                readingBuf = new List<byte>();
                this.Comport.Write(txBuf, 0, idx);

                Int32 rxExpectedCnt = 8;
                if (!waitForResponse(rxExpectedCnt, MB_RESPONSE_TIMEOUT))
                {
                    logger.Create(String.Format(" -> WriteSingleRegister {0:X4}/{1:X4}: no response!", regAddr, regValue), LogLevel.Error);
                    return false;
                }
                byte[] rxBuf = new byte[rxExpectedCnt];
                Array.Copy(readingBuf.ToArray(), rxBuf, rxExpectedCnt);

                if ((rxBuf[0] != plcMbAddr) || (rxBuf[1] != functionCode))
                {
                    logger.Create(String.Format(" -> WriteSingleRegister {0:X4}/{1:X4}: invalid DeviceAddress or FunctionCode!", regAddr, regValue), LogLevel.Error);
                    return false;
                }
                if (!VerifyChecksum(rxBuf))
                {
                    logger.Create(String.Format(" -> WriteSingleRegister {0:X4}/{1:X4}: invalid checksum!", regAddr, regValue), LogLevel.Error);
                    return false;
                }
                if (((rxBuf[2] << 8) + rxBuf[3]) != regAddr)
                {
                    logger.Create(String.Format(" -> WriteSingleRegister {0:X4}/{1:X4}: invalid Rsp.RegAddr!", regAddr, regValue), LogLevel.Error);
                    return false;
                }
                if (((rxBuf[4] << 8) + rxBuf[5]) != regValue)
                {
                    logger.Create(String.Format(" -> WriteSingleRegister {0:X4}/{1:X4}: invalid Rsp.RegVal!", regAddr, regValue), LogLevel.Error);
                    return false;
                }
                return true;
            }
        }

        private Boolean WriteMultipleRegisters(UInt16 startAddr, UInt16[] regValues)
        {
            byte[] txBuf = new byte[32];
            Int32 idx = 0;
            Byte functionCode = FC_WRITE_MULTIPLE_REGISTERS;

            txBuf[idx++] = plcMbAddr;
            txBuf[idx++] = functionCode;
            txBuf[idx++] = (Byte)(startAddr >> 8);
            txBuf[idx++] = (Byte)(startAddr & 0xff);
            UInt16 regCnt = (UInt16)regValues.Length;
            txBuf[idx++] = (Byte)(regCnt >> 8);
            txBuf[idx++] = (Byte)(regCnt & 0xff);
            txBuf[idx++] = (Byte)(regCnt * 2);
            for (int regIdx = 0; regIdx < regCnt; regIdx++)
            {
                UInt16 reg = regValues[regIdx];
                txBuf[idx++] = (Byte)(reg >> 8);
                txBuf[idx++] = (Byte)(reg & 0xff);
            }
            UInt16 crc = CRC16(txBuf, idx);
            txBuf[idx++] = (Byte)(crc & 0xff);
            txBuf[idx++] = (Byte)(crc >> 8);

            lock (ComportLock)
            {
                isReading = true;
                readingBuf = new List<byte>();
                this.Comport.Write(txBuf, 0, idx);

                Int32 rxExpectedCnt = 8;
                if (!waitForResponse(rxExpectedCnt, MB_RESPONSE_TIMEOUT))
                {
                    logger.Create(String.Format(" -> WriteMultipleRegisters {0:X4}/cnt={1}: no response!", startAddr, regCnt), LogLevel.Error);
                    return false;
                }
                byte[] rxBuf = new byte[rxExpectedCnt];
                Array.Copy(readingBuf.ToArray(), rxBuf, rxExpectedCnt);

                if ((rxBuf[0] != plcMbAddr) || (rxBuf[1] != functionCode))
                {
                    logger.Create(String.Format(" -> WriteMultipleRegisters {0:X4}/cnt={1}: invalid DeviceAddress or FunctionCode!", startAddr, regCnt), LogLevel.Error);
                    return false;
                }
                if (!VerifyChecksum(rxBuf))
                {
                    logger.Create(String.Format(" -> WriteMultipleRegisters {0:X4}/cnt={1}: invalid checksum!", startAddr, regCnt), LogLevel.Error);
                    return false;
                }
                if (((rxBuf[2] << 8) + rxBuf[3]) != startAddr)
                {
                    logger.Create(String.Format(" -> WriteMultipleRegisters {0:X4}/{1:X4}: invalid Rsp.StartAddr!", startAddr, regCnt), LogLevel.Error);
                    return false;
                }
                if (((rxBuf[4] << 8) + rxBuf[5]) != regCnt)
                {
                    logger.Create(String.Format(" -> WriteMultipleRegisters {0:X4}/{1:X4}: invalid Rsp.RegCnt!", startAddr, regCnt), LogLevel.Error);
                    return false;
                }
                return true;
            }
        }
    }

}
