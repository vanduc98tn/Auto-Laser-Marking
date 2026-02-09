using KeyPad;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace Development
{
    /// <summary>
    /// Interaction logic for PgTeachingMenu03.xaml
    /// </summary>
    public partial class PgTeachingMenu03 : Page
    {

        private MyLogger logger = new MyLogger("PgTeachingMenu");
        private volatile bool isUpdate = false;

        int pointSelectButton;

        Button _previousButton;

        private List<bool> L_ListUpdateDevicePLC_12600 = new List<bool>();
        private List<short> D_ListUpdateDevicePLC_500 = new List<short>();

        public PgTeachingMenu03()
        {
            InitializeComponent();

            this.Loaded += PgTeachingMenu03_Loaded;
            this.Unloaded += PgTeachingMenu03_Unloaded;

            this.btMenuTab00.Click += BtMenuTab00_Click;
            this.btMenuTab01.Click += BtMenuTab01_Click;
            this.btMenuTab02.Click += BtMenuTab02_Click;
            this.btMenuTab03.Click += BtMenuTab03_Click;
            this.btMenuTab04.Click += BtMenuTab04_Click;

            this.btLoadTab1Pos0.Click += btLoadTab1Pos0_Click;
            this.btLoadTab1Pos1.Click += btLoadTab1Pos1_Click;
            this.btLoadTab1Pos2.Click += btLoadTab1Pos2_Click;
            this.btLoadTab1Pos3.Click += btLoadTab1Pos3_Click;
            this.btLoadTab1Pos4.Click += btLoadTab1Pos4_Click;
            this.btLoadTab1Pos5.Click += btLoadTab1Pos5_Click;
            this.btLoadTab1Pos6.Click += btLoadTab1Pos6_Click;
            //this.btLoadTab1Pos7.Click += btLoadTab1Pos7_Click;
            //this.btLoadTab1Pos8.Click += btLoadTab1Pos8_Click;
            //this.btLoadTab1Pos9.Click += btLoadTab1Pos9_Click;

            this.btLoadTab2Pos0.Click += btLoadTab2Pos0_Click;
            this.btLoadTab2Pos1.Click += btLoadTab2Pos1_Click;
            this.btLoadTab2Pos2.Click += btLoadTab2Pos2_Click;
            this.btLoadTab2Pos3.Click += btLoadTab2Pos3_Click;
            this.btLoadTab2Pos4.Click += btLoadTab2Pos4_Click;
            this.btLoadTab2Pos5.Click += btLoadTab2Pos5_Click;
            this.btLoadTab2Pos6.Click += btLoadTab2Pos6_Click;
            this.btLoadTab2Pos7.Click += btLoadTab2Pos7_Click;
            //this.btLoadTab2Pos8.Click += btLoadTab2Pos8_Click;
            //this.btLoadTab2Pos9.Click += btLoadTab2Pos9_Click;

            //this.btLoadTab3Pos0.Click += btLoadTab3Pos0_Click;
            //this.btLoadTab3Pos1.Click += btLoadTab3Pos1_Click;
            //this.btLoadTab3Pos2.Click += btLoadTab3Pos2_Click;
            //this.btLoadTab3Pos3.Click += btLoadTab3Pos3_Click;
            //this.btLoadTab3Pos4.Click += btLoadTab3Pos4_Click;
            //this.btLoadTab3Pos5.Click += btLoadTab3Pos5_Click;
            //this.btLoadTab3Pos6.Click += btLoadTab3Pos6_Click;
            //this.btLoadTab3Pos7.Click += btLoadTab3Pos7_Click;
            //this.btLoadTab3Pos8.Click += btLoadTab3Pos8_Click;
            //this.btLoadTab3Pos9.Click += btLoadTab3Pos9_Click;

            this.btRunPoint.Clicked += BtRunPoint_Clicked;

        }


        private void BtRunPoint_Clicked(object sender, RoutedEventArgs e)
        {
            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim go to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.D, 511, Convert.ToInt32(lbPoint.Content));
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 2605, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 2605, false);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////

        private void btLoadTab3Pos9_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5272, 9);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15776, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15776, false);

        }
        private void btLoadTab3Pos8_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5272, 8);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15776, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15776, false);

        }
        private void btLoadTab3Pos7_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5272, 7);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15776, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15776, false);

        }
        private void btLoadTab3Pos6_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5272, 6);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15776, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15776, false);

        }
        private void btLoadTab3Pos5_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5272, 5);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15776, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15776, false);

        }
        private void btLoadTab3Pos4_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5272, 4);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15776, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15776, false);

        }
        private void btLoadTab3Pos3_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5272, 3);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15776, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15776, false);

        }
        private void btLoadTab3Pos2_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5272, 2);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15776, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15776, false);

        }
        private void btLoadTab3Pos1_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5272, 1);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15776, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15776, false);

        }
        private void btLoadTab3Pos0_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5272, 0);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15776, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15776, false);

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////


        private void btLoadTab2Pos9_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5262, 9);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15766, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15766, false);

        }
        private void btLoadTab2Pos8_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5262, 8);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15766, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15766, false);

        }
        private void btLoadTab2Pos7_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5262, 7);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15766, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15766, false);

        }
        private void btLoadTab2Pos6_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5262, 6);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15766, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15766, false);

        }
        private void btLoadTab2Pos5_Click(object sender, RoutedEventArgs e)
        {

            //WndComfirm comfirmYesNo = new WndComfirm();
            //if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            //UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5262, 5);
            //Thread.Sleep(10);
            //UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15766, true);
            //Thread.Sleep(10);
            //UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15766, false);

        }
        private void btLoadTab2Pos4_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5262, 4);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15766, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15766, false);

        }
        private void btLoadTab2Pos3_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5262, 3);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15766, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15766, false);

        }
        private void btLoadTab2Pos2_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5262, 2);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15766, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15766, false);

        }
        private void btLoadTab2Pos1_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5262, 1);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15766, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15766, false);

        }
        private void btLoadTab2Pos0_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5262, 0);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15766, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15766, false);

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////

        private void btLoadTab1Pos9_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5252, 9);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15756, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15756, false);

        }
        private void btLoadTab1Pos8_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5252, 8);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15756, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15756, false);

        }
        private void btLoadTab1Pos7_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5252, 7);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15756, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15756, false);

        }
        private void btLoadTab1Pos6_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5252, 6);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15756, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15756, false);

        }
        private void btLoadTab1Pos5_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5252, 5);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15756, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15756, false);

        }
        private void btLoadTab1Pos4_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5252, 4);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15756, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15756, false);

        }
        private void btLoadTab1Pos3_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5252, 3);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15756, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15756, false);

        }
        private void btLoadTab1Pos2_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5252, 2);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15756, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15756, false);

        }
        private void btLoadTab1Pos1_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5252, 1);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15756, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15756, false);

        }
        private void btLoadTab1Pos0_Click(object sender, RoutedEventArgs e)
        {

            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo($"Confrim Load Current to POS")) return;
            UiManager.Instance.PLC.device.WriteWord(DeviceCode.R, 5252, 0);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15756, true);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 15756, false);

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

        private void PgTeachingMenu03_Unloaded(object sender, RoutedEventArgs e)
        {
            this.isUpdate = false;
        }

        private void PgTeachingMenu03_Loaded(object sender, RoutedEventArgs e)
        {
            this.isUpdate = true;
            new Thread(new ThreadStart(this.ReadPLC))
            {
                IsBackground = true
            }.Start();

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////

        private void GridMatrixCreat()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                int point = D_ListUpdateDevicePLC_500[12]; // Tổng số nút bạn muốn tạo
                int columns = 1; // Số cột cố định
                int rows = (int)Math.Ceiling((double)point / columns); // Tính số hàng

                //Xoá
                grMatrixMGZ.Children.Clear();
                grMatrixMGZ.ColumnDefinitions.Clear();
                grMatrixMGZ.RowDefinitions.Clear();
                #region
                //// Đặt số cột và hàng cho Grid
                //for (int i = 0; i < columns; i++)
                //{
                //    grMatrixMGZ.ColumnDefinitions.Add(new ColumnDefinition());
                //}

                //for (int i = 0; i < rows; i++)
                //{
                //    grMatrixMGZ.RowDefinitions.Add(new RowDefinition());
                //}

                //// Tạo các nút và thêm vào Grid

                //for (int i = 0; i < point; i++)
                //{
                //    Button btMatrix = new Button
                //    {
                //        Content = $"{i + 1}",
                //        Tag = i + 1 // Lưu số tương ứng vào Tag
                //    };

                //    // Đăng ký sự kiện Click cho nút
                //    btMatrix.Click += BtMatrix_Click;

                //    // Tính toán vị trí cột và hàng cho từng nút
                //    int row = rows - 1 - (i / columns); //từ dưới lên
                //    if (D_ListUpdateDevicePLC_500[4] < 0)
                //    {
                //        row = i / columns; //từ trên xuống
                //    }
                //    int column = i % columns;

                //    Grid.SetRow(btMatrix, row);
                //    Grid.SetColumn(btMatrix, column);

                //    grMatrixMGZ.Children.Add(btMatrix);

                //}
                #endregion

                int max = Math.Max(Math.Max(columns, rows), point);
                for (int i = 0; i < max; i++)
                {
                    // Tạo Column
                    if (i < columns)
                    {
                        grMatrixMGZ.ColumnDefinitions.Add(new ColumnDefinition());
                    }

                    // Tạo Row
                    if (i < rows)
                    {
                        grMatrixMGZ.RowDefinitions.Add(new RowDefinition());
                    }

                    // Tạo Button
                    if (i < point)
                    {
                        Button btMatrix = new Button
                        {
                            Content = (i + 1).ToString(),
                            Tag = i + 1
                        };

                        btMatrix.Click += BtMatrix_Click;

                        int row = rows - 1 - (i / columns); // từ dưới lên
                        if (D_ListUpdateDevicePLC_500[14] < 0)
                        {
                            row = i / columns; // từ trên xuống
                        }

                        int column = i % columns;

                        Grid.SetRow(btMatrix, row);
                        Grid.SetColumn(btMatrix, column);

                        grMatrixMGZ.Children.Add(btMatrix);
                    }
                }

                this.lbPoint.MouseDown -= LbPoint_MouseDown;
                this.lbPoint.MouseDown += LbPoint_MouseDown;
            }));


        }
        private void BtMatrix_Click(object sender, RoutedEventArgs e)
        {


            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                // Đổi màu nền cho nút hiện tại
                clickedButton.Background = Brushes.LightGreen;

                // Đặt lại màu nền của nút trước đó về màu mặc định
                if (_previousButton != null && _previousButton != clickedButton)
                {
                    _previousButton.Background = Brushes.White; // Màu nền mặc định
                }

                // Lưu nút hiện tại làm nút đã được nhấn trước đó
                _previousButton = clickedButton;

                // Lấy số tương ứng từ Tag và in ra
                pointSelectButton = (int)clickedButton.Tag;

                lbPoint.Content = pointSelectButton.ToString();


            }
        }
        private void LbPoint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keypad kp = new Keypad(false);
            if (kp.ShowDialog() == true)
            {
                int value = Convert.ToInt32(kp.Result);
                int min = 0;
                int max = D_ListUpdateDevicePLC_500[12];
                value = value < min ? min : (value > max ? max : value);

                lbPoint.Content = value.ToString();
            }
        }
        private void ReadPLC()
        {
            try
            {
                while (this.isUpdate)
                {
                    if (UiManager.Instance.PLC.device.isOpen())
                    {

                        UiManager.Instance.PLC.device.ReadMultiWord(DeviceCode.D, 500, 20, out D_ListUpdateDevicePLC_500);

                        if (D_ListUpdateDevicePLC_500.Count > 0)
                        {
                            this.isUpdate = false;
                        }
                        //this.UpdateUI();
                    }

                    Thread.Sleep(20);
                }
                if (UiManager.Instance.PLC.device.isOpen())
                {
                    GridMatrixCreat();
                }
            }
            catch (Exception ex)
            {
                this.logger.Create("ReadPLC Error: " + ex.Message, LogLevel.Error);
            }

        }
        private void UpdateUI()
        {
            if (!UiManager.Instance.PLC.device.isOpen() || !isUpdate) return;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    if (!isUpdate) return;

                    if (L_ListUpdateDevicePLC_12600.Count > 0)
                    {

                    }
                    if (D_ListUpdateDevicePLC_500.Count > 0)
                    {

                    }

                }
                catch (Exception ex)
                {
                    this.logger.Create("Update UI Error: " + ex.Message, LogLevel.Error);
                }
            }));
        }


    }
}
