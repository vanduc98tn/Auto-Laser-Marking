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
    /// Interaction logic for WndMessenger.xaml
    /// </summary>
    public partial class WndMessenger : Window
    {
        public WndMessenger()
        {
            InitializeComponent();
            this.btOk.Click += BtOk_Click;
        }
        private void BtOk_Click(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }
        public void MessengerShow(string message, Window owner = null)
        {
            this.Owner = owner;
            this.tbxMessage.Text = message;
            this.ShowDialog();
           
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
