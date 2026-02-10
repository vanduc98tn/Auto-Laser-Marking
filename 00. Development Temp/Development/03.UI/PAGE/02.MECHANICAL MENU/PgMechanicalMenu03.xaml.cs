using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for PgMechanicalMenu03.xaml
    /// </summary>
    public partial class PgMechanicalMenu03 : Page
    {
        private SettingDevice settingDevice;

        public PgMechanicalMenu03()
        {
            InitializeComponent();

            this.Loaded += PgMechanicalMenu03_Loaded;

            this.btMenuTab01.Click += BtMenuTab01_Click;
            this.btMenuTab02.Click += BtMenuTab02_Click;
            this.btMenuTab03.Click += BtMenuTab03_Click;
            //this.btMenuTab04.Click += BtMenuTab04_Click;
            this.btMenuTab05.Click += BtMenuTab05_Click;

            this.btSetting.Click += BtSetting_Click;
            this.btSave.Click += BtSave_Click;

            this.btOpen.Click += BtOpen_Click;
            this.btClose.Click += BtClose_Click;
            this.btRead.Click += BtRead_Click;

        }

        private void BtSetting6_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_MECHANICAL_MENU_05);

        }

        private void BtRead_Click(object sender, RoutedEventArgs e)
        {
            string Result = UiManager.Instance.scannerTCP.ReadQR();
            UpdateLogs($"QR: {Result}");

        }
        private void BtClose_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.scannerTCP.Stop();
            UpdateUiButton();
        }
        private void BtOpen_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.scannerTCP.Start();
            UpdateUiButton();
        }
        private void UpdateUiButton()
        {
            if (UiManager.Instance.scannerTCP.IsConnected)
            {
                UpdateLogs("Connect scanner Complete");
                btClose.Background = Brushes.White;
                btOpen.Background = Brushes.LightGreen;
            }
            else
            {
                UpdateLogs("Scanner Not Connected");
                btClose.Background = Brushes.OrangeRed;
                btOpen.Background = Brushes.White;
            }
        }
        private void PgMechanicalMenu03_Loaded(object sender, RoutedEventArgs e)
        {
            settingDevice = UiManager.appSetting.settingDevice;
            UpdateUiButton();
        }
        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo("You Want Save Setting?")) return;
            UiManager.appSetting.settingDevice = settingDevice;
            UpdateLogs($"Save Setting Scanner Keyence Complete");
            UiManager.SaveAppSetting();
        }
        private void BtSetting_Click(object sender, RoutedEventArgs e)
        {
            WndMCTCPSetting wndMC = new WndMCTCPSetting();
            var settingNew = wndMC.DoSettings(Window.GetWindow(this), this.settingDevice.ScannerTCP);
            if (settingNew != null)
            {
                this.settingDevice.ScannerTCP = settingNew;
                UpdateLogs($"Device Seting IP :{settingNew.Ip.ToString()}");
                UpdateLogs($"Device Seting Port :{settingNew.Port}");

                UpdateLogs($"Click Button Save to Complete");
            }
        }
        private void UpdateLogs(string notify)
        {
            this.Dispatcher.Invoke(() => {
                this.txtLogs.Text += "\r\n" + notify;
                this.txtLogs.ScrollToEnd();
            });
        }
        private void BtMenuTab05_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_MECHANICAL_MENU_05);
        }
        private void BtMenuTab04_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_MECHANICAL_MENU_04);
        }
        private void BtMenuTab03_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_MECHANICAL_MENU_03);
        }
        private void BtMenuTab02_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_MECHANICAL_MENU_02);
        }
        private void BtMenuTab01_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_MECHANICAL_MENU_01);
        }
    }
}
