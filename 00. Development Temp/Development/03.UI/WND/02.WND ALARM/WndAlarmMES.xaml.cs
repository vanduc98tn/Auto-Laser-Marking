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
    /// Interaction logic for WndAlarmMES.xaml
    /// </summary>
    public partial class WndAlarmMES : Window 
    {
        private int Number = 0;
        public static WndAlarmMES Instance { get; private set; }
        public WndAlarmMES()
        {
            InitializeComponent();
            Instance = this;
            this.btnClose.Click += BtnClose_Click;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public  void Messenger (string message, string Solution , Window owner = null)
        {
            this.txtMessage.Text = message;
            this.txtSolution.Text = Solution;
            this.txtTime.Text = DateTime.Now.ToString("yyyy/MM/dd : HH/mm/ss");
            this.txtCode.Text = "1";
            this.txtSeqId.Text = (Number + 1).ToString();
            this.Show();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        public void CloseAlarm()
        {
           
            this.Close();
           
        }
    }
}
