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
    /// Interaction logic for PgManual01.xaml
    /// </summary>
    public partial class PgManual01 : Page
    {
        public PgManual01()
        {
            InitializeComponent();

            this.btMenuTab00.Click += BtMenuTab00_Click;
            this.btMenuTab01.Click += BtMenuTab01_Click;


        }



        private void BtMenuTab01_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_MANUAL_OPERATION_01);
        }
        private void BtMenuTab00_Click(object sender, RoutedEventArgs e)
        {
            if (UserManager.IsLogOn() == 3)
            {
                UiManager.Instance.SwitchPage(PAGE_ID.PAGE_TEACHING_MENU_01);
            }
            else
            {
                WndMessenger wndMessenger = new WndMessenger();
                wndMessenger.MessengerShow("Use Login AutoTeam To Page Teaching");
            }
        }
    }
}
