

/*
 * Copyright (c) 2008, Andrzej Rusztowicz (ekus.net)
* All rights reserved.

* Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

* Neither the name of ekus.net nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
/*
 * Added by Michele Cattafesta (mesta-automation.com) 29/2/2011
 * The code has been totally rewritten to create a control that can be modified more easy even without knowing the MVVM pattern.
 * If you need to check the original source code you can download it here: http://wosk.codeplex.com/
 */
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KeyPad
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class Keypad : Window, INotifyPropertyChanged
    {
        #region Public Properties

        public string _result;
        public string Result
        {
            get { return _result; }
            set
            {
                _result = value;
                OnPropertyChanged(nameof(Result));
            }
        }

        #endregion

        public Keypad(bool useTouch = false, string currentValue = "")
        {
            InitializeComponent();
            this.DataContext = this;
            Result = currentValue ?? "";
            if (!useTouch)
            {
            }

            this.Loaded += Keypad_Loaded;

        }


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(ref POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }
        private void TxtDisplay_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;   // chặn nhập từ keyboard
        }
        private void Keypad_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                txtDisplay.SelectAll();
                txtDisplay.Focus();
            }), System.Windows.Threading.DispatcherPriority.Render);

            // Chỉ đặt vị trí dưới con trỏ chuột nếu không có sự kiện cảm ứng
            if (!TouchesOver.Any())
            {


            }

        }
        
        private async void Setposition()
        {
            await PositionWindowUnderCursor();
        }
        private async Task PositionWindowUnderCursor()
        {
            POINT cursorPos = new POINT();
            if (GetCursorPos(ref cursorPos) && !TouchesOver.Any()) // Chỉ thay đổi khi không có sự kiện cảm ứng
            {
                // Đặt vị trí dưới con trỏ chuột
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double screenHeight = SystemParameters.PrimaryScreenHeight;
                double windowWidth = this.Width;
                double windowHeight = this.Height;

                double newLeft = cursorPos.X;
                double newTop = cursorPos.Y + 20;

                if (newLeft + windowWidth > screenWidth)
                {
                    newLeft = (screenWidth - windowWidth) / 2;
                }

                if (newTop + windowHeight > screenHeight)
                {
                    newTop = (screenHeight - windowHeight) / 2;
                }
                await Task.Delay(1);
                this.Left = newLeft;
                this.Top = newTop;
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {


            switch (e.Key)
            {
                case Key.D0:
                case Key.NumPad0:
                    ButtonClick(button0);
                    break;
                case Key.D1:
                case Key.NumPad1:
                    ButtonClick(button1);
                    break;
                case Key.D2:
                case Key.NumPad2:
                    ButtonClick(button2);
                    break;
                case Key.D3:
                case Key.NumPad3:
                    ButtonClick(button3);
                    break;
                case Key.D4:
                case Key.NumPad4:
                    ButtonClick(button4);
                    break;
                case Key.D5:
                case Key.NumPad5:
                    ButtonClick(button5);
                    break;
                case Key.D6:
                case Key.NumPad6:
                    ButtonClick(button6);
                    break;
                case Key.D7:
                case Key.NumPad7:
                    ButtonClick(button7);
                    break;
                case Key.D8:
                case Key.NumPad8:
                    ButtonClick(button8);
                    break;
                case Key.D9:
                case Key.NumPad9:
                    ButtonClick(button9);
                    break;
                case Key.Enter:
                    ButtonClick(buttonEnter);
                    break;
                case Key.Escape:
                    ButtonClick(buttonEsc);
                    break;
                case Key.Back:
                    ButtonClick(buttonBackspace);
                    break;
                case Key.OemMinus:
                case Key.Subtract:
                    ButtonClick(buttonMinus);
                    break;
                case Key.OemPeriod:
                case Key.Decimal:
                    ButtonClick(buttonDecimal);
                    break;
            }
        }
        private void ButtonClick(Button button)
        {
            button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
        //private void button_TouchDown(object sender, TouchEventArgs e)
        //{
        //    ButtonClick((Button)sender);
        //}
        private void button_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Button button = sender as Button;
                int start = txtDisplay.SelectionStart;
                int length = txtDisplay.SelectionLength;

                switch (button.CommandParameter.ToString())
                {

                    case "ESC":
                        this.DialogResult = false;
                        break;

                    case "RETURN":
                        if (string.IsNullOrWhiteSpace(Result))
                        {
                            Result = "0"; // gán mặc định là 0
                            this.DialogResult = false;
                            break;
                        }

                        // Kiểm tra có phải số hợp lệ không
                        if (!Regex.IsMatch(Result, @"^-?\d+(\.\d+)?$"))
                        {
                            MessageBox.Show("Value is not number :))",
                                            "Input error!",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Warning);
                            return; // không đóng keypad
                        }

                        this.DialogResult = true;
                        break;

                    case "BACK":
                        
                        if (length > 0)
                        {
                            // Có bôi → xoá vùng chọn
                            Result = Result.Remove(start, length);
                            txtDisplay.SelectionStart = start;
                        }
                        else if (start > 0)
                        {
                            // Không bôi → xoá ký tự bên trái caret
                            Result = Result.Remove(start - 1, 1);
                            txtDisplay.SelectionStart = start - 1;
                        }

                        txtDisplay.SelectionLength = 0;
                        txtDisplay.Focus();
                        break;

                    //case "MINUS":
                    //    Result += "-";
                    //    break;

                    default:

                        string input = button.Content.ToString();

                        if (length > 0)
                        {
                            // Có bôi đen → thay thế đúng đoạn đó
                            Result = Result.Remove(start, length).Insert(start, input);
                        }
                        else
                        {
                            // Không bôi → chèn tại vị trí con trỏ
                            Result = Result.Insert(start, input);
                        }

                        // Đưa con trỏ về sau ký tự vừa nhập
                        txtDisplay.SelectionStart = start + input.Length;
                        txtDisplay.SelectionLength = 0;

                        txtDisplay.Focus();
                        txtDisplay.CaretIndex = start + input.Length;


                        break;
                }
            }
            catch (Exception) { }



        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion

    }
}
