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
    /// Interaction logic for RectangleLamp.xaml
    /// </summary>
    public partial class RectangleLamp : UserControl, IObserverChangeBits
    {
        private MyLogger logger = new MyLogger("RectangleLamp");

        public static readonly DependencyProperty  DeviceLampProperty = DependencyProperty.Register(
            "DeviceLamp", typeof(DeviceCode), typeof(RectangleLamp), new PropertyMetadata(DeviceCode.M));

        public static readonly DependencyProperty AddressLampProperty = DependencyProperty.Register(
            "AddressLamp", typeof(object), typeof(RectangleLamp), new PropertyMetadata(null));

        public static readonly DependencyProperty BackgroundLampONProperty = DependencyProperty.Register(
            "BackgroundLampON", typeof(Brush), typeof(RectangleLamp), new PropertyMetadata(Brushes.Green));

        public static readonly DependencyProperty BackgroundLampOFFProperty = DependencyProperty.Register(
            "BackgroundLampOFF", typeof(Brush), typeof(RectangleLamp), new PropertyMetadata(Brushes.LightGray));
        public static readonly DependencyProperty TextOFFProperty = DependencyProperty.Register(
            "TextOFF", typeof(string), typeof(RectangleLamp), new PropertyMetadata(""));
        public static readonly DependencyProperty TextONProperty = DependencyProperty.Register(
            "TextON", typeof(string), typeof(RectangleLamp), new PropertyMetadata(""));

        public static readonly DependencyProperty IsShowInWindowProperty = DependencyProperty.Register(
            "IsShowInWindow", typeof(bool), typeof(RectangleLamp), new PropertyMetadata(false));

        public static readonly DependencyProperty IsTabItemProperty = DependencyProperty.Register(
            "IsTabItem", typeof(bool), typeof(RectangleLamp), new PropertyMetadata(false));

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

        public DeviceCode DeviceLamp
        {
            get { return (DeviceCode)GetValue(DeviceLampProperty); }
            set { SetValue(DeviceLampProperty, value); }
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
        public object AddressLamp
        {
            get { return GetValue(AddressLampProperty); }
            set { SetValue(AddressLampProperty, value); }
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

        private NotifyPLCBits notifyPLCBits = new NotifyPLCBits();
        private bool isInTabItem;

        public RectangleLamp()
        {
            InitializeComponent();
            this.Loaded += RectangleLamp_Loaded;
            this.Unloaded += RectangleLamp_Unloaded;
        }

        private void RectangleLamp_Unloaded(object sender, RoutedEventArgs e)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                // Bỏ qua các hành động trong chế độ Design
                return;
            }
            if (this.isInTabItem) return;
            UnregisterNotifyBits();

            if (this.IsShowInWindow) return;
            this.RemoveAddress();
        }

        private void RectangleLamp_Loaded(object sender, RoutedEventArgs e)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                // Bỏ qua các hành động trong chế độ Design
                return;
            }
            if (this.isInTabItem) return;
            this.RemoveAddress();
            this.RegisterNotifyBits();
            this.Initial();
            this.AddAddress();
            this.isInTabItem = this.IsTabItem;
        }
        private void Initial()
        {
            if (this.txt == null) return;
            if (this.rec == null) return;
            this.rec.Fill = BackgroundLampOFF;
            this.txt.Text = this.TextOFF.ToString();
        }
        private void RegisterNotifyBits()
        {
            try
            {
                this.notifyPLCBits = SystemsManager.Instance.NotifyPLCBits;
                if (this.notifyPLCBits == null) return;
                this.notifyPLCBits.Attach(this);
            }
            catch (Exception ex)
            {
                logger.Create("RegisterNotifyBits: " + ex.Message, LogLevel.Error);
            }
        }
        private void UnregisterNotifyBits()
        {
            if (this.notifyPLCBits == null) return;
            this.notifyPLCBits.Detach(this);
        }
        //private void ChangeBrushLamp(bool status, Rectangle rec)
        //{
        //    Dispatcher.Invoke(() => 
        //    {
        //        if (this.rec == null) return;
        //        if (this.txt == null) return;
        //        this.Dispatcher.Invoke(() =>
        //        {
        //            if (!status)
        //            {
        //                rec.Fill = BackgroundLampOFF;
        //                this.txt.Text = this.TextOFF.ToString();
        //            }
        //            else
        //            {
        //                rec.Fill = BackgroundLampON;
        //                this.txt.Text = this.TextON.ToString();
        //            }
        //        });
        //    });

            
        //}
        private void ChangeBrushLamp(bool status, Rectangle rec)
        {
            Dispatcher.Invoke(() =>
            {
                if (rec == null) return;

                if (!status)
                {
                    rec.Fill = BackgroundLampOFF;
                    txt.Text = this.TextOFF.ToString();
                    //this.SetCurrentValue(TextOFFProperty, this.TextOFF); // giữ TextOFF
                }
                else
                {
                    rec.Fill = BackgroundLampON;
                    txt.Text = this.TextON.ToString();
                    //this.SetCurrentValue(TextONProperty, this.TextON); // hiển thị TextON thay cho OFF
                }
            });
        }

        public void NotifyChangeBits(string key, bool status)
        {
            Dispatcher.Invoke(() =>
            {
                if (this.AddressLamp == null) return;
                if (this.DeviceLamp.ToString() + this.AddressLamp.ToString() != key) 
                return;
                this.ChangeBrushLamp(status, this.rec);
            });
           
            
        }
        private void AddAddress()
        {
            if (this.AddressLamp == null) return;
            var address = ushort.Parse(this.AddressLamp.ToString());
            UiManager.Instance.PLC.AddBitAddress(this.DeviceLamp.ToString(), address);
        }
        private void RemoveAddress()
        {
            if (this.AddressLamp == null) return;
            var address = ushort.Parse(this.AddressLamp.ToString());
            UiManager.Instance.PLC.RemoveBitAddress(this.DeviceLamp.ToString(), address);
        }
    }
}
