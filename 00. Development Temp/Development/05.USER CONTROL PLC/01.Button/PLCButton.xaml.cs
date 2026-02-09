using OfficeOpenXml;
using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace Development
{
    /// <summary>
    /// Interaction logic for PLCButton.xaml
    /// </summary>
    public partial class PLCButton : UserControl, IObserverChangeBits
    {
        public static readonly DependencyProperty ActionProperty = DependencyProperty.Register(
            "Action", typeof(ActionType), typeof(PLCButton), new PropertyMetadata(ActionType.Alternative));

        public static readonly DependencyProperty DeviceLampProperty = DependencyProperty.Register(
            "DeviceLamp", typeof(DeviceCode), typeof(PLCButton), new PropertyMetadata(DeviceCode.M));

        public static readonly DependencyProperty DeviceWriteProperty = DependencyProperty.Register(
            "DeviceWrite", typeof(DeviceCode), typeof(PLCButton), new PropertyMetadata(DeviceCode.M));

        public static readonly DependencyProperty AddressWriteProperty = DependencyProperty.Register(
            "AddressWrite", typeof(object), typeof(PLCButton), new PropertyMetadata(null));

        public static readonly DependencyProperty AddressLampProperty = DependencyProperty.Register(
            "AddressLamp", typeof(object), typeof(PLCButton), new PropertyMetadata(null));

        public static new readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
            "FontSize", typeof(int), typeof(PLCButton), new PropertyMetadata(20));

        public static new readonly DependencyProperty ContentProperty = DependencyProperty.Register(
             "Content", typeof(object), typeof(PLCButton),
             new PropertyMetadata("Name Button", OnContentChanged));

        public static new readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            "Foreground", typeof(Brush), typeof(PLCButton), new PropertyMetadata(Brushes.White));

        public static readonly DependencyProperty UseLampCoditionProperty =
            DependencyProperty.Register("UseLampCodition", typeof(bool), typeof(PLCButton), new PropertyMetadata(false));

        public static readonly DependencyProperty BackgroundLampONProperty = DependencyProperty.Register(
            "BackgroundLampON", typeof(Brush), typeof(PLCButton), new PropertyMetadata(Brushes.Green));

        public static readonly DependencyProperty BackgroundLampOFFProperty = DependencyProperty.Register(
            "BackgroundLampOFF", typeof(Brush), typeof(PLCButton), new PropertyMetadata(Brushes.LightGray));

        public static readonly DependencyProperty UseImageProperty = DependencyProperty.Register(
            "UseImage", typeof(bool), typeof(PLCButton), new PropertyMetadata(false));

        public static readonly DependencyProperty ImageOFFProperty = DependencyProperty.Register(
            "ImageOFF", typeof(string), typeof(PLCButton), new PropertyMetadata(""));

        public static readonly DependencyProperty ImageONProperty = DependencyProperty.Register(
            "ImageON", typeof(string), typeof(PLCButton), new PropertyMetadata(""));

        public static readonly DependencyProperty TextOFFProperty = DependencyProperty.Register(
            "TextOFF", typeof(string), typeof(PLCButton), new PropertyMetadata("", OnTextOFFChanged));

        public static readonly DependencyProperty TextONProperty = DependencyProperty.Register(
            "TextON", typeof(string), typeof(PLCButton), new PropertyMetadata(""));

        public static readonly DependencyProperty ImageSourceProperty =
        DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(PLCButton));

        public static readonly DependencyProperty IsShowInWindowProperty = DependencyProperty.Register(
            "IsShowInWindow", typeof(bool), typeof(PLCButton), new PropertyMetadata(false));

        public static readonly DependencyProperty IsTabItemProperty = DependencyProperty.Register(
            "IsTabItem", typeof(bool), typeof(PLCButton), new PropertyMetadata(false));

        public static readonly DependencyProperty IsComfirm = DependencyProperty.Register(
           "IsComfirm", typeof(bool), typeof(PLCButton), new PropertyMetadata(false));

        public static readonly DependencyProperty IsMesComfirmProperty = DependencyProperty.Register(
           "IsMesComfirm", typeof(string), typeof(PLCButton), new PropertyMetadata(""));

        public static new readonly DependencyProperty HorizontalContentAlignmentProperty = DependencyProperty.Register(
           "HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(PLCButton), new PropertyMetadata(HorizontalAlignment.Center));

        public static new readonly DependencyProperty VerticalContentAlignmentProperty = DependencyProperty.Register(
           "VerticalContentAlignment", typeof(VerticalAlignment), typeof(PLCButton), new PropertyMetadata(VerticalAlignment.Center));

        public static new readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
           "Padding", typeof(Thickness), typeof(PLCButton), new PropertyMetadata(new Thickness(0)));


        public new Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }
        public new VerticalAlignment VerticalContentAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }
        public new HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }
        public string IsMesComfrim
        {
            get { return (string)GetValue(IsMesComfirmProperty); }
            set { SetValue(IsMesComfirmProperty, value); }
        }
        public bool IsComfrim
        {
            get { return (bool)GetValue(IsComfirm); }
            set { SetValue(IsComfirm, value); }
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

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public ActionType Action
        {
            get { return (ActionType)GetValue(ActionProperty); }
            set { SetValue(ActionProperty, value); }
        }

        public DeviceCode DeviceLamp
        {
            get { return (DeviceCode)GetValue(DeviceLampProperty); }
            set { SetValue(DeviceLampProperty, value); }
        }

        public DeviceCode DeviceWrite
        {
            get { return (DeviceCode)GetValue(DeviceWriteProperty); }
            set { SetValue(DeviceWriteProperty, value); }
        }

        public object TextOFF
        {
            get { return GetValue(TextOFFProperty); }
            set { SetValue(TextOFFProperty, value); }
        }
        public object TextON
        {
            get { return GetValue(TextONProperty); }
            set { SetValue(TextONProperty, value); }
        }
        public bool UseImage
        {
            get { return (bool)GetValue(UseImageProperty); }
            set { SetValue(UseImageProperty, value); }
        }

        public string ImageON
        {
            get { return (string)GetValue(ImageONProperty); }
            set { SetValue(ImageONProperty, value); }
        }
        public string ImageOFF
        {
            get { return (string)GetValue(ImageOFFProperty); }
            set { SetValue(ImageOFFProperty, value); }
        }
        public new int FontSize
        {
            get { return (int)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public Brush BackgroundLampON
        {
            get { return (Brush)GetValue(BackgroundLampONProperty); }
            set { SetValue(BackgroundLampONProperty, value); }
        }

        public Brush BackgroundLampOFF
        {
            get { return (Brush)GetValue(BackgroundLampOFFProperty); }
            set { SetValue(BackgroundLampOFFProperty, value); }
        }

        public new Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public new object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        public bool UseLampCodition
        {
            get { return (bool)GetValue(UseLampCoditionProperty); }
            set { SetValue(UseLampCoditionProperty, value); }
        }

        public object AddressWrite
        {
            get { return GetValue(AddressWriteProperty); }
            set { SetValue(AddressWriteProperty, value); }
        }

        public object AddressLamp
        {
            get { return GetValue(AddressLampProperty); }
            set { SetValue(AddressLampProperty, value); }
        }

        public event RoutedEventHandler Clicked;

        private bool isToggled = false;
        private NotifyPLCBits notifyPLCBits = new NotifyPLCBits();
        private CancellationTokenSource monitorCancellation;
        private DispatcherTimer timer;
        private bool isInTabItem;
        private MyLogger logger = new MyLogger("AlternativeBtn");


        private bool isMousePressed = false;
        private HashSet<TouchDevice> activeTouchDevices = new HashSet<TouchDevice>();
        private Button btnTarget = null;

        public PLCButton()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += Timer_Tick;
            InitializeComponent();
            this.Loaded += AlternativeButtonUserControl_Loaded;
            this.Unloaded += AlternativeButtonUserControl_Unloaded;
        }
        private static void OnTextOFFChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PLCButton)d;
            if (control.btn != null)
            {
                control.btn.Content = e.NewValue;
            }
        }
        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PLCButton)d;
            if (control.btn != null)
            {
                control.btn.Content = e.NewValue;
            }
        }
        protected virtual void OnClicked()
        {
            Clicked?.Invoke(this, new RoutedEventArgs());
        }
        private void AlternativeButtonUserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                // Bỏ qua các hành động trong chế độ Design
                return;
            }
            if (this.isInTabItem) return;
            this.DeleteDevice(this.IsShowInWindow);
            this.StopTimer();
            this.UnregisterNotifyBits();
        }
        private void DeleteDevice(bool cmd)
        {
            if (this.UseLampCodition)
            {
                this.monitorCancellation?.Cancel();
                if (this.AddressLamp == null) return;
                var address = ushort.Parse(this.AddressLamp.ToString());
                if (!cmd)
                {
                    this.RemoveAddress(address);
                }
            }
            if (Action == ActionType.Alternative)
            {
                this.monitorCancellation?.Cancel();
                if (this.AddressWrite == null) return;
                var address = ushort.Parse(this.AddressWrite.ToString());
                if (!cmd)
                {
                    UiManager.Instance.PLC.RemoveBitAddress(this.DeviceWrite.ToString(), address);
                }
            }
        }
        private void AlternativeButtonUserControl_Loaded(object sender, RoutedEventArgs e)
        {

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                // Bỏ qua các hành động trong chế độ Design
                return;
            }
            if (this.isInTabItem) return;
            this.DeleteDevice(false);
            Initial();
            this.RegisterNotifyBits();
            this.monitorCancellation = new CancellationTokenSource();
            if (this.UseLampCodition)
            {
                if (this.AddressLamp == null) return;
                var address = ushort.Parse(this.AddressLamp.ToString());
                this.AddAddress(address);
            }
            if (Action == ActionType.Alternative)
            {
                if (this.AddressWrite == null) return;
                var address = ushort.Parse(this.AddressWrite.ToString());
                UiManager.Instance.PLC.AddBitAddress(this.DeviceWrite.ToString(), address);
            }
            this.isInTabItem = this.IsTabItem;
        }
        private void Initial()
        {
            if (TextON == (object)string.Empty)
            {
                TextON = Content;
            }
            if (TextOFF == (object)string.Empty)
            {
                TextOFF = Content;
            }
            this.btn.Background = BackgroundLampOFF;
            this.btn.Content = TextOFF;
            this.SetImagePath(ImageOFF);
        }
        private void RegisterNotifyBits()
        {
            this.notifyPLCBits = SystemsManager.Instance.NotifyPLCBits;
            if (this.notifyPLCBits == null) return;
            this.notifyPLCBits.Attach(this);
        }
        private void UnregisterNotifyBits()
        {
            this.notifyPLCBits.Detach(this);
        }
        private void ChangeBrushLamp(bool status, Button btn)
        {

            if (!status)
            {
                this.btn.Background = BackgroundLampOFF;
                this.btn.Content = TextOFF;
                this.SetImagePath(ImageOFF);
            }
            else
            {
                this.btn.Background = BackgroundLampON;
                this.btn.Content = TextON;
                this.SetImagePath(ImageON);
            }
        }


        private void SetImagePath(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;
            if (!this.UseImage) return;
            // Tạo một Uri từ đường dẫn ảnh
            Uri imageUri = new Uri(imagePath, UriKind.RelativeOrAbsolute);

            // Tạo một BitmapImage và gán vào Image
            BitmapImage bitmapImage = new BitmapImage(imageUri);
            this.img.Source = bitmapImage;
        }
        private void EventLog(string message, string type)
        {
            try
            {
                UserManager.createUserLog(UserActions.CLICK_BTN, $"{message}:{type}");
            }
            catch (Exception ex)
            {
                logger.Create("AlarmLog: " + ex.Message, LogLevel.Error);
            }
        }
        public void NotifyChangeBits(string key, bool status)
        {
            Dispatcher.Invoke(() =>
            {
                if (this.UseLampCodition)
                {
                    if (this.AddressLamp != null)
                    {
                        var device = this.DeviceLamp.ToString() + this.AddressLamp.ToString();
                        if (device == key)
                        {
                            ChangeBrushLamp(status, this.btn);
                        }
                    }

                }
                this.isToggled = status ? true : false;

                if (this.AddressWrite == null) return;
                if (this.DeviceWrite.ToString() + this.AddressWrite.ToString() != key) return;
                //if ("M" + this.AddressWrite.ToString() != key) return;

            });

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnClicked();
            if (this.Action == ActionType.Momentary) return;


            if (this.Action == ActionType.Alternative)
            {
                this.Alternative_Click();
            }
            else if (this.Action == ActionType.Set)
            {
                this.Set_Click();
            }
            else if (this.Action == ActionType.Reset)
            {
                this.Reset_Click();
            }
        }
        private void Button_TouchDown(object sender, TouchEventArgs e)
        {
            if (this.Action == ActionType.Momentary) return;
            if (this.Action == ActionType.Alternative)
            {

                this.Alternative_TouchDown();
            }
            else if (this.Action == ActionType.Set)
            {

                this.Set_TouchDown();
            }
            else if (this.Action == ActionType.Reset)
            {

                this.Reset_TouchDown();
            }
        }

        // Alternative.
        private void Alternative_Click()
        {
            if (this.AddressWrite == null) return;
            var address = ushort.Parse(this.AddressWrite.ToString());
            if (!isToggled)
            {
                if (IsComfrim)
                {
                    WndComfirm comfirmYesNo = new WndComfirm();
                    if (!comfirmYesNo.DoComfirmYesNo($"{IsMesComfrim}")) return;
                }
                UiManager.Instance.PLC.device.WriteBit(DeviceWrite, address, true);
                this.EventLog("AlternativeBtn Address('" + DeviceWrite.ToString() + address + "') - " + this.Content, "true");
            }
            else
            {
                if (IsComfrim)
                {
                    WndComfirm comfirmYesNo = new WndComfirm();
                    if (!comfirmYesNo.DoComfirmYesNo($"{IsMesComfrim}")) return;
                }
                UiManager.Instance.PLC.device.WriteBit(DeviceWrite, address, false);
                this.EventLog("AlternativeBtn Address('" + DeviceWrite.ToString() + address + "') - " + this.Content, "false");
            }
        }
        private void Alternative_TouchDown()
        {
            this.Alternative_Click();
        }
        //SET.
        private void Set_Click()
        {
            if (IsComfrim)
            {
                WndComfirm comfirmYesNo = new WndComfirm();
                if (!comfirmYesNo.DoComfirmYesNo($"{IsMesComfrim}")) return;
            }
            if (this.AddressWrite == null) return;
            var address = ushort.Parse(this.AddressWrite.ToString());
            UiManager.Instance.PLC.device.WriteBit(DeviceWrite, address, true);
            this.EventLog("OnBtn Address('" + DeviceWrite.ToString() + address + "') - " + this.Content, "true");
        }
        private void Set_TouchDown()
        {
            this.Set_Click();
        }
        //Reset
        private void Reset_Click()
        {
            if (IsComfrim)
            {
                WndComfirm comfirmYesNo = new WndComfirm();
                if (!comfirmYesNo.DoComfirmYesNo($"{IsMesComfrim}")) return;
            }
            if (this.AddressWrite == null) return;
            var address = ushort.Parse(this.AddressWrite.ToString());
            UiManager.Instance.PLC.device.WriteBit(DeviceWrite, address, false);
            this.EventLog("OffBtn Address('" + DeviceWrite.ToString() + address + "') - " + this.Content, "false");
        }
        private void Reset_TouchDown()
        {
            this.Reset_Click();
        }
        //Momentary

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            if (this.Action != ActionType.Momentary) return;
            if (this.AddressWrite == null) return;

            if (IsComfrim)
            {
                WndComfirm comfirmYesNo = new WndComfirm();
                if (!comfirmYesNo.DoComfirmYesNo($"{IsMesComfrim}")) return;
            }
            if (e.StylusDevice != null && e.StylusDevice.TabletDevice.Type == TabletDeviceType.Touch)
            {
                return;
            }


            isMousePressed = true;
            btnTarget = sender as Button;
            btnTarget.CaptureMouse();
            var address = ushort.Parse(this.AddressWrite.ToString());
            UiManager.Instance.PLC.device.WriteBit(DeviceWrite, address, true);
            this.EventLog("MomentaryBtn Address('" + DeviceWrite.ToString() + address + "') - " + this.Content, "true");

            if (IsComfrim)
            {
                Thread.Sleep(40);
                UiManager.Instance.PLC.device.WriteBit(DeviceWrite, address, false);
                this.EventLog("MomentaryBtn Address('" + DeviceWrite.ToString() + address + "') - " + this.Content, "false");
            }

        }
        private void Button_PreviewMouseMove(object sender, MouseEventArgs e)
        {

            if (this.Action != ActionType.Momentary) return;
            Point position = e.GetPosition(btn);
            if (isMousePressed)
            {
                Button btn = sender as Button;
                Point pos = e.GetPosition(btn);
                if (pos.X < 0 || pos.X > btn.ActualWidth || pos.Y < 0 || pos.Y > btn.ActualHeight)
                {
                    isMousePressed = false;
                    btn.ReleaseMouseCapture();
                    var address = ushort.Parse(this.AddressWrite.ToString());
                    UiManager.Instance.PLC.device.WriteBit(DeviceWrite, address, false);
                    this.EventLog("MomentaryBtn Address('" + DeviceWrite.ToString() + address + "') - " + this.Content, "false");
                    btnTarget = null;
                }
            }

        }
        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.Action != ActionType.Momentary) return;

            if (isMousePressed)
            {
                isMousePressed = false;
                if (this.AddressWrite == null) return;
                var address = ushort.Parse(this.AddressWrite.ToString());
                UiManager.Instance.PLC.device.WriteBit(DeviceWrite, address, false);
                this.EventLog("MomentaryBtn Address('" + DeviceWrite.ToString() + address + "') - " + this.Content, "false");
                (sender as Button).ReleaseMouseCapture();
                btnTarget = null;
            }
        }



        private void Button_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            if (this.Action != ActionType.Momentary) return;
            if (!activeTouchDevices.Contains(e.TouchDevice))
            {
                activeTouchDevices.Add(e.TouchDevice);
                btnTarget = sender as Button;
                btnTarget.CaptureTouch(e.TouchDevice);
                if (this.AddressWrite == null) return;

                if (IsComfrim)
                {
                    WndComfirm comfirmYesNo = new WndComfirm();
                    if (!comfirmYesNo.DoComfirmYesNo($"{IsMesComfrim}")) return;
                }

                isMousePressed = true;
                btnTarget = sender as Button;
                btnTarget.CaptureMouse();
                var address = ushort.Parse(this.AddressWrite.ToString());
                UiManager.Instance.PLC.device.WriteBit(DeviceWrite, address, true);
                this.EventLog("MomentaryBtn Address('" + DeviceWrite.ToString() + address + "') - " + this.Content, "true");

                if (IsComfrim)
                {
                    Thread.Sleep(40);
                    UiManager.Instance.PLC.device.WriteBit(DeviceWrite, address, false);
                    this.EventLog("MomentaryBtn Address('" + DeviceWrite.ToString() + address + "') - " + this.Content, "false");
                }
                e.Handled = true;
            }

        }
        private void Button_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            if (this.Action != ActionType.Momentary) return;

            if (activeTouchDevices.Contains(e.TouchDevice))
            {
                activeTouchDevices.Remove(e.TouchDevice);
                if (this.AddressWrite == null) return;
                var address = ushort.Parse(this.AddressWrite.ToString());
                UiManager.Instance.PLC.device.WriteBit(DeviceWrite, address, false);
                this.EventLog("MomentaryBtn Address('" + DeviceWrite.ToString() + address + "') - " + this.Content, "false");
                (sender as Button).ReleaseTouchCapture(e.TouchDevice);
                btnTarget = null;
            }

        }
        private void Button_LostTouchCapture(object sender, TouchEventArgs e)
        {
            if (this.Action != ActionType.Momentary) return;

            if (activeTouchDevices.Contains(e.TouchDevice))
            {
                activeTouchDevices.Remove(e.TouchDevice);
                Button_PreviewTouchUp(sender, e);
                btnTarget = null;
            }

        }
        private void Button_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            Button btn = sender as Button;
            if (activeTouchDevices.Contains(e.TouchDevice))
            {
                Point pos = e.GetTouchPoint(btn).Position;
                if (pos.X < 0 || pos.X > btn.ActualWidth || pos.Y < 0 || pos.Y > btn.ActualHeight)
                {
                    activeTouchDevices.Remove(e.TouchDevice);
                    btn.ReleaseTouchCapture(e.TouchDevice);
                    if (this.AddressWrite == null) return;
                    var address = ushort.Parse(this.AddressWrite.ToString());
                    UiManager.Instance.PLC.device.WriteBit(DeviceWrite, address, false);
                    this.EventLog("MomentaryBtn Address('" + DeviceWrite.ToString() + address + "') - " + this.Content, "false");
                    btnTarget = null;
                }
            }
        }



        private void Timer_Tick(object sender, EventArgs e)
        {
            //if (this.Action != ActionType.Momentary) return;

            //if (!IsMouseOverElement())
            //{
            //    // Chuột rời khỏi UserControl
            //    // Thực hiện các xử lý ở đây
            //    if (this.AddressWrite == null) return;
            //    var address = ushort.Parse(this.AddressWrite.ToString());
            //    UiManager.Instance.PLC.device.WriteBit(DeviceWrite, address, false);
            //    Mouse.Capture(null);
            //    StopTimer();
            //}
        }
        private void StartTimer()
        {

            timer?.Start();
        }
        private void StopTimer()
        {


            timer?.Stop();
        }
        private void AddAddress(ushort address)
        {
            UiManager.Instance.PLC.AddBitAddress(this.DeviceLamp.ToString(), address);
        }
        private void RemoveAddress(ushort address)
        {
            UiManager.Instance.PLC.RemoveBitAddress(this.DeviceLamp.ToString(), address);
        }
    }
    public enum ActionType
    {
        Alternative,
        Momentary,
        Set,
        Reset
    }
}
