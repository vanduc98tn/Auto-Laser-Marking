using Newtonsoft.Json.Linq;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Development
{
    /// <summary>
    /// Interaction logic for WndCheckUpdate.xaml
    /// </summary>
    public partial class WndCheckUpdate : Window
    {
        private static WndCheckUpdate _instance; // Biến tĩnh lưu cửa sổ hiện tại
        public WndCheckUpdate()
        {
           
            if (_instance != null)
            {
                _instance.Close();
            }
            _instance = this;


            InitializeComponent();
            this.Loaded += WndCheckUpdate_Loaded;
           
            this.btnNo.Click += BtnNo_Click;
            this.btnYes.Click += BtnYes_Click;

        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            Updater.DownloadFTP();
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WndCheckUpdate_Loaded(object sender, RoutedEventArgs e)
        {

            this.btnYes.IsEnabled = false;

            // Animation từ 0% đến 15%
            DoubleAnimation animationTo15 = new DoubleAnimation
            {
                From = 0,
                To = 25,
                Duration = new Duration(System.TimeSpan.FromSeconds(1)) // Chạy trong 0.5 giây
            };

            // Animation từ 25% đến 100% (sẽ chạy sau khi kiểm tra xong)
            DoubleAnimation animationTo100 = new DoubleAnimation
            {
                From = 25,
                To = 101,
                Duration = new Duration(System.TimeSpan.FromSeconds(1)) // Phần còn lại của animation
            };

            // Sự kiện cập nhật % trong ProgressBar
            animationTo15.CurrentTimeInvalidated += (s, ev) =>
            {
                progressText.Text = $"{(int)progressBar.Value}%";
            };

            animationTo100.CurrentTimeInvalidated += (s, ev) =>
            {
                progressText.Text = $"{(int)progressBar.Value}%";
            };
            // Bắt đầu animation ban đầu từ 0% đến 15%
            progressBar.BeginAnimation(System.Windows.Controls.Primitives.RangeBase.ValueProperty, animationTo15);

            // Khi animation tới 15%, bắt đầu kiểm tra kết nối
            animationTo15.Completed += async (s, ev) =>
            {
                bool isConnected = await Task.Run(() => Updater.CheckConnect());


                progressBar.BeginAnimation(System.Windows.Controls.Primitives.RangeBase.ValueProperty, animationTo100);

                await Dispatcher.InvokeAsync(async () =>
                {
                    if (isConnected)
                    {
                        string MessengerUpdater = "";
                        bool UpdateVer = false;
                        string Ver = "";
                        Updater.ReadFileUpdate(out MessengerUpdater, out Ver, out UpdateVer);

                        if (!UpdateVer)
                        {
                            this.lblMessage.Content = "Không Có Phiên Bản Nào Mới";
                            await Task.Delay(1000);
                            this.Close();
                        }
                        else
                        {
                            this.lblMessage.Content = $"Đã Có Phiên Bản Mới : {Ver} - {MessengerUpdater}";
                            this.lbUpdate.Content = "Bạn Có Muốn Cập Nhật Phiên Bản";
                            this.btnYes.IsEnabled = true;
                        }
                    }
                    else
                    {
                        this.lblMessage.Content = "Kết nối Không Thành Công FTP Server";
                        await Task.Delay(2000);
                        this.Close();
                        return; 
                    }

                });
            };


        }
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Y:
              
                    this.Close();
                    e.Handled = true;
                    break;
                case Key.N:
                   
                    this.Close();
                    e.Handled = true;
                    break;
            }
            return;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Kiểm tra nếu nút chuột trái được nhấn
            if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
            {
                // Gọi phương thức DragMove để di chuyển cửa sổ
                this.DragMove();
            }
        }
    }
}
