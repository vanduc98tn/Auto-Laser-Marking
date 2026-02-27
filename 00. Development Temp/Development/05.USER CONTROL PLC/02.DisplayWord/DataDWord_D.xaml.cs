using System;
using System.Collections.Generic;
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
using ITM_Semiconductor;
using KeyPad;
namespace Development
{
    /// <summary>
    /// Interaction logic for DataDWord_D.xaml
    /// </summary>
    public partial class DataDWord_D : UserControl, IObserverChangeDWord, IObserverChangeDWord_ZR, IObserverChangeDWord_R
    {
        public enum NameDevice
        { 
            R,
            D,
            ZR,
        }
        private CancellationTokenSource monitorCancellation;

        public static readonly DependencyProperty CodeDeviceProperty = DependencyProperty.Register(
           "CodeDevice", typeof(NameDevice), typeof(DataDWord_D), new PropertyMetadata(NameDevice.D));
        //public static readonly DependencyProperty BackgroundPropertyUC = DependencyProperty.Register(
        //    "Background", typeof(Brush), typeof(DataDWord_D), new PropertyMetadata(Brushes.White));
        //public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
        //    "Foreground", typeof(Brush), typeof(DataDWord_D), new PropertyMetadata(Brushes.White));

        public static readonly DependencyProperty DeviceProperty = DependencyProperty.Register(
            "Device", typeof(object), typeof(DataDWord_D), new PropertyMetadata(null));


        //public static readonly DependencyProperty NoOfDisplayProperty =
        //    DependencyProperty.Register("NoOfDisplay", typeof(int), typeof(DataDWord_D), new PropertyMetadata(8));
        //public static readonly DependencyProperty NoOfDecimalDigitsProperty =
        //    DependencyProperty.Register("NoOfDecimalDigits", typeof(int), typeof(DataDWord_D), new PropertyMetadata(0));


        public static readonly DependencyProperty NoOfDisplayProperty =
             DependencyProperty.Register("NoOfDisplay", typeof(int), typeof(DataDWord_D),
        new PropertyMetadata(8, OnFormatPropertyChanged));

        public static readonly DependencyProperty NoOfDecimalDigitsProperty =
            DependencyProperty.Register("NoOfDecimalDigits", typeof(int), typeof(DataDWord_D),
                new PropertyMetadata(0, OnFormatPropertyChanged));


        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
        "IsReadOnly", typeof(bool), typeof(DataDWord_D), new PropertyMetadata(false));
        //public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
        //    "FontSize", typeof(int), typeof(DataDWord_D), new PropertyMetadata(20));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(DataDWord_D), new PropertyMetadata("0", OnContentChanged));

        public static readonly DependencyProperty IsShowInWindowProperty = DependencyProperty.Register(
            "IsShowInWindow", typeof(bool), typeof(DataDWord_D), new PropertyMetadata(false));

        public static readonly DependencyProperty IsTabItemProperty = DependencyProperty.Register(
            "IsTabItem", typeof(bool), typeof(DataDWord_D), new PropertyMetadata(false));

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



        private bool isShowKeypad;
        private bool isInTabItem;
        private NotifyPLCDWord notifyPLCDWord = new NotifyPLCDWord();
        private NotifyPLCDWord_ZR notifyPLCDWord_ZR = new NotifyPLCDWord_ZR();
        private NotifyPLCDWord_R notifyPLCDWord_R = new NotifyPLCDWord_R();


        private MyLogger logger = new MyLogger("DataDWord_D");
        public event EventHandler ContentChanged;

        private HashSet<TouchDevice> activeTouchDevices = new HashSet<TouchDevice>();
        public DataDWord_D()
        {
            InitializeComponent();
            this.Loaded += TxtReadOnlyDWord_Loaded;
            this.Unloaded += TxtReadOnlyDWord_Unloaded;
        }
        private static void OnFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DataDWord_D;
            control?.UpdateDefaultText();
        }
        private void UpdateDefaultText()
        {
            if (txt == null) return;

            string zeroString = new string('0', NoOfDisplay);

            if (NoOfDecimalDigits > 0)
            {
                zeroString += "." + new string('0', NoOfDecimalDigits);
            }

            txt.Text = zeroString;
        }
        private void TxtReadOnlyDWord_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                {
                    return;
                }
                if (this.isInTabItem) return;
                this.UnregisterNotify();
                this.monitorCancellation?.Cancel();
                if (this.IsShowInWindow) return;
                this.RemoveDevice();
            }
            catch (Exception ex)
            {
                this.logger.Create("TxtReadOnlyWord_Unloaded: " + ex.Message, LogLevel.Error);
            }
        }
        private void TxtReadOnlyDWord_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.isInTabItem) return;
                this.RemoveDevice();
                this.Initial();
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
            var control = d as DataDWord_D;
            control?.OnContentChanged(e);
        }
        protected virtual void OnContentChanged(DependencyPropertyChangedEventArgs e)
        {
            ContentChanged?.Invoke(this, EventArgs.Empty);
        }
        private void AddDevice()
        {
            if (this.Device == null) return;
            if (CodeDevice == NameDevice.D)
            {
                var address = ushort.Parse(this.Device.ToString());
                UiManager.Instance.PLC.AddAddressDeviceDWord_D(address);
            }
            else if (CodeDevice == NameDevice.ZR)
            {
                var address = ushort.Parse(this.Device.ToString());
                UiManager.Instance.PLC.AddAddressDeviceDWord_ZR(address);
            }
            else if (CodeDevice == NameDevice.R)
            {
                var address = ushort.Parse(this.Device.ToString());
                UiManager.Instance.PLC.AddAddressDeviceDWord_R(address);
            }
            else
            {
                return;
            }

        }
        private void RemoveDevice()
        {
            if (this.Device == null) return;
            if(CodeDevice == NameDevice.D)
            {
                var address = ushort.Parse(this.Device.ToString());
                UiManager.Instance.PLC.RemoveAddressDeviceDWord_D(address);
            }    
            else if (CodeDevice == NameDevice.ZR)
            {
                var address = ushort.Parse(this.Device.ToString());
                UiManager.Instance.PLC.RemoveAddressDeviceDWord_ZR(address);
            }
            else if (CodeDevice == NameDevice.R)
            {
                var address = ushort.Parse(this.Device.ToString());
                UiManager.Instance.PLC.RemoveAddressDeviceDWord_R(address);
            }
            else
            {
                return;
            }    
           
        }
        private void Initial()
        {
            var value = this.convertValue(Convert.ToDouble(0), NoOfDisplay, NoOfDecimalDigits);
            txt.Text = value;
            if(CodeDevice == NameDevice.D)
            {
                this.notifyPLCDWord = SystemsManager.Instance.NotifyPLCDWord;
            }    
            else if (CodeDevice == NameDevice.ZR)
            {
                this.notifyPLCDWord_ZR = SystemsManager.Instance.NotifyPLCDWord_ZR;
            }
            else if (CodeDevice == NameDevice.R)
            {
                this.notifyPLCDWord_R = SystemsManager.Instance.NotifyPLCDWord_R;
            }



            this.RegisterNotify();
        }
        private void RegisterNotify()
        {
            if (CodeDevice == NameDevice.D)
            {
                this.notifyPLCDWord.Attach(this);
            }
            else if (CodeDevice == NameDevice.ZR)
            {
                this.notifyPLCDWord_ZR.Attach(this);
            }
            else if (CodeDevice == NameDevice.R)
            {
                this.notifyPLCDWord_R.Attach(this);
            }
        }
        private void UnregisterNotify()
        {
            if (CodeDevice == NameDevice.D)
            {
                this.notifyPLCDWord.Detach(this);
            }
            else if (CodeDevice == NameDevice.ZR)
            {
                this.notifyPLCDWord_ZR.Detach(this);
            }
            else if (CodeDevice == NameDevice.R)
            {
                this.notifyPLCDWord_R.Detach(this);
            }
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
        private void EventLog(string message, string type)
        {
            try
            {
                DbWrite.createEventLog($"DWord D :{message}", type);
            }
            catch (Exception ex)
            {
                logger.Create("AlarmLog: " + ex.Message, LogLevel.Error);
            }
        }
        public void NotifyChangeDWord(string key, int value)
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

        public void NotifyChangeDWord_ZR(string key, int value)
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
        public void NotifyChangeDWord_R(string key, int value)
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
        private void Button_LostTouchCapture(object sender, TouchEventArgs e)
        {


            if (!IsTouchOverButton(e.TouchDevice, (UIElement)sender))
            {

                ((UIElement)sender).ReleaseTouchCapture(e.TouchDevice);
                e.Handled = true;
            }
        }
        private void txt_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox txt = sender as TextBox;


            if (txt.IsReadOnly || isShowKeypad) return;


            if (e.StylusDevice != null && e.StylusDevice.TabletDevice.Type == TabletDeviceType.Touch)
            {
                return;
            }

            isShowKeypad = true;
            string numberBefore = txt.Text;

            Keypad keyboardWindow = new Keypad(false);
            if (keyboardWindow.ShowDialog() == true)
            {
                txt.Text = keyboardWindow.Result;
            }

            var number = txt.Text;
            string x = "1";
            string y = x.PadRight(this.NoOfDecimalDigits + 1, '0');
            var numberData = number;

            Text = txt.Text;
            if (string.IsNullOrEmpty(numberData))
            {
                isShowKeypad = false;
                return;
            }

            int value = (int)(Convert.ToDouble(numberData) * Convert.ToInt16(y));
            var address = ushort.Parse(this.Device.ToString());

            //// SELECT DEVICE AND WRITE VALUE
            if (CodeDevice == NameDevice.D)
            {
                if (UiManager.Instance.PLC.device.WriteDoubleWord(DeviceCode.D, address, value))
                {
                    this.EventLog(address + " Changed value DoubleWord D: ", $"{numberBefore} > {value.ToString()}");
                }
            }
            else if (CodeDevice == NameDevice.ZR)
            {
                if (UiManager.Instance.PLC.device.WriteDoubleWord(DeviceCode.ZR, address, value))
                {
                    this.EventLog(address + " Changed value DoubleWord ZR: ", $"{numberBefore} > {value.ToString()}");
                }
            }
            else if (CodeDevice == NameDevice.R)
            {
                if (UiManager.Instance.PLC.device.WriteDoubleWord(DeviceCode.R, address, value))
                {
                    this.EventLog(address + " Changed value DoubleWord R: ", $"{numberBefore} > {value.ToString()}");
                }
            }
            else
            {
                return;
            }
            

            isShowKeypad = false;
            e.Handled = true;
        }
        private void txt_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt.IsReadOnly || isShowKeypad) return;

            isShowKeypad = true;
            string numberBefore = txt.Text;

            Keypad keyboardWindow = new Keypad(true);
            if (keyboardWindow.ShowDialog() == true)
            {
                txt.Text = keyboardWindow.Result;
            }

            var number = txt.Text;
            string x = "1";
            string y = x.PadRight(this.NoOfDecimalDigits + 1, '0');
            var numberData = number;

            Text = txt.Text;
            if (string.IsNullOrEmpty(numberData))
            {
                isShowKeypad = false;
                return;
            }

            int value = (int)(Convert.ToDouble(numberData) * Convert.ToInt16(y));
            var address = ushort.Parse(this.Device.ToString());

            //// SELECT DEVICE AND WRITE VALUE
            if (CodeDevice == NameDevice.D)
            {
                if (UiManager.Instance.PLC.device.WriteDoubleWord(DeviceCode.D, address, value))
                {
                    this.EventLog(address + " Changed value DoubleWord D: ", $"{numberBefore} > {value.ToString()}");
                }
            }
            else if (CodeDevice == NameDevice.ZR)
            {
                if (UiManager.Instance.PLC.device.WriteDoubleWord(DeviceCode.ZR, address, value))
                {
                    this.EventLog(address + " Changed value DoubleWord ZR: ", $"{numberBefore} > {value.ToString()}");
                }
            }
            else if (CodeDevice == NameDevice.R)
            {
                if (UiManager.Instance.PLC.device.WriteDoubleWord(DeviceCode.R, address, value))
                {
                    this.EventLog(address + " Changed value DoubleWord R: ", $"{numberBefore} > {value.ToString()}");
                }
            }
            else
            {
                return;
            }


            isShowKeypad = false;
            e.Handled = true; 
        }
        private void txt_TouchUp(object sender, TouchEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt.IsFocused) 
            {
                txt.ReleaseTouchCapture(e.TouchDevice);
            }
            e.Handled = true; 
        }
        private bool IsTouchOverButton(TouchDevice touchDevice, UIElement element)
        {
            Point pos = touchDevice.GetTouchPoint(element).Position;
            return pos.X >= 0 && pos.X <= element.RenderSize.Width && pos.Y >= 0 && pos.Y <= element.RenderSize.Height;
        }
        private void txt_LostTouchCapture(object sender, TouchEventArgs e)
        {
            if (activeTouchDevices.Contains(e.TouchDevice))
            {
                activeTouchDevices.Remove(e.TouchDevice);
                isShowKeypad = false;
                e.Handled = true;
            }
        }
    }
}
