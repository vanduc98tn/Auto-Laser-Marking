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
    /// Interaction logic for PgMechanicalMenu05.xaml
    /// </summary>
    public partial class PgMechanicalMenu05 : Page
    {
        private SettingDevice settingDevice;
        public PgMechanicalMenu05()
        {
            InitializeComponent();

            this.Loaded += PgMechanicalMenu05_Loaded;

            this.btMenuTab01.Click += BtMenuTab01_Click;
            this.btMenuTab02.Click += BtMenuTab02_Click;
            this.btMenuTab03.Click += BtMenuTab03_Click;
            //this.btMenuTab04.Click += BtMenuTab04_Click;
            this.btMenuTab05.Click += BtMenuTab05_Click;

            this.btLogClear.Click += BtLogClear_Click;
            this.btSetting.Click += BtSetting_Click;
            this.btSave.Click += BtSave_Click;
            this.btOpen.Click += BtOpen_Click;
            this.btClose.Click += BtClose_Click;

            this.btSend.Click += BtSend_Click;

        }

        private void BtSend_Click(object sender, RoutedEventArgs e)
        {
            string strResult = "";
            string strData = "Test";

            UpdateLogs($"Send: {strData}");

            byte[] arr1 = Encoding.UTF8.GetBytes(strData).ToArray();
            byte[] arr2 = { 0x0D };

            byte[] byData = new byte[arr1.Length + arr2.Length];

            Array.Copy(arr1, 0, byData, 0, arr1.Length);
            Array.Copy(arr2, 0, byData, arr1.Length, arr2.Length);


            byte[] byResult = UiManager.Instance.laserCOM.SendWaitResponse(byData);


            if (byResult != null)
            {
                strResult = ASCIIEncoding.ASCII.GetString(byResult.ToArray());
            }
            else
            {
                strResult = "Time out!";
            }

            UpdateLogs($"Receive: {strResult}");
        }

        private void BtClose_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.laserCOM.Close();
            UpdateUiButton();
        }
        private void BtOpen_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.laserCOM.Open();
            UpdateUiButton();
        }
        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo("You Want Save Setting?")) return;
            UiManager.appSetting.settingDevice.COMLaser = settingDevice.COMLaser;
            UpdateLogs($"Save Setting Com Laser Complete");
            UiManager.SaveAppSetting();
        }
        private void BtSetting_Click(object sender, RoutedEventArgs e)
        {
            WndComSetting wndMC = new WndComSetting();
            var settingNew = wndMC.DoSettings(Window.GetWindow(this), this.settingDevice.COMLaser);
            if (settingNew != null)
            {
                this.settingDevice.COMLaser = settingNew;
                UpdateLogs($"Device Seting PortName :{settingNew.portName.ToString()}");
                UpdateLogs($"Device Seting Parity :{settingNew.parity}");
                UpdateLogs($"Device Seting Databis :{settingNew.dataBits.ToString()}");
                UpdateLogs($"Device Seting Stopbit :{settingNew.stopBits.ToString()}");
                UpdateLogs($"Device Seting Handshake :{settingNew.Handshake.ToString()}");
                UpdateLogs($"Click Button Save to Complete");
            }
        }
        private void BtLogClear_Click(object sender, RoutedEventArgs e)
        {
            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo("You Want Clear?")) return;
            this.ClearLogs();
        }

        private void PgMechanicalMenu05_Loaded(object sender, RoutedEventArgs e)
        {
            settingDevice = UiManager.appSetting.settingDevice;
            UpdateUiButton();
        }

        private void UpdateUiButton()
        {
            if (UiManager.Instance.laserCOM.isOpen())
            {
                UpdateLogs("Connect Tester Complete");
                btClose.Background = Brushes.White;
                btOpen.Background = Brushes.LightGreen;
            }
            else
            {
                UpdateLogs("Disconnect Tester");
                btClose.Background = Brushes.OrangeRed;
                btOpen.Background = Brushes.White;
            }
        }
        private void UpdateLogs(string notify)
        {
            this.Dispatcher.Invoke(() => {
                this.txtLogs.Text += "\r\n" + notify;
                this.txtLogs.ScrollToEnd();
            });
        }
        private void ClearLogs()
        {
            this.Dispatcher.Invoke(() => {
                this.txtLogs.Clear();
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
