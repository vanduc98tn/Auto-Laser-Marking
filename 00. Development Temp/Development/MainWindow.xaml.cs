using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Development
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
     
        public const String AppVersionNumber = "1.2.23";
        public const String AppVersionTime = "14-Jun-2021";
        private System.Timers.Timer clock = new System.Timers.Timer(1000); 
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Unloaded += MainWindow_Closed;
            

            this.Closed += MainWindow_Closed;
            this.btPower.Click += BtPower_Click;
            this.btMenu.Click += BtMenu_Click;
            this.btMain.Click += BtMain_Click;
            this.btIO.Click += BtIO_Click;
            this.btLastJam.Click += BtLastJam_Click;

            this.bthide.Click += Bthide_Click;
            this.btminimized.Click += Btminimized_Click;
            this.btClose.Click += BtClose_Click;


        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void BtClose_Click(object sender, RoutedEventArgs e)
        {
          
            App.Current.Shutdown();
        }
        private void Btminimized_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized; // Phóng to toàn màn hình
            else
                this.WindowState = WindowState.Normal; // Trở về kích thước cũ
        }
        private void Bthide_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized; // Thu nhỏ xuống Taskbar nhưng vẫn chạy
        }
        public void MainWindow_Closed(object sender, EventArgs e)
        {
            this.Close();

            UiManager.Instance.PLC.NotUseDevice();
            UiManager.Instance.DisconncetPLC();

            Environment.Exit(0);
        }
        private void BtLastJam_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_ALARM);
        }
        private void BtIO_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_IO_INPUT_PLC);
        }
        private void BtMain_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_MAIN);
        }
        private void BtMenu_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_MENU);
        }
        private void BtPower_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            this.StartTime();
            this.UpdateUI();
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_MAIN);
            this.LoginApp();

        }
        private void StartTime()
        {
            clock.AutoReset = true;
            clock.Elapsed += this.Clock_Elapsed;
            clock.Start();
        }
        private void LoginApp()
        {
            var setting = UiManager.managerSetting.assignSystem;
            if (setting.LoginApp == true && setting.LoginMes == true)
            {
                var Result = ManagerLogin.DoCheck();
                if (Result == 0)
                {
                    UiManager.Instance.wndMain.MainWindow_Closed(this, null);
                }
            }
            UserManager.isLogOn = 0;
        }
        private void UpdateUI()
        {
            this.txbNameMachine.Text = UiManager.managerSetting.assignSystem.NameMachine;
            //version
            Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            string exePath = Assembly.GetExecutingAssembly().Location;
            // Lấy ngày cập nhật file EXE
            DateTime lastUpdate = File.GetLastWriteTime(exePath);
            this.TblVersion.Content = $"Ver {currentVersion} - Updated: {lastUpdate:yyyy-MM-dd HH:mm}";
        }
        private void Clock_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                this.Dispatcher.Invoke(() => { this.lblCurrentDate.Content = String.Format("TIME : {0:yyyy-MM-dd HH:mm:ss}", DateTime.Now); });
            }
            catch { }

        }
        public void UpdateMainContent(object obj)
        {
            this.frmMainContent.Navigate(obj);
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
           
            if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
            {
                
                this.DragMove();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
