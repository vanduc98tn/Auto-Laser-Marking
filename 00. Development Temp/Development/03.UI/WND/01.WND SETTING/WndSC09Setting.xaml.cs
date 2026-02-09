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
using System.Windows.Shapes;

namespace Development
{
    /// <summary>
    /// Interaction logic for WndSC09Setting.xaml
    /// </summary>
    public partial class WndSC09Setting : Window
    {
        public SC09Setting sc09Setting;
        private MyLogger logger = new MyLogger("WndSC09Setting");
        public WndSC09Setting()
        {
            InitializeComponent();
            this.Loaded += WndSC09Setting_Loaded;
            this.btnOk.Click += BtnOk_Click;
            this.btnCancle.Click += BtnCancle_Click;
        }

        private void BtnCancle_Click(object sender, RoutedEventArgs e)
        {
            sc09Setting = null;
            this.Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               this.sc09Setting.COM = cbSelecCom.SelectedItem.ToString();
                this.Close();
            }
            catch (Exception ex)
            {
                logger.Create("BtnOk_Click: " + ex.Message, LogLevel.Error);
            }
        }

        private void WndSC09Setting_Loaded(object sender, RoutedEventArgs e)
        {
            this.cbSelecCom.SelectedItem = UiManager.appSetting.settingDevice.sc09Setting.COM;
            LoadPort();
        }

        private void LoadPort()
        {
            cbSelecCom.Items.Clear(); 
            string[] ports = System.IO.Ports.SerialPort.GetPortNames(); 
            foreach (var port in ports)
            {
                cbSelecCom.Items.Add(port);
            }

            if (cbSelecCom.Items.Count == 0)
            {
                cbSelecCom.Items.Add("Không có cổng COM");
            }
        }
        public SC09Setting DoSettings(Window owner, SC09Setting oldSettings)
        {
            this.sc09Setting = oldSettings;
            try
            {
                 this.cbSelecCom.SelectedItem = sc09Setting.COM;
               
                this.ShowDialog();
            }
            catch (Exception ex)
            {
                logger.Create("DoSettings: " + ex.Message, LogLevel.Error);
            }
            return this.sc09Setting;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
