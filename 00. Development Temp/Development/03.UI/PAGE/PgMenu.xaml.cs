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
    /// Interaction logic for PgMenu.xaml
    /// </summary>
    public partial class PgMenu : Page
    {
        private WndCheckUpdate WndUpdate;
        public PgMenu()
        {
            InitializeComponent();
            this.Loaded += PgMenu_Loaded;

            this.btLogin.Click += BtLogin_Click;
            this.btLogout.Click += BtLogout_Click;
            this.btUpdate.Click += BtUpdate_Click;

            this.btTeaching.Click += BtTeaching_Click;
            this.btMechanical.Click += BtMechanical_Click;
            this.btSystem.Click += BtSystem_Click;
            this.btManual.Click += BtManual_Click;
            this.btStatus.Click += BtStatus_Click;
            this.btModel.Click += BtModel_Click;
            this.btSuperUser.Click += BtSuperUser_Click;
            this.btAssignMenu.Click += BtAssignMenu_Click;
            this.btCamera.Click += BtCamera_Click;
        }

        private void BtCamera_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_CAMERA);
        }

        private void BtSuperUser_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_SUPER_USER_MENU_01);
        }

        private void BtModel_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_MODEL);
        }

        private void BtStatus_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_STATUS_MENU);
        }

        private void BtManual_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_MANUAL_OPERATION_01);
        }

        private void BtSystem_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_SYSTEM_MENU_01);
        }

        private void BtTeaching_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_TEACHING_MENU_01);
        }

        private void BtMechanical_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_MECHANICAL_MENU_01);
        }

        private void BtLogout_Click(object sender, RoutedEventArgs e)
        {
            UserManager.LogOut();
            updateUI();
        }

        private void PgMenu_Loaded(object sender, RoutedEventArgs e)
        {
            UserManager.IsLogOn();
            updateUI();
        }

        private void BtLogin_Click(object sender, RoutedEventArgs e)
        {
            var Result = ManagerLogin.DoCheck();
            updateUI();


        }

        private void BtUpdate_Click(object sender, RoutedEventArgs e)
        {
            WndUpdate = new WndCheckUpdate();
            WndUpdate.Show();

        }

        private void BtAssignMenu_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_ASSIGN_MENU);
        }
        private void updateUI()
        {
           
            if (UserManager.IsLogOn() == 2)
            {
                myImage.Source = new BitmapImage(new Uri("/01.Image/Manager2.png", UriKind.RelativeOrAbsolute));
                this.lblCurrentTime.Content = DateTime.Now.ToString("HH:mm:ss yyyy-MM-dd");
                this.lblMode.Content = "Manager";


                this.btTeaching.IsEnabled = true;
                this.btMechanical.IsEnabled = true;
                this.btManual.IsEnabled = true;
                this.btStatus.IsEnabled = true;
                this.btModel.IsEnabled = true;
                this.btSuperUser.IsEnabled = false;
                this.btSystem.IsEnabled = true;
                this.btAssignMenu.IsEnabled = false;
                this.btCamera.IsEnabled = true;
            }
            if (UserManager.IsLogOn() == 3)
            {
                myImage.Source = new BitmapImage(new Uri("/01.Image/Autotem2.png", UriKind.RelativeOrAbsolute));
                this.lblCurrentTime.Content = DateTime.Now.ToString("HH:mm:ss yyyy-MM-dd");
                this.lblMode.Content = "AutoTeams";

                this.btTeaching.IsEnabled = true;
                this.btMechanical.IsEnabled = true;
                this.btManual.IsEnabled = true;
                this.btStatus.IsEnabled = true;
                this.btModel.IsEnabled = true;
                this.btSuperUser.IsEnabled = true;
                this.btSystem.IsEnabled = true;
                this.btAssignMenu.IsEnabled = true;
                this.btCamera.IsEnabled = true;
            }
            if (UserManager.IsLogOn() == 1)
            {
                myImage.Source = new BitmapImage(new Uri("/01.Image/Operator.png", UriKind.RelativeOrAbsolute));
                this.lblCurrentTime.Content = DateTime.Now.ToString("HH:mm:ss yyyy-MM-dd");
                this.lblMode.Content = "Operator";
                

                this.btTeaching.IsEnabled = false;
                this.btMechanical.IsEnabled = false;
                this.btManual.IsEnabled = true;
                this.btStatus.IsEnabled = true;
                this.btModel.IsEnabled = true;
                this.btSuperUser.IsEnabled = false;
                this.btSystem.IsEnabled = false;
                this.btAssignMenu.IsEnabled = false;
                this.btCamera.IsEnabled = false;

            }
            if (UserManager.IsLogOn() == 0)
            {
                this.lblCurrentTime.Content = DateTime.Now.ToString("HH:mm:ss yyyy-MM-dd");

                this.btTeaching.IsEnabled = false;
                this.btMechanical.IsEnabled = false;
                this.btManual.IsEnabled = false;
                this.btStatus.IsEnabled = false;
                this.btModel.IsEnabled = false;
                this.btSuperUser.IsEnabled = false;
                this.btSystem.IsEnabled = false;
                this.btAssignMenu.IsEnabled = false;
                this.btCamera.IsEnabled = false;
            }
        }
    }
}
