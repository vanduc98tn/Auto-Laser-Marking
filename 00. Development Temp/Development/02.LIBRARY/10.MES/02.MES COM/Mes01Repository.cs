using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class Mes01Repository
    {
        private MyLogger logger = new MyLogger("MESCOM");
        //Notify
        protected NotifyEvenMES notifyEvenMES;
        // TCP server:
        protected TcpListener listener;
        protected TcpClient mesClients;
        protected bool isRunning = false;
        public bool isAccept = false;
        //
        protected bool isReceiver = false;
        protected string DataReceiver { get; set; }

        public Mes01Repository(string ip, int port)
        {
            try
            {
                this.LoadNotifyEvenMES();
                this.listener = new TcpListener(IPAddress.Parse(ip), port);
            }
            catch (Exception ex)
            {
                logger.Create("BaseRepositoryMES :" + ex.Message, LogLevel.Error);
            }
        }
        private void LoadNotifyEvenMES()
        {
            this.notifyEvenMES = SystemsManager.Instance.NotifyEvenMES;
        }
        public async Task Start()
        {
            if (this.isRunning)
            {
                this.notifyEvenMES.NotifyToUI("Notify : MES Is Opend!!!");
                return;
            }
            try
            {

                this.listener.Start();
                this.isRunning = true;
                this.notifyEvenMES.NotifyToUI("Notify : Listen MES...!!!");
                while (isRunning)
                {
                    TcpClient mesClient = await listener.AcceptTcpClientAsync();
                    if (mesClients == null)
                    {
                        mesClients = mesClient;
                        this.notifyEvenMES.NotifyToUI("Notify : Server Connected To MES!!!");
                        this.notifyEvenMES.NotifyMESConnect(true);
                        // Lấy địa chỉ IP của client
                        string clientIP = ((IPEndPoint)mesClient.Client.RemoteEndPoint).Address.ToString();
                        int clientPort = ((IPEndPoint)mesClient.Client.RemoteEndPoint).Port;
                        this.notifyEvenMES.GetInformationFromClientConnect(clientIP, clientPort);
                        this.isAccept = true;
                        this.notifyEvenMES.NotifyToUI($"Notify : Client connected from {clientIP}:{clientPort}");
                        await Task.Run(() => HandleMesClient(mesClients));
                    }
                    else
                    {
                        this.notifyEvenMES.NotifyToUI("Notify : MES Is Disconnect!!!");
                        this.notifyEvenMES.NotifyMESConnect(false);
                        this.isAccept = false;
                        mesClient.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                this.notifyEvenMES.NotifyToUI("Notify : MES Connect Faild " + ex.Message);
                this.logger.Create("MES Connect Faild " + ex.Message, LogLevel.Information);
                this.notifyEvenMES.NotifyMESConnect(false);
                this.isAccept = false;
            }
        }
        public void Stop()
        {
            if (isRunning)
            {
                isRunning = false;
                listener.Stop();
                mesClients.Close();
                mesClients = null;
                this.notifyEvenMES.NotifyToUI("Notify : Server Is Closed!!!");
                this.notifyEvenMES.NotifyMESConnect(false);
                this.isAccept = false;
            }
        }
        public bool CheckMESConnection()
        {
            return mesClients != null && mesClients.Connected;
        }
        public async Task HandleMesClient(TcpClient mesClient)
        {
            NetworkStream stream = mesClient.GetStream();
            byte[] buffer = new byte[1024];
            while (true)
            {
                try
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        this.notifyEvenMES.NotifyToUI("Notify : Connect From MES Is Closed!!!");
                        this.notifyEvenMES.NotifyMESConnect(false);
                        this.isAccept = false;
                        mesClient.Close();
                        await ReconnectToMES();
                        break;
                    }
                    string requestData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    string responseData = requestData;
                    string equip = UiManager.appSetting.MESSettings.EquimentID;
                    equip = equip.PadRight(9, ' ');
                    if (!responseData.Contains(equip + "E001"))
                    {
                        this.DataReceiver = responseData;
                        this.isReceiver = true;
                    }
                    this.notifyEvenMES.NotifyMESResult($"Notify - MES Receiver :{responseData}");
                }
                catch (Exception ex)
                {
                    this.notifyEvenMES.NotifyToUI("Notify : Error Access MES : " + ex.Message);
                    this.notifyEvenMES.NotifyMESConnect(false);
                    this.isAccept = false;
                    mesClient.Close();
                    isRunning = false;
                    mesClients = null;
                    break;
                }
            }
        }
        public async Task SendToMes(byte[] txBuf)
        {
            if (!isRunning)
            {
                this.notifyEvenMES.NotifyToUI("Notify : Server Is Closed or MES is Closed : ");
                this.notifyEvenMES.NotifyMESConnect(false);
                this.isAccept = false;
                return;
            }
            NetworkStream stream = mesClients.GetStream();
            await stream.WriteAsync(txBuf, 0, txBuf.Length);
            byte[] buffer = new byte[1024];
        }
        private async Task ReconnectToMES()
        {
            isRunning = false;
            mesClients = null;
            await Start();
        }
        public async Task<MES01Check> Send(MES01Check entity, string CH)
        {
            this.isReceiver = false;
            try
            {
                if (!this.CheckMESConnection())
                {
                    logger.Create(" -> TCP connection not ready -> discard sending SendReady!", LogLevel.Warning);
                    return null;
                }
                var packet = new List<byte>();
                //HEADER
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes(entity.EquipmentId));
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes(entity.Status));
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes(entity.LotNo));
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes(";"));
                // BODY
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes(entity.PCB_Code));
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes(";"));
                //Check SUM
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes(entity.CheckSum));

                var txBuf = packet.ToArray();
                logger.Create($@"MES.SEND PCB{CH}:" + ASCIIEncoding.ASCII.GetString(txBuf), LogLevel.Information);
                this.notifyEvenMES.NotifyToUI($@"Notify [MES{CH}]: SEND TO MES ->" + ASCIIEncoding.ASCII.GetString(txBuf));
                await this.SendToMes(txBuf);
                this.notifyEvenMES.NotifyToUI($@"Notify [MES{CH}]: Wait MES Receiver...");
                await WaitMESReturnData();
                if (this.isReceiver && !string.IsNullOrEmpty(this.DataReceiver))
                {
                    this.isReceiver = false;
                    logger.Create($@"MES.RECEIVER PCB{CH}:" + this.DataReceiver, LogLevel.Information);
                    //this.notifyEvenMES.NotifyToUI($@"MES.RECEIVER PCB{CH}:" + this.DataReceiver);
                    return FilterData(this.DataReceiver, entity, CH);
                }
                this.notifyEvenMES.NotifyToUI($@"Notify [MES{CH}]: No Response from MES");
                logger.Create($@"Notify [MES{CH}]: No Response from MES", LogLevel.Warning);
            }
            catch (Exception ex)
            {
                logger.Create($@"Send{CH} : " + ex.Message, LogLevel.Error);
            }
            return null;
        }
        public async Task<bool> SendReady(MES01Check entity, string CH)
        {
            this.isReceiver = false;
            try
            {
                if (!this.CheckMESConnection())
                {
                    logger.Create(" -> TCP connection not ready -> discard sending SendReady!", LogLevel.Warning);
                    return false;
                }
                var packet = new List<byte>();
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes(entity.EquipmentId));
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes("E001"));
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes("READYOK                                          "));
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes(";"));
                var txBuf = packet.ToArray();
                logger.Create($@"MES.SEND Ready{CH}:" + ASCIIEncoding.ASCII.GetString(txBuf), LogLevel.Information);
                this.notifyEvenMES.NotifyToUI($@"Notify [MES{CH}]: SEND TO MES ->" + ASCIIEncoding.ASCII.GetString(txBuf));
                await this.SendToMes(txBuf);
                this.notifyEvenMES.NotifyToUI($@"Notify [MES{CH}]: Wait MES Receiver...");
                await WaitMESReturnData();
                if (this.isReceiver && !string.IsNullOrEmpty(this.DataReceiver))
                {
                    this.isReceiver = false;
                    logger.Create($@"MES.RECEIVER Ready{CH}:" + this.DataReceiver, LogLevel.Information);
                    var mesData = FilterData(this.DataReceiver, entity, CH);
                    if (mesData != null) return true;
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Create($@"SendReady{CH} : " + ex.Message, LogLevel.Error);
            }
            return false;
        }
        public async Task<MES01Check> SendLogin(MES01Check entity, string ch)
        {
            this.isReceiver = false;
            try
            {
                if (!this.CheckMESConnection())
                {
                    logger.Create(" -> TCP connection not ready -> discard sending Send Login!", LogLevel.Warning);
                    return null;
                }
                var packet = new List<byte>();
                // HEAD
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes(entity.EquipmentId));
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes("P002"));
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes("          ")); // 10 lotno
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes(";"));
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes(entity.MESCheckLogIn.Type));
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes("^"));
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes(entity.MESCheckLogIn.User));
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes("^"));
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes(entity.MESCheckLogIn.Password));
                packet.AddRange(ASCIIEncoding.ASCII.GetBytes(";"));
                var txBuf = packet.ToArray();
                logger.Create($@"MES.SEND Login [MES{ch}]:" + ASCIIEncoding.ASCII.GetString(txBuf), LogLevel.Information);
                this.notifyEvenMES.NotifyToUI($@"Notify [MES{ch}]: SEND TO MES ->" + ASCIIEncoding.ASCII.GetString(txBuf));
                await this.SendToMes(txBuf);
                this.notifyEvenMES.NotifyToUI($@"Notify [MES{ch}]: Wait MES Receiver...");
                await WaitMESReturnData();
                if (this.isReceiver && !string.IsNullOrEmpty(this.DataReceiver))
                {
                    this.isReceiver = false;
                    logger.Create($@"MES.RECEIVER Login [MES{ch}]:" + this.DataReceiver, LogLevel.Information);
                    return FilterData(this.DataReceiver, entity, ch);
                }
            }
            catch (Exception ex)
            {
                logger.Create($@"SendReady {ch}: " + ex.Message, LogLevel.Error);
            }
            return null;
        }
        private async Task WaitMESReturnData()
        {
            int counterDelayReceiver = 0;
            await Task.Run(async () => {
                while (!this.isReceiver)
                {
                    if (counterDelayReceiver > 1000)
                    {
                        break;
                    }
                    await Task.Delay(10); // Đợi 10 giây
                    counterDelayReceiver++;
                }
            });
        }
        private MES01Check FilterData(string data, MES01Check mesOld, string CH)
        {
            try
            {
                MES01Check newMESCheck = new MES01Check();
                int idex = 0;
                // EQUIPMENT ID
                string equipmentId = data.Substring(idex, 9);
                if (mesOld.EquipmentId != equipmentId)
                {
                    logger.Create($@"EquipmentId Is Diffrent {CH}: Old('" + mesOld.EquipmentId + "') , New('" + equipmentId + "')", LogLevel.Information);
                    return null;
                }
                logger.Create($@"MES.RECEIVER EquipmentId {CH}:" + equipmentId, LogLevel.Information);
                newMESCheck.EquipmentId = equipmentId;
                idex += 9;
                // STATUS
                string status = data.Substring(idex, 4);
                if (status == "E002")
                {
                    logger.Create($@"MES.RECEIVER Status {CH}:" + status, LogLevel.Information);
                    newMESCheck.Status = status;
                    return newMESCheck;
                }
                logger.Create($@"MES.RECEIVER Status {CH}:" + status, LogLevel.Information);
                newMESCheck.Status = status;

                idex += 4;
                // LOT NO
                string lotNo = data.Substring(idex, 9);

                logger.Create($@"MES.RECEIVER LotNo {CH}:" + lotNo, LogLevel.Information);
                newMESCheck.LotNo = lotNo;
                idex += 9;
                idex += 1;
                StringBuilder stringBuilder = new StringBuilder(data);
                stringBuilder.Remove(0, idex);
                var body = stringBuilder.ToString().Split(';');

                if (body.Length == 2)
                {
                    var x = body[1].Split(';');
                    newMESCheck.MES_Result = body[0];
                    newMESCheck.CheckSum = body[1];
                }
                //if (newMESCheck.Status == "E092")
                //{
                //    if (body.Length >= 3)
                //    {
                //        int i = 0;
                //        int Row = UiManager.appSetting.runSetting.RowSetting;
                //        int Colum = UiManager.appSetting.runSetting.ColumnSetting;
                //        for (i = 0; i < Row * Colum; i++)
                //        {
                //            newMESCheck.PCB_Result.Add(body[i]);
                //        }
                //        newMESCheck.CheckSum = body[i].Trim();
                //    }
                //    else
                //    {
                //        return null;
                //    }
                //}

                return newMESCheck;
            }
            catch (Exception ex)
            {
                logger.Create($@"FilterData {CH}: " + ex.Message, LogLevel.Error);
            }
            return null;
        }
    }
}
