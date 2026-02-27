using ITM_Semiconductor;
using Newtonsoft.Json.Converters;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;


namespace Development
{
    
    public partial class PgMain : Page, IObserverChangeBits , IObserverMES
    {
        private MyLogger logger = new MyLogger("PgMain");
        private DispatcherTimer timer;
        private LotInData lotInData;
        private readonly CompositeViewModel compositeViewModel;

        private bool isUpdate = false;
        private List<short> D_ListShortDevicePLC_0_299 = new List<short>();
        private List<bool> M_ListBitPLC_0_99 = new List<bool>();
        private List<bool> M_ListBitPLC_500_549 = new List<bool>();
        private bool hasClearedError = false;


        private MediaPlayer mediaPlayer = new MediaPlayer();
        private bool isLooping = false;

        private Thread AutoRunThread;
        private object LockRun = new object();
        private bool isRunning = false;
        private bool isQR = false;
        private bool isVision = false;

        private PatternSetting pattern = UiManager.appSetting.Pattern;
        private Brush MES_COLOR = Brushes.Red;
        private Brush VISION_COLOR = Brushes.Purple;
        private Brush BOTH_COLOR;

        private DataPCB DataPCB;

        public PgMain()
        {
            // ON Binding addlog
            compositeViewModel = new CompositeViewModel
            {
                LogEntries = new ObservableCollection<logEntry>()
            };
            DataContext = compositeViewModel;
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

            InitializeErrorCodes();
            InitBothColor();
            InitializeComponent();
            


            // Event Page Main
            this.Loaded += PgMain_Loaded;
            this.Unloaded += PgMain_Unloaded;
            this.btStart.Click += BtStart_Click;
            this.btStop.Click += BtStop_Click;
            this.btReset.PreviewMouseDown += BtReset_Click;
            //this.btHome.Click += BtHome_Click;
            this.btLotIn.Click += BtLotIn_Click;
            this.btLotOut.Click += BtLotOut_Click;

        }

        private void Running()
        {
            List<bool> M_ListBitPLC_ = new List<bool>();

            bool CLR_ALL_QR = false;
            bool TRIGGER_QR = false;

            bool WORKOUT_ON = false;

            while (isRunning)
            {
                if (UiManager.Instance.PLC.device.isOpen())
                {
                    UiManager.Instance.PLC.device.ReadMultiBits(DeviceCode.M, 500, 50, out M_ListBitPLC_);
                    if (M_ListBitPLC_.Count > 0)
                    {
                        CLR_ALL_QR = M_ListBitPLC_[12];  // M512
                        TRIGGER_QR = M_ListBitPLC_[10];  // M510

                        WORKOUT_ON = M_ListBitPLC_[20];  // M520
                    }
                }

                if (WORKOUT_ON)
                {

                    addLog("--- Workout MES --- ");
                    UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 520, false);
                    this.addLog("Write Bit M520 = OFF");
                    if (UiManager.appSetting.RUN.MESOnline)
                    {
                        
                        this.CheckMESWorkout();
                    }

                }

                if (CLR_ALL_QR)
                {
                    this.addLog("--- Clear all QR --- ");
                    UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 512, false);
                    this.addLog("Write Bit M512 = OFF");

                    this.UpdateUIQR("", false);

                    this.DataPCB = new DataPCB();
                    this.isQR = false;

                }

                // CHECK START SCANNER ---------------------------------------------------
                if (TRIGGER_QR)
                {

                    addLog("--- CHECK SCANNER --- ");
                    this.UpdateUIMES("START TRIGGER SCANNER", Brushes.LightGreen);

                    UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 510, false);
                    addLog("Write Bit M510 = OFF");

                    if (UiManager.appSetting.RUN.CheckScanner)
                    {
                        string QR = UiManager.Instance.scannerTCP.ReadQR();
                        //QR = "PCBID1234567890";
                        //QR = "";

                        if (!string.IsNullOrEmpty(QR))
                        {
                            this.UpdateUIQR(QR, true);

                            //UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 615, true);
                            //UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 616, false);
                            //addLog("Write Bit Scanner Trigger OK M615 = ON");
                            //addLog("Write Bit Scanner Trigger NG M616 = OFF");

                            this.UpdateUIMES($"SCANNER TRIGGER COMPLETE", Brushes.LightGreen);

                            this.DataPCB.BARCODE_PCB = QR;

                            if (UiManager.appSetting.RUN.MESOnline)
                            {
                                this.CheckMESWorkin();
                            }
                            else
                            {
                                var numbers = pattern.positionNGs;
                                var set = new HashSet<int>(numbers);

                                int max = set.Max();
                                StringBuilder sb = new StringBuilder();

                                for (int i = 1; i <= max; i++)
                                {
                                    sb.Append(set.Contains(i) ? '1' : '0');
                                }

                                string result = sb.ToString();

                                DataPCB.PRE_BIN_CODE = result;

                                int[] workinNG = BinCodeNG(DataPCB.PRE_BIN_CODE);
                                this.UpdateUIMESRESULT(workinNG);

                                this.isQR = true;


                            }    
                        }
                        else
                        {
                            this.UpdateUIQR("Scanner Error", false);

                            UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 615, false);
                            UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 616, true);
                            addLog("Write Bit Scanner Trigger OK M615 = OFF");
                            addLog("Write Bit Scanner Trigger NG M616 = ON");

                            this.UpdateUIMES($"ERROR SCANNER  : Unable to Read Code", Brushes.OrangeRed);

                        }
                    }
                    else
                    {
                        addLog("BY PASS SCANNER ");

                        this.UpdateUIMES($"BY PASS TRIGGER SCANNER", Brushes.LightGreen);

                        var numbers = pattern.positionNGs;
                        var set = new HashSet<int>(numbers);

                        int max = set.Max();
                        StringBuilder sb = new StringBuilder();

                        for (int i = 1; i <= max; i++)
                        {
                            sb.Append(set.Contains(i) ? '1' : '0');
                        }

                        string result = sb.ToString();

                        DataPCB.PRE_BIN_CODE = result;

                        int[] workinNG = BinCodeNG(DataPCB.PRE_BIN_CODE);
                        this.UpdateUIMESRESULT(workinNG);

                        this.isQR = true;

                    }
                    
                }

                if (this.isQR && this.isVision)
                {


                    if (UiManager.Instance.laserCOM.isOpen())
                    {
                        SendLaser();

                        this.isQR = false;
                        this.isVision = false;
                    }
                    else
                    {
                        string message = "- Check Laser connection again / COM not respond:\r\n" +
                                    "  + Verify Laser configuration\r\n" +
                                    "  + Verify COM connectivity\r\n" +
                                    "- Kiểm tra lại kết nối Laser / COM không phản hồi:\r\n" +
                                    "  + Kiểm tra lại setting Laser\r\n" +
                                    "  + Kiểm tra lại đường truyền COM\r\n";
                        AddErrorMES($"Error: Laser COM port is not open", message);

                        // SEND PLC LASER OK
                        UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 615, true);
                        UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 616, false);
                        addLog("Write Bit Laser OK M615 = ON");
                        addLog("Write Bit Laser NG M616 = OFF");

                    }

                }

                Thread.Sleep(20);
            }
        }
        private void VisionRunning()
        {
            List<bool> M_ListBitPLC_ = new List<bool>();

            bool CLR_ALL_VISION = false;
            bool TRIGGER1_VISION = false;
            bool TRIGGER2_VISION = false;

            while (isRunning)
            {
                if (UiManager.Instance.PLC.device.isOpen())
                {
                    UiManager.Instance.PLC.device.ReadMultiBits(DeviceCode.M, 500, 50, out M_ListBitPLC_);
                    if (M_ListBitPLC_.Count > 0)
                    {
                        CLR_ALL_VISION = M_ListBitPLC_[4];  // M504
                        TRIGGER1_VISION = M_ListBitPLC_[0];  // M500
                        TRIGGER2_VISION = M_ListBitPLC_[1];  // M501

                    }
                }


                if (CLR_ALL_VISION)
                {
                    this.addLog("--- Clear all Vision --- ");
                    UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 504, false);
                    this.addLog("Write Bit M504 = OFF");
                    this.DataPCB = new DataPCB();
                    this.isVision = false;

                }
                if (TRIGGER1_VISION)
                {
                    this.addLog("--- Trigger1 Vision --- ");
                    UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 500, false);
                    this.addLog("Write Bit M500 = OFF");

                    UpdateUICLRRESULT();

                }
                if (TRIGGER2_VISION)
                {
                    this.addLog("--- Trigger2 Vision --- ");
                    UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 501, false);
                    this.addLog("Write Bit M501 = OFF");

                    int[] arr = {1, 3, 4};
                    DataPCB.VISION_NG = arr;
                    UpdateUIVISIONRESULT(DataPCB.VISION_NG);

                    this.isVision = true;
                }

                Thread.Sleep(20);

            }
        }

        private Mes00Check GetDataWorkin()
        {
            Mes00Check DATA = new Mes00Check();
            Dispatcher.Invoke(() =>
            {

                DATA.FormatS010.AUTHORITY = UiManager.UserNameLoginMesOP_ME;
                DATA.FormatS010.ID = UiManager.CodeUserLoginMesOP_ME;
                DATA.FormatS010.LOT_NUMBER = UiManager.appSetting.LotinData.LotId;
                DATA.FormatS010.EQUIPMENT = UiManager.appSetting.MESSettings.EquimentID.ToString();
                DATA.FormatS010.CONFIG = UiManager.appSetting.LotinData.DeviceId;
                DATA.FormatS010.RECIPE = UiManager.appSetting.MESSettings.Repice;
                DATA.FormatS010.TRANS_TIME = DateTime.Now.ToString("yyyyMMddHHmmss");

                DATA.FormatS010.PCB_ID = DataPCB.BARCODE_PCB;

                DATA.EquipmentId = UiManager.appSetting.MESSettings.EquimentID.ToString();
                DATA.DIV = "WORKIN";
                DATA.Status = "S030";
                DATA.CheckSum = "WORKIN";

            });

            return DATA;
        }
        private async void CheckMESWorkin()
        {
            var Data = this.GetDataWorkin();

            UpdateUIMES("MES SEND READY ", Brushes.Yellow);

            // Send Ready
            var MESREADY = await UiManager.Instance.MES.SendReady(Data);

            if (!MESREADY)
            {
                // SEND PLC MES NG
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 615, false);
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 616, true);
                addLog("Write Bit Workin MES OK M615 = OFF");
                addLog("Write Bit Workin MES NG M616 = ON");

                string message = "- Check MES connection again:\r\n" +
                                 "  + Verify IP configuration\r\n" +
                                 "  + Verify network connectivity\r\n" +
                                 "- Kiểm tra lại kết nối MES:\r\n" +
                                 "  + Kiểm tra lại setting IP\r\n" +
                                 "  + Kiểm tra lại đường truyền\r\n";

                UpdateUIMES("MES CHECK READY IS FAIL", Brushes.Red);
                addLog("MES CHECK READY IS FAIL");
                AddErrorMES("MES CHECK READY IS FAIL", message);


                return;
            }

            // Send Workin
            UpdateUIMES("MES SEND WORKIN...", Brushes.Yellow);

            var MES = await UiManager.Instance.MES.SendWorkin(Data);

            if (MES == null)
            {
                // SEND PLC MES NG
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 615, false);
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 616, true);
                addLog("Write Bit Workin MES OK M615 = OFF");
                addLog("Write Bit Workin MES NG M616 = ON");

                string message = "- Check MES connection again / MES not respond. :\r\n" +
                                "  + Verify IP configuration\r\n" +
                                "  + Verify network connectivity\r\n" +
                                "- Kiểm tra lại kết nối MES / MES không phản hồi:\r\n" +
                                "  + Kiểm tra lại setting IP\r\n" +
                                "  + Kiểm tra lại đường truyền\r\n";

                UpdateUIMES("MES SEND WORKIN FAIL . MES NOT RESPOND", Brushes.Red);
                addLog("MES SEND WORKIN FAIL . MES NOT RESPOND");
                AddErrorMES("MES NOT RESPOND", message);

                return;

            }

            // MES CHECK NG
            if (MES.FormatS011.WORK_IN_RESULT.Contains("NG"))
            {
                // SEND PLC MES NG
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 615, false);
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 616, true);
                addLog("Write Bit Workin MES OK M615 = OFF");
                addLog("Write Bit Workin MES NG M616 = ON");

                UpdateUIMES("MES CHECK PCB WORKIN NG . PLEASE CHECK !!!!!", Brushes.Red);
                AddErrorMES("MES CHECK PCB WORKIN NG . PLEASE CHECK !!!!! ", MES.FormatS011.WORK_IN_MSG);


                return;

            }

            // MES CHECK OK
            else
            {

                DataPCB.WORK_IN_RESULT = MES.FormatS011.WORK_IN_RESULT;
                DataPCB.WORK_IN_MSG = MES.FormatS011.WORK_IN_MSG;
                DataPCB.CUR_BIN_CHAR = MES.FormatS011.CUR_BIN_CHAR;
                DataPCB.PRE_BIN_CODE = MES.FormatS011.PRE_BIN_CODE;

                addLog("MES CHECK PCB WORKIN OK");
                UpdateUIMES("MES CHECK PCB WORKIN OK", Brushes.LightGreen);

                int[] workinNG = BinCodeNG(DataPCB.PRE_BIN_CODE);
                this.UpdateUIMESRESULT(workinNG);

                this.isQR = true;

                return;
            }
        }
        private Mes00Check GetDataWorkout()
        {
            var CUR_BIN_MSG = MergeResult(DataPCB.VISION_NG, DataPCB.PRE_BIN_CODE, DataPCB.CUR_BIN_CHAR);

            Mes00Check DATA = new Mes00Check();
            Dispatcher.Invoke(() =>
            {
                DATA.FormatS020.AUTHORITY = UiManager.UserNameLoginMesOP_ME;
                DATA.FormatS020.ID = UiManager.CodeUserLoginMesOP_ME;
                DATA.FormatS020.LOT_NUMBER = UiManager.appSetting.LotinData.LotId;
                DATA.FormatS020.EQUIPMENT = UiManager.appSetting.MESSettings.EquimentID.ToString();
                DATA.FormatS020.CONFIG = UiManager.appSetting.LotinData.DeviceId;
                DATA.FormatS020.RECIPE = UiManager.appSetting.MESSettings.Repice;
                DATA.FormatS020.TRANS_TIME = DateTime.Now.ToString("yyyyMMddHHmmss");

                DATA.FormatS020.PCB_ID = DataPCB.BARCODE_PCB;
                DATA.FormatS020.CUR_BIN_CHAR = DataPCB.CUR_BIN_CHAR;
                DATA.FormatS020.CUR_BIN_CODE = CUR_BIN_MSG;

                DATA.EquipmentId = UiManager.appSetting.MESSettings.EquimentID.ToString();
                DATA.DIV = "WORKOUT";
                DATA.Status = "S040";
                DATA.CheckSum = "WORKOUT";
            });


            return DATA;
        }
        private async void CheckMESWorkout()
        {
            if (string.IsNullOrEmpty(DataPCB.PRE_BIN_CODE))
            {
                AddErrorMES("DataPCB.PRE_BIN_CODE IS ERROR ", "KHÔNG NHẬN DC BINCODE TỪ MES");
            }
            //if (DataPCB.RESULT_PCB.Count != DataPCB.PRE_BIN_CODE.Length)
            //{
            //    string message = $"RESULT_PCB.Count = {DataPCB.RESULT_PCB.Count}\r\n" +
            //                     $"PRE_BIN_CODE.Count = {DataPCB.PRE_BIN_CODE.Length}\r\n" +
            //                     $"Số lượng PCB hiện tại không khớp với dữ liệu MES gửi về";
            //    AddErrorMES("DATA RESULT_PCB NOT MATCH PRE_BIN_CODE ", message);
            //    return;
            //}
            var Data = this.GetDataWorkout();

            UpdateUIMES("MES SEND READY ", Brushes.Yellow);

            // Send Ready
            var MESREADY = await UiManager.Instance.MES.SendReady(Data);

            if (!MESREADY)
            {
                string message = "- Check MES connection again:\r\n" +
                                 "  + Verify IP configuration\r\n" +
                                 "  + Verify network connectivity\r\n" +
                                 "- Kiểm tra lại kết nối MES:\r\n" +
                                 "  + Kiểm tra lại setting IP\r\n" +
                                 "  + Kiểm tra lại đường truyền\r\n";


                // SEND PLC MES NG
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 620, false);
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 621, true);
                addLog("Write Bit Workout MES OK M620 = OFF");
                addLog("Write Bit Workout MES OK M621 = ON");


                UpdateUIMES("MES CHECK READY IS FAIL", Brushes.Red);
                addLog("MES CHECK READY IS FAIL");

                AddErrorMES("MES CHECK READY IS FAIL", message);
                return;
            }

            // Send Workin
            UpdateUIMES("MES SEND WORKOUT...", Brushes.Yellow);

            var MES = await UiManager.Instance.MES.SendWorkout(Data);

            if (MES == null)
            {
                string message = "- Check MES connection again / MES not respond. :\r\n" +
                                "  + Verify IP configuration\r\n" +
                                "  + Verify network connectivity\r\n" +
                                "- Kiểm tra lại kết nối MES / MES không phản hồi:\r\n" +
                                "  + Kiểm tra lại setting IP\r\n" +
                                "  + Kiểm tra lại đường truyền\r\n";


                // SEND PLC MES NG
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 620, false);
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 621, true);
                addLog("Write Bit Workout MES OK M620 = OFF");
                addLog("Write Bit Workout MES OK M621 = ON");


                UpdateUIMES("MES SEND WORKOUT FAIL . MES NOT RESPOND", Brushes.Red);
                addLog("MES SEND WORKOUT FAIL . MES NOT RESPOND");
                AddErrorMES("MES NOT RESPOND", message);




                return;

            }

            // MES CHECK NG
            if (MES.FormatS021.WORK_OUT_RESULT.Contains("NG"))
            {
                DataPCB.CUR_BIN_CHAR = MES.FormatS021.CUR_BIN_CHAR;
                DataPCB.RECEVIE_CUR_BIN_MSG = MES.FormatS021.CUR_BIN_CODE;

                DataPCB.SEND_CUR_BIN_MSG = MergeResult(DataPCB.VISION_NG, DataPCB.PRE_BIN_CODE, DataPCB.CUR_BIN_CHAR);
                DataPCB.RECEVIE_CUR_BIN_MSG = MES.FormatS021.CUR_BIN_CODE;

                DataPCB.WORK_OUT_RESULT = MES.FormatS021.WORK_OUT_RESULT;
                DataPCB.WORK_OUT_MSG = MES.FormatS021.WORK_OUT_MSG;

                DataPCB.TRAN_TIME = DateTime.Now.ToString("yyyyMMddHHmmss");
                logger.UpdateLogMes(DataPCB);

                // SEND PLC MES NG
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 620, false);
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 621, true);
                addLog("Write Bit Workout MES OK M620 = OFF");
                addLog("Write Bit Workout MES OK M621 = ON");

                UpdateUIMES("MES CHECK PCB WORKOUT NG", Brushes.Red);
                addLog("MES CHECK PCB WORKOUT NG");
                AddErrorMES("MES CHECK PCB WORKOUT NG . PLEASE CHECK !!!!! ", MES.FormatS021.WORK_OUT_MSG);

                UpdateOK_NG(0, 1);

                return;

            }

            // MES CHECK OK
            else
            {
                DataPCB.CUR_BIN_CHAR = MES.FormatS021.CUR_BIN_CHAR;
                DataPCB.RECEVIE_CUR_BIN_MSG = MES.FormatS021.CUR_BIN_CODE;

                DataPCB.SEND_CUR_BIN_MSG = MergeResult(DataPCB.VISION_NG, DataPCB.PRE_BIN_CODE, DataPCB.CUR_BIN_CHAR);
                DataPCB.RECEVIE_CUR_BIN_MSG = MES.FormatS021.CUR_BIN_CODE;

                DataPCB.WORK_OUT_RESULT = MES.FormatS021.WORK_OUT_RESULT;
                DataPCB.WORK_OUT_MSG = MES.FormatS021.WORK_OUT_MSG;


                DataPCB.TRAN_TIME = DateTime.Now.ToString("yyyyMMddHHmmss");
                logger.UpdateLogMes(DataPCB);
                addLog("MES CHECK PCB WORKOUT OK");
                UpdateUIMES("MES CHECK PCB WORKOUT OK", Brushes.LightGreen);

                // SEND PLC MES OK
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 620, true);
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 621, false);
                addLog("Write Bit Workout MES OK M620 = ON");
                addLog("Write Bit Workout MES OK M621 = OFF");

                UpdateOK_NG(1, 0);

                return;

            }
        }

        public void SendLaser()
        {

            int[] arrBlock = BinCodeNG(DataPCB.PRE_BIN_CODE);

            string switchprg = UiManager.Instance.laserCOM.SendSwitchPrg(pattern.PrgLaser);

            if (switchprg != "NG")
            {
                string blockon = UiManager.Instance.laserCOM.SendBlockOn(pattern.PrgLaser, arrBlock);

                if (blockon != "NG")
                {
                    // SEND PLC LASER OK
                    UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 615, true);
                    UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 616, false);
                    addLog("Write Bit Laser OK M615 = ON");
                    addLog("Write Bit Laser NG M616 = OFF");
                    return;
                }
            }

            string message = "- Check MES connection again / MES not respond. :\r\n" +
                                "  + Verify IP configuration\r\n" +
                                "  + Verify network connectivity\r\n" +
                                "  + Verify Laser READY status\r\n" +
                                "  + Verify Laser Command Code\r\n" +
                                "  + Verify index MES and Vision\r\n" +
                                "- Kiểm tra lại kết nối MES / MES không phản hồi:\r\n" +
                                "  + Kiểm tra lại setting IP\r\n" +
                                "  + Kiểm tra lại đường truyền\r\n" +
                                "  + Kiểm tra lại trạng thái READY của Laser\r\n" +
                                "  + Kiểm tra lại chuỗi gửi Laser\r\n" +
                                "  + Kiểm tra lại giá trị index NG của MES và Vision\r\n";

            AddErrorMES($"Error: Laser COM Response error!", message);

            // SEND PLC LASER NG
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 615, false);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 616, true);
            addLog("Write Bit Laser OK M615 = OFF");
            addLog("Write Bit Laser NG M616 = ON");

        }
        public string MergeResult(int[] vision, string bincode, string replaceChar)
        {
            if (string.IsNullOrEmpty(bincode) || vision == null || string.IsNullOrEmpty(replaceChar))
                return bincode;

            char[] arr = bincode.ToCharArray();
            char newChar = replaceChar[0];

            foreach (int pos in vision)
            {
                int index = pos - 1;

                if (index >= 0 && index < arr.Length)
                {
                    if (arr[index] == '0')
                    {
                        arr[index] = newChar;
                    }
                }
                else
                {
                    string message = $"Vision NG position {pos} vượt giới hạn chuỗi (1 → {arr.Length})";
                    AddErrorMES($"Error: Array Vision NG Error", message);
                }
            }

            return new string(arr);
        }
        public int[] BinCodeNG(string bincode)
        {
            List<int> lts = new List<int>();

            for (int i = 0; i < bincode.Length; i++)
            {
                if (bincode[i] != '0')
                {
                    lts.Add(i + 1);   // Lưu vị trí index
                }
            }
            return lts.ToArray();

        }


        private void InitBothColor()
        {
            var mesColor = ((SolidColorBrush)MES_COLOR).Color;
            var visionColor = ((SolidColorBrush)VISION_COLOR).Color;

            var brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };

            brush.GradientStops.Add(new GradientStop(mesColor, 0.0));
            brush.GradientStops.Add(new GradientStop(visionColor, 0.9));

            brush.Freeze(); // tối ưu render

            BOTH_COLOR = brush;
        }
        private void UpdateUIQR(string QR,bool Result)
        {
            Dispatcher.Invoke(() =>
            {
                if (Result && !string.IsNullOrEmpty(QR))
                {
                    this.lbQRPcb.Content = QR;
                    this.BdQR.Background = Brushes.LightGreen;
                }
                else if (!Result && !string.IsNullOrEmpty(QR))
                {
                    this.lbQRPcb.Content = QR;
                    this.BdQR.Background = Brushes.OrangeRed;
                }
                else if(!Result && string.IsNullOrEmpty(QR))
                {
                    this.lbQRPcb.Content = QR;
                    this.BdQR.Background = Brushes.White;
                }
                else
                {
                    this.lbQRPcb.Content = QR;
                    this.BdQR.Background = Brushes.White;
                }    
                
            });
        }
        private void UpdateUIMES(string QR, Brush color)
        {
            Dispatcher.Invoke(() =>
            {
               
                this.lbMES.Content = QR;
                this.lbMES.Background = color;
            });
        }
        private void UpdateUIMESRESULT(int[] list)
        {
            var set = new HashSet<int>(list);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var child in gridPos.Children)
                {
                    if (child is Label lbl && lbl.Tag is int number)
                    {
                        if (set.Contains(number))
                        {
                            if (lbl.Background == VISION_COLOR)
                            {
                                lbl.Background = BOTH_COLOR;
                                lbl.Foreground = Brushes.White;
                            }
                            else
                            {
                                lbl.Background = MES_COLOR;
                                lbl.Foreground = Brushes.White;
                            }
                        }
                    }
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
        private void UpdateUIVISIONRESULT(int[] list)
        {
            var set = new HashSet<int>(list);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var child in gridPos.Children)
                {
                    if (child is Label lbl && lbl.Tag is int number)
                    {
                        if (set.Contains(number))
                        {
                            if (lbl.Background == MES_COLOR)
                            {
                                lbl.Background = BOTH_COLOR;
                                lbl.Foreground = Brushes.White;
                            }
                            else
                            {
                                lbl.Background = VISION_COLOR;
                                lbl.Foreground = Brushes.White;
                            }
                        }
                    }
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
        private void UpdateUICLRRESULT()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var child in gridPos.Children)
                {
                    if (child is Label lbl)
                    {
                        lbl.Background = Brushes.LightBlue;
                        lbl.Foreground = Brushes.Black;
                    }
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void generateCells(int rowCnt, int colCnt, int pattern, bool Use2Matrix)
        {
            //lstButtonPos = new List<Button>();
            gridPos.Children.Clear();
            gridPos.RowDefinitions.Clear();
            gridPos.ColumnDefinitions.Clear();

            // Rows
            for (int r = 0; r < rowCnt; r++)
            {
                gridPos.RowDefinitions.Add(
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                );
            }

            int matrixCols = Use2Matrix ? colCnt / 2 : colCnt;
            int gapCols = Use2Matrix ? 1 : 0;
            int totalCols = Use2Matrix ? matrixCols * 2 + gapCols : colCnt;

            // Columns (có GAP)
            for (int c = 0; c < totalCols; c++)
            {
                if (Use2Matrix && c == matrixCols)
                {
                    // Cột trống giữa 2 matrix
                    gridPos.ColumnDefinitions.Add(
                        new ColumnDefinition { Width = new GridLength(5) } // px
                    );
                }
                else
                {
                    gridPos.ColumnDefinitions.Add(
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    );
                }
            }

            int matrixSize = rowCnt * matrixCols;

            // Create cells
            for (int id = 1; id <= rowCnt * colCnt; id++)
            {
                int r = GetCellRow(id, pattern, rowCnt, colCnt, Use2Matrix);
                int c = GetCellColumn(id, pattern, rowCnt, colCnt, Use2Matrix);

                var cell = createCell(id);
                gridPos.Children.Add(cell);
                Grid.SetRow(cell, r);
                Grid.SetColumn(cell, c);
            }
        }

        private int GetCellRow(int cellId, int pattern, int row, int column, bool use2Matrix)
        {
            if (!use2Matrix)
                return GetCellRowCaculator(cellId, pattern, row, column);

            int matrixCols = column / 2;
            int matrixSize = row * matrixCols;

            int localCellId = (cellId - 1) % matrixSize + 1;

            return GetCellRowCaculator(localCellId, pattern, row, matrixCols);
        }
        private int GetCellColumn(int cellId, int pattern, int row, int column, bool use2Matrix)
        {
            if (!use2Matrix)
                return GetCellColumnCaculator(cellId, pattern, row, column);

            int matrixCols = column / 2;
            int matrixSize = row * matrixCols;
            int gapCols = 1;

            int matrixIndex = (cellId - 1) / matrixSize; // 0 hoặc 1
            int localCellId = (cellId - 1) % matrixSize + 1;

            int col = GetCellColumnCaculator(localCellId, pattern, row, matrixCols);

            return col + matrixIndex * (matrixCols + gapCols);
        }
        private int GetCellRowCaculator(int cellId, int pattern, int row, int column)
        {
            int xSize = column;
            int ySize = row;
            // new
            if (pattern == 1 || pattern == 2)
            {
                return ySize - 1 - (cellId - 1) / xSize;
            }
            else if (pattern == 3 || pattern == 4)
            {
                return (cellId - 1) / xSize;
            }
            else if (pattern == 5)
            {
                if (((cellId - 1) / ySize) % 2 == 0)
                {
                    return ySize - 1 - ((cellId - 1) % ySize);
                }
                else
                {
                    return (cellId - 1) % ySize;
                }
            }
            else if (pattern == 6)
            {
                if (((cellId - 1) / ySize) % 2 != 0)
                {
                    return ySize - 1 - ((cellId - 1) % ySize);
                }
                else
                {
                    return (cellId - 1) % ySize;
                }
            }
            else if (pattern == 7)
            {
                if ((column - 1) % 2 != 0)
                {
                    if ((xSize - 1 - (cellId - 1) / ySize) % 2 != 0)
                    {
                        return (cellId - 1) % ySize;
                    }
                    else
                    {
                        return ySize - 1 - ((cellId - 1) % ySize);
                    }
                }
                else
                {
                    if ((xSize - 1 - (cellId - 1) / ySize) % 2 == 0)
                    {
                        return (cellId - 1) % ySize;
                    }
                    else
                    {
                        return ySize - 1 - ((cellId - 1) % ySize);
                    }
                }
            }
            else if (pattern == 8)
            {
                if ((column - 1) % 2 != 0)
                {
                    if ((xSize - 1 - (cellId - 1) / ySize) % 2 == 0)
                    {
                        return (cellId - 1) % ySize;
                    }
                    else
                    {
                        return ySize - 1 - ((cellId - 1) % ySize);
                    }
                }
                else
                {
                    if ((xSize - 1 - (cellId - 1) / ySize) % 2 != 0)
                    {
                        return (cellId - 1) % ySize;
                    }
                    else
                    {
                        return ySize - 1 - ((cellId - 1) % ySize);
                    }
                }
            }
            else if (pattern == 9 || pattern == 10)
            {
                return ySize - 1 - (cellId - 1) / xSize;
            }
            else if (pattern == 11 || pattern == 12)
            {
                return (cellId - 1) / xSize;
            }
            else if (pattern == 13 || pattern == 15)
            {
                return ySize - 1 - ((cellId - 1) % ySize);
            }
            else if (pattern == 14 || pattern == 16)
            {
                return (cellId - 1) % ySize;
            }
            return 0;
        }
        private int GetCellColumnCaculator(int cellId, int pattern, int row, int column)
        {
            int xSize = column;
            int ySize = row;

            if (pattern == 1)
            {
                if ((row - 1) % 2 != 0)
                {
                    if ((ySize - 1 - (cellId - 1) / xSize) % 2 != 0)
                    {
                        return (cellId - 1) % xSize;
                    }
                    else
                    {
                        return xSize - 1 - ((cellId - 1) % xSize);
                    }
                }
                else
                {
                    if ((ySize - 1 - (cellId - 1) / xSize) % 2 == 0)
                    {
                        return (cellId - 1) % xSize;
                    }
                    else
                    {
                        return xSize - 1 - ((cellId - 1) % xSize);
                    }
                }
            }
            else if (pattern == 2)
            {
                if ((row - 1) % 2 != 0)
                {
                    if ((ySize - 1 - (cellId - 1) / xSize) % 2 == 0)
                    {
                        return (cellId - 1) % xSize;
                    }
                    else
                    {
                        return xSize - 1 - ((cellId - 1) % xSize);
                    }
                }
                else
                {
                    if ((ySize - 1 - (cellId - 1) / xSize) % 2 != 0)
                    {
                        return (cellId - 1) % xSize;
                    }
                    else
                    {
                        return xSize - 1 - ((cellId - 1) % xSize);
                    }
                }
            }
            else if (pattern == 3)
            {
                if (((cellId - 1) / xSize) % 2 == 0)
                {
                    return (cellId - 1) % xSize;
                }
                else
                {
                    return xSize - 1 - ((cellId - 1) % xSize);
                }
            }
            else if (pattern == 4)
            {
                if (((cellId - 1) / xSize) % 2 != 0)
                {
                    return (cellId - 1) % xSize;
                }
                else
                {
                    return xSize - 1 - ((cellId - 1) % xSize);
                }
            }
            else if (pattern == 5 || pattern == 6)
            {
                return (cellId - 1) / ySize;
            }
            else if (pattern == 7 || pattern == 8)
            {
                return xSize - 1 - (cellId - 1) / ySize;
            }
            else if (pattern == 9 || pattern == 12)
            {
                return (cellId - 1) % xSize;
            }
            else if (pattern == 10 || pattern == 11)
            {
                return xSize - 1 - ((cellId - 1) % xSize);
            }
            else if (pattern == 13 || pattern == 14)
            {
                return (cellId - 1) / ySize;
            }
            else if (pattern == 15 || pattern == 16)
            {
                return xSize - 1 - (cellId - 1) / ySize;
            }
            return 0;
        }
        private Label createCell(int number)
        {
            var cell = new Label();
            cell.Content = createCellContent(String.Format("{0}", number));
            //cell.Content = number;
            //cell.Name = String.Format("lblCell{0:00}", number);
            cell.Tag = number;
            cell.HorizontalContentAlignment = HorizontalAlignment.Center;
            cell.VerticalContentAlignment = VerticalAlignment.Center;
            cell.Margin = new Thickness(1, 1, 1, 1);
            cell.FontWeight = FontWeights.Bold;
            cell.Background = Brushes.LightBlue;
            cell.Foreground = Brushes.Black;
            return cell;
        }
        private object createCellContent(String qr)
        {
            var cellText = new TextBlock();
            cellText.TextWrapping = TextWrapping.Wrap;
            cellText.Text = String.Format("{0}", qr);
            cellText.FontSize = 10;
            return cellText;
        }

        private void PgMain_Unloaded(object sender, RoutedEventArgs e)
        {
            this.isUpdate = false;
            this.RemoveDevicePLC();
            this.RemoveNotifyMES();
            this.isRunning = false;





        }
        private void PgMain_Loaded(object sender, RoutedEventArgs e)
        {
            // Event Write Log - Addlog()
            isWriteLog = UiManager.managerSetting.assignSystem.WriteLog;
            this.CbLog.IsChecked = isWriteLog;

            this.isUpdate = true;
            this.isRunning = true;
            this.StartTimer();
            this.LoadLotData();
            this.RegisterDevicePLC();

            this.RegisterNotifyMES();
            this.CheckConnectionMES(UiManager.Instance.MES.isAccept);

            this.generateCells(pattern.xRow, pattern.yColumn, pattern.CurrentPatern, pattern.Use2Matrix);

            this.ThreadUpDatePLC();

            // Start CH1;
            this.AutoRunThread = new Thread(new ThreadStart(Running));
            this.AutoRunThread.IsBackground = true;
            this.AutoRunThread.Start();

            this.AutoRunThread = new Thread(new ThreadStart(VisionRunning));
            this.AutoRunThread.IsBackground = true;
            this.AutoRunThread.Start();



            this.UpdateUIMES($"WAIT.....", Brushes.Yellow);

        }
        private void ThreadUpDatePLC()
        {
            new Thread(new ThreadStart(this.ReadPLC))
            {
                IsBackground = true
            }.Start();
        }
        private void ReadPLC()
        {
            while (this.isUpdate)
            {
                bool flag = UiManager.Instance.PLC.device.isOpen();
                if (flag)
                {
                    UiManager.Instance.PLC.device.ReadMultiWord(DeviceCode.D, 0, 300, out this.D_ListShortDevicePLC_0_299);
                    UiManager.Instance.PLC.device.ReadMultiBits(DeviceCode.M, 0, 100, out this.M_ListBitPLC_0_99);
                    UiManager.Instance.PLC.device.ReadMultiBits(DeviceCode.M, 500, 50, out this.M_ListBitPLC_500_549);

                    this.UpdateError();
                }

                Thread.Sleep(20);
            }
        }
       
        private void UpdateError()
        {
            Application.Current.Dispatcher.Invoke(delegate ()
            {
                bool flag = UiManager.Instance.PLC.device.isOpen();
                if (flag)
                {
                    bool flag2 = this.D_ListShortDevicePLC_0_299.Count >= 1;
                    if (flag2)
                    {
                        this.AddError(this.D_ListShortDevicePLC_0_299[200]);
                        this.AddError(this.D_ListShortDevicePLC_0_299[201]);
                        this.AddError(this.D_ListShortDevicePLC_0_299[202]);
                        this.AddError(this.D_ListShortDevicePLC_0_299[203]);
                        this.AddError(this.D_ListShortDevicePLC_0_299[204]);
                        this.AddError(this.D_ListShortDevicePLC_0_299[205]);
                        this.AddError(this.D_ListShortDevicePLC_0_299[206]);
                        this.AddError(this.D_ListShortDevicePLC_0_299[207]);
                        this.AddError(this.D_ListShortDevicePLC_0_299[208]);
                        this.AddError(this.D_ListShortDevicePLC_0_299[209]);

                        bool flag3 = this.D_ListShortDevicePLC_0_299[200] == 0 && !this.hasClearedError;
                        if (flag3)
                        {
                            this.ClearError();
                            this.hasClearedError = true;
                        }
                        else
                        {
                            bool flag4 = this.D_ListShortDevicePLC_0_299[200] != 0;
                            if (flag4)
                            {
                                this.hasClearedError = false;
                            }
                        }
                        bool flag5 = this.M_ListBitPLC_0_99[7];
                        if (flag5)
                        {
                            this.ClearError();
                        }
                    }
                }
            });
        }
        private void AddDevicePLC()
        {
            //UiManager.Instance.PLC.AddBitAddress("M", 100);
        }
        private void BtStart_Click(object sender, RoutedEventArgs e)
        {
            string QR = "B0226046110039500172361";
            this.UpdateUIQR(QR, true);

            DataPCB = new DataPCB();
            DataPCB.BARCODE_PCB = QR;
            CheckMESWorkin();
        }
        private void BtStop_Click(object sender, RoutedEventArgs e)
        {
            int[] arr = { 1, 3, 4, 9, 21, 19, 42 };

            DataPCB.VISION_NG = arr;
            UpdateUIVISIONRESULT(DataPCB.VISION_NG);
            CheckMESWorkout();
        }
        private void BtReset_Click(object sender, RoutedEventArgs e)
        {
            this.addLog("Click Reset Machine");
            this.ClearError();
          
        }
        private void BtHome_Click(object sender, RoutedEventArgs e)
        {

        }

        #region NotifyMES
        private void LoadUI_MES()
        {
            var arr = UiManager.appSetting.MESSettings.Ip.Split('.');
            if (arr.Length == 4)
            {
                this.txtLocalIp1.Text = arr[0];
                this.txtLocalIp2.Text = arr[1];
                this.txtLocalIp3.Text = arr[2];
                this.txtLocalIp4.Text = arr[3];
            }
            this.chkMcsListen.IsChecked = true;
            this.txtMcsLocalPort.Text = UiManager.appSetting.MESSettings.Port.ToString();
        }
        private void RemoveNotifyMES()
        {
            
            SystemsManager.Instance.NotifyEvenMES.Detach(this);


        }
        private void RegisterNotifyMES()
        {
            this.LoadUI_MES();
           
            SystemsManager.Instance.NotifyEvenMES.Attach(this);
           
        }
        public void CheckConnectionMES(bool connected)
        {
            if(UiManager.appSetting.RUN.MESOnline)
            {
                if (connected)
                {
                    Dispatcher.Invoke(() =>
                    {
                        this.lbMesConnect.Content = "Connect";
                        this.lbMesConnect.Background = Brushes.Green;
                        this.chkMcsListen.IsChecked = false;
                        this.chkMcsAccepted.IsChecked = true;
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        this.lbMesConnect.Content = "Disconnect";
                        this.lbMesConnect.Background = Brushes.Red;
                        this.chkMcsListen.IsChecked = true;
                        this.chkMcsAccepted.IsChecked = false;

                        this.lblMcsStatus.Content = $"Waiting Connect .....";
                        this.lblMcsStatus.Background = Brushes.Orange;

                    });
                }
            }    
            else
            {
                if (connected)
                {
                    Dispatcher.Invoke(() =>
                    {
                        this.lbMesConnect.Content = "Connect/OFF";
                        this.lbMesConnect.Background = Brushes.CadetBlue;
                        this.chkMcsListen.IsChecked = false;
                        this.chkMcsAccepted.IsChecked = true;
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        this.lbMesConnect.Content = "Disconnect/OFF";
                        this.lbMesConnect.Background = Brushes.CadetBlue;
                        this.chkMcsListen.IsChecked = true;
                        this.chkMcsAccepted.IsChecked = false;

                        this.lblMcsStatus.Content = $"Waiting Connect .....";
                        this.lblMcsStatus.Background = Brushes.Orange;

                    });
                }
            }    
            
        }
        public void FollowingDataMES(string MESResult)
        {
            addLog($"Notify MESResult : {MESResult}");
        }
        public void GetInformationFromClientConnectMES(string clientIP, int clientPort)
        {
            Dispatcher.Invoke(() =>
            {
                this.lblMcsStatus.Content = $"Client connect {clientIP} {clientPort}";
                this.lblMcsStatus.Background = Brushes.LightGreen;
            });
           
          
        }
        public void UpdateNotifyToUIMES(string Notify)
        {
            addLog($"NotifyMES : {Notify}");
        }
        #endregion

        #region NotifyChangeBit
        private void RemoveDevicePLC()
        {
            //UiManager.Instance.PLC.RemoveBitAddress("M", 100);
            SystemsManager.Instance.NotifyPLCBits.Detach(this);

        }
        private void RegisterDevicePLC()
        {
            SystemsManager.Instance.NotifyPLCBits.Attach(this);
            this.AddDevicePLC();
        }
        public void NotifyChangeBits(string key, bool status)
        {

            //if (key == "M" + 100 && status)
            //{
            //    a++;
            //    UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 100, false);
            //    MessageBox.Show($"M 100 online {a}");
            //}
            //if (key == "M" + 3010 && status)
            //{
            //    MessageBox.Show("M 3010 online");
            //}
            //if (key == "M" + 5010 && status)
            //{
            //    MessageBox.Show("M 5010 online");
            //}
        }
        #endregion

        #region Update UI 
        private void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Cập nhật mỗi giây
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateUI_TimeTick();


        }
        private void UpdateUI_TimeTick()
        {
            /// Update Ui PLC
            if (UiManager.Instance.PLC.device.isOpen())
            {
                this.lbPlcConnect.Content = "Connect";
                this.lbPlcConnect.Background = Brushes.Green;
            }
            else
            {
                this.lbPlcConnect.Content = "Disconnect";
                this.lbPlcConnect.Background = Brushes.Red;
            }

            if (UiManager.Instance.scannerTCP.IsConnected)
            {
                this.lbScannerConnect.Content = "Connect";
                this.lbScannerConnect.Background = Brushes.Green;
            }
            else
            {
                this.lbScannerConnect.Content = "Disconnect";
                this.lbScannerConnect.Background = Brushes.Red;
            }
            if (UiManager.Instance.laserCOM.isOpen())
            {
                this.lbLaserConnect.Content = "Connect";
                this.lbLaserConnect.Background = Brushes.Green;
            }
            else
            {
                this.lbLaserConnect.Content = "Disconnect";
                this.lbLaserConnect.Background = Brushes.Red;
            }


        }
        #endregion



        #region Update LOTDATA


        private Mes00Check GetDataLotin()
        {
            Mes00Check DataLotin = new Mes00Check();

            DataLotin.FormatS000.AUTHORITY = UiManager.UserNameLoginMesOP_ME;
            DataLotin.FormatS000.ID = UiManager.CodeUserLoginMesOP_ME;
            DataLotin.FormatS000.RECIPE = UiManager.appSetting.MESSettings.Repice.ToString();

            DataLotin.EquipmentId = UiManager.appSetting.MESSettings.EquimentID.ToString();
            DataLotin.DIV = "CONFIG";
            DataLotin.CheckSum = "CONFIG";

            return DataLotin; 
        }
        private void BtLotOut_Click(object sender, RoutedEventArgs e)
        {
            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo("You Want to LotOut ?")) return;
            this.LotOut();
        }
        private async void BtLotIn_Click(object sender, RoutedEventArgs e)
        {
            /// Create Wnd Login
            var setting = UiManager.managerSetting.assignSystem;
            if (setting.LoginApp == true && setting.LoginMes == true)
            {
                var Result = ManagerLogin.DoCheck();
                if (Result == 0)
                {
                    return;
                }
            }
            UserManager.isLogOn = 0;


            //
            WndLotin wndLot = new WndLotin();
            var newLot = wndLot.DoSettings(Window.GetWindow(this), this.lotInData);
            if (newLot == null)
            {
                return;
            }
            else
            {
                if(UiManager.appSetting.RUN.MESOnline)
                {
                    

                    var DataLotin = GetDataLotin();
                    string DeviceID = newLot.DeviceId;

                    var MESREADY = await UiManager.Instance.MES.SendReady(DataLotin);


                    if(!MESREADY)
                    {
                        string message = "- Check MES connection again:\r\n" +
                                         "  + Verify IP configuration\r\n" +
                                         "  + Verify network connectivity\r\n" +
                                         "- Kiểm tra lại kết nối MES:\r\n" +
                                         "  + Kiểm tra lại setting IP\r\n" +
                                         "  + Kiểm tra lại đường truyền\r\n";

                        UpdateUIMES("MES CHECK READY IS NG", Brushes.Red);

                        AddErrorMES("MES CHECK READY IS NG" , message);
                        return;
                    }

                    var MES = await UiManager.Instance.MES.SendConfig(DataLotin, DeviceID, DataLotin.FormatS000.RECIPE);

                    if (MES == null)
                    {
                        string message = "- Check MES connection again / MES not respond. :\r\n" +
                                        "  + Verify IP configuration\r\n" +
                                        "  + Verify network connectivity\r\n" +
                                        "- Kiểm tra lại kết nối MES / MES không phản hồi:\r\n" +
                                        "  + Kiểm tra lại setting IP\r\n" +
                                        "  + Kiểm tra lại đường truyền\r\n";
                        UpdateUIMES("MES NOT RESPOND", Brushes.Red);

                        AddErrorMES("MES NOT RESPOND", message);

                        return;
                    }    

                    if(MES.MES_Result == "NG")
                    {
                        UpdateUIMES("MES CHECK CONFIG LOT IN NG", Brushes.Red);

                        AddErrorMES("MES CHECK CONFIG LOT IN NG", MES.MES_MSG);
                        return;
                    }   
                    else
                    {
                        UpdateUIMES("MES CHECK CONFIG LOT IN OK", Brushes.LightGreen);

                        lotInData = newLot;
                        UpdateInformationLOTIN(lotInData);
                    }    
                    
                }   
                else
                {
                  
                    lotInData = newLot;
                    UpdateInformationLOTIN(lotInData);
                }    
               
            }
        }
        private void LotOut()
        {
            lotInData = UiManager.appSetting.LotinData;

            lotInData.OKCount = 0;
            lotInData.NGCount = 0;
            lotInData.TotalCount = 0;
            lotInData.Yield = 0;
            lotInData.LotQty = 0;
            lotInData.DeviceId = "";
            lotInData.LotId = "";

            this.lbLotID.Content = lotInData.LotId?.ToString();
            this.lbConfig.Content = lotInData.DeviceId?.ToString();
            this.lbQty.Content = lotInData.LotQty.ToString();
            this.lbOkCount.Content = lotInData.OKCount.ToString();
            this.lbNgCount.Content = lotInData.NGCount.ToString();
            this.lbTotalCount.Content = lotInData.TotalCount.ToString();
            this.lbYield.Content = lotInData.Yield.ToString();
            UiManager.appSetting.LotinData = lotInData;
            UiManager.SaveAppSetting();
        }
        private void UpdateOK_NG(int OK, int NG)
        {
            Dispatcher.Invoke(() =>
            {

                lotInData.OKCount = lotInData.OKCount + OK;
                lotInData.NGCount = lotInData.NGCount + NG;
                lotInData.TotalCount = lotInData.TotalCount + OK + NG;
                lotInData.Yield = Math.Round((double)lotInData.OKCount / lotInData.TotalCount * 100, 2);

                this.lbQty.Content = lotInData.LotQty.ToString();
                this.lbOkCount.Content = lotInData.OKCount.ToString();
                this.lbNgCount.Content = lotInData.NGCount.ToString();
                this.lbTotalCount.Content = lotInData.TotalCount.ToString();
                this.lbYield.Content = lotInData.Yield.ToString();
            });
        }
        private void UpdateInformationLOTIN(LotInData lotin)
        {

            lotin.OKCount = 0;
            lotin.NGCount = 0;
            lotin.TotalCount = 0;
            lotin.Yield = 0;

            this.lbLotID.Content = lotin.LotId?.ToString();
            this.lbConfig.Content = lotin.DeviceId?.ToString();

            this.lbQty.Content = lotin.LotQty.ToString();
            this.lbOkCount.Content = lotin.OKCount.ToString();
            this.lbNgCount.Content = lotin.NGCount.ToString();
            this.lbTotalCount.Content = lotin.TotalCount.ToString();
            this.lbYield.Content = lotin.Yield.ToString();
            UiManager.appSetting.LotinData = lotin;
            UiManager.SaveAppSetting();
        }
        private void LoadLotData()
        {
            lotInData = UiManager.appSetting.LotinData;

            this.lbLotID.Content = lotInData.LotId?.ToString();
            this.lbConfig.Content = lotInData.DeviceId?.ToString();
            this.lbRecipe.Content = UiManager.appSetting.MESSettings.Repice.ToString();
            this.lbEquipmentId.Content = UiManager.appSetting.MESSettings.EquimentID.ToString();

            this.lbQty.Content = lotInData.LotQty.ToString();
            this.lbOkCount.Content = lotInData.OKCount.ToString();
            this.lbNgCount.Content = lotInData.NGCount.ToString();
            this.lbTotalCount.Content = lotInData.TotalCount.ToString();
            this.lbYield.Content = lotInData.Yield.ToString();
        }
        #endregion


        #region ALARM 
        private void Playsound()
        {
            try
            {
                string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "03.Sound", "Minion.m4a");
                bool flag = File.Exists(text);
                if (flag)
                {
                    this.mediaPlayer.Open(new Uri(text));
                    this.mediaPlayer.Play();
                    this.isLooping = true;
                }
            }
            catch (Exception)
            {
            }
        }
        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            bool flag = this.isLooping;
            if (flag)
            {
                this.mediaPlayer.Position = TimeSpan.Zero;
                this.mediaPlayer.Play();
            }
        }
        private void StopSound()
        {
            this.mediaPlayer.Stop();
        }


        #region Add Error MES
        private void AddErrorMES(string Messenger, string Solution)
        {
            Dispatcher.Invoke(() =>
            {
                WndAlarmMES ShowAlarm = new WndAlarmMES();
                ShowAlarm.Messenger(Messenger, Solution);
                addLog($"Allarm{Messenger} {Solution}");
            });
            

        }
        #endregion
        #region ALARM LOG


        private List<int> errorCodes;
        List<DateTime> timeerror = new List<DateTime>();
        private void InitializeErrorCodes()
        {
            errorCodes = new List<int>();
            timeerror = new List<DateTime>();

        }
        private void AddError(short errorCode)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (errorCode == 0)
                {
                    return;
                }

                if (errorCodes.Contains(errorCode))
                {
                    return;
                }

                else if (errorCodes.Count >= 31)
                {
                    errorCodes.Add(1);
                    return;
                }

                //Thêm lỗi vào SQL
                if (errorCode <= 100)
                {
                    string mes = AlarmInfo.getMessage(errorCode);
                    string sol = AlarmList.GetSolution(errorCode);
                    var alarm = new AlarmInfo(errorCode, mes, sol);
                    DbWrite.createAlarm(alarm);
                }
                else
                {
                    string mes = AlarmList.GetMes(errorCode);
                    string sol = AlarmList.GetSolution(errorCode);
                    var alarm = new AlarmInfo(errorCode, mes, sol);
                    DbWrite.createAlarm(alarm);
                }
                errorCodes.Add(errorCode);
                timeerror.Add(DateTime.Now);

                //GirdInfor.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Star);
                //GirdInfor.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);

                for (int i = 0; i < errorCodes.Count; i++)

                {
                    int code = errorCodes[i];
                    switch (i)
                    {
                        case 0: this.DisplayAlarm(1, code); break;
                        case 1: this.DisplayAlarm(2, code); break;
                        case 2: this.DisplayAlarm(3, code); break;
                        case 3: this.DisplayAlarm(4, code); break;
                        case 4: this.DisplayAlarm(5, code); break;
                        case 5: this.DisplayAlarm(6, code); break;
                        case 6: this.DisplayAlarm(7, code); break;
                        case 7: this.DisplayAlarm(8, code); break;
                        case 8: this.DisplayAlarm(9, code); break;
                        case 9: this.DisplayAlarm(10, code); break;
                        case 10: this.DisplayAlarm(11, code); break;
                        case 11: this.DisplayAlarm(12, code); break;
                        case 12: this.DisplayAlarm(13, code); break;
                        case 13: this.DisplayAlarm(14, code); break;
                        case 14: this.DisplayAlarm(15, code); break;
                        case 15: this.DisplayAlarm(16, code); break;
                        case 16: this.DisplayAlarm(17, code); break;
                        case 17: this.DisplayAlarm(18, code); break;
                        case 18: this.DisplayAlarm(19, code); break;
                        case 19: this.DisplayAlarm(20, code); break;
                        case 20: this.DisplayAlarm(21, code); break;
                        case 21: this.DisplayAlarm(22, code); break;
                        case 22: this.DisplayAlarm(23, code); break;
                        case 23: this.DisplayAlarm(24, code); break;
                        case 24: this.DisplayAlarm(25, code); break;
                        case 25: this.DisplayAlarm(26, code); break;
                        case 26: this.DisplayAlarm(27, code); break;
                        case 27: this.DisplayAlarm(28, code); break;
                        case 28: this.DisplayAlarm(29, code); break;
                        case 29: this.DisplayAlarm(30, code); break;

                        default:
                            break;
                    }
                }
                if (!isAlarmWindowOpen)
                {
                    this.ShowAlarm();
                }

                this.Number_Alarm();
                //this.Playsound();
            });


        }
        private void Number_Alarm()
        {
            int NumberAlarm = errorCodes.Count;
            this.CbShow.Content = NumberAlarm > 0 ? $"Errors : {NumberAlarm}" : "Not Show";
        }
        private void AlarmCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            isAlarmWindowOpen = true;
        }
        private void AlarmCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            isAlarmWindowOpen = false;
        }
        private void WriteLog_Checked(object sender, RoutedEventArgs e)
        {
            isWriteLog = true;
           
        }
        private void WriteLog_Unchecked(object sender, RoutedEventArgs e)
        {
            isWriteLog = false;
        }
        private bool isAlarmWindowOpen = false;
        private bool isWriteLog = false;
        private void ShowAlarm()
        {
            WndAlarm wndAlarm = new WndAlarm();
            wndAlarm.UpdateErrorList(errorCodes);
            wndAlarm.UpdateTimeList(timeerror);
            if (!isAlarmWindowOpen)
            {
                wndAlarm.Show();
            }
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (errorCodes.Count >= 1)
            {
                WndAlarm wndAlarm = new WndAlarm();
                wndAlarm.UpdateErrorList(errorCodes);
                wndAlarm.UpdateTimeList(timeerror);
                wndAlarm.Show();
            }

        }
        public void ClearError()
        {
            timeerror.Clear();
            errorCodes.Clear();
            Dispatcher.Invoke(new Action(() =>
            {
                for (int i = 1; i <= 30; i++)
                {
                    var label = (Label)FindName("lbMes" + i);
                    label.Content = "";
                    label.Background = Brushes.Black;
                }
            }));

            WndAlarm wndAlarm = new WndAlarm();
            wndAlarm.UpdateErrorList(errorCodes);
            wndAlarm.UpdateTimeList(timeerror);
            this.Number_Alarm();

            GirdInfor.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            GirdInfor.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Star);

            if (WndAlarmMES.Instance != null)
            {
                WndAlarmMES.Instance.CloseAlarm();
            }
            this.StopSound();

        }
        private void DisplayAlarm(int index, int code)
        {
            try
            {
                if (code <= 100)
                {
                    Label label = (Label)FindName($"lbMes{index}");
                    if (label != null)
                    {
                        string mes = AlarmInfo.getMessage(code);
                        this.Dispatcher.Invoke(() =>
                        {
                            DateTime currentTime = DateTime.Now;
                            string currentTimeString = currentTime.ToString();
                            string newContent = currentTime.ToString() + " : " + mes;

                            label.Content = newContent;
                            label.Background = Brushes.Red;
                            //label.FontWeight = FontWeights.ExtraBold;
                            //label.Foreground = Brushes.Black;
                        });
                    }
                }
                else
                {
                    Label label = (Label)FindName($"lbMes{index}");
                    if (label != null)
                    {
                        string mes = AlarmList.GetMes(code);
                        this.Dispatcher.Invoke(() =>
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss");
                            string currentTimeString = currentTime.ToString();
                            string newContent = currentTime.ToString() + " : " + mes;

                            label.Content = newContent;
                            label.Background = Brushes.Red;
                            //label.FontWeight = FontWeights.ExtraBold;
                            //label.Foreground = Brushes.Black;
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Create($"DisplayAlarm PgMain: {ex.Message}", LogLevel.Error);
            }
        }
        #endregion
        #region AddLog
        public Boolean uiLogEnable { get; set; } = true;
        private String lastLog = "";
        private int gLogIndex;
        private bool autoScrollMode = true;
        private void addLog(String log)
        {
            try
            {
                if (log != null && !log.Equals(lastLog))
                {
                    lastLog = log;

                    if (isWriteLog)
                    {
                        logger.Create("addLog:" + log, LogLevel.Information);
                    }


                    // UI log:
                    if (true)
                    {
                        logEntry x = new logEntry()
                        {
                            logIndex = gLogIndex++,
                            logTime = DateTime.Now.ToString("HH:mm:ss.ff"),
                            logMessage = log,
                        };
                        this.Dispatcher.Invoke(() =>
                        {
                            compositeViewModel.LogEntries.Add(x);

                            // Nếu số lượng log vượt quá 1000
                            if (compositeViewModel.LogEntries.Count > 300)
                            {
                                // Giữ lại 50 dòng gần nhất
                                var recentLogs = compositeViewModel.LogEntries.Skip(compositeViewModel.LogEntries.Count - 100).ToList();
                                compositeViewModel.LogEntries.Clear();
                                foreach (var item in recentLogs)
                                    compositeViewModel.LogEntries.Add(item);
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Create("addLog error:" + ex.Message,LogLevel.Error);
            }
        }
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            try
            {
                if (e.Source.GetType().Equals(typeof(ScrollViewer)))
                {
                    ScrollViewer sv = (ScrollViewer)e.Source;

                    if (sv != null)
                    {
                        // User scroll event : set or unset autoscroll mode
                        if (e.ExtentHeightChange == 0)
                        {   // Content unchanged : user scroll event
                            if (sv.VerticalOffset == sv.ScrollableHeight)
                            {   // Scroll bar is in bottom -> Set autoscroll mode
                                autoScrollMode = true;
                            }
                            else
                            {   // Scroll bar isn't in bottom -> Unset autoscroll mode
                                autoScrollMode = false;
                            }
                        }

                        // Content scroll event : autoscroll eventually
                        if (autoScrollMode && e.ExtentHeightChange != 0)
                        {   // Content changed and autoscroll mode set -> Autoscroll
                            sv.ScrollToVerticalOffset(sv.ExtentHeight);
                        }
                    }
                }
            }
            catch
            {
            }
        }
        #endregion
        #region Show Error Machine
        #region StatusMachine
        private void AddStatus(int code)
        {
            if (code == 0)
            {
                // Nếu là số 0, không làm gì cả và thoát khỏi phương thức
                return;
            }
            string mes = StatusMachine.GetMes(code);
            Dispatcher.Invoke(() =>
            {
                this.lblStatust.Content = mes;
            });
        }
        #endregion
    }

    public class CompositeViewModel
    {
        public ObservableCollection<logEntry> LogEntries { get; set; } = new ObservableCollection<logEntry>();

        
    }
    public class logEntry : PropertyChangedBase
    {
        public int logIndex { get; set; }
        public String logTime { get; set; }
        public string logMessage { get; set; }
    }
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        private static MyLogger Logger = new MyLogger("LogEntry");
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    PropertyChangedEventHandler handler = PropertyChanged;
                    if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
                }));
            }
            catch (Exception ex)
            {
                Logger.Create(String.Format("Binding Property Of Logger Error: " + ex.Message) ,LogLevel.Error);
            }
        }
    }

    #endregion
    #endregion
}
