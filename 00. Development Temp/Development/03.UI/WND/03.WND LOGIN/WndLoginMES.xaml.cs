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
    /// Interaction logic for WndLoginMES.xaml
    /// </summary>
    public partial class WndLoginMES : Window , IObserverMES
    {
        private int isLogonSuccess = 0;
        private bool selectLoginMes = false;
        private bool OnKeyBoard = false;
        public WndLoginMES()
        {
            InitializeComponent();
            this.Loaded += WndLoginMES_Loaded;
            this.Unloaded += WndLoginMES_Unloaded;

            this.btOperator.Click += BtOperator_Click;
            this.btManager.Click += BtManager_Click;
            this.btAutoteam.Click += BtAutoteam_Click;
            this.btcancel.Click += Btcancel_Click;
            this.btSignin.Click += BtSignin_Click;

            this.txtPassword.PreviewTouchDown += TxtPassword_PreviewTouchDown;
            this.tbxCodeUserName.PreviewTouchDown += Txt_PreviewTouchDown;

            this.txtPassword.PreviewMouseDown += TxtPassword_PreviewMouseDown;
            this.tbxCodeUserName.PreviewMouseDown += TbxCodeUserName_PreviewMouseDown;

            this.btSwitchOnKeyBoard.Click += BtSwitchOnKeyBoard_Click;
            this.btSwitchOnKeyBoard.TouchDown += BtSwitchOnKeyBoard_TouchDown;
        }

       
        #region NotifyMES
        private void UpdateUimes()
        {

        }
        private void RemoveNotifyMES()
        {
            SystemsManager.Instance.NotifyEvenMES.Detach(this);
        }
        private void RegisterNotifyMES()
        {
            
            SystemsManager.Instance.NotifyEvenMES.Attach(this);

            UpdateUimes();

        }
        public void CheckConnectionMES(bool connected)
        {
            if (connected)
            {
                Dispatcher.Invoke(() =>
                {
                    this.txtMES.Text = "Connected MES Server";
                    this.txtMES.Background = Brushes.Green;
                });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    this.txtMES.Text = "Disconnected MES Server";
                    this.txtMES.Background = Brushes.OrangeRed;
                    
                    this.txtClient.Text = "";
                });
            }
        }
        public void FollowingDataMES(string MESResult)
        {

        }
        public void GetInformationFromClientConnectMES(string clientIP, int clientPort)
        {
            Dispatcher.Invoke(() =>
            {
                this.txtClient.Text = $"Client connect {clientIP} {clientPort}";
               
            });


        }
        public void UpdateNotifyToUIMES(string Notify)
        {

        }



        #endregion
        private void BtSwitchOnKeyBoard_TouchDown(object sender, TouchEventArgs e)
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

        private void TbxCodeUserName_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (OnKeyBoard)
            {
                TextBox txt = sender as TextBox;
                VirtualKeyboard keyboardWindow = new VirtualKeyboard();
                if (keyboardWindow.ShowDialog() == true)
                {
                    txt.Text = keyboardWindow.Result;
                }
            }
        }

        private void TxtPassword_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (OnKeyBoard)
            {
                PasswordBox txt = sender as PasswordBox;
                VirtualKeyboard keyboardWindow = new VirtualKeyboard();
                if (keyboardWindow.ShowDialog() == true)
                {
                    txt.Password = keyboardWindow.Result;
                }
            }
        }

        private void TxtPassword_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            if (OnKeyBoard)
            {
                PasswordBox txt = sender as PasswordBox;
                VirtualKeyboard keyboardWindow = new VirtualKeyboard();
                if (keyboardWindow.ShowDialog() == true)
                {
                    txt.Password = keyboardWindow.Result;
                }
            }

        }

        private void Txt_PreviewTouchDown(object sender, TouchEventArgs e)
        {
          if (OnKeyBoard)
            {
                TextBox txt = sender as TextBox;
                VirtualKeyboard keyboardWindow = new VirtualKeyboard();
                if (keyboardWindow.ShowDialog() == true)
                {
                    txt.Text = keyboardWindow.Result;
                }
            }
        }

        private void BtSignin_Click(object sender, RoutedEventArgs e)
        {
            this.Login();
        }

        private void Btcancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WndLoginMES_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateUi();
            this.RegisterNotifyMES();
        }
        private void WndLoginMES_Unloaded(object sender, RoutedEventArgs e)
        {
            this.RemoveNotifyMES();
        }
        private void UpdateUi()
        {
            this.tbxCodeUserName.IsEnabled = false;
            this.txtPassword.IsEnabled = false;

            this.selectLoginMes = false;

            this.tbxCodeUserName.IsEnabled = true;
            this.txtPassword.IsEnabled = true;
            this.tbxCodeUserName.Text = "Nhập Mã Nhân viên : Operator";

            this.txtPassword.Focus();
            Keyboard.Focus(txtPassword);
        }
        private void BtAutoteam_Click(object sender, RoutedEventArgs e)
        {
            this.selectLoginMes = true;

            this.tbxCodeUserName.IsEnabled = true;
            this.txtPassword.IsEnabled = true;
            this.tbxCodeUserName.Text = "Nhập Mã Nhân viên : Auto Team";
            this.tbxCodeUserName.Text = "Auto Team";


            this.txtPassword.Focus();
            Keyboard.Focus(txtPassword);



            this.textId.Text = UiManager.managerSetting.loginApp.UseNameADM;
        }

        private void BtManager_Click(object sender, RoutedEventArgs e)
        {
            this.selectLoginMes = false;

            this.tbxCodeUserName.IsEnabled = true;
            this.txtPassword.IsEnabled = true;
            this.tbxCodeUserName.Text = "Nhập Mã Nhân viên : Manager";

            this.txtPassword.Focus();
            Keyboard.Focus(txtPassword);


            this.textId.Text = UiManager.managerSetting.loginApp.UseNameEN;
        }

        private void BtOperator_Click(object sender, RoutedEventArgs e)
        {
            this.selectLoginMes = false;

            this.tbxCodeUserName.IsEnabled = true;
            this.txtPassword.IsEnabled = true;
            this.tbxCodeUserName.Text = "Nhập Mã Nhân viên : Operator";

            this.txtPassword.Focus();
            Keyboard.Focus(txtPassword);


            this.textId.Text = UiManager.managerSetting.loginApp.UseNameOPE;
        }
        private void Enter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Login();
            }
        }
        private void textId_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.textId.Focus();

        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void Image_Mouseup(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        public int DoCheck()
        {

            this.WindowState = WindowState.Normal;
            this.Topmost = true;
            this.ShowDialog();
            return isLogonSuccess;
        }
        private void tbxCodeUserName_GotFocus(object sender, RoutedEventArgs e)
        {
            tbxCodeUserName.Text = "";
        }
        private async Task Login()
        {
            if(selectLoginMes)
            {
                if(this.tbxCodeUserName.Text == "Nhập Mã Nhân viên : Auto Team")
                {
                    this.tbxCodeUserName.Text = "";
                }    
                string codeUser = tbxCodeUserName.Text;
                if (String.IsNullOrEmpty(codeUser))
                {
                    MessageBox.Show("Please re-confirm the employee code!");
                }
                else
                {
                    string usename = this.textId.Text;
                    UserManager.createUserLog(UserActions.LOGON_BUTTON_ENTER);

                    // use name OP/ME/ADM : 0P = 1  ME = 2  ADM = 3
                    isLogonSuccess = UserManager.LogOn(usename, this.txtPassword.Password);
                    if (isLogonSuccess == 0)
                    {
                        MessageBox.Show("Wrong Password!");
                    }
                    else
                    {
                        // Luu lại thông tin đăng nhập MES là OP/ME
                        UiManager.UserNameLoginMesOP_ME = UiManager.managerSetting.loginApp.LabelMesNameME;
                        UiManager.CodeUserLoginMesOP_ME = tbxCodeUserName.Text;
                        this.Close();
                        selectLoginMes = false;
                    }
                }
                
            }
            else

            // LOGIN OPERATOR - MANAGER
            {
                if (this.tbxCodeUserName.Text == "Nhập Mã Nhân viên : Operator" || this.tbxCodeUserName.Text == "Nhập Mã Nhân viên : Manager")
                {
                    this.tbxCodeUserName.Text = "";
                }
                string codeUser = tbxCodeUserName.Text;
                if (String.IsNullOrEmpty(codeUser))
                {
                    MessageBox.Show("Please re-confirm the employee code!");
                }
                else
                {
                    Mes00Check data = new Mes00Check();
                    data.EquipmentId = UiManager.appSetting.MESSettings.EquimentID.ToString();

                    if(textId.Text.Contains("Operator"))
                    {
                        data.MESCheckLogIn.Type = UiManager.managerSetting.loginApp.LabelMesNameOPE;
                        UiManager.UserNameLoginMesOP_ME = UiManager.managerSetting.loginApp.LabelMesNameOPE;
                        UiManager.CodeUserLoginMesOP_ME = codeUser;
                    }    
                    else
                    {
                        data.MESCheckLogIn.Type = UiManager.managerSetting.loginApp.LabelMesNameME;
                        UiManager.UserNameLoginMesOP_ME = UiManager.managerSetting.loginApp.LabelMesNameME;
                        UiManager.CodeUserLoginMesOP_ME = codeUser;
                    }    
                    data.MESCheckLogIn.User = codeUser;
                    data.MESCheckLogIn.Password = this.txtPassword.Password;

                    data.DIV = "CONFIG";
                    data.CheckSum = "CONFIG";

                    var MESREADY = await UiManager.Instance.MES.SendReady(data);


                    if (!MESREADY)
                    {
                        string message = "- Check MES connection again:\r\n" +
                                         "  + Verify IP configuration\r\n" +
                                         "  + Verify network connectivity\r\n" +
                                         "- Kiểm tra lại kết nối MES:\r\n" +
                                         "  + Kiểm tra lại setting IP\r\n" +
                                         "  + Kiểm tra lại đường truyền\r\n";

                        AddErrorMES("MES CHECK READY IS NG", message);
                        return;
                    }
                    var MES = await UiManager.Instance.MES.SendLogIn(data);

                    if (MES == null)
                    {
                        string message = "- Check MES connection again / MES not respond. :\r\n" +
                                        "  + Verify IP configuration\r\n" +
                                        "  + Verify network connectivity\r\n" +
                                        "- Kiểm tra lại kết nối MES / MES không phản hồi:\r\n" +
                                        "  + Kiểm tra lại setting IP\r\n" +
                                        "  + Kiểm tra lại đường truyền\r\n";

                        AddErrorMES("MES NOT RESPOND", message);
                        return;
                    }

                    if (MES.MES_Result == "NG")
                    {
                        AddErrorMES("MES CHECK LOGIN NG", MES.MES_MSG);
                        return;
                    }
                    else
                    {
                        string usename = this.textId.Text;
                        isLogonSuccess = UserManager.LogOn1(usename);
                        this.Close();
                        selectLoginMes = false;
                    }
                
                }



            }
        }
        private void AddErrorMES(string Messenger, string Solution)
        {

            WndAlarmMES ShowAlarm = new WndAlarmMES();
            ShowAlarm.Messenger(Messenger, Solution);
           

        }
    }
}
