using ITM_Semiconductor;
using KeyPad;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using static Development.DataDWord_D;

namespace Development
{
    /// <summary>
    /// Interaction logic for DataWord_D.xaml
    /// </summary>
    public partial class DataWord_D : UserControl, IObserverChangeWords, IObserverChangeWord_ZR, IObserverChangeWord_R
    {
        private CancellationTokenSource monitorCancellation;
        private MyLogger logger = new MyLogger("DataWord_D");
        //public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
        //    "Background", typeof(Brush), typeof(DataWord_D), new PropertyMetadata(Brushes.White));
        //public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
        //    "Foreground", typeof(Brush), typeof(DataWord_D), new PropertyMetadata(Brushes.White));
        public static readonly DependencyProperty CodeDeviceProperty = DependencyProperty.Register(
          "CodeDevice", typeof(NameDevice), typeof(DataWord_D), new PropertyMetadata(NameDevice.D));
        public static readonly DependencyProperty DeviceProperty = DependencyProperty.Register(
            "Device", typeof(object), typeof(DataWord_D), new PropertyMetadata(null));
        public static readonly DependencyProperty NoOfDisplayProperty =
            DependencyProperty.Register("NoOfDisplay", typeof(int), typeof(DataWord_D), new PropertyMetadata(8));
        public static readonly DependencyProperty NoOfDecimalDigitsProperty =
            DependencyProperty.Register("NoOfDecimalDigits", typeof(int), typeof(DataWord_D), new PropertyMetadata(0));


        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
        "IsReadOnly", typeof(bool), typeof(DataWord_D), new PropertyMetadata(false));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        "Text", typeof(string), typeof(DataWord_D), new PropertyMetadata("0"));

        //public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
        //    "FontSize", typeof(int), typeof(DataWord_D), new PropertyMetadata(20));

        public static readonly DependencyProperty IsShowInWindowProperty = DependencyProperty.Register(
            "IsShowInWindow", typeof(bool), typeof(DataWord_D), new PropertyMetadata(false));

        public static readonly DependencyProperty IsTabItemProperty = DependencyProperty.Register(
            "IsTabItem", typeof(bool), typeof(DataWord_D), new PropertyMetadata(false));

        public NameDevice CodeDevice
        {
            get { return (NameDevice)GetValue(CodeDeviceProperty); }
            set { SetValue(CodeDeviceProperty, value); }
        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public bool IsTabItem
        {
            get { return (bool)GetValue(IsTabItemProperty); }
            set { SetValue(IsTabItemProperty, value); }
        }

        public bool IsShowInWindow
        {
            get { return (bool)GetValue(IsShowInWindowProperty); }
            set { SetValue(IsShowInWindowProperty, value); }
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }
        //public int FontSize
        //{
        //    get { return (int)GetValue(FontSizeProperty); }
        //    set { SetValue(FontSizeProperty, value); }
        //}
        public int NoOfDisplay
        {
            get { return (int)GetValue(NoOfDisplayProperty); }
            set { SetValue(NoOfDisplayProperty, value); }
        }
        public int NoOfDecimalDigits
        {
            get { return (int)GetValue(NoOfDecimalDigitsProperty); }
            set { SetValue(NoOfDecimalDigitsProperty, value); }
        }
        //public Brush Background
        //{
        //    get { return (Brush)GetValue(BackgroundProperty); }
        //    set { SetValue(BackgroundProperty, value); }
        //}
        //public Brush Foreground
        //{
        //    get { return (Brush)GetValue(ForegroundProperty); }
        //    set { SetValue(ForegroundProperty, value); }
        //}
        public object Device
        {
            get { return GetValue(DeviceProperty); }
            set { SetValue(DeviceProperty, value); }
        }
        public string Value
        {
            get { return _content; }
            set
            {
                if (_content != value)
                {
                    string oldValue = _content;
                    _content = value;
                    OnPropertyChanged(nameof(Value));
                    OnContentChanged(oldValue, _content);
                    ContentChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        private string _content;
        public event EventHandler ContentChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isShowKeypad;
        private bool isInTabItem;
        private NotifyPLCWord notifyPLCWord = new NotifyPLCWord();
        private NotifyPLCWord_ZR notifyPLCWord_ZR = new NotifyPLCWord_ZR();
        private NotifyPLCWord_R notifyPLCWord_R = new NotifyPLCWord_R();
        public DataWord_D()
        {
            InitializeComponent();
            this.isShowKeypad = false;
            this.Loaded += TxtReadOnlyWord_Loaded;
            this.Unloaded += TxtReadOnlyWord_Unloaded;
        }

        private void TxtReadOnlyWord_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                {
                    return;
                }
                if (this.isInTabItem) return;
                this.Unregister();
                this.monitorCancellation?.Cancel();
                if (this.isShowKeypad) return;
                this.RemoveDevice();
            }
            catch (Exception ex)
            {
                this.logger.Create("TxtReadOnlyWord_Unloaded: " + ex.Message, LogLevel.Error);
            }
        }

        private void TxtReadOnlyWord_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.isInTabItem) return;
                this.Initial();
                this.RemoveDevice();
                this.LoadNotifyPLCWord();
                this.RegisterNotify();
                this.monitorCancellation = new CancellationTokenSource();
                this.AddDevice();
                this.isInTabItem = this.IsTabItem;
            }
            catch (Exception ex)
            {
                this.logger.Create("TxtReadOnlyWord_Loaded: " + ex.Message, LogLevel.Error);
            }
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DataWord_D;
            control?.OnContentChanged();
        }
        protected virtual void OnContentChanged()
        {
            ContentChanged?.Invoke(this, EventArgs.Empty);
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void AddDevice()
        {
            if (this.Device == null) return;
            if (CodeDevice == NameDevice.D)
            {
                var address = ushort.Parse(this.Device.ToString());
                UiManager.Instance.PLC.AddAddressDeviceWord_D(address);
            }
            else if (CodeDevice == NameDevice.ZR)
            {
                var address = ushort.Parse(this.Device.ToString());
                UiManager.Instance.PLC.AddAddressDeviceWord_ZR(address);
            }
            else if (CodeDevice == NameDevice.R)
            {
                var address = ushort.Parse(this.Device.ToString());
                UiManager.Instance.PLC.AddAddressDeviceWord_R(address);
            }
            else
            {
                return;
            }
        }
        private void RemoveDevice()
        {
            if (this.Device == null) return;
          
            if (CodeDevice == NameDevice.D)
            {
                var address = ushort.Parse(this.Device.ToString());
                UiManager.Instance.PLC.RemoveAddressDeviceWord_D(address);
            }
            else if (CodeDevice == NameDevice.ZR)
            {
                var address = ushort.Parse(this.Device.ToString());
                UiManager.Instance.PLC.RemoveAddressDeviceWord_ZR(address);
            }
            else if (CodeDevice == NameDevice.R)
            {
                var address = ushort.Parse(this.Device.ToString());
                UiManager.Instance.PLC.RemoveAddressDeviceWord_R(address);
            }
            else
            {
                return;
            }
           
        }
        private void LoadNotifyPLCWord()
        {
            if (CodeDevice == NameDevice.D)
            {
                this.notifyPLCWord = SystemsManager.Instance.NotifyPLCWord;
            }
            else if (CodeDevice == NameDevice.ZR)
            {
                this.notifyPLCWord_ZR = SystemsManager.Instance.NotifyPLCWord_ZR;
            }
            else if (CodeDevice == NameDevice.R)
            {
                this.notifyPLCWord_R = SystemsManager.Instance.NotifyPLCWord_R;
            }
        }
        private void RegisterNotify()
        {
            if (CodeDevice == NameDevice.D)
            {
                this.notifyPLCWord.Attach(this);
            }
            else if (CodeDevice == NameDevice.ZR)
            {
                this.notifyPLCWord_ZR.Attach(this);
            }
            else if (CodeDevice == NameDevice.R)
            {
                this.notifyPLCWord_R.Attach(this);
            }
        }
        private void Unregister()
        {

            if (CodeDevice == NameDevice.D)
            {
                this.notifyPLCWord.Detach(this);
            }
            else if (CodeDevice == NameDevice.ZR)
            {
                this.notifyPLCWord_ZR.Detach(this);
            }
            else if (CodeDevice == NameDevice.R)
            {
                this.notifyPLCWord_R.Detach(this);
            }
        }
        private void Initial()
        {
            var value = this.convertValue(Convert.ToDouble(0), NoOfDisplay, NoOfDecimalDigits);
            txt.Text = value;
        }
        public string convertValue(double value, int totalDigitsBeforeDecimal, int totalDigitsAfterDecimal)
        {
            string formatString = "";
            try
            {
                string numberString = value.ToString();
                string numberStringSource = value.ToString();
                numberString = numberString.Replace('-', ' ');
                numberString = numberString.Trim();
                formatString = numberString.PadLeft(totalDigitsBeforeDecimal + totalDigitsAfterDecimal, '0');

                // Tìm vị trí của dấu thập phân và chèn dấu thập phân
                int decimalPointIndex = formatString.Length - totalDigitsAfterDecimal;
                if (decimalPointIndex >= 0 && decimalPointIndex < formatString.Length)
                {
                    formatString = formatString.Insert(decimalPointIndex, ".");
                }
                if (numberStringSource.Contains("-"))
                {
                    formatString = "-" + formatString;
                }
            }
            catch (Exception ex)
            {
                this.logger.Create("convertValue: " + ex.Message, LogLevel.Error);
            }
            return this.FillString(formatString);
        }
        private string FillString(string input)
        {
            int indexOfMinus = input.IndexOf('-');
            if (indexOfMinus != -1)
            {
                string afterMinus = input.Substring(indexOfMinus + 1).TrimStart('0');

                if (afterMinus.StartsWith("."))
                {
                    afterMinus = "0" + afterMinus;
                }

              
                if (string.IsNullOrEmpty(afterMinus))
                {
                    afterMinus = "0";
                }

                return input.Substring(0, indexOfMinus + 1) + afterMinus;
            }
            else
            {
                string result = input.TrimStart('0');

                if (result.StartsWith("."))
                {
                    result = "0" + result;
                }

                
                if (string.IsNullOrEmpty(result))
                {
                    result = "0";
                }

                return result;
            }
        }
        private void txt_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox txt = sender as TextBox;

            if (txt.IsReadOnly) return;
            this.isShowKeypad = true;
            string numberBefore = txt.Text;

            Keypad keyboardWindow = new Keypad(true, txt.Text);
            if (keyboardWindow.ShowDialog() == true)
            {
                txt.Text = keyboardWindow.Result;

            }
            var number = txt.Text;
            string x = "1";
            string y = x.PadRight(this.NoOfDecimalDigits + 1, '0');
            

            var numberData = number; ;

            Text = txt.Text;
            if (string.IsNullOrEmpty(numberData)) return;
            int value = (int)(Convert.ToDouble(numberData) * Convert.ToInt16(y));
            var address = ushort.Parse(this.Device.ToString());
           
            //// SELECT DEVICE AND WRITE VALUE
            if (CodeDevice == NameDevice.D)
            {
                if (UiManager.Instance.PLC.device.WriteWord(DeviceCode.D, address, value))
                {
                    this.EventLog(address + " Changed value Word D: ", $"{numberBefore} > {value.ToString()}");
                }
            }
            else if (CodeDevice == NameDevice.ZR)
            {
                if (UiManager.Instance.PLC.device.WriteWord(DeviceCode.ZR, address, value))
                {
                    this.EventLog(address + " Changed value Word ZR: ", $"{numberBefore} > {value.ToString()}");
                }
            }
            else if (CodeDevice == NameDevice.R)
            {
                if (UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, address, value))
                {
                    this.EventLog(address + " Changed value Word R: ", $"{numberBefore} > {value.ToString()}");
                }
            }
            else
            {
                return;
            }


            this.isShowKeypad = false;



            
        }
        private void txt_PreviewTouchDown(object sender, TouchEventArgs e)
        {

            TextBox txt = sender as TextBox;

            if (txt.IsReadOnly) return;
            this.isShowKeypad = true;
            string numberBefore = txt.Text;

            Keypad keyboardWindow = new Keypad(true, txt.Text);
            if (keyboardWindow.ShowDialog() == true)
            {
                txt.Text = keyboardWindow.Result;

            }
            var number = txt.Text;
            string x = "1";
            string y = x.PadRight(this.NoOfDecimalDigits + 1, '0');


            var numberData = number; ;

            Text = txt.Text;
            if (string.IsNullOrEmpty(numberData)) return;
            int value = (int)(Convert.ToDouble(numberData) * Convert.ToInt16(y));
            var address = ushort.Parse(this.Device.ToString());

            //// SELECT DEVICE AND WRITE VALUE
            if (CodeDevice == NameDevice.D)
            {
                if (UiManager.Instance.PLC.device.WriteDoubleWord(DeviceCode.D, address, value))
                {
                    this.EventLog(address + " Changed value Word D: ", $"{numberBefore} > {value.ToString()}");
                }
            }
            else if (CodeDevice == NameDevice.ZR)
            {
                if (UiManager.Instance.PLC.device.WriteDoubleWord(DeviceCode.ZR, address, value))
                {
                    this.EventLog(address + " Changed value Word ZR: ", $"{numberBefore} > {value.ToString()}");
                }
            }
            else if (CodeDevice == NameDevice.R)
            {
                if (UiManager.Instance.PLC.device.WriteDoubleWord(DeviceCode.R, address, value))
                {
                    this.EventLog(address + " Changed value Word R: ", $"{numberBefore} > {value.ToString()}");
                }
            }
            else
            {
                return;
            }
            this.isShowKeypad = false;

        }
        private void EventLog(string message, string type)
        {
            try
            {
                DbWrite.createEventLog($"Word D :{message}", type);
            }
            catch (Exception ex)
            {
                logger.Create("AlarmLog: " + ex.Message, LogLevel.Error);
            }
        }
        
        public void NotifyChangeWord(string key, short value)
        {
            Dispatcher.Invoke(() =>
            {
                if (this.Device == null) return;
                if (key != this.Device.ToString()) return;
                var data = this.convertValue(Convert.ToDouble(value), NoOfDisplay, NoOfDecimalDigits);
                if (!isShowKeypad)
                {
                    if (this.CodeDevice == NameDevice.D)
                    {
                        txt.Text = data;
                        if (Text != data)
                        {
                            Text = data;
                        }
                    }

                }
            });
        }
        public void NotifyChangeWord_ZR(string key, int value)
        {
            Dispatcher.Invoke(() =>
            {
                if (this.Device == null) return;
                if (key != this.Device.ToString()) return;
                var data = this.convertValue(Convert.ToDouble(value), NoOfDisplay, NoOfDecimalDigits);
                if (!isShowKeypad)
                {
                    if (this.CodeDevice == NameDevice.ZR)
                    {
                        txt.Text = data;
                        if (Text != data)
                        {
                            Text = data;
                        }
                    }

                }
            });
        }
        public void NotifyChangeWord_R(string key, int value)
        {
            Dispatcher.Invoke(() =>
            {
                if (this.Device == null) return;
                if (key != this.Device.ToString()) return;
                var data = this.convertValue(Convert.ToDouble(value), NoOfDisplay, NoOfDecimalDigits);
                if (!isShowKeypad)
                {
                    if (this.CodeDevice == NameDevice.R)
                    {
                        txt.Text = data;
                        if (Text != data)
                        {
                            Text = data;
                        }
                    }

                }
            });
        }
        
    }
}
