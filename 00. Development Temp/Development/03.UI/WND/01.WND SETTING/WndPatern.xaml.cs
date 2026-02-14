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
        private PatternSetting pattern = UiManager.appSetting.Pattern;
        public WndPatern()
        {
            InitializeComponent();
            this.Loaded += WndPatern_Loaded;
            this.btnSave.Click += BtnSave_Click;
            this.btnCancle.Click += BtnCancle_Click;
        }

        private void BtnCancle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                logger.Create("BtnCancle_Click : " + ex.Message, LogLevel.Error);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!CheckInputFormat()) return;
                if (rdbPatern1.IsChecked == true)
                {
                    pattern.CurrentPatern = 1;
                }
                else if (rdbPatern2.IsChecked == true)
                {
                    pattern.CurrentPatern = 2;
                }
                else if (rdbPatern3.IsChecked == true)
                {
                    pattern.CurrentPatern = 3;
                }
                else if (rdbPatern4.IsChecked == true)
                {
                    pattern.CurrentPatern = 4;
                }
                else if (rdbPatern5.IsChecked == true)
                {
                    pattern.CurrentPatern = 5;
                }
                else if (rdbPatern6.IsChecked == true)
                {
                    pattern.CurrentPatern = 6;
                }
                else if (rdbPatern7.IsChecked == true)
                {
                    pattern.CurrentPatern = 7;
                }
                else if (rdbPatern8.IsChecked == true)
                {
                    pattern.CurrentPatern = 8;
                }
                else if (rdbPatern9.IsChecked == true)
                {
                    pattern.CurrentPatern = 9;
                }
                else if (rdbPatern10.IsChecked == true)
                {
                    pattern.CurrentPatern = 10;
                }
                else if (rdbPatern11.IsChecked == true)
                {
                    pattern.CurrentPatern = 11;
                }
                else if (rdbPatern12.IsChecked == true)
                {
                    pattern.CurrentPatern = 12;
                }
                else if (rdbPatern13.IsChecked == true)
                {
                    pattern.CurrentPatern = 13;
                }
                else if (rdbPatern14.IsChecked == true)
                {
                    pattern.CurrentPatern = 14;
                }
                else if (rdbPatern15.IsChecked == true)
                {
                    pattern.CurrentPatern = 15;
                }
                else if (rdbPatern16.IsChecked == true)
                {
                    pattern.CurrentPatern = 16;
                }
                //pattern.PitchX = Convert.ToDouble(txtPitchX.Text);
                //pattern.PitchY = Convert.ToDouble(txtPitchY.Text);
                pattern.xRow = Convert.ToInt32(txtRow.Text);
                pattern.yColumn = Convert.ToInt32(txtColumn.Text);
                //pattern.offsetX = Convert.ToDouble(txtOffsetX.Text);
                //pattern.offsetY = Convert.ToDouble(txtOffsetY.Text);
                pattern.Use2Matrix = Convert.ToBoolean(cb2Matrix.IsChecked);
                UiManager.SaveAppSetting();
                this.Close();
            }
            catch (Exception ex)
            {
                logger.Create("BtnSave_Click : " + ex.Message, LogLevel.Error);
            }
        }

        private void WndPatern_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadPara();
            }
            catch (Exception ex)
            {
                logger.Create("WndPatern_Loaded : " + ex.Message, LogLevel.Error);
            }
        }
        private void LoadPara()
        {
            try
            {
                LoadPatern();
                //txtPitchX.Text = pattern.PitchX.ToString();
                //txtPitchY.Text = pattern.PitchY.ToString();
                txtRow.Text = pattern.xRow.ToString();
                txtColumn.Text = pattern.yColumn.ToString();
                //txtOffsetX.Text = pattern.offsetX.ToString();
                //txtOffsetY.Text = pattern.offsetY.ToString();
                cb2Matrix.IsChecked = pattern.Use2Matrix;
            }
            catch (Exception ex)
            {
                logger.Create("LoadPara : " + ex.Message, LogLevel.Error);
            }
        }
        private void LoadPatern()
        {
            try
            {
                var ID = pattern.CurrentPatern;
                if (ID == 1)
                {
                    rdbPatern1.IsChecked = true;
                }
                else if (ID == 2)
                {
                    rdbPatern2.IsChecked = true;
                }
                else if (ID == 3)
                {
                    rdbPatern3.IsChecked = true;
                }
                else if (ID == 4)
                {
                    rdbPatern4.IsChecked = true;
                }
                else if (ID == 5)
                {
                    rdbPatern5.IsChecked = true;
                }
                else if (ID == 6)
                {
                    rdbPatern6.IsChecked = true;
                }
                else if (ID == 7)
                {
                    rdbPatern7.IsChecked = true;
                }
                else if (ID == 8)
                {
                    rdbPatern8.IsChecked = true;
                }
                else if (ID == 9)
                {
                    rdbPatern9.IsChecked = true;
                }
                else if (ID == 10)
                {
                    rdbPatern10.IsChecked = true;
                }
                else if (ID == 11)
                {
                    rdbPatern11.IsChecked = true;
                }
                else if (ID == 12)
                {
                    rdbPatern12.IsChecked = true;
                }
                else if (ID == 13)
                {
                    rdbPatern13.IsChecked = true;
                }
                else if (ID == 14)
                {
                    rdbPatern14.IsChecked = true;
                }
                else if (ID == 15)
                {
                    rdbPatern15.IsChecked = true;
                }
                else if (ID == 16)
                {
                    rdbPatern16.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                logger.Create("LoadPatern : " + ex.Message, LogLevel.Error);
            }
        }
        private bool CheckInputFormat()
        {
            int x;
            double b;
            if (!int.TryParse(txtRow.Text, out x))
            {
                txtRow.Focus();
                MessageBox.Show("Input [Row] incorrect!", "Notification", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!int.TryParse(txtColumn.Text, out x))
            {
                txtColumn.Focus();
                MessageBox.Show("Input [Column] incorrect!", "Notification", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            //if (!double.TryParse(txtPitchX.Text, out b))
            //{
            //    txtPitchX.Focus();
            //    MessageBox.Show("Input [PitchX] incorrect!", "Notification", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return false;
            //}
            //if (!double.TryParse(txtPitchY.Text, out b))
            //{
            //    txtPitchY.Focus();
            //    MessageBox.Show("Input [PitchY] incorrect!", "Notification", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return false;
            //}
            //if (!double.TryParse(txtOffsetX.Text, out b))
            //{
            //    txtOffsetX.Focus();
            //    MessageBox.Show("Input [OffsetX] incorrect!", "Notification", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return false;
            //}
            //if (!double.TryParse(txtOffsetY.Text, out b))
            //{
            //    txtOffsetY.Focus();
            //    MessageBox.Show("Input [OffsetY] incorrect!", "Notification", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return false;
            //}
            return true;
        }
        public Boolean DoConfirmYesNo(Window owner = null)
        {
            this.ShowDialog();
            return true;
        }
    }
}
