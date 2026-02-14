using OpenCvSharp.XFeatures2D;
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
    /// Interaction logic for PgMechanicalMenu05.xaml
    /// </summary>
    public partial class PgMechanicalMenu05 : Page
    {
        private MyLogger logger = new MyLogger("PgMechanical05Menu");
        private PatternSetting pattern = UiManager.appSetting.Pattern;
        private SettingDevice settingDevice;
        private Brush EM_COLOR = Brushes.Red;

        public PgMechanicalMenu05()
        {
            InitializeComponent();

            this.Loaded += PgMechanicalMenu05_Loaded;

            this.btMenuTab01.Click += BtMenuTab01_Click;
            this.btMenuTab02.Click += BtMenuTab02_Click;
            this.btMenuTab03.Click += BtMenuTab03_Click;
            //this.btMenuTab04.Click += BtMenuTab04_Click;
            this.btMenuTab05.Click += BtMenuTab05_Click;

            this.btLogClear.Click += BtLogClear_Click;
            this.btSetting.Click += BtSetting_Click;
            this.btSave.Click += BtSave_Click;
            this.btOpen.Click += BtOpen_Click;
            this.btClose.Click += BtClose_Click;

            this.btSend.Click += BtSend_Click;
            this.btPattern.Click += BtPattern_Click;
            this.btAllPos.Click += BtAllPos_Click;
            this.btClrPos.Click += BtClrPos_Click;

        }



        private void BtClrPos_Click(object sender, RoutedEventArgs e)
        {
            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo("You Want To..?")) return;

            foreach (var child in gridPos.Children)
            {
                if (child is Button bt)
                {
                    bt.Tag = false;
                    bt.ClearValue(Button.BackgroundProperty);
                    bt.Foreground = Brushes.Black;
                }
            }
        }
        private void BtAllPos_Click(object sender, RoutedEventArgs e)
        {
            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo("You Want To..?")) return;

            foreach (var child in gridPos.Children)
            {
                if (child is Button bt)
                {
                    bt.Tag = true;
                    bt.Background = EM_COLOR;
                    bt.Foreground = Brushes.White;
                }
            }
        }
        private void BtPattern_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WndPatern comfirmYesNo = new WndPatern();
                if (!comfirmYesNo.DoConfirmYesNo()) return;
                this.generateCells(pattern.xRow, pattern.yColumn, pattern.CurrentPatern, pattern.Use2Matrix);
                UpdateLogs($"Click Button Save to Complete");
            }
            catch (Exception ex)
            {
                this.logger.Create("BtPattern_Click: " + ex.Message, LogLevel.Error);
            }
        }
        private void BtSend_Click(object sender, RoutedEventArgs e)
        {
            string strResult = "";
            string strData = "Test";

            UpdateLogs($"Send: {strData}");

            byte[] arr1 = Encoding.UTF8.GetBytes(strData).ToArray();
            byte[] arr2 = { 0x0D };

            byte[] byData = new byte[arr1.Length + arr2.Length];

            Array.Copy(arr1, 0, byData, 0, arr1.Length);
            Array.Copy(arr2, 0, byData, arr1.Length, arr2.Length);


            byte[] byResult = UiManager.Instance.laserCOM.SendWaitResponse(byData);


            if (byResult != null)
            {
                strResult = ASCIIEncoding.ASCII.GetString(byResult.ToArray());
            }
            else
            {
                strResult = "Time out!";
            }

            UpdateLogs($"Receive: {strResult}");
        }

        private void BtClose_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.laserCOM.Close();
            UpdateUiButton();
        }
        private void BtOpen_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.laserCOM.Open();
            UpdateUiButton();
        }
        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo("You Want Save Setting?")) return;
            UiManager.appSetting.settingDevice.COMLaser = settingDevice.COMLaser;
            this.SaveSelectPositon();
            UpdateLogs($"Save Setting Com Laser Complete");
            UiManager.SaveAppSetting();
        }
        private void BtSetting_Click(object sender, RoutedEventArgs e)
        {
            WndComSetting wndMC = new WndComSetting();
            var settingNew = wndMC.DoSettings(Window.GetWindow(this), this.settingDevice.COMLaser);
            if (settingNew != null)
            {
                this.settingDevice.COMLaser = settingNew;
                UpdateLogs($"Device Seting PortName :{settingNew.portName.ToString()}");
                UpdateLogs($"Device Seting Parity :{settingNew.parity}");
                UpdateLogs($"Device Seting Databis :{settingNew.dataBits.ToString()}");
                UpdateLogs($"Device Seting Stopbit :{settingNew.stopBits.ToString()}");
                UpdateLogs($"Device Seting Handshake :{settingNew.Handshake.ToString()}");
                UpdateLogs($"Click Button Save to Complete");
            }
        }
        private void BtLogClear_Click(object sender, RoutedEventArgs e)
        {
            WndComfirm comfirmYesNo = new WndComfirm();
            if (!comfirmYesNo.DoComfirmYesNo("You Want Clear?")) return;
            this.ClearLogs();
        }

        private void PgMechanicalMenu05_Loaded(object sender, RoutedEventArgs e)
        {
            
            try
            {
                settingDevice = UiManager.appSetting.settingDevice;
                this.generateCells(pattern.xRow, pattern.yColumn, pattern.CurrentPatern, pattern.Use2Matrix);
                LoadPosition();
                UpdateUiButton();
            }
            catch (Exception ex)
            {
                this.logger.Create("PgTeachingMenu_Loaded: " + ex.Message, LogLevel.Error);
            }
            UpdateUiButton();
        }

        private void UpdateUiButton()
        {
            if (UiManager.Instance.laserCOM.isOpen())
            {
                UpdateLogs("Connect Tester Complete");
                btClose.Background = Brushes.White;
                btOpen.Background = Brushes.LightGreen;
            }
            else
            {
                UpdateLogs("Disconnect Tester");
                btClose.Background = Brushes.OrangeRed;
                btOpen.Background = Brushes.White;
            }
        }
        private void UpdateLogs(string notify)
        {
            this.Dispatcher.Invoke(() => {
                this.txtLogs.Text += "\r\n" + notify;
                this.txtLogs.ScrollToEnd();
            });
        }
        private void ClearLogs()
        {
            this.Dispatcher.Invoke(() => {
                this.txtLogs.Clear();
            });
        }
        private void generateCells(int rowCnt, int colCnt, int pattern, bool Use2Matrix)
        {
            gridPos.Children.Clear();
            gridPos.RowDefinitions.Clear();
            gridPos.ColumnDefinitions.Clear();

            // Rows
            for (int r = 0; r < rowCnt; r++)
            {
                gridPos.RowDefinitions.Add(
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                );
            }

            int matrixCols = Use2Matrix ? colCnt / 2 : colCnt;
            int gapCols = Use2Matrix ? 1 : 0;
            int totalCols = Use2Matrix ? matrixCols * 2 + gapCols : colCnt;

            // Columns (có GAP)
            for (int c = 0; c < totalCols; c++)
            {
                if (Use2Matrix && c == matrixCols)
                {
                    // Cột trống giữa 2 matrix
                    gridPos.ColumnDefinitions.Add(
                        new ColumnDefinition { Width = new GridLength(10) } // px
                    );
                }
                else
                {
                    gridPos.ColumnDefinitions.Add(
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    );
                }
            }

            int matrixSize = rowCnt * matrixCols;

            // Create cells
            for (int id = 1; id <= rowCnt * colCnt; id++)
            {
                int r = GetCellRow(id, pattern, rowCnt, colCnt, Use2Matrix);
                int c = GetCellColumn(id, pattern, rowCnt, colCnt, Use2Matrix);

                var cell = createCell(id);
                gridPos.Children.Add(cell);
                Grid.SetRow(cell, r);
                Grid.SetColumn(cell, c);
            }
        }
        private Button createCell(int number)
        {
            //var cell = new Button();
            //cell.Content = String.Format("{0}", number);
            //cell.FontWeight = FontWeights.Bold;
            //cell.FontSize = 12;
            ////cell.Margin = new Thickness(1, 1, 1, 1);
            //cell.Name = String.Format("btCell{0:00}", number);
            //cell.Background = Brushes.LightGray;
            //cell.Click += this.Cell_Click;
            //cell.TouchDown += this.Cell_Click;
            //lstButtonPos.Add(cell);
            //return cell;

            var cell = new Button();
            cell.Tag = false; // Unselected
            cell.Content = createCellContent(String.Format("{0}", number));
            cell.Name = String.Format("lblCell{0:00}", number);
            cell.HorizontalContentAlignment = HorizontalAlignment.Center;
            cell.VerticalContentAlignment = VerticalAlignment.Center;
            cell.BorderThickness = new Thickness(1);
            cell.BorderBrush = Brushes.Gray;

            cell.Click += this.Cell_Click;
            //cell.PreviewMouseDown += this.Cell_Click;
            return cell;
        }
        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cell = sender as Button;
                if ((bool)cell.Tag)
                {
                    cell.Tag = false;
                    cell.ClearValue(Button.BackgroundProperty);
                    cell.Foreground = Brushes.Black;
                }
                else
                {
                    cell.Tag = true;
                    cell.Background = EM_COLOR;
                    cell.Foreground = Brushes.White;
                }
                //updateCounter();
            }
            catch (Exception ex)
            {
                logger.Create("Cell_Click:" + ex.Message, LogLevel.Error);
            }
        }
        private object createCellContent(String qr)
        {
            var cellText = new TextBlock();
            cellText.TextWrapping = TextWrapping.Wrap;
            cellText.FontSize = 10;
            cellText.Text = String.Format("{0}", qr);
            return cellText;
        }
        private void SaveSelectPositon()
        {
            List<int> lstPos = new List<int>();
            foreach (var cell in gridPos.Children)
            {
                var btCell = cell as Button;

                if (btCell != null && btCell.Background == EM_COLOR)
                {
                    var textcell = btCell.Content as TextBlock;
                    lstPos.Add(Convert.ToInt32(textcell.Text.ToString()));
                }
            }
            lstPos.Sort();
            pattern.positionNGs = lstPos;
            UiManager.SaveAppSetting();
        }
        private void LoadPosition()
        {
            var pos = pattern.positionNGs;
            foreach (var cell in gridPos.Children)
            {
                if (cell is Button btCell && btCell.Content is TextBlock textcell && pos.Contains(int.Parse(textcell.Text)))
                {
                    btCell.Background = EM_COLOR;
                    btCell.Foreground = Brushes.White;
                }
            }

        }

        private int GetCellRow(int cellId, int pattern, int row, int column, bool use2Matrix)
        {
            if (!use2Matrix)
                return GetCellRowCaculator(cellId, pattern, row, column);

            int matrixCols = column / 2;
            int matrixSize = row * matrixCols;

            int localCellId = (cellId - 1) % matrixSize + 1;

            return GetCellRowCaculator(localCellId, pattern, row, matrixCols);
        }
        private int GetCellColumn(int cellId, int pattern, int row, int column, bool use2Matrix)
        {
            if (!use2Matrix)
                return GetCellColumnCaculator(cellId, pattern, row, column);

            int matrixCols = column / 2;
            int matrixSize = row * matrixCols;
            int gapCols = 1;

            int matrixIndex = (cellId - 1) / matrixSize; // 0 hoặc 1
            int localCellId = (cellId - 1) % matrixSize + 1;

            int col = GetCellColumnCaculator(localCellId, pattern, row, matrixCols);

            return col + matrixIndex * (matrixCols + gapCols);
        }
        private int GetCellRowCaculator(int cellId, int pattern, int row, int column)
        {
            int xSize = column;
            int ySize = row;
            // new
            if (pattern == 1 || pattern == 2)
            {
                return ySize - 1 - (cellId - 1) / xSize;
            }
            else if (pattern == 3 || pattern == 4)
            {
                return (cellId - 1) / xSize;
            }
            else if (pattern == 5)
            {
                if (((cellId - 1) / ySize) % 2 == 0)
                {
                    return ySize - 1 - ((cellId - 1) % ySize);
                }
                else
                {
                    return (cellId - 1) % ySize;
                }
            }
            else if (pattern == 6)
            {
                if (((cellId - 1) / ySize) % 2 != 0)
                {
                    return ySize - 1 - ((cellId - 1) % ySize);
                }
                else
                {
                    return (cellId - 1) % ySize;
                }
            }
            else if (pattern == 7)
            {
                if ((column - 1) % 2 != 0)
                {
                    if ((xSize - 1 - (cellId - 1) / ySize) % 2 != 0)
                    {
                        return (cellId - 1) % ySize;
                    }
                    else
                    {
                        return ySize - 1 - ((cellId - 1) % ySize);
                    }
                }
                else
                {
                    if ((xSize - 1 - (cellId - 1) / ySize) % 2 == 0)
                    {
                        return (cellId - 1) % ySize;
                    }
                    else
                    {
                        return ySize - 1 - ((cellId - 1) % ySize);
                    }
                }
            }
            else if (pattern == 8)
            {
                if ((column - 1) % 2 != 0)
                {
                    if ((xSize - 1 - (cellId - 1) / ySize) % 2 == 0)
                    {
                        return (cellId - 1) % ySize;
                    }
                    else
                    {
                        return ySize - 1 - ((cellId - 1) % ySize);
                    }
                }
                else
                {
                    if ((xSize - 1 - (cellId - 1) / ySize) % 2 != 0)
                    {
                        return (cellId - 1) % ySize;
                    }
                    else
                    {
                        return ySize - 1 - ((cellId - 1) % ySize);
                    }
                }
            }
            else if (pattern == 9 || pattern == 10)
            {
                return ySize - 1 - (cellId - 1) / xSize;
            }
            else if (pattern == 11 || pattern == 12)
            {
                return (cellId - 1) / xSize;
            }
            else if (pattern == 13 || pattern == 15)
            {
                return ySize - 1 - ((cellId - 1) % ySize);
            }
            else if (pattern == 14 || pattern == 16)
            {
                return (cellId - 1) % ySize;
            }
            return 0;
        }
        private int GetCellColumnCaculator(int cellId, int pattern, int row, int column)
        {
            int xSize = column;
            int ySize = row;

            if (pattern == 1)
            {
                if ((row - 1) % 2 != 0)
                {
                    if ((ySize - 1 - (cellId - 1) / xSize) % 2 != 0)
                    {
                        return (cellId - 1) % xSize;
                    }
                    else
                    {
                        return xSize - 1 - ((cellId - 1) % xSize);
                    }
                }
                else
                {
                    if ((ySize - 1 - (cellId - 1) / xSize) % 2 == 0)
                    {
                        return (cellId - 1) % xSize;
                    }
                    else
                    {
                        return xSize - 1 - ((cellId - 1) % xSize);
                    }
                }
            }
            else if (pattern == 2)
            {
                if ((row - 1) % 2 != 0)
                {
                    if ((ySize - 1 - (cellId - 1) / xSize) % 2 == 0)
                    {
                        return (cellId - 1) % xSize;
                    }
                    else
                    {
                        return xSize - 1 - ((cellId - 1) % xSize);
                    }
                }
                else
                {
                    if ((ySize - 1 - (cellId - 1) / xSize) % 2 != 0)
                    {
                        return (cellId - 1) % xSize;
                    }
                    else
                    {
                        return xSize - 1 - ((cellId - 1) % xSize);
                    }
                }
            }
            else if (pattern == 3)
            {
                if (((cellId - 1) / xSize) % 2 == 0)
                {
                    return (cellId - 1) % xSize;
                }
                else
                {
                    return xSize - 1 - ((cellId - 1) % xSize);
                }
            }
            else if (pattern == 4)
            {
                if (((cellId - 1) / xSize) % 2 != 0)
                {
                    return (cellId - 1) % xSize;
                }
                else
                {
                    return xSize - 1 - ((cellId - 1) % xSize);
                }
            }
            else if (pattern == 5 || pattern == 6)
            {
                return (cellId - 1) / ySize;
            }
            else if (pattern == 7 || pattern == 8)
            {
                return xSize - 1 - (cellId - 1) / ySize;
            }
            else if (pattern == 9 || pattern == 12)
            {
                return (cellId - 1) % xSize;
            }
            else if (pattern == 10 || pattern == 11)
            {
                return xSize - 1 - ((cellId - 1) % xSize);
            }
            else if (pattern == 13 || pattern == 14)
            {
                return (cellId - 1) / ySize;
            }
            else if (pattern == 15 || pattern == 16)
            {
                return xSize - 1 - (cellId - 1) / ySize;
            }
            return 0;
        }

        private void BtMenuTab05_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_MECHANICAL_MENU_05);
        }
        private void BtMenuTab04_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_MECHANICAL_MENU_04);
        }
        private void BtMenuTab03_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_MECHANICAL_MENU_03);
        }
        private void BtMenuTab02_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_MECHANICAL_MENU_02);
        }
        private void BtMenuTab01_Click(object sender, RoutedEventArgs e)
        {
            UiManager.Instance.SwitchPage(PAGE_ID.PAGE_MECHANICAL_MENU_01);
        }
    }
}
