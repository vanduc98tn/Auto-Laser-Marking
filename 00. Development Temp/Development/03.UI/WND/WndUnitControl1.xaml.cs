using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for WndUnitControl1.xaml
    /// </summary>
    public partial class WndUnitControl1 : Window
    {
        public WndUnitControl1()
        {
            InitializeComponent();

            this.Loaded += WndUnitControl1_Loaded;
            this.Unloaded += WndUnitControl1_Unloaded;

            //this.btMenuTab00.Click += BtMenuTab00_Click;
            //this.btMenuTab01.Click += BtMenuTab01_Click;
            //this.btMenuTab02.Click += BtMenuTab02_Click;
            //this.btMenuTab03.Click += BtMenuTab03_Click;
            //this.btMenuTab04.Click += BtMenuTab04_Click;

            this.btClose.Click += BtClose_Click;
        }

        private void BtClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////////

        private void BtMenuTab04_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_TEACHING_MENU_04);
        }
        private void BtMenuTab03_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_TEACHING_MENU_03);
        }
        private void BtMenuTab02_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_TEACHING_MENU_02);
        }
        private void BtMenuTab01_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_TEACHING_MENU_01);
        }
        private void BtMenuTab00_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_MANUAL_OPERATION_01);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////

        private void WndUnitControl1_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void WndUnitControl1_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch
            {
                // tránh lỗi khi double click quá nhanh
            }
        }
    }
}
