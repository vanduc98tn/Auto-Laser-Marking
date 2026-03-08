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
    /// Interaction logic for RegionProperty.xaml
    /// </summary>
    public partial class SetupPLCBitROI : Window
    {
        public ROISettings roiSettings = UiManager.currentModel.VisionModel.ROI;
        public int indexROI = 0;
        public SetupPLCBitROI()
        {
            InitializeComponent();
            this.btnOK.Click += BtnOK_Click;
            this.btnCancle.Click += BtnCancle_Click;
            this.Loaded += RegionProperty_Loaded;
        }
        public SetupPLCBitROI(int indexROI)
        {
            InitializeComponent();
            this.indexROI = indexROI;
            this.btnOK.Click += BtnOK_Click;
            this.btnCancle.Click += BtnCancle_Click;
            this.Loaded += RegionProperty_Loaded;
        }

        private void RegionProperty_Loaded(object sender, RoutedEventArgs e)
        {
            if (roiSettings.listPLCBit.TryGetValue(indexROI, out int value))
            {
                xctPLCBit.Value = value;
            }
        }

        private void BtnCancle_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (roiSettings.listPLCBit.ContainsKey(indexROI))
                roiSettings.listPLCBit[indexROI] = (int)xctPLCBit.Value;
            else
                roiSettings.listPLCBit.Add(indexROI, (int)xctPLCBit.Value);
            if(roiSettings.listPLCBit.Count < roiSettings.listRectangle.Count)
            {
                for (int i = 1; i < roiSettings.listRectangle.Count + 1; i++)
                {
                    if (!roiSettings.listPLCBit.ContainsKey(i))
                    {
                        roiSettings.listPLCBit.Add(i, -1);
                    }
                }
            }
            UiManager.SaveCurrentModelSettings();
            this.Close();
        }

        public bool DoConfirmMatrix(Point p, Window owner = null)
        {
            //UserManager.createUserLog(UserActions.CONFIRM_SHOW_YESNO);
            this.Owner = owner;
            this.Left = p.X;
            this.Top = p.Y;
            this.ShowDialog();
            return true;
        }


    }
}
