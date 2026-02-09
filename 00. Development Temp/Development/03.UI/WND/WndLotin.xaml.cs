using KeyPad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Development
{
    /// <summary>
    /// Interaction logic for WndLotin.xaml
    /// </summary>
    public partial class WndLotin : Window
    {
        private MyLogger logger = new MyLogger("WndLotIn");
        private LotInData lotInData;
        private bool isOK = false;
        private bool OnKeyBoard = false;
        public WndLotin()
        {
            InitializeComponent();
            isOK = false;
            InitializeComponent();
            this.Closed += WndLotIn_Closed;

            this.btnOK.Click += BtnOK_Click;
            this.btnOK.TouchDown += BtnOK_Click;

            this.btnCancel.Click += BtnCancel_Click;
            this.btnCancel.TouchDown += BtnCancel_Click;

            this.txtWorkGroup.PreviewKeyDown += TxtWorkGroup_PreviewKeyDown;
            this.txtDeviceId.PreviewKeyDown += TxtWorkGroup_PreviewKeyDown;
            this.txtLotId.PreviewKeyDown += TxtWorkGroup_PreviewKeyDown;
            this.txtLotQty.PreviewKeyDown += TxtWorkGroup_PreviewKeyDown;

            this.txtWorkGroup.PreviewMouseDown += Txt_PreviewMouseDown;
            this.txtDeviceId.PreviewMouseDown += Txt_PreviewMouseDown;
            this.txtLotId.PreviewMouseDown += Txt_PreviewMouseDown;
            this.txtLotQty.PreviewMouseDown += Txt_PreviewMouseDown;

            this.txtWorkGroup.TouchDown += Txt_PreviewTouchDown;
            this.txtDeviceId.TouchDown += Txt_PreviewTouchDown;
            this.txtLotId.TouchDown += Txt_PreviewTouchDown;
            this.txtLotQty.TouchDown += Txt_PreviewTouchDown;

            this.btSwitchOnKeyBoard.Click += BtSwitchOnKeyBoard_Click;
            this.btSwitchOnKeyBoard.TouchDown += BtSwitchOnKeyBoard_PreviewTouchDown;
        }

        private void BtSwitchOnKeyBoard_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            if (sender is ToggleButton toggle)
            {
                this.OnKeyBoard = toggle.IsChecked == true ? true : false;
            }
        }

        private void BtSwitchOnKeyBoard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggle)
            {
                this.OnKeyBoard = toggle.IsChecked == true ? true : false;
            }
        }

        private void Txt_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            if (this.OnKeyBoard)
            {
                TextBox txt = sender as TextBox;
                VirtualKeyboard keyboardWindow = new VirtualKeyboard();
                if (keyboardWindow.ShowDialog() == true)
                {
                    txt.Text = keyboardWindow.Result;
                }
            }
        }

        private void Txt_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.OnKeyBoard)
            {
                TextBox txt = sender as TextBox;
                VirtualKeyboard keyboardWindow = new VirtualKeyboard();
                if (keyboardWindow.ShowDialog() == true)
                {
                    txt.Text = keyboardWindow.Result;
                }
            }    
            
        }

        private void WndLotIn_Closed(object sender, EventArgs e)
        {
            this.BtnCancel_Click(this, null);
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!isOK)
                {
                    this.lotInData = null;
                }
                this.Close();
            }
            catch (Exception ex)
            {
                this.logger.Create("BtnCancel_Click: " + ex.Message, LogLevel.Error);
            }
        }
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate data:
                if (this.txtLotId.Text.Length < 1)
                {
                    MessageBox.Show("Invalid LOT ID: it must has more 1 characters!", "PARAMETER ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (String.IsNullOrEmpty(this.txtDeviceId.Text))
                {
                    MessageBox.Show("Invalid Device ID: it must has atleast 1 character!", "PARAMETER ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                int lotQty = 0;
                try
                {
                    lotQty = int.Parse(txtLotQty.Text);
                }
                catch
                {
                    lotQty = 0;
                }
                const int MAX_QTY = 99999;
                if (lotQty == 0 || lotQty > MAX_QTY)
                {
                    var msg = String.Format("Invalid LOT QTY: it must be a positive number and not over {0}!", MAX_QTY);
                    MessageBox.Show(msg, "PARAMETER ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                this.lotInData = new LotInData();
                this.lotInData.WorkGroup = this.txtWorkGroup.Text;
                this.lotInData.DeviceId = this.txtDeviceId.Text;
                this.lotInData.LotId = this.txtLotId.Text;
                this.lotInData.LotQty = int.Parse(this.txtLotQty.Text);
                isOK = true;
                this.Close();
            }
            catch (Exception ex)
            {
                this.logger.Create("BtnOK_Click: " + ex.Message, LogLevel.Error);
            }
        }
        private void TxtWorkGroup_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    //if (!IsFromScanner(e))
            //    //{
            //    //    e.Handled = true;
            //    //}
            //    if (e.Key != Key.Enter) return;
            //    TextBox currentTextBox = sender as TextBox;
            //    if (currentTextBox == null) return;
            //    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
            //    currentTextBox.MoveFocus(request);


            //    e.Handled = true;
            //}
            //catch (Exception ex)
            //{
            //    this.logger.Create("TxtWorkGroup_PreviewKeyDown: " + ex.Message, LogLevel.Error);
            //}
        }

        public LotInData DoSettings(Window owner, LotInData oldSettings)
        {
            this.lotInData = oldSettings;

            txtWorkGroup.Text = this.lotInData.WorkGroup;
            txtDeviceId.Text = this.lotInData.DeviceId;
            txtLotId.Text = this.lotInData.LotId;
            if (this.lotInData.LotQty > 0)
            {
                txtLotQty.Text = this.lotInData.LotQty.ToString();
            }

            this.ShowDialog();
            return this.lotInData;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
