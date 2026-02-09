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
    /// Interaction logic for WndComfirm.xaml
    /// </summary>
    public partial class WndComfirm : Window
    {
        private Boolean isConfirmYes = false;
        public WndComfirm()
        {
            InitializeComponent();
            this.btOk.Click += BtOk_Click;
            this.btCaner.Click += BtCaner_Click;
        }

        private void BtCaner_Click(object sender, RoutedEventArgs e)
        {
            this.isConfirmYes = false;
            this.Close();
        }

        private void BtOk_Click(object sender, RoutedEventArgs e)
        {
            this.isConfirmYes = true;
            this.Close();
        }
        public bool DoComfirmYesNo(string message, Window owner = null)
        {
            this.Owner = owner;
            this.tbxMessage.Text = message;

            this.ShowDialog();
            return isConfirmYes;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
