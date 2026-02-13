using ITM_Semiconductor;
using MvCamCtrl.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Development
{
    public enum PAGE_ID
    {
        // Page Main
        PAGE_MAIN = 0,

        // Page Menu
        PAGE_MENU,

        PAGE_TEACHING_MENU_01,
        PAGE_TEACHING_MENU_02,
        PAGE_TEACHING_MENU_03,
        PAGE_TEACHING_MENU_04,
        PAGE_TEACHING_MENU_05,
        PAGE_TEACHING_MENU_06,
        PAGE_TEACHING_MENU_07,


        PAGE_MECHANICAL_MENU_01,
        PAGE_MECHANICAL_MENU_02,
        PAGE_MECHANICAL_MENU_03,
        PAGE_MECHANICAL_MENU_04,
        PAGE_MECHANICAL_MENU_05,
    


        PAGE_SYSTEM_MENU_01,
        PAGE_SYSTEM_MENU_02,

        PAGE_MANUAL_OPERATION_01,
        PAGE_MANUAL_OPERATION_02,
        PAGE_MANUAL_OPERATION_03,
        PAGE_MANUAL_OPERATION_04,
        PAGE_MANUAL_OPERATION_05,
        PAGE_MANUAL_OPERATION_06,

        PAGE_STATUS_MENU,

        PAGE_MODEL,

        PAGE_SUPER_USER_MENU_01,
        PAGE_SUPER_USER_MENU_02,
        PAGE_SUPER_USER_MENU_03,
        PAGE_SUPER_USER_MENU_04,
        PAGE_SUPER_USER_MENU_05,
        PAGE_SUPER_USER_MENU_06,
        PAGE_SUPER_USER_MENU_07,
        PAGE_SUPER_USER_MENU_08,

        PAGE_ASSIGN_MENU,
        PAGE_ASSIGN_ALARM_SETTING,

        // Page I/O
        PAGE_IO_INPUT_PLC,
        PAGE_IO_OUTPUT_PLC,

        // Page Alarm 
        PAGE_ALARM,

        //Page Camera Setting
        PAGE_CAMERA,
    }


    class UiManager
    {
        private static UiManager instance = new UiManager();
        public static UiManager Instance => instance;
        public MainWindow wndMain;
        public static WndCheckUpdate WndCheckver;
        private static MyLogger logger = new MyLogger("UiManager");
        public static Hashtable pageTable = new Hashtable();
        public static AppSetting appSetting = new AppSetting();
        public static ManagerSetting managerSetting = new ManagerSetting();
        public static ModelSettings currentModel = new ModelSettings();

        public SelectDevice PLC;

        public MES00Service MES;

        public KeyenceScannerCom scannerCOM;

        public KeyenceScannerTCP scannerTCP;

        public LaserCOM laserCOM;

        //Camera
        public static HikCamDevice hikCamera = new HikCamDevice();
        public static HikCam Cam1 = new HikCam();
        public static HikCam Cam2 = new HikCam();

        #region Sử dụng Use Đăng Nhập MES
        public static string UserNameLoginMesOP_ME { get; set; }
        public static string CodeUserLoginMesOP_ME { get; set; }
        #endregion

        public void Startup()
        {
            logger.Create("Startup:",LogLevel.Information);
            try
            {
                // Load Settings
                LoadAppSetting();
                SaveAppSetting();

                // Load ManagerSetting
                LoadManagerSetting();
                SaveManagerSetting();

                //ChangeLanguage
                ChangeLanguage(managerSetting.assignSystem.Language);

                // Create Database if not existed
                Dba.createDatabaseIfNotExisted();

                // Create Database imageAlarm if not existed
                SQLimageAlarm.createDatabaseIfNotExisted();

                // Load Excel file for alarms
                AlarmList.LoadAlarm(managerSetting.assignSystem.Language);

                // Load Excel file for status
                StatusMachine.LoadStatus(managerSetting.assignSystem.Language);

                // Initialize Page in Project
                initPageTable();


                // Connect PLC 
                this.ConncetPLC();

                this.LoadMES();

                this.ConnectScannerTCP();

                this.ConnectLaserCOM();


                // Load MainWindow
                wndMain = new MainWindow();

                // Create Main window:
                wndMain.frmMainContent.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
                wndMain.Show();

                // Creatr Wnd CheckUpdate
                if( managerSetting.assignSystem.AutoCheckUpdate)
                {
                    WndCheckver = new WndCheckUpdate();
                    WndCheckver.Show();
                }

                CameraListAcq();
                ConectCamera();



                logger.Create("Uimanager Program Start Up",LogLevel.Information);
            }
            catch (Exception ex)
            {
                logger.Create("Startup error:" + ex.Message,LogLevel.Error);
            }



        }
        public void ChangeLanguage(string langCode)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(langCode);
            Development.Properties.Resources.Culture = new CultureInfo(langCode);
        }
        private static void initPageTable()
        {

            pageTable.Add(PAGE_ID.PAGE_MAIN, new PgMain());
            pageTable.Add(PAGE_ID.PAGE_ALARM, new PgAlarm());
            pageTable.Add(PAGE_ID.PAGE_IO_INPUT_PLC, new PgInput());
            pageTable.Add(PAGE_ID.PAGE_IO_OUTPUT_PLC, new PgOutput());
            pageTable.Add(PAGE_ID.PAGE_MENU, new PgMenu());

            pageTable.Add(PAGE_ID.PAGE_TEACHING_MENU_01, new PgTeachingMenu01());
            pageTable.Add(PAGE_ID.PAGE_TEACHING_MENU_02, new PgTeachingMenu02());
            pageTable.Add(PAGE_ID.PAGE_TEACHING_MENU_03, new PgTeachingMenu03());
            pageTable.Add(PAGE_ID.PAGE_TEACHING_MENU_04, new PgTeachingMenu04());
            

            pageTable.Add(PAGE_ID.PAGE_MECHANICAL_MENU_01, new PgMechanicalMenu01());
            pageTable.Add(PAGE_ID.PAGE_MECHANICAL_MENU_02, new PgMechanicalMenu02());
            pageTable.Add(PAGE_ID.PAGE_MECHANICAL_MENU_03, new PgMechanicalMenu03());
            pageTable.Add(PAGE_ID.PAGE_MECHANICAL_MENU_04, new PgMechanicalMenu04());
            pageTable.Add(PAGE_ID.PAGE_MECHANICAL_MENU_05, new PgMechanicalMenu05());


            pageTable.Add(PAGE_ID.PAGE_SYSTEM_MENU_01, new PgSystemMenu01());
            pageTable.Add(PAGE_ID.PAGE_SYSTEM_MENU_02, new PgSystemMenu02());

            pageTable.Add(PAGE_ID.PAGE_MANUAL_OPERATION_01, new PgManual01());
            //pageTable.Add(PAGE_ID.PAGE_MANUAL_OPERATION_02, new PgManual02());
            //pageTable.Add(PAGE_ID.PAGE_MANUAL_OPERATION_03, new PgManual03());
            //pageTable.Add(PAGE_ID.PAGE_MANUAL_OPERATION_04, new PgManual04());

            pageTable.Add(PAGE_ID.PAGE_STATUS_MENU, new PgStatusMenu());

            pageTable.Add(PAGE_ID.PAGE_MODEL, new PgModel());

            pageTable.Add(PAGE_ID.PAGE_SUPER_USER_MENU_01, new PgSuperUserMenu01());
            pageTable.Add(PAGE_ID.PAGE_SUPER_USER_MENU_02, new PgSuperUserMenu02());
            pageTable.Add(PAGE_ID.PAGE_SUPER_USER_MENU_03, new PgSuperUserMenu03());
            pageTable.Add(PAGE_ID.PAGE_SUPER_USER_MENU_04, new PgSuperUserMenu04());

            pageTable.Add(PAGE_ID.PAGE_ASSIGN_MENU, new PgAssignMenu());
            pageTable.Add(PAGE_ID.PAGE_ASSIGN_ALARM_SETTING, new PgAssignAlarmSetting());

            pageTable.Add(PAGE_ID.PAGE_CAMERA, new PgCamera());
        }
        public void SwitchPage(PAGE_ID pgID)     // ham de thay dd
        {
            if (pageTable.ContainsKey(pgID))
            {
                var pg = (System.Windows.Controls.Page)pageTable[pgID];
                wndMain.UpdateMainContent(pg);
                wndMain.btMenu.ClearValue(Button.BackgroundProperty);
                wndMain.btMain.ClearValue(Button.BackgroundProperty);
                wndMain.btIO.ClearValue(Button.BackgroundProperty);
                wndMain.btLastJam.ClearValue(Button.BackgroundProperty);

                if (pgID == PAGE_ID.PAGE_MAIN)
                {
                    wndMain.btMain.Background = Brushes.Orange;
                }
                if ((pgID >= PAGE_ID.PAGE_MENU) & (pgID <= PAGE_ID.PAGE_ASSIGN_MENU))
                {
                    wndMain.btMenu.Background = Brushes.Orange;
                }
                if ((pgID >= PAGE_ID.PAGE_IO_INPUT_PLC) & (pgID <= PAGE_ID.PAGE_IO_OUTPUT_PLC))
                {
                    wndMain.btIO.Background = Brushes.Orange;
                }
                if (pgID == PAGE_ID.PAGE_ALARM)
                {
                    wndMain.btLastJam.Background = Brushes.Orange;
                }

            }
        }
        public static void SaveAppSetting()            ///  LUU THONG SO SETTING
        {
            try
            {
                if (appSetting == null)
                {
                    appSetting = new AppSetting();
                }
                string str = appSetting.TOJSON();
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppSetting.SETTING_FILE_NAME);   // duong dan den file exe de mo ung dung
                using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(path))
                {
                    streamWriter.WriteLine(str);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Create("SaveAppSetting" + ex.Message,LogLevel.Error);
            }

        }
        public static void LoadAppSetting()           // LOAD DU LIEU SETTING
        {

            String filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), AppSetting.SETTING_FILE_NAME);
            if (File.Exists(filePath))
            {
                using (StreamReader file = File.OpenText(filePath))
                {
                    appSetting = AppSetting.FromJSON(file.ReadToEnd());
                }
            }
            else
            {
                appSetting = new AppSetting();
            }
        }

        public static void SaveManagerSetting()            ///  LUU THONG SO SETTING
        {
            try
            {
                if (managerSetting == null)
                {
                    managerSetting = new ManagerSetting();
                }
                string str = managerSetting.TOJSON();
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ManagerSetting.SETTING_FILE_NAME);  
                using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(path))
                {
                    streamWriter.WriteLine(str);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Create("SaveAppSetting" + ex.Message, LogLevel.Error);
            }

        }
        public static void LoadManagerSetting()           // LOAD DU LIEU SETTING
        {

            String filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), ManagerSetting.SETTING_FILE_NAME);
            if (File.Exists(filePath))
            {
                using (StreamReader file = File.OpenText(filePath))
                {
                    managerSetting = ManagerSetting.FromJSON(file.ReadToEnd());
                }
            }
            else
            {
                managerSetting = new ManagerSetting();
            }
        }


        #region Conncet PLC
        public void ConncetPLC()
        {
            PLC = new SelectDevice(UiManager.appSetting.selectDevice, UiManager.appSetting.settingDevice);
            PLC.device.Open();
            Task.Run(() => { this.PLC.MonitorDevice(); });

        }
        public void DisconncetPLC()
        {
            if(PLC != null)
            {
                PLC.device.Close();
            }    
           
        }
        #endregion
        #region Connect MES
        private async void LoadMES()
        {
            await ConnectLoadMES();
        }
        public async Task ConnectLoadMES()
        {
            this.MES = new MES00Service(appSetting.MESSettings);
            var tsk = Task.Run(async () => {
                await this.MES.Start();
            });
            await Task.Delay(1);
        }
        #endregion

        #region Connect Scanner TCP
        public void ConnectScannerTCP()
        {
            var Setting = UiManager.appSetting.settingDevice.ScannerTCP;
            scannerTCP = new KeyenceScannerTCP(UiManager.appSetting.settingDevice.ScannerTCP);
            scannerTCP.Start();
        }
        public void DisconnectScannerTCP()
        {
            scannerTCP.Stop();
        }
        #endregion

        #region Connect Laser COM
        public void ConnectLaserCOM()
        {
            var Setting = UiManager.appSetting.settingDevice.COMLaser;
            laserCOM = new LaserCOM(Setting);
            laserCOM.Open();
        }
        public void DisconnectLaserCOM()
        {
            laserCOM.Close();
        }
        #endregion

        #region Models
        public static void SaveCurrentModelSettings()
        {
            ModelStore.UpdateModelSettings(currentModel);
        }
        public static void ReplaceModel(ModelSettings model)
        {
            try
            {
                if (model != null)
                {
                    currentModel = model;
                    appSetting.currentModel = model.modelName;
                    SaveAppSetting();
                }
            }
            catch (Exception ex)
            {
                logger.Create(String.Format("ReplaceModel error:" + ex.Message), LogLevel.Error);
            }
        }
        #endregion

        #region Camera
        public static void CameraListAcq()
        {
            hikCamera.DeviceListAcq();
            ConectCamera();
        }
        public static void ConectCamera()
        {
            MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList = hikCamera.m_pDeviceList;

            MyCamera.MV_CC_DEVICE_INFO device1 = UiManager.appSetting.connection.camSettings.device;
            int ret1 = Cam1.Open(device1, HikCam.AquisMode.AcquisitionMode);
            Cam1.SetExposeTime((int)UiManager.appSetting.connection.camSettings.ExposeTime);
            Thread.Sleep(2);
        }
        #endregion
    }
}
