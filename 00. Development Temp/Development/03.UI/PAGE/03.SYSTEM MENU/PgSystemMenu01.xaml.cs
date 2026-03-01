using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            //this.btSettingPatern.Click += BtSettingPatern_Click;

            this.tbExclusion.LostFocus += TbExclusion_LostFocus;
        }

        
        private void TbExclusion_LostFocus(object sender, RoutedEventArgs e)
        {
            string input = tbExclusion.Text;
            if (!IsValidFormat(input))
            {
                MessageBox.Show("Input format eror: A;B;C;1;2;3");
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    tbExclusion.Focus();
                }), System.Windows.Threading.DispatcherPriority.Input);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(input))
                {
                    string[] arr = input.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                    UiManager.appSetting.RUN.MES_EXCLUSION = arr;
                }
            }    
            
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

        private void PgSystemMenu01_Unloaded(object sender, RoutedEventArgs e)
        {
            UiManager.SaveAppSetting();
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

            tbExclusion.Text = string.Join(";", UiManager.appSetting.RUN.MES_EXCLUSION);
        }
        private bool IsValidFormat(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return true;

            string pattern = @"^[A-Za-z0-9]+(;[A-Za-z0-9]+)*$";

            return Regex.IsMatch(input, pattern);
        }


        private void BtMenuTab02_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_SYSTEM_MENU_02);
        }
        private void BtMenuTab01_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_SYSTEM_MENU_01);
        }

        private void Page_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tbExclusion.IsKeyboardFocusWithin && !tbExclusion.IsMouseOver)
            {
                this.Focus();   // <-- Quan trọng
            }
        }


    }
}
