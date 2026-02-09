using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;
using Mitsubishi;
namespace Development
{
    /// <summary>
    /// Interaction logic for PgMechanicalMenu01.xaml
    /// </summary>
    public partial class PgMechanicalMenu01 : Page
    {

        private SettingDevice settingDevice;
        private MyLogger logger = new MyLogger("PgMechanical01Menu");

        private DispatcherTimer timer;
        public PgMechanicalMenu01()
        {
            InitializeComponent();

            this.Loaded += PgMechanicalMenu01_Loaded;

            this.btMenuTab01.Click += BtMenuTab01_Click;
            this.btMenuTab02.Click += BtMenuTab02_Click;
            this.btMenuTab03.Click += BtMenuTab03_Click;
            //this.btMenuTab04.Click += BtMenuTab04_Click;
            this.btMenuTab05.Click += BtMenuTab05_Click;

            this.btnOpen.Click += BtnOpen_Click;
            this.btnClose.Click += BtnClose_Click;


            this.btLogClear.Click += BtLogClear_Click;
            this.btnSettingDevice.Click += BtnSettingDevice_Click;

            this.btnSave.Click += BtnSave_Click;

            this.btReadBit.Click += BtReadBit_Click;
            this.btReadWord.Click += BtReadWord_Click;
            this.btReadDWord.Click += BtReadDWord_Click;
            this.btReadString.Click += BtReadString_Click;

            this.btWriteBit.Click += BtWriteBit_Click;
            this.btWriteWord.Click += BtWriteWord_Click;
            this.btWriteDWord.Click += BtWriteDWord_Click;
            this.btWriteString.Click += BtWriteString_Click;

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

        #region Button Action 
        private void BtWriteString_Click(object sender, RoutedEventArgs e)
        {
            if (this.cbSelectDeviceOut.SelectedValue == null) return;
            if (this.cbSelectDeviceOut.SelectedValue.ToString() == "M")
            {
                string Date = DateTime.Now.ToString("dd/MM/yyyy : HH:mm:ss");
                MessageBox.Show($"{Date} Read Failed \rDescription : Cloud not find any recognizable digits");
            }
            if (this.cbSelectDeviceOut.SelectedValue.ToString() == "D")
            {
                string numberInputWrite = tbxValueinput2.Text;

                if (!int.TryParse(tbxValueInputW.Text, out int numberInput))
                {
                    MessageBox.Show("Invalid value in ValueInput.");
                    return;
                }
                if (UiManager.Instance.PLC == null)
                {
                    MessageBox.Show("PLC Disconnect ");
                    return;
                }
                if (UiManager.Instance.PLC.device.isOpen())
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    if (UiManager.Instance.PLC.device.WriteString(DeviceCode.D, numberInput, numberInputWrite))
                    {
                        stopwatch.Stop();
                        long executionTimeMs = stopwatch.ElapsedMilliseconds;
                        UpdateLogs($"Time: {executionTimeMs} ms");

                        string Date = DateTime.Now.ToString("dd/MM/yyyy : HH:mm:ss");
                        MessageBox.Show($"{Date} Write Success");
                        UpdateLogs("Write PLC Complete");
                    }
                    else
                    {
                        stopwatch.Stop();
                        long executionTimeMs = stopwatch.ElapsedMilliseconds;
                        UpdateLogs($"Time: {executionTimeMs} ms");

                        string Date = DateTime.Now.ToString("dd/MM/yyyy : HH:mm:ss");
                        MessageBox.Show($"{Date} Write Faild");
                        UpdateLogs("Write PLC Faild");
                    }

                }
                else
                {
                    MessageBox.Show("PLC Disconnect ");
                }

            }
        }
        private void BtWriteDWord_Click(object sender, RoutedEventArgs e)
        {
            if (this.cbSelectDeviceOut.SelectedValue == null) return;
            if (this.cbSelectDeviceOut.SelectedValue.ToString() == "M")
            {
                string Date = DateTime.Now.ToString("dd/MM/yyyy : HH:mm:ss");
                MessageBox.Show($"{Date} Read Failed \rDescription : Cloud not find any recognizable digits");
            }
            if (this.cbSelectDeviceOut.SelectedValue.ToString() == "D")
            {

                if (!int.TryParse(tbxValueinput2.Text, out int numberInputWrite))
                {
                    MessageBox.Show("Invalid value in ValueInput.");
                    return;
                }

                if (!int.TryParse(tbxValueInputW.Text, out int numberInput))
                {
                    MessageBox.Show("Invalid value in ValueInput.");
                    return;
                }
                if (UiManager.Instance.PLC == null)
                {
                    MessageBox.Show("PLC Disconnect ");
                    return;
                }
                if (UiManager.Instance.PLC.device.isOpen())
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    if (UiManager.Instance.PLC.device.WriteDoubleWord(DeviceCode.D, numberInput, numberInputWrite))
                    {
                        string Date = DateTime.Now.ToString("dd/MM/yyyy : HH:mm:ss");
                        MessageBox.Show($"{Date} Write Success");
                        UpdateLogs("Write PLC Complete");
                    }
                    else
                    {
                        string Date = DateTime.Now.ToString("dd/MM/yyyy : HH:mm:ss");
                        MessageBox.Show($"{Date} Write Faild");
                        UpdateLogs("Write PLC Faild");
                    }
                    stopwatch.Stop();
                    long executionTimeMs = stopwatch.ElapsedMilliseconds;
                    UpdateLogs($"Time: {executionTimeMs} ms");
                }
                else
                {
                    MessageBox.Show("PLC Disconnect ");
                }

            }
        }
        private void BtWriteWord_Click(object sender, RoutedEventArgs e)
        {
            if (this.cbSelectDeviceOut.SelectedValue == null) return;
            if (this.cbSelectDeviceOut.SelectedValue.ToString() == "M")
            {
                string Date = DateTime.Now.ToString("dd/MM/yyyy : HH:mm:ss");
                MessageBox.Show($"{Date} Read Failed \rDescription : Cloud not find any recognizable digits");
            }
            if (this.cbSelectDeviceOut.SelectedValue.ToString() == "D")
            {

                if (!int.TryParse(tbxValueinput2.Text, out int numberInputWrite))
                {
                    MessageBox.Show("Invalid value in ValueInput.");
                    return;
                }

                if (!int.TryParse(tbxValueInputW.Text, out int numberInput))
                {
                    MessageBox.Show("Invalid value in ValueInput.");
                    return;
                }
                if (UiManager.Instance.PLC == null)
                {
                    MessageBox.Show("PLC Disconnect ");
                    return;
                }
                if (UiManager.Instance.PLC.device.isOpen())
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    if (UiManager.Instance.PLC.device.WriteWord(DeviceCode.D, numberInput, numberInputWrite))
                    {
                        stopwatch.Stop();
                        long executionTimeMs = stopwatch.ElapsedMilliseconds;
                        UpdateLogs($"Time: {executionTimeMs} ms");
                        string Date = DateTime.Now.ToString("dd/MM/yyyy : HH:mm:ss");
                        MessageBox.Show($"{Date} Write Success");
                        UpdateLogs("Write PLC Complete");

                    }
                    else
                    {
                        stopwatch.Stop();
                        long executionTimeMs = stopwatch.ElapsedMilliseconds;
                        UpdateLogs($"Time: {executionTimeMs} ms");

                        string Date = DateTime.Now.ToString("dd/MM/yyyy : HH:mm:ss");
                        MessageBox.Show($"{Date} Write Faild");

                        UpdateLogs("Write PLC Faild");
                    }

                }
                else
                {
                    MessageBox.Show("PLC Disconnect ");
                }

            }
        }
        private void BtWriteBit_Click(object sender, RoutedEventArgs e)
        {
            if (this.cbSelectDeviceOut.SelectedValue == null) return;
            if (this.cbSelectDeviceOut.SelectedValue.ToString() == "D")
            {
                string Date = DateTime.Now.ToString("dd/MM/yyyy : HH:mm:ss");
                MessageBox.Show($"{Date} Read Failed \rDescription : Cloud not find any recognizable digits");
            }
            if (this.cbSelectDeviceOut.SelectedValue.ToString() == "M")
            {

                bool outputBit = false;
                string converNumber = "";
                converNumber = this.tbxValueinput2.Text;
                if (converNumber != "fasle" && converNumber != "true" && converNumber != "0" && converNumber != "1")
                {
                    MessageBox.Show("Invalid value in ValueList. Please enter value true,false,0,1");
                }
                else
                {
                    if (converNumber == "fasle" || converNumber == "0")
                    {
                        outputBit = false;
                    }
                    else if (converNumber == "true" || converNumber == "1")
                    {
                        outputBit = true;
                    }
                    else
                    {
                        outputBit = false;
                        MessageBox.Show("Invalid value in ValueList. Please enter value true,false,0,1");
                    }
                }

                if (!int.TryParse(tbxValueInputW.Text, out int numberInput))
                {
                    MessageBox.Show("Invalid value in ValueInput.");
                    return;
                }
                if (UiManager.Instance.PLC == null)
                {
                    MessageBox.Show("PLC Disconnect ");
                    return;
                }
                if (UiManager.Instance.PLC.device.isOpen())
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    if (UiManager.Instance.PLC.device.WriteBit(DeviceCode.M, numberInput, outputBit))
                    {
                        stopwatch.Stop();
                        long executionTimeMs = stopwatch.ElapsedMilliseconds;
                        UpdateLogs($"Time: {executionTimeMs} ms");

                        string Date = DateTime.Now.ToString("dd/MM/yyyy : HH:mm:ss");
                        MessageBox.Show($"{Date} Write Success");
                        UpdateLogs("Write PLC Complete");
                    }
                    else
                    {
                        stopwatch.Stop();
                        long executionTimeMs = stopwatch.ElapsedMilliseconds;
                        UpdateLogs($"Time: {executionTimeMs} ms");

                        string Date = DateTime.Now.ToString("dd/MM/yyyy : HH:mm:ss");
                        MessageBox.Show($"{Date} Write Faild");
                        UpdateLogs("Write PLC Faild");
                    }

                }
                else
                {
                    MessageBox.Show("PLC Disconnect ");
                }

            }
        }
        private void BtReadString_Click(object sender, RoutedEventArgs e)
        {
            if (this.cbSelectDevice.SelectedValue == null) return;
            if (this.cbSelectDevice.SelectedValue.ToString() == "M")
            {
                string Date = DateTime.Now.ToString("dd/MM/yyyy : HH:mm:ss");
                MessageBox.Show($"{Date} Read Failed \rDescription : Cloud not find any recognizable digits");
            }
            if (this.cbSelectDevice.SelectedValue.ToString() == "D")
            {
                if (!int.TryParse(tbxValueList.Text, out int numberList) || numberList <= 0)
                {
                    MessageBox.Show("Invalid value in ValueList. Please enter a positive number.");
                    return;
                }

                if (!int.TryParse(tbxValueInput.Text, out int numberInput))
                {
                    MessageBox.Show("Invalid value in ValueInput.");
                    return;
                }
                if (UiManager.Instance.PLC == null)
                {
                    MessageBox.Show("PLC Disconnect ");
                    return;
                }
                if (UiManager.Instance.PLC.device.isOpen())
                {
                    tbxValueOutput.Text = "";
                    int NumberList = Convert.ToInt32(this.tbxValueList.Text);
                    int NumberInPut = Convert.ToInt32(this.tbxValueInput.Text);
                    string ListOutPut = "";

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    if (UiManager.Instance.PLC.device.ReadASCIIString(DeviceCode.D, NumberInPut, NumberList, out ListOutPut))
                    {
                        UpdateLogs("Read PLC Complete");

                        tbxValueOutput.Text = ListOutPut;

                    }
                    else
                    {
                        UpdateLogs("Read PLC Faild");
                    }
                    stopwatch.Stop();
                    long executionTimeMs = stopwatch.ElapsedMilliseconds;
                    UpdateLogs($"Time: {executionTimeMs} ms");
                }
                else
                {
                    MessageBox.Show("PLC Disconnect ");
                }
            }
        }
        private void BtReadDWord_Click(object sender, RoutedEventArgs e)
        {
            if (this.cbSelectDevice.SelectedValue == null) return;
            if (this.cbSelectDevice.SelectedValue.ToString() == "M")
            {
                string Date = DateTime.Now.ToString("dd/MM/yyyy : HH:mm:ss");
                MessageBox.Show($"{Date} Read Failed \rDescription : Cloud not find any recognizable digits");
            }
            if (this.cbSelectDevice.SelectedValue.ToString() == "D")
            {
                if (!int.TryParse(tbxValueList.Text, out int numberList) || numberList <= 0)
                {
                    MessageBox.Show("Invalid value in ValueList. Please enter a positive number.");
                    return;
                }

                if (!int.TryParse(tbxValueInput.Text, out int numberInput))
                {
                    MessageBox.Show("Invalid value in ValueInput.");
                    return;
                }
                if (UiManager.Instance.PLC == null)
                {
                    MessageBox.Show("PLC Disconnect ");
                    return;
                }
                if (UiManager.Instance.PLC.device.isOpen())
                {
                    tbxValueOutput.Text = "";
                    int NumberList = Convert.ToInt32(this.tbxValueList.Text);
                    int NumberInPut = Convert.ToInt32(this.tbxValueInput.Text);
                    List<int> ListOutPut = new List<int>();

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    if (UiManager.Instance.PLC.device.ReadMultiDoubleWord(DeviceCode.D, NumberInPut, NumberList, out ListOutPut))
                    {
                        UpdateLogs("Read PLC Complete");
                        // Hiển thị dạng [0]False, [1]True,...
                        tbxValueOutput.Text = string.Join(", ", ListOutPut.Select((value, index) => $"[{numberInput + index}]{value}"));
                        //tbxValueOutput.Text = string.Join(", ", ListOutPut);
                    }
                    else
                    {
                        UpdateLogs("Read PLC Faild");
                    }
                    stopwatch.Stop();
                    long executionTimeMs = stopwatch.ElapsedMilliseconds;
                    UpdateLogs($"Time: {executionTimeMs} ms");

                }
                else
                {
                    MessageBox.Show("PLC Disconnect ");
                }
            }
        }
        private void BtReadWord_Click(object sender, RoutedEventArgs e)
        {
            if (this.cbSelectDevice.SelectedValue == null) return;
            if (this.cbSelectDevice.SelectedValue.ToString() == "M")
            {
                string Date = DateTime.Now.ToString("dd/MM/yyyy : HH:mm:ss");
                MessageBox.Show($"{Date} Read Failed \rDescription : Cloud not find any recognizable digits");
            }
            if (this.cbSelectDevice.SelectedValue.ToString() == "D")
            {
                if (!int.TryParse(tbxValueList.Text, out int numberList) || numberList <= 0)
                {
                    MessageBox.Show("Invalid value in ValueList. Please enter a positive number.");
                    return;
                }

                if (!int.TryParse(tbxValueInput.Text, out int numberInput))
                {
                    MessageBox.Show("Invalid value in ValueInput.");
                    return;
                }
                if (UiManager.Instance.PLC == null)
                {
                    MessageBox.Show("PLC Disconnect ");
                    return;
                }
                if (UiManager.Instance.PLC.device.isOpen())
                {
                    tbxValueOutput.Text = "";
                    int NumberList = Convert.ToInt32(this.tbxValueList.Text);
                    int NumberInPut = Convert.ToInt32(this.tbxValueInput.Text);
                    List<short> ListOutPut = new List<short>();
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    if (UiManager.Instance.PLC.device.ReadMultiWord(DeviceCode.D, NumberInPut, NumberList, out ListOutPut))
                    {
                        UpdateLogs("Read PLC Complete");
                        // Hiển thị dạng [0]False, [1]True,...
                        tbxValueOutput.Text = string.Join(", ", ListOutPut.Select((value, index) => $"[{numberInput + index}]{value}"));
                        //tbxValueOutput.Text = string.Join(", ", ListOutPut);
                    }
                    else
                    {
                        UpdateLogs("Read PLC Faild");
                    }
                    stopwatch.Stop();
                    long executionTimeMs = stopwatch.ElapsedMilliseconds;
                    UpdateLogs($"Time: {executionTimeMs} ms");
                }
                else
                {
                    MessageBox.Show("PLC Disconnect ");
                }
            }
        }
        private void BtReadBit_Click(object sender, RoutedEventArgs e)
        {
            if (this.cbSelectDevice.SelectedValue == null) return;
            if (this.cbSelectDevice.SelectedValue.ToString() == "D")
            {
                string Date = DateTime.Now.ToString("dd/MM/yyyy : HH:mm:ss");
                MessageBox.Show($"{Date} Read Failed \rDescription : Cloud not find any recognizable digits");
            }
            if (this.cbSelectDevice.SelectedValue.ToString() == "M")
            {
                if (!int.TryParse(tbxValueList.Text, out int numberList) || numberList <= 0)
                {
                    MessageBox.Show("Invalid value in ValueList. Please enter a positive number.");
                    return;
                }

                if (!int.TryParse(tbxValueInput.Text, out int numberInput))
                {
                    MessageBox.Show("Invalid value in ValueInput.");
                    return;
                }
                if (UiManager.Instance.PLC == null)
                {
                    MessageBox.Show("PLC Disconnect ");
                    return;
                }
                if (UiManager.Instance.PLC.device.isOpen())
                {
                    tbxValueOutput.Text = "";
                    int NumberList = Convert.ToInt32(this.tbxValueList.Text);
                    int NumberInPut = Convert.ToInt32(this.tbxValueInput.Text);
                    List<bool> ListOutPut = new List<bool>();

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    if (UiManager.Instance.PLC.device.ReadMultiBits(DeviceCode.M, NumberInPut, NumberList, out ListOutPut))
                    {
                        // Hiển thị dạng [0]False, [1]True,...
                        tbxValueOutput.Text = string.Join(", ", ListOutPut.Select((value, index) => $"[{numberInput + index}]{value}"));
                        //tbxValueOutput.Text = string.Join(", ", ListOutPut);
                        UpdateLogs("Read PLC Complete");
                    }
                    else
                    {
                        UpdateLogs("Read PLC Faild");
                    }


                    stopwatch.Stop();
                    long executionTimeMs = stopwatch.ElapsedMilliseconds;
                    UpdateLogs($"Time: {executionTimeMs} ms");
                }
                else
                {
                    MessageBox.Show("PLC Disconnect ");
                }

            }
        }
        #endregion
        private void BtLogClear_Click(object sender, RoutedEventArgs e)
        {
            this.txtLogs.Text = "";
        }
        private void PgMechanicalMenu01_Loaded(object sender, RoutedEventArgs e)
        {
            Time_Ticke();
            settingDevice = UiManager.appSetting.settingDevice;
            this.cbSelectDeviceType.SelectedValue = UiManager.appSetting.selectDevice.ToString();

            if (!UiManager.managerSetting.assignSystem.LockSettingplc)
            {
                this.cbSelectDeviceType.IsEnabled = true;
                this.btWriteBit.IsEnabled = true;
                this.btWriteWord.IsEnabled = true;
                this.btWriteDWord.IsEnabled = true;
                this.btWriteString.IsEnabled = true;

            }
            else
            {
                this.cbSelectDeviceType.IsEnabled = false;
                this.btWriteBit.IsEnabled = false;
                this.btWriteWord.IsEnabled = false;
                this.btWriteDWord.IsEnabled = false;
                this.btWriteString.IsEnabled = false;
            }
        }
        public void Time_Ticke()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000); // Thiết lập thời gian lặp (1 giây)
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (UiManager.Instance.PLC != null)
            {
                if (UiManager.Instance.PLC.device.isOpen())
                {
                    this.lbConnect.Content = "PLC Connect Success";
                    this.lbConnect.Background = Brushes.Green;
                }
                else
                {
                    this.lbConnect.Content = "PLC Disconnect";
                    this.lbConnect.Background = Brushes.Red;
                }
            }

        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WndComfirm comfirmYesNo = new WndComfirm();
                if (!comfirmYesNo.DoComfirmYesNo("You Want Save Setting?")) return;
                this.SaveDeviceTypeSetting();
                UiManager.SaveAppSetting();
            }
            catch (Exception ex)
            {
                this.logger.Create("BtnSave_Click: " + ex.Message, LogLevel.Error);
                this.UpdateLogs("Save: " + ex.Message);
            }

        }
        private void SaveDeviceTypeSetting()
        {
            try
            {
                if (this.cbSelectDeviceType.SelectedValue == null) return;
                if (this.cbSelectDeviceType.SelectedValue.ToString() == "Mitsubishi_MC_Protocol_Binary_TCP")
                {

                    UiManager.appSetting.selectDevice = SaveDevice.Mitsubishi_MC_Protocol_Binary_TCP;
                    UpdateLogs($"Save Mitsubishi_MC_Protocol_Binary_TCP to Complete");
                }
                else if (this.cbSelectDeviceType.SelectedValue.ToString() == "Mitsubishi_RS422_SC09")
                {

                    UiManager.appSetting.selectDevice = SaveDevice.Mitsubishi_RS422_SC09;
                    UpdateLogs($"Save Mitsubishi_RS422_SC09 to Complete");
                }
                else if (this.cbSelectDeviceType.SelectedValue.ToString() == "LS_XGTServer_TCP")
                {

                    UiManager.appSetting.selectDevice = SaveDevice.LS_XGTServer_TCP;
                    UpdateLogs($"Save  LS_XGTServer_TCP to Complete");
                }
                else if (this.cbSelectDeviceType.SelectedValue.ToString() == "LS_XGTServer_COM")
                {

                    UiManager.appSetting.selectDevice = SaveDevice.LS_XGTServer_COM;
                    UpdateLogs($"Save  LS_XGTServer_COM to Complete");
                }
                UpdateLogs($"Save to Complete");
            }
            catch (Exception ex)
            {
                this.logger.Create("SaveDeviceTypeSetting: " + ex.Message, LogLevel.Error);
                this.UpdateLogs("SaveDeviceTypeSetting: " + ex.Message);
            }
        }
        private void UpdateLogs(string notify)
        {
            this.Dispatcher.Invoke(() => {
                this.txtLogs.Text += "\r\n" + notify;
                this.txtLogs.ScrollToEnd();
            });
        }
        private void BtnSettingDevice_Click(object sender, RoutedEventArgs e)
        {
            if (this.cbSelectDeviceType.SelectedValue == null) return;
            if (this.cbSelectDeviceType.SelectedValue.ToString() == "Mitsubishi_MC_Protocol_Binary_TCP")
            {
                WndMCTCPSetting wndMC = new WndMCTCPSetting();
                var settingNew = wndMC.DoSettings(Window.GetWindow(this), this.settingDevice.MC_TCP_Binary);
                if (settingNew != null)
                {
                    this.settingDevice.MC_TCP_Binary = settingNew;
                    UpdateLogs($"Device Seting IP :{settingNew.Ip.ToString()},{settingNew.Port}");
                    UpdateLogs($"Device Seting Port :{settingNew.Port}");
                    UpdateLogs($"Click Button Save to Complete");
                }
            }

            else if (this.cbSelectDeviceType.SelectedValue.ToString() == "Mitsubishi_RS422_SC09")
            {
                WndSC09Setting wndMB = new WndSC09Setting();
                var settingNew = wndMB.DoSettings(Window.GetWindow(this), this.settingDevice.sc09Setting);
                if (settingNew != null)
                {
                    this.settingDevice.sc09Setting = settingNew;
                    UpdateLogs($"Device Seting IP :{settingNew.COM.ToString()}");
                    UpdateLogs($"Click Button Save to Complete");
                }
            }
            else if (this.cbSelectDeviceType.SelectedValue.ToString() == "LS_XGTServer_TCP")
            {
                WndMCTCPSetting wndMC = new WndMCTCPSetting();
                var settingNew = wndMC.DoSettings(Window.GetWindow(this), this.settingDevice.LSXGTServerTCPSetting);
                if (settingNew != null)
                {
                    this.settingDevice.MC_TCP_Binary = settingNew;
                    UpdateLogs($"Device Seting IP :{settingNew.Ip.ToString()},{settingNew.Port}");
                    UpdateLogs($"Device Seting Port :{settingNew.Port}");
                    UpdateLogs($"Click Button Save to Complete");
                }
            }
            else if (this.cbSelectDeviceType.SelectedValue.ToString() == "LS_XGTServer_COM")
            {
                WndModbusComSetting wndMC = new WndModbusComSetting();
                var settingNew = wndMC.DoSettings(Window.GetWindow(this), this.settingDevice.XGTServerCOMSetting);
                if (settingNew != null)
                {
                    this.settingDevice.XGTServerCOMSetting = settingNew;
                    UpdateLogs($"Device Seting PortName :{settingNew.portName.ToString()}");
                    UpdateLogs($"Device Seting Parity :{settingNew.parity}");
                    UpdateLogs($"Device Seting Databis :{settingNew.dataBits.ToString()}");
                    UpdateLogs($"Device Seting Stopbit :{settingNew.stopBits.ToString()}");
                    UpdateLogs($"Device Seting Handshake :{settingNew.Handshake.ToString()}");
                    UpdateLogs($"Device Seting Address :{settingNew.AddressSlave.ToString()}");
                    UpdateLogs($"Click Button Save to Complete");
                }
            }
        }
        private void BtnReadBit_Click(object sender, RoutedEventArgs e)
        {
            bool Result = false;
            UiManager.Instance.PLC.device.ReadBit(DeviceCode.M, 1000, out Result);
            if (Result)
            {

                UpdateLogs("  M1000 = true");
            }
            else
            {

                UpdateLogs("  M1000 = fasle");
            }
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (UiManager.Instance.PLC.device != null || UiManager.Instance.PLC.device.isOpen())
            {
                UiManager.Instance.DisconncetPLC();
            }

        }
        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.ConncetPLC();
        }
    }
}
