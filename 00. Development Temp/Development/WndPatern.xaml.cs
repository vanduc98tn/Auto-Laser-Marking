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
using System.Windows.Shapes;

namespace Development
{
    /// <summary>
    /// Interaction logic for WndPatern.xaml
    /// </summary>
    public partial class WndPatern : Window
    {
        private MyLogger logger = new MyLogger("WndPatern");
        public WndPatern()
        {
            InitializeComponent();
            this.Loaded += WndPatern_Loaded;
            this.btnSave.Click += BtnSave_Click;
            this.btnCancle.Click += BtnCancle_Click;
        }

        private void BtnCancle_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (rdbPatern1.IsChecked == true)
            {
                UiManager.appSetting.RUN.Patern = 0;
            }
            else if (rdbPatern2.IsChecked == true)
            {
                UiManager.appSetting.RUN.Patern = 1;
            }
            else if (rdbPatern3.IsChecked == true)
            {
                UiManager.appSetting.RUN.Patern = 2;
            }
            else if (rdbPatern4.IsChecked == true)
            {
                UiManager.appSetting.RUN.Patern = 3;
            }
            else if (rdbPatern5.IsChecked == true)
            {
                UiManager.appSetting.RUN.Patern = 4;
            }
            else if (rdbPatern6.IsChecked == true)
            {
                UiManager.appSetting.RUN.Patern = 5;
            }
            else if (rdbPatern7.IsChecked == true)
            {
                UiManager.appSetting.RUN.Patern = 6;
            }
            else if (rdbPatern8.IsChecked == true)
            {
                UiManager.appSetting.RUN.Patern = 7;
            }
            else if (rdbPatern9.IsChecked == true)
            {
                UiManager.appSetting.RUN.Patern = 8;
            }
            else if (rdbPatern10.IsChecked == true)
            {
                UiManager.appSetting.RUN.Patern = 9;
            }
            else if (rdbPatern11.IsChecked == true)
            {
                UiManager.appSetting.RUN.Patern = 10;
            }
            else if (rdbPatern12.IsChecked == true)
            {
                UiManager.appSetting.RUN.Patern = 11;
            }
            else if (rdbPatern13.IsChecked == true)
            {
                UiManager.appSetting.RUN.Patern = 12;
            }
            else if (rdbPatern14.IsChecked == true)
            {
                UiManager.appSetting.RUN.Patern = 13;
            }
            else if (rdbPatern15.IsChecked == true)
            {
                UiManager.appSetting.RUN.Patern = 14;
            }
            else if (rdbPatern16.IsChecked == true)
            {
                UiManager.appSetting.RUN.Patern = 15;
            }
            UiManager.SaveAppSetting();
            this.Close();
        }

        private void WndPatern_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPatern();
        }

        private void LoadPatern()
        {
            try
            {
                var ID = UiManager.appSetting.RUN.Patern;
                if (ID == 0)
                {
                    rdbPatern1.IsChecked = true;
                }
                else if (ID == 1)
                {
                    rdbPatern2.IsChecked = true;
                }
                else if (ID == 2)
                {
                    rdbPatern3.IsChecked = true;
                }
                else if (ID == 3)
                {
                    rdbPatern4.IsChecked = true;
                }
                else if (ID == 4)
                {
                    rdbPatern5.IsChecked = true;
                }
                else if (ID == 5)
                {
                    rdbPatern6.IsChecked = true;
                }
                else if (ID == 6)
                {
                    rdbPatern7.IsChecked = true;
                }
                else if (ID == 7)
                {
                    rdbPatern8.IsChecked = true;
                }
                else if (ID == 8)
                {
                    rdbPatern9.IsChecked = true;
                }
                else if (ID == 9)
                {
                    rdbPatern10.IsChecked = true;
                }
                else if (ID == 10)
                {
                    rdbPatern11.IsChecked = true;
                }
                else if (ID == 11)
                {
                    rdbPatern12.IsChecked = true;
                }
                else if (ID == 12)
                {
                    rdbPatern13.IsChecked = true;
                }
                else if (ID == 13)
                {
                    rdbPatern14.IsChecked = true;
                }
                else if (ID == 14)
                {
                    rdbPatern15.IsChecked = true;
                }
                else if (ID == 15)
                {
                    rdbPatern16.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                logger.Create("LoadPatern : " + ex.Message, LogLevel.Error);
            }
        }
    }
}
