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
        private List<short> D_ListShortDevicePLC_0 = new List<short>();
        private List<bool> M_ListBitPLC0_500 = new List<bool>();
        private bool hasClearedError = false;


        private MediaPlayer mediaPlayer = new MediaPlayer();
        private bool isLooping = false;

        private Thread AutoRunThread;
        private object LockRun = new object();
        private bool isRunning = false;



        #region MAIN MANUAL VISION
        // --- THÔNG SỐ CẤU HÌNH ---
        public int TotalRows  = 5;
        public int TotalCols  = 10;
        public int VisionRows = 2;
        public int VisionCols  = 3;

        private int _runDirection = 0;
        public int RunDirection
        {
            get => _runDirection;
            set { _runDirection = value; ResetSystem(); }
        }

        private int _stepIndex = 0;
        private List<List<WorkItem>> InspectionPath = new List<List<WorkItem>>();

        #endregion

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
            InitializeComponent();


            // Event Page Main
            this.Loaded += PgMain_Loaded;
            this.Unloaded += PgMain_Unloaded;
            this.btStart.Click += BtStart_Click;
            this.btStop.Click += BtStop_Click;
            //this.btReset.Click += BtReset_Click;
            //this.btHome.Click += BtHome_Click;
            this.btLotIn.Click += BtLotIn_Click;
            this.btLotOut.Click += BtLotOut_Click;

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
                addLog("Write Bit MES CHECK NG (M3860) = ON");
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 3860, true);

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
                addLog("Write Bit MES CHECK NG (M3860) = ON");
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 3860, true);

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
                addLog("Write Bit MES CHECK NG (M3860) = ON");
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 3860, true);

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

                // SEND PLC MES OK
                addLog("Write Bit MES CHECK OK (M3859) = ON");
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 3859, true);


                return;

            }
        }
        public string MergeResult(List<string> list, string bin, string replaceChar)
        {
            char[] result = bin.ToCharArray();

            if (string.IsNullOrEmpty(replaceChar))
                return new string(result);

            char replace = replaceChar[0];
            if (list.Count != bin.Length)
            {
                return new string(result);
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (result[i] == '1')
                {
                    continue;
                }
                if (list[i].Equals("NG", StringComparison.OrdinalIgnoreCase))
                {
                    result[i] = replace;
                }
               
            }
            return new string(result);
        }
        

        private Mes00Check GetDataWorkout()
        {
            var CUR_BIN_MSG = MergeResult(DataPCB.RESULT_PCB, DataPCB.PRE_BIN_CODE, DataPCB.CUR_BIN_CHAR);

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
            if(string.IsNullOrEmpty(DataPCB.PRE_BIN_CODE))
            {
                AddErrorMES("DataPCB.PRE_BIN_CODE IS ERROR ", "KHÔNG NHẬN DC BINCODE TỪ MES");
            }    
            if (DataPCB.RESULT_PCB.Count  != DataPCB.PRE_BIN_CODE.Length)
            {
                string message = $"RESULT_PCB.Count = {DataPCB.RESULT_PCB.Count}\r\n" +
                                 $"PRE_BIN_CODE.Count = {DataPCB.PRE_BIN_CODE.Length}\r\n"+
                                 $"Số lượng PCB hiện tại không khớp với dữ liệu MES gửi về";
                AddErrorMES("DATA RESULT_PCB NOT MATCH PRE_BIN_CODE ", message);
                return;
            }
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
                addLog("Write Bit MES CHECK NG (M3860) = ON");
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 3860, true);


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
                addLog("Write Bit MES CHECK NG (M3860) = ON");
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 3860, true);


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

                DataPCB.SEND_CUR_BIN_MSG = MergeResult(DataPCB.RESULT_PCB, DataPCB.PRE_BIN_CODE, DataPCB.CUR_BIN_CHAR);
                DataPCB.RECEVIE_CUR_BIN_MSG = MES.FormatS021.CUR_BIN_CODE;

                DataPCB.WORK_OUT_RESULT = MES.FormatS021.WORK_OUT_RESULT;
                DataPCB.WORK_OUT_MSG = MES.FormatS021.WORK_OUT_MSG;

                DataPCB.TRAN_TIME = DateTime.Now.ToString("yyyyMMddHHmmss");
                logger.UpdateLogMes(DataPCB);

                // SEND PLC MES NG
                addLog("Write Bit MES CHECK NG (M3860) = ON");
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 3860, true);

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

                DataPCB.SEND_CUR_BIN_MSG = MergeResult(DataPCB.RESULT_PCB, DataPCB.PRE_BIN_CODE, DataPCB.CUR_BIN_CHAR);
                DataPCB.RECEVIE_CUR_BIN_MSG = MES.FormatS021.CUR_BIN_CODE;

                DataPCB.WORK_OUT_RESULT = MES.FormatS021.WORK_OUT_RESULT;
                DataPCB.WORK_OUT_MSG = MES.FormatS021.WORK_OUT_MSG;


                DataPCB.TRAN_TIME = DateTime.Now.ToString("yyyyMMddHHmmss");
                logger.UpdateLogMes(DataPCB);
                addLog("MES CHECK PCB WORKOUT OK");
                UpdateUIMES("MES CHECK PCB WORKOUT OK", Brushes.LightGreen);

                // SEND PLC MES OK
                addLog("Write Bit MES CHECK OK (M3859) = ON");
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 3859, true);

                UpdateOK_NG(1, 0);

                return;

            }
        }
        private void Running()
        {
            // x run : D3156 (DWORD)
            // Y RUN : D3204 (DWORD)

            // X : THUC TE CON HANG D3236
            // Y : THUC TE CON HANG D3237

            // X : DOC BAO NHIEU CON HANG D3259 (WORD)
            // Y : DOC BAO NHIEU CON HANG D3260 (WORD)

            // DIEM CHAY :3240 (WORD)
            // M NEXT : M3854
            // M BACK : M3855

            // M START M3857:
            // M END M3856:

            // M DOC CODE : M3850
            // M CHECK OK : M3851
            // M CHECK NG : M3852

            // M CHECK SCANNER AGAIN : M3853


            lock (LockRun)
            {
                bool M_START = false;  // M3857:
                bool M_STOP = false;   // M3856:

                bool M_START_SCANNER = false;     // M3850
                //bool M_SCANNER_CHECK_OK = false;  // M3851
                //bool M_SCANNER_CHECK_NG = false;  // M3852
                //bool M_SCANNER_CHECK_SCANNER_AGAIN = false;  // M3853
                bool M_NEXT_STEP = false; // M3854
                bool M_BACK_STEP = false; // M3855
                List<short> D_INDEX_RUNNING = new List<short>();  // D3240 (WORD)



                while (isRunning)
                {
                    // START --------------------------------------------------------
                    UiManager.Instance.PLC.device.ReadBit(DeviceCode.M,3857, out M_START);
                    if (M_START)
                    {
                        this.addLog("--- Start RunAuto --- ");
                        UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 3857, false);
                        this.addLog("Write Bit START M3857 = OFF");

                      
                        _stepIndex = 0;
                        Dispatcher.Invoke(() =>
                        {
                            foreach (var item in compositeViewModel.MasterItems) { item.IsNG = false; item.HasBeenReached = false; item.IsCurrent = false; }
                        });
                       
                        this.UpdateUI();
                        this.addLog("CLEAR ALL UI ");


                        this.UpdateUIQR("",false);
                        this.UpdateUIMES("START RUNAUTO", Brushes.LightGreen);

                        this.DataPCB = new DataPCB();
                    
                    }

                    // CHECK END -------------------------------------------------------
                    UiManager.Instance.PLC.device.ReadBit(DeviceCode.M,3856, out M_STOP);
                    if (M_STOP)
                    {

                        addLog("--- End RunAuto --- ");

                        Dispatcher.Invoke(() =>
                        {
                            foreach (var item in compositeViewModel.MasterItems)
                            {
                                item.IsCurrent = false;
                            }


                            List<string> resultList = compositeViewModel.MasterItems
                                .OrderBy(x => x.Index)
                                .Select(x => x.IsNG ? "NG" : "OK")
                                .ToList();
                            System.Diagnostics.Debug.WriteLine(string.Join(", ", resultList));

                            UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 3856, false);
                            addLog("Write Bit END AUTO M3856 = OFF");

                            this.UpdateUIMES($"END AUTO", Brushes.LightGreen);

                            DataPCB.RESULT_PCB = resultList;
                        });

                        if(UiManager.appSetting.RUN.MESOnline)
                        {
                            this.CheckMESWorkout();
                        }    
                       


                    }

                    // CHECK START SCANNER ---------------------------------------------------
                    UiManager.Instance.PLC.device.ReadBit(DeviceCode.M, 3850, out M_START_SCANNER);
                    if (M_START_SCANNER)
                    {

                        addLog("--- CHECK SCANNER --- ");
                        this.UpdateUIMES("START TRIGGER SCANNER", Brushes.LightGreen);


                        UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 3850, false);
                        addLog("Write Bit START Scanner  (M3850) = OFF");

                     

                        if(UiManager.appSetting.RUN.CheckScanner)
                        {
                            string QR = UiManager.Instance.scannerCOM.ReadQRKeyence();
                            //string QR = "PCBID1234567890";

                            if (!string.IsNullOrEmpty(QR))
                            {
                                this.UpdateUIQR(QR, true);

                                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 3851, true);
                                addLog("Write Bit Scanner Check OK (M3851) = ON");

                                this.UpdateUIMES($"SCANNER TRIGGER COMPLETE", Brushes.LightGreen);


                                /// Check MES S010
                                this.DataPCB.BARCODE_PCB = QR;

                                if(UiManager.appSetting.RUN.MESOnline)
                                {
                                    this.CheckMESWorkin();
                                }    
                                


                            }
                            else
                            {
                                this.UpdateUIQR("Scanner Error", false);
                                this.UpdateUIMES($"ERROR SCANNER  : Unable to Read Code", Brushes.OrangeRed);

                                UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 3852, true);
                                this.addLog("Write Bit Scanner Check NG (M3852) = ON");

                                // WRITE ERROR PLC LỖI 1000;
                                UiManager.Instance.PLC.device.WriteWord(DeviceCode.D, 100, 1000);

                                //AddError(1000);
                            }
                        }    
                        else
                        {
                            addLog("BY PASS CHECK SCANNER ");

                            UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 3851, true);
                            addLog("Write Bit Scanner Check OK (M3851) = ON");

                            this.UpdateUIMES($"BY PASS TRIGGER SCANNER", Brushes.LightGreen);

                        }    
                       

                    }


                    // CHECK NEXT STEP ------------------------------------------------------
                    UiManager.Instance.PLC.device.ReadBit(DeviceCode.M, 3854, out M_NEXT_STEP);
                    if (M_NEXT_STEP)
                    {

                        addLog("--- CHECK NEXT STEP --- ");

                        Thread.Sleep(100);
                        _stepIndex = 0;
                        UiManager.Instance.PLC.device.ReadMultiWord(DeviceCode.D, 3240,10, out D_INDEX_RUNNING);
                        if(D_INDEX_RUNNING[0] >=1)
                        {
                            _stepIndex = Convert.ToInt32(D_INDEX_RUNNING[0]);
                            addLog($"--- STEP {_stepIndex}--- ");

                            if (_stepIndex <= InspectionPath.Count)
                            { 
                                UpdateUI(); 
                            }
                        }
                        this.UpdateUIMES($"START CHECK STEP {_stepIndex}", Brushes.LightGreen);



                        UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 3854, false);
                        addLog("Write Bit Next Step (M3854) = OFF");
                    }

                    // CHECK BACK STEP ------------------------------------------------------
                    UiManager.Instance.PLC.device.ReadBit(DeviceCode.M, 3855, out M_BACK_STEP);
                    if (M_BACK_STEP)
                    {

                        addLog("--- CHECK BACK STEP --- ");
                       
                        _stepIndex = 0;
                        UiManager.Instance.PLC.device.ReadMultiWord(DeviceCode.D, 3240, 10, out D_INDEX_RUNNING);
                        if (D_INDEX_RUNNING[0] >= 1)
                        {
                            _stepIndex = Convert.ToInt16(D_INDEX_RUNNING[0]);
                            addLog($"--- STEP {_stepIndex}--- ");
                            if (_stepIndex < InspectionPath.Count - 1) { UpdateUI(); }
                        }
                        this.UpdateUIMES($"START BACK STEP {_stepIndex}", Brushes.LightGreen);
                        UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, 3855, false);
                        addLog("Write Bit Back Step (M3855) = OFF");
                    }
                    Thread.Sleep(30);
                }
            }
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
        private void UpdateParammter()
        {
            try
            {
                int runDirection = 1;

                List<short> Data = new List<short>(new short[30]);
                

                compositeViewModel.TotalRows = 2; // Ví dụ: 2
                compositeViewModel.TotalCols = 10; // Ví dụ: 20

                this.TotalRows = 2;
                this.TotalCols = 10;
                this.VisionRows = 2;
                this.VisionCols = 2;
                this._runDirection = runDirection;


                this.ResetSystem();
               
            }
            catch (Exception ex)
            {
                logger.Create($"Error UpdateParammter : {ex}", LogLevel.Error);
                
            }
            
        }
        #region MAIN MANUAL VISION
        private void ResetSystem()
        {
            _stepIndex = 0;
            InitializeMasterItems();
            GeneratePath();
            UpdateUI();
        }
        private void InitializeMasterItems()
        {
            compositeViewModel.MasterItems.Clear();
            for (int r = 0; r < TotalRows; r++)
                for (int c = 0; c < TotalCols; c++)
                    compositeViewModel.MasterItems.Add(new WorkItem { PhysR = r, PhysC = c, Index = GetLogicalIndex(r, c) });
        }
        private int GetLogicalIndex(int r, int c)
        {
            int corner = RunDirection / 4;
            bool isVert = (RunDirection % 4) / 2 == 1;
            bool isZig = (RunDirection % 2) == 1;
            int pR = r, pC = c;
            if (corner == 1 || corner == 3) pC = (TotalCols - 1) - c;
            if (corner == 2 || corner == 3) pR = (TotalRows - 1) - r;

            if (!isVert)
            {
                if (isZig && pR % 2 != 0) return (pR * TotalCols) + (TotalCols - 1 - pC) + 1;
                return (pR * TotalCols) + pC + 1;
            }
            else
            {
                if (isZig && pC % 2 != 0) return (pC * TotalRows) + (TotalRows - 1 - pR) + 1;
                return (pC * TotalRows) + pR + 1;
            }
        }
        private void GeneratePath()
        {
            InspectionPath.Clear();
            int corner = RunDirection / 4;
            bool isVert = (RunDirection % 4) / 2 == 1;
            bool isZigMaster = (RunDirection % 2) == 1;
            int stepsH = (int)Math.Ceiling((double)TotalCols / VisionCols);
            int stepsV = (int)Math.Ceiling((double)TotalRows / VisionRows);


            //int currentVisionCols = TotalCols;
            //int currentVisionRows = VisionRows; 

            //int stepsH = (int)Math.Ceiling((double)TotalCols / currentVisionCols);
            //int stepsV = (int)Math.Ceiling((double)TotalRows / currentVisionRows);

            if (!isVert)
            {
                for (int v = 0; v < stepsV; v++)
                {
                    bool rev = isZigMaster && (v % 2 != 0);
                    for (int h = 0; h < stepsH; h++) AddStep(rev ? (stepsH - 1 - h) : h, v, corner);
                }
            }
            else
            {
                for (int h = 0; h < stepsH; h++)
                {
                    bool rev = isZigMaster && (h % 2 != 0);
                    for (int v = 0; v < stepsV; v++) AddStep(h, rev ? (stepsV - 1 - v) : v, corner);
                }
            }
        }
        private void AddStep(int hS, int vS, int corner)
        {
            int cMin, cMax;
            if (corner == 1 || corner == 3)
            {
                cMax = (TotalCols - 1) - (hS * VisionCols);
                cMin = Math.Max(0, cMax - VisionCols + 1);
            }
            else
            {
                cMin = hS * VisionCols;
                cMax = Math.Min(TotalCols - 1, cMin + VisionCols - 1);
            }

            int rMin, rMax;
            if (corner == 2 || corner == 3)
            {
                rMax = (TotalRows - 1) - (vS * VisionRows);
                rMin = Math.Max(0, rMax - VisionRows + 1);
            }
            else
            {
                rMin = vS * VisionRows;
                rMax = Math.Min(TotalRows - 1, rMin + VisionRows - 1);
            }

            var items = compositeViewModel.MasterItems.Where(x => x.PhysR >= rMin && x.PhysR <= rMax && x.PhysC >= cMin && x.PhysC <= cMax).ToList();
            if (items.Any()) InspectionPath.Add(items);
        }
        private void UpdateUI()
        {
            Dispatcher.Invoke(() =>
            {
                if (_stepIndex == 0)
                {
                    foreach (var item in compositeViewModel.MasterItems)
                    {
                        item.IsCurrent = false;
                        item.HasBeenReached = false;
                    }
                    compositeViewModel.CurrentCheckItems = null;
                    compositeViewModel.CurrentDisplayCols = 1;
                    return;
                }

                if (InspectionPath == null || !InspectionPath.Any()) return;


                foreach (var item in compositeViewModel.MasterItems) item.IsCurrent = false;

                int pathIndex = _stepIndex - 1;
                if (pathIndex >= InspectionPath.Count) return;

                var currentSet = InspectionPath[pathIndex];

                foreach (var item in currentSet)
                {
                    item.HasBeenReached = true;
                    item.IsCurrent = true;
                }


                compositeViewModel.CurrentDisplayCols = currentSet.Select(x => x.PhysC).Distinct().Count();


                compositeViewModel.CurrentCheckItems = new ObservableCollection<WorkItem>(currentSet.OrderBy(x => x.PhysR).ThenBy(x => x.PhysC));
            });
            
        }
        


        private void ExportResults_Click(object sender, RoutedEventArgs e)
        {
            
            foreach (var item in compositeViewModel.MasterItems)
            {
                item.IsCurrent = false;
            }

           
            List<string> resultList = compositeViewModel.MasterItems
                .OrderBy(x => x.Index)
                .Select(x => x.IsNG ? "NG" : "OK")
                .ToList();

            MessageBox.Show(string.Join(", ", resultList));
            System.Diagnostics.Debug.WriteLine(string.Join(", ", resultList));
        }
        private void NextStep_Click(object sender, RoutedEventArgs e)
        {
            if (_stepIndex < InspectionPath.Count) // Cho phép tăng đến tối đa số bước
            {
                _stepIndex++;
                UpdateUI();
            }
        }
        private void PrevStep_Click(object sender, RoutedEventArgs e)
        {
            if (_stepIndex > 0)
            {
                _stepIndex--;
                UpdateUI();
            }
        }
        private void ClearData_Click(object sender, RoutedEventArgs e)
        {
            _stepIndex = 0;
            foreach (var item in compositeViewModel.MasterItems) { item.IsNG = false; item.HasBeenReached = false; item.IsCurrent = false; }
            UpdateUI();
        }

        #endregion
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



            this.ThreadUpDatePLC();
            // Start CH1;
            this.AutoRunThread = new Thread(new ThreadStart(Running));
            this.AutoRunThread.IsBackground = true;
            this.AutoRunThread.Start();

            this.UpdateParammter();

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
            List<short> D_ListShortDevicePLC_6500 = new List<short>();
            while (this.isUpdate)
            {
                bool flag = UiManager.Instance.PLC.device.isOpen();
                if (flag)
                {
                    UiManager.Instance.PLC.device.ReadMultiWord(DeviceCode.D, 0, 500, out this.D_ListShortDevicePLC_0);
                    UiManager.Instance.PLC.device.ReadMultiWord(DeviceCode.D, 6500, 10, out D_ListShortDevicePLC_6500);
                    UiManager.Instance.PLC.device.ReadMultiBits(DeviceCode.M, 6176, 30, out this.M_ListBitPLC0_500);
                    if(M_ListBitPLC0_500.Count > 0 && M_ListBitPLC0_500[2]  == true)
                    {
                        addLog("BUTTON RESET :M3862 = ON ");
                        addLog("");
                        UiManager.Instance.PLC.device.WriteWord(DeviceCode.D, 100, 0);
                    }

                    this.UpdateError();
                }

                Thread.Sleep(20);
            }
        }
       
        private void UpdateError()
        {
             Dispatcher.Invoke(delegate ()
            {
                bool flag = UiManager.Instance.PLC.device.isOpen();
                if (flag)
                {
                    bool flag2 = this.D_ListShortDevicePLC_0.Count >= 1;
                    if (flag2)
                    {
                        this.AddError(this.D_ListShortDevicePLC_0[100]);
                        this.AddError(this.D_ListShortDevicePLC_0[101]);
                        this.AddError(this.D_ListShortDevicePLC_0[102]);
                        this.AddError(this.D_ListShortDevicePLC_0[103]);
                        this.AddError(this.D_ListShortDevicePLC_0[104]);
                        this.AddError(this.D_ListShortDevicePLC_0[105]);
                        this.AddError(this.D_ListShortDevicePLC_0[106]);
                        this.AddError(this.D_ListShortDevicePLC_0[107]);
                        this.AddError(this.D_ListShortDevicePLC_0[108]);
                        this.AddError(this.D_ListShortDevicePLC_0[109]);
                        this.AddError(this.D_ListShortDevicePLC_0[110]);
                        this.AddError(this.D_ListShortDevicePLC_0[111]);
                        this.AddError(this.D_ListShortDevicePLC_0[112]);
                        this.AddError(this.D_ListShortDevicePLC_0[113]);
                        this.AddError(this.D_ListShortDevicePLC_0[114]);
                        this.AddError(this.D_ListShortDevicePLC_0[115]);
                        bool flag3 = this.D_ListShortDevicePLC_0[100] == 0 && !this.hasClearedError;
                        if (flag3)
                        {
                            this.ClearError();
                            this.hasClearedError = true;
                        }
                        else
                        {
                            bool flag4 = this.D_ListShortDevicePLC_0[100] != 0;
                            if (flag4)
                            {
                                this.hasClearedError = false;
                            }
                        }
                        bool flag5 = this.M_ListBitPLC0_500[2];
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
        private void BtReset_Click(object sender, RoutedEventArgs e)
        {
            this.addLog("Click Reset Machine");
            this.ClearError();
          
        }
        private async void BtStart_Click(object sender, RoutedEventArgs e)
        {
            string QR = "QR1234";
            this.UpdateUIQR(QR, true);

            DataPCB = new DataPCB();
            DataPCB.BARCODE_PCB = QR;
            CheckMESWorkin();
        }
        private async void BtHome_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtStop_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(DataPCB.PRE_BIN_CODE))
            {
                return;
            }
            if (DataPCB.PRE_BIN_CODE.Length > 0)
            {
                List<string> list = new List<string>();
                list.Add("NG");

                for (int i = 1; i < DataPCB.PRE_BIN_CODE.Length; i++)
                {
                    list.Add("OK");
                }
                DataPCB.RESULT_PCB = list;
                CheckMESWorkout();
            }
            else
            {
                addLog($"DataPCB.PRE_BIN_CODE.Length = {DataPCB.PRE_BIN_CODE.Length}");
                addLog("");
            }
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

    public class CompositeViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<logEntry> LogEntries { get; set; } = new ObservableCollection<logEntry>();

        #region BANG VISION MANUAL
        public ObservableCollection<WorkItem> MasterItems { get; set; } = new ObservableCollection<WorkItem>();

        private ObservableCollection<WorkItem> _currentCheckItems;
        public ObservableCollection<WorkItem> CurrentCheckItems
        {
            get => _currentCheckItems;
            set { _currentCheckItems = value; OnPropertyChanged(nameof(CurrentCheckItems)); }
        }

        private int _currentDisplayCols;
        public int CurrentDisplayCols
        {
            get => _currentDisplayCols;
            set { _currentDisplayCols = value; OnPropertyChanged(nameof(CurrentDisplayCols)); }
        }

    
        private string _currentStepDisplay;
        public string CurrentStepDisplay
        {
            get => _currentStepDisplay;
            set { _currentStepDisplay = value; OnPropertyChanged(nameof(CurrentStepDisplay)); }
        }


        private int _totalCols = 10; 
        public int TotalCols
        {
            get => _totalCols;
            set { _totalCols = value; OnPropertyChanged(nameof(TotalCols)); }
        }

        private int _totalRows = 5;
        public int TotalRows
        {
            get => _totalRows;
            set { _totalRows = value; OnPropertyChanged(nameof(TotalRows)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion
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
