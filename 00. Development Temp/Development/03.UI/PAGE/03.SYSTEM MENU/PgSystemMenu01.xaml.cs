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
    /// Interaction logic for PgSystemMenu01.xaml
    /// </summary>
    public partial class PgSystemMenu01 : Page
    {
        public PgSystemMenu01()
        {
            InitializeComponent();

            this.btMenuTab01.Click += BtMenuTab01_Click;
            this.btMenuTab02.Click += BtMenuTab02_Click;

            this.Loaded += PgSystemMenu01_Loaded;
            this.Unloaded += PgSystemMenu01_Unloaded;
            this.btMesOff.Click += BtMesOff_Click;
            this.btMesOn.Click += BtMesOn_Click;

            this.btOnScanner.Click += BtOnScanner_Click;
            this.btOffScanner.Click += BtOffScanner_Click;

            this.btSettingPatern.Click += BtSettingPatern_Click;
        }

        private void PgSystemMenu01_Unloaded(object sender, RoutedEventArgs e)
        {
            UiManager.SaveAppSetting();
        }

        private void BtSettingPatern_Click(object sender, RoutedEventArgs e)
        {
           WndPatern wndPatern = new WndPatern();
           wndPatern.ShowDialog();
        }

        private void BtOffScanner_Click(object sender, RoutedEventArgs e)
        {
            if(UiManager.appSetting.RUN.MESOnline == false)
            {
                UiManager.appSetting.RUN.CheckScanner = false;
                UpdateUI();
            }
           
        }

        private void BtOnScanner_Click(object sender, RoutedEventArgs e)
        {
            if (UiManager.appSetting.RUN.MESOnline == false)
            {
                UiManager.appSetting.RUN.CheckScanner = true;
                UpdateUI();
            }
            
        }

        private void BtMesOn_Click(object sender, RoutedEventArgs e)
        {
            UiManager.appSetting.RUN.MESOnline = true;
            UiManager.appSetting.RUN.CheckScanner = true;
            UpdateUI();
        }

        private void BtMesOff_Click(object sender, RoutedEventArgs e)
        {
            UiManager.appSetting.RUN.MESOnline = false;
            UpdateUI();

        }

        private void PgSystemMenu01_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateUI();
        }
        
        private void UpdateUI()
        {
            if (UiManager.appSetting.RUN.MESOnline)
            {
                this.btMesOn.Background = Brushes.LightGreen;
                this.btMesOff.Background = Brushes.LightGray;
            }
            else
            {
                this.btMesOn.Background = Brushes.LightGray;
                this.btMesOff.Background = Brushes.LightCoral;
            }
            if (UiManager.appSetting.RUN.CheckScanner)
            {
                this.btOnScanner.Background = Brushes.LightGreen;
                this.btOffScanner.Background = Brushes.LightGray;
            }
            else
            {
                this.btOnScanner.Background = Brushes.LightGray;
                this.btOffScanner.Background = Brushes.LightCoral;
            }
        }
        private void BtMenuTab02_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_SYSTEM_MENU_02);
        }
        private void BtMenuTab01_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_SYSTEM_MENU_01);
        }


    }
}
