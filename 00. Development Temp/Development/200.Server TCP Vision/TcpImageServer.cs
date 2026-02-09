using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    class TcpImageServer
    {
        private static MyLogger logger = new MyLogger(" TcpImageServer");

        protected NotifyEvenDataVision NotifyEvenDataVision;
        protected TcpListener listener;
        protected TcpClient mesClients;
        public bool isRunning = false;
        protected bool isReceiver = false;
        public bool isAccept = false;
        protected string DataReceiver { get; set; }
        private string Ip;
        private int Port;
        public TcpImageServer(string ip, int port)
        {
            this.LoadNotifyEvenDataVision();
            this.Port = port;
            this.Ip = ip;
            listener = new TcpListener(IPAddress.Parse(Ip), Port);
        }
        private void LoadNotifyEvenDataVision()
        {
            //this.NotifyEvenDataVision = SystemsManager.Instance.NotifyEvenDataVision;
        }
        public async Task StartAsync()
        {
            try
            {

                this.isRunning = true;
                listener.Start();
                isRunning = true;
                this.NotifyEvenDataVision.NotifyToUI($"Notify TCPVision Open Server...");


                while (isRunning)
                {
                    try
                    {
                        TcpClient mesClient = await listener.AcceptTcpClientAsync();
                        if (mesClients == null)
                        {
                            mesClients = mesClient;
                            string clientIP = ((IPEndPoint)mesClient.Client.RemoteEndPoint).Address.ToString();
                            int clientPort = ((IPEndPoint)mesClient.Client.RemoteEndPoint).Port;

                            this.isAccept = true;
                            NotifyEvenDataVision.NotifyConnectDataVision(true);
                            NotifyEvenDataVision.GetInformationFromClientConnect(clientIP, clientPort);
                            NotifyEvenDataVision.NotifyToUI($"Client Connect IP: {clientIP} - Port :{clientPort}");
                            logger.CreateDataVision($"Client Connect IP: {clientIP} - Port : {clientPort} ",LogLevel.Information);

                            _ = Task.Run(() => HandleClientAsync(mesClient));
                        }
                        else
                        {
                            NotifyEvenDataVision.NotifyToUI($"Client Disconnect ");
                            NotifyEvenDataVision.NotifyConnectDataVision(false);
                            this.isAccept = false;
                            mesClient.Close();
                        }


                    }
                    catch (Exception ex)
                    {
                        this.NotifyEvenDataVision.NotifyToUI("Notify : TCPVision Connection Error - " + ex.Message);
                        this.NotifyEvenDataVision.NotifyConnectDataVision(false);
                        logger.CreateDataVision("TCPVision Connection Error - " + ex.Message,LogLevel.Error);
                        this.isAccept = false;
                        await Task.Delay(300);
                    }

                }
            }
            catch (Exception ex)
            {

                this.NotifyEvenDataVision.NotifyToUI("Notify : TCPVision Connection Failed - " + ex.Message);
                logger.CreateDataVision("TCPVision Connection Failed - " + ex.Message, LogLevel.Error);


                NotifyEvenDataVision.NotifyConnectDataVision(false);
                this.isAccept = false;
            }
        }
        public async Task StopAsync()
        {
            if (isRunning)
            {
                isRunning = false;
                listener.Stop();


                if (mesClients != null)
                {
                    mesClients.Close();
                    mesClients = null;
                }

                await Task.CompletedTask;
            }

        }
        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] lengthBuffer = new byte[4];
                while (true)
                {
                    int bytesReadLenght = await stream.ReadAsync(lengthBuffer, 0, 4);
                    if (bytesReadLenght == 0)
                    {
                        this.NotifyEvenDataVision.NotifyToUI("Notify : Connect From TCPVision Is Closed!!!");
                        this.NotifyEvenDataVision.NotifyConnectDataVision(false);
                        this.isAccept = false;
                        client.Close();
                        this.ReconnectToMES();
                        break;
                    }
                    int length = BitConverter.ToInt32(lengthBuffer, 0);
                    byte[] dataBuffer = new byte[length];
                    int bytesRead = 0;
                    while (bytesRead < length)
                    {
                        bytesRead += await stream.ReadAsync(dataBuffer, bytesRead, length - bytesRead);

                        if (bytesRead == 0)
                        {
                            this.NotifyEvenDataVision.NotifyToUI("Notify : Connect From TCPVision Is Closed!!!");
                            this.NotifyEvenDataVision.NotifyConnectDataVision(false);
                            this.isAccept = false;
                            client.Close();
                            this.ReconnectToMES();
                            break;
                        }
                    }

                    string requestData = Encoding.UTF8.GetString(dataBuffer, 0, bytesRead);
                    string responseData = requestData;

                    //string responseData = requestData.Replace("\r\n", "").Trim();
                    //logger.CreateDataVision($@"MES.RECEIVER Status :" + responseData);

                    string qui = "DATAVISION";
                    qui = qui.PadRight(10, ' ');

                    if (!responseData.Contains(qui + "AUTO01"))
                    {
                        this.DataReceiver = responseData;
                        this.isReceiver = true;

                        DATACheck newMESCheck = new DATACheck();
                        newMESCheck.EquipmentId = "DATAVISION";
                        newMESCheck.Status = "AUTO10";
                        var DataCheck = FillData(DataReceiver, newMESCheck);





                    }
                    if (responseData.Contains(qui + "AUTO01"))
                    {
                        this.NotifyEvenDataVision.NotifyToUI($"Notify TCPVision RECEIVER : {responseData} ");
                        logger.CreateDataVision($@"TCPVision.RECEIVER Status :" + responseData,LogLevel.Information);
                        string responseToSend = qui + "AUTO02";
                        byte[] responseBytes = Encoding.UTF8.GetBytes(responseToSend);
                        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                        logger.CreateDataVision($@"TCPVision.SEND Status :" + responseToSend, LogLevel.Information);
                        this.NotifyEvenDataVision.NotifyToUI($"Notify TCPVision SEND : {responseData} ");
                        await stream.FlushAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.CreateDataVision($@"HandleClientAsync ERROR :" + ex, LogLevel.Error);

            }


        }
        private async void ReconnectToMES()
        {
            isRunning = false;
            mesClients = null;
            await StartAsync();
        }
        public DATACheck FillData(string data, DATACheck mesOld)
        {
            try
            {
                DATACheck newMESCheck = new DATACheck();
                int idex = 0;
                // EQUIPMENT ID
                string equipmentId = data.Substring(idex, 10);
                if (mesOld.EquipmentId != equipmentId)
                {
                    logger.CreateDataVision($@"EquipmentId Is Diffrent: Old('" + mesOld.EquipmentId + "') , New('" + equipmentId + "')", LogLevel.Information);
                    return null;
                }
                logger.CreateDataVision($@"TCPVision.RECEIVER EquipmentId:" + equipmentId, LogLevel.Information);
                newMESCheck.EquipmentId = equipmentId;
                idex += 10;

                // STATUS
                string status = data.Substring(idex, 6);
                if (status == "AUTO10")
                {
                }
                logger.CreateDataVision($@"TCPVision.RECEIVER Status :" + status, LogLevel.Information);
                newMESCheck.Status = status;
                idex += 6;

                //BODY
                idex += 1;
                StringBuilder stringBuilder = new StringBuilder(data);
                stringBuilder.Remove(0, idex);
                var body = stringBuilder.ToString();
                if (newMESCheck.Status == "AUTO10")
                {
                    List<FormatVision> DATA = new List<FormatVision>();
                    var formatVision = new { DATA };
                    var j = JsonConvert.DeserializeAnonymousType(body, formatVision);
                    if (j == null || j.DATA.Count <= 0) return null;
                    mesOld.FormatVision = j.DATA[0];
                    NotifyEvenDataVision.NotifyToUI($"RECEIVE AUTO10 DATACHECK COMPLETE !");
                    NotifyEvenDataVision.NotifyResultDataVision(mesOld);
                }





                //StringBuilder stringBuilder = new StringBuilder(data);
                //stringBuilder.Remove(0, idex);

                //var checkResult = stringBuilder[0];

                //if (checkResult == '{' || checkResult == '[') // Format
                //{
                //    string mesCheck = "";
                //    var body = stringBuilder.ToString().Split(';');
                //    if (body.Length < 2) return null;
                //    if (body.Length == 3)
                //    {
                //        mesCheck = body[0] + body[1];
                //    }
                //    else
                //    {
                //        mesCheck = body[0];
                //    }
                //    if (newMESCheck.Status == "AUTO10")
                //    {
                //        List<FormatVision> DATA = new List<FormatVision>();
                //        var formatVision = new { DATA };
                //        var j = JsonConvert.DeserializeAnonymousType(mesCheck, formatVision);
                //        if (j == null || j.DATA.Count <= 0) return null;
                //        mesOld.FormatVision = j.DATA[0];

                //        NotifyEvenDataVision.NotifyResultDataVision(mesOld);
                //    }
                //}



            }
            catch (Exception ex)
            {

                logger.CreateDataVision($@"FillData ERROR :" + ex, LogLevel.Error);
            }

            return mesOld;
        }
        public async void SendReady()
        {
            try
            {
                string qui = "DATAVISION";
                string responseToSend = qui + "AUTO02";
                byte[] responseBytes = Encoding.UTF8.GetBytes(responseToSend);
                await SendToMES(responseBytes);
                logger.CreateDataVision($@"TCPVision.SEND Status :" + responseToSend, LogLevel.Information);
            }
            catch (Exception ex)
            {

                logger.CreateDataVision($@"SendToTCPVision SendReady Error :" + ex, LogLevel.Error);
            }


        }
        public async Task SendToMES(byte[] txBuf)
        {
            try
            {
                if (!isRunning)
                {
                    return;
                }

                byte[] lengthBytes = BitConverter.GetBytes(txBuf.Length);

                NetworkStream stream = mesClients.GetStream();


                await stream.WriteAsync(txBuf, 0, txBuf.Length);
            }
            catch (Exception ex)
            {
                logger.CreateDataVision($@"SendToTCPVision Error :" + ex, LogLevel.Error);
            }


        }
    }
}
