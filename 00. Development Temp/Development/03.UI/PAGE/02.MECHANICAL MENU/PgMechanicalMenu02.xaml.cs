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
    /// Interaction logic for PgMechanicalMenu02.xaml
    /// </summary>
    public partial class PgMechanicalMenu02 : Page
    {
        public PgMechanicalMenu02()
        {
            InitializeComponent();

            this.Loaded += PgMechanicalMenu02_Loaded;
            this.Unloaded += PgMechanicalMenu02_Unloaded;

            this.btMenuTab01.Click += BtMenuTab01_Click;
            this.btMenuTab02.Click += BtMenuTab02_Click;
            this.btMenuTab03.Click += BtMenuTab03_Click;
            //this.btMenuTab04.Click += BtMenuTab04_Click;
            this.btMenuTab05.Click += BtMenuTab05_Click;


            this.btLogClear.Click += BtLogClear_Click;
            //this.btSetting.Click += BtSetting_Click;
            this.btSave.Click += BtSave_Click;
            //this.btOpen.Click += BtOpen_Click;
            //this.btClose.Click += BtClose_Click;
        }

        

        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            SaveSetting();
        }
        private void BtLogClear_Click(object sender, RoutedEventArgs e)
        {
            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo("You Want Clear?")) return;
            this.ClearLogs();
        }

        private void PgMechanicalMenu02_Unloaded(object sender, RoutedEventArgs e)
        {
            ClearLogs();
        }
        private void PgMechanicalMenu02_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateUI();
        }

        private void UpdateUI()
        {
            this.tbIPMES.Text = UiManager.appSetting.MESSettings.Ip.ToString();
            this.tbPortMES.Text = UiManager.appSetting.MESSettings.Port.ToString();
            this.tbEquipment.Text = UiManager.appSetting.MESSettings.EquimentID.ToString();
            this.tbRecipe.Text = UiManager.appSetting.MESSettings.Repice.ToString();

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
        private void SaveSetting()
        {
            UiManager.appSetting.MESSettings.Ip = this.tbIPMES.Text;
            UiManager.appSetting.MESSettings.Port = Convert.ToInt32(this.tbPortMES.Text);
            UiManager.appSetting.MESSettings.EquimentID = this.tbEquipment.Text;
            UiManager.appSetting.MESSettings.Repice = this.tbRecipe.Text;

            UiManager.SaveAppSetting();
            UpdateLogs($"Setting Ip : {UiManager.appSetting.MESSettings.Ip}");
            UpdateLogs($"Setting Port : {UiManager.appSetting.MESSettings.Port}");
            UpdateLogs($"Setting Equipment : {UiManager.appSetting.MESSettings.EquimentID}");
            UpdateLogs($"Setting Recipe : {UiManager.appSetting.MESSettings.Repice}");

            UpdateLogs($"Save Setting Complete !");


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
