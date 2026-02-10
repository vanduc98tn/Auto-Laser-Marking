using System;
using System.Collections.Generic;
using System.IO.Ports;
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
    /// Interaction logic for WndComSetting.xaml
    /// </summary>
    public partial class WndComSetting : Window
    {
        private MyLogger logger = new MyLogger("WndComSetting");
        private COMSetting comSetting;
        public WndComSetting()
        {
            InitializeComponent();

            InitializeComponent();

            //this.Loaded += WndComSetting_Loaded;
            this.btOk.Click += BtOk_Click;
            this.btOk.TouchDown += BtOk_Click;

            this.btCancel.Click += BtCancel_Click;
            this.btCancel.TouchDown += BtCancel_Click;

         
        }
        private void BtCancel_Click(object sender, RoutedEventArgs e)
        {
            this.comSetting = null;
            this.Close();
        }

        private void BtOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.comSetting.portName = this.cbPortName.SelectedValue.ToString();
                this.comSetting.baudrate = int.Parse(this.cbBaudrate.SelectedValue.ToString());
                this.comSetting.dataBits = int.Parse(this.cbDataBits.SelectedValue.ToString());
                this.comSetting.stopBits = COMSetting.ParseStopBits(this.cbStopBits.SelectedValue.ToString());
                this.comSetting.parity = COMSetting.ParseParity(this.cbParity.SelectedValue.ToString());
                this.comSetting.Handshake = Handshake.None;
                this.Close();
            }
            catch (Exception ex)
            {
                logger.Create("BtOk_Click: " + ex.Message, LogLevel.Error);
            }
        }

        private void WndComSetting_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadComPort();
        }

        private void LoadComPort()
        {
            try
            {
                var portNames = SerialPort.GetPortNames();
                foreach (var pn in portNames)
                {
                    var cbi = new ComboBoxItem();
                    cbi.Content = pn;
                    this.cbPortName.Items.Add(cbi);
                }
            }
            catch (Exception ex)
            {
                logger.Create("LoadComPort: " + ex.Message, LogLevel.Error);
            }
        }
        public COMSetting DoSettings(Window owner, COMSetting oldSettings)
        {
            this.comSetting = oldSettings;
            try
            {
                this.cbPortName.SelectedValue = this.comSetting.portName;
                this.cbBaudrate.SelectedValue = this.comSetting.baudrate.ToString();
                this.cbDataBits.SelectedValue = this.comSetting.dataBits.ToString();
                var s = this.comSetting.parity.ToString();
                this.cbParity.SelectedValue = s;
                s = this.comSetting.stopBits.ToString();
                this.cbStopBits.SelectedValue = s;
                this.ShowDialog();
            }
            catch (Exception ex)
            {
                logger.Create("DoSettings: " + ex.Message, LogLevel.Error);
            }
            return comSetting;
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
