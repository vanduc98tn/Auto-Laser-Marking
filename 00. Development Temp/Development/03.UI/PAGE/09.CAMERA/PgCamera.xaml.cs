using Microsoft.Win32;
using MvCamCtrl.NET;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Path = System.IO.Path;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Development
{
    /// <summary>
    /// Interaction logic for PgCamera.xaml
    /// </summary>
    public partial class PgCamera : Page, INotifyPropertyChanged
    {
        private bool autoScrollMode = true;
        private ConnectionSettings connectionSettings = UiManager.appSetting.connection;
        private MyLogger logger = new MyLogger("Camera Page");
        private HikCam searchCam = new HikCam();
        CameraOperator m_pOperator = new CameraOperator();
        private System.Timers.Timer clock;
        private bool stopCamera = true;

        private VisionAL vision1 = new VisionAL(VisionAL.Chanel.Ch1);
        private VisionAL vision2 = new VisionAL(VisionAL.Chanel.Ch2);
        private Object cameraTrigger = new Object();
        private Mat Image;

        //Canvas
        protected bool isDragging;
        private System.Windows.Point clickPosition;
        private TranslateTransform originTT;
        //Save Image & Delete Oldest File
        public FileQueueManager FileQM = new FileQueueManager();

        //[*********************************** BINDING ***********************************]
        public event PropertyChangedEventHandler PropertyChanged;
        #region Internal Field
        private string _fileName = "Default", _folderPath = @"C:\", _freeDisk = "0.0", _diskSize = "GB";
        private string _imageFormat = "BMP";
        private double _numUDImgStorage = 0d, _maxImgStorage = 10d;
        private bool _isAddDateTime = false, _isUseFolderOK = false, _isUseFolderNG = false;
        #endregion

        #region Property
        public string txtFileName { get => _fileName; set { _fileName = value; OnPropertyChanged(nameof(txtFileName)); } }
        public string txtFolderPath { get => _folderPath; set { _folderPath = value; OnPropertyChanged(nameof(txtFolderPath)); } }
        public string ImageFormatSelected { get => _imageFormat; set { _imageFormat = value; OnPropertyChanged(nameof(ImageFormatSelected)); } }
        public string DiskSize { get => _diskSize; set { _diskSize = value; OnPropertyChanged(nameof(DiskSize)); } }
        public bool IsAddDateTime { get => _isAddDateTime; set { _isAddDateTime = value; OnPropertyChanged(nameof(IsAddDateTime)); } }
        public bool IsUseFolderOK { get => _isUseFolderOK; set { _isUseFolderOK = value; OnPropertyChanged(nameof(IsUseFolderOK)); } }
        public bool IsUseFolderNG { get => _isUseFolderNG; set { _isUseFolderNG = value; OnPropertyChanged(nameof(IsUseFolderNG)); } }
        public string txtFreeDisk { get => _freeDisk; set { _freeDisk = value; OnPropertyChanged(nameof(txtFreeDisk)); } }
        public double NumUDImageStorage { get => _numUDImgStorage; set { _numUDImgStorage = value; OnPropertyChanged(nameof(NumUDImageStorage)); } }
        public double MaxImageStorage { get => _maxImgStorage; set { _maxImgStorage = value; OnPropertyChanged(nameof(MaxImageStorage)); } }
        protected void OnPropertyChanged(string pptName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pptName));
        #endregion
        public PgCamera()
        {
            InitializeComponent();
            this.DataContext = this;
            this.clock = new System.Timers.Timer(500);
            this.clock.AutoReset = true;


            this.Cam1ExposeTime.ValueChanged += Cam1ExposeTime_ValueChanged;
            this.Cam2ExposeTime.ValueChanged += Cam2ExposeTime_ValueChanged;

            this.btnCamSaveSetting.Click += this.btnCamSave_Clicked;
            this.btnCamChooseFile.Click += BtnCamChooseFile_Click;

            this.sldGamma.ValueChanged += SldGamma_ValueChanged;
            this.sldAlpha.ValueChanged += SldAlpha_ValueChanged;
            this.sldBeta.ValueChanged += SldBeta_ValueChanged;

            this.txtGammasld.PreviewKeyDown += TxtGammasld_PreviewKeyDown;
            this.txtAlphasld.PreviewKeyDown += TxtAlphasld_PreviewKeyDown;
            this.txtBetasld.PreviewKeyDown += TxtBetasld_PreviewKeyDown;

            //Canvas
            //Image Source Update Event
            var prop = DependencyPropertyDescriptor.FromProperty(System.Windows.Controls.Image.SourceProperty, typeof(System.Windows.Controls.Image));
            prop.AddValueChanged(this.imgView, SourceChangedHandler);

            //Offset Jig
            this.btnSetOffset.Click += BtnSetOffset_Click;

            //Otion Cam
            this.btnCamOneShot.Click += BtnCamOneShot_Click;
            this.btnCamCtn.Click += BtnCamCtn_Click;
            this.btnVSJob.Click += BtnVSJob_Click;

            //Tabar
            this.btnCameraZoomOut.Click += BtnCameraZoomOut_Click;
            this.btnCameraZoomIn.Click += BtnCameraZoomIn_Click;

            //SetModel
            this.cbxCameraCh.SelectionChanged += CbxCamera_SelectionChanged;

            this.Loaded += this.PgCamera_Load;
            this.Unloaded += PgCamera_Unloaded;

            Canvas.SetLeft(myCanvas, 100);
            Canvas.SetTop(myCanvas, 100);

            //Creat ROI
            btnCreatRoi.Click += BtnCreatRoi_Click;
            btnCreatRegion.Click += BtnCreatRegion_Click;
            btnDeleteRegionAll.Click += BtnDeleteRegionAll_Click;
            this.KeyDown += PgCamera1_KeyDown;
            this.cbxShowRoi.Click += CbxShowRoi_Click;
            this.cbxRoiMtrix.Click += CbxRoiMtrix_Click;
            this.cbxRoiManual.Click += CbxRoiManual_Click;
            this.cbxAutoIndexRoi.Click += CbxAutoIndexRoi_Click;
            this.cbxManualIndexRoi.Click += CbxManualIndexRoi_Click;
            this.btnROIUp.Click += BtnROIUp_Click;
            this.btnROIDown.Click += BtnROIDown_Click;
            this.btnROIRight.Click += BtnROIRight_Click;
            this.btnROILeft.Click += BtnROILeft_Click;

        }
        #region Event tab

        private void BtnSetOffset_Click(object sender, RoutedEventArgs e)
        {
            int[] offSet = new int[2];
            string channel = "";
            this.Dispatcher.Invoke(() => {
                channel = this.cbxCameraCh.SelectedValue.ToString();
            });
            if (channel == "CH1")
            {
                Mat src = UiManager.Cam1.CaptureImage();
                if (src != null)
                {
                    src.SaveImage("temp1.bmp");
                    Mat src1 = Cv2.ImRead("temp1.bmp", ImreadModes.Color);
                    offSet = vision1.SetBarcodeOffSet(src1);

                }
            }
            else if (channel == "CH2")
            {
                Mat src = UiManager.Cam2.CaptureImage();
                if (src != null)
                {
                    src.SaveImage("temp2.bmp");
                    Mat src1 = Cv2.ImRead("temp2.bmp", ImreadModes.Color);
                    offSet = vision2.SetBarcodeOffSet(src1);
                }

            }
            this.xOffset.Value = offSet[0];
            this.yOffset.Value = offSet[1];
        }

        private void PgCamera_Unloaded(object sender, RoutedEventArgs e)
        {
            this.ShutDownCam();
            DeleteAllRegion();
        }
        private void ShutDownCam()
        {
            stopCamera = true;
        }
        private void BtnCamCtn_Click(object sender, RoutedEventArgs e)
        {
            stopCamera = false;
            callThreadStartLoop();
        }

        private void BtnCamOneShot_Click(object sender, RoutedEventArgs e)
        {
            stopCamera = true;
            callThreadStartLoop();
        }

        private void BtnCamChooseFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (openFileDialog.ShowDialog() == true)
                {
                    txtCamChoosefile.Text = openFileDialog.FileName;
                    this.Image = Cv2.ImRead(openFileDialog.FileName, ImreadModes.Color);
                    this.Dispatcher.Invoke(() =>
                    {
                        ShowImage();
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Create("open file load config dialog cam1 err : " + ex.ToString(), LogLevel.Error);
            }
        }



        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            try
            {
                if (e.Source.GetType().Equals(typeof(ScrollViewer)))
                {
                    ScrollViewer sv = (ScrollViewer)e.Source;
                    if (sv != null)
                    {
                        // User scroll event : set or unset autoscroll mode
                        if (e.ExtentHeightChange == 0)
                        {   // Content unchanged : user scroll event
                            if (sv.VerticalOffset == sv.ScrollableHeight)
                            {   // Scroll bar is in bottom -> Set autoscroll mode
                                autoScrollMode = true;
                            }
                            else
                            {   // Scroll bar isn't in bottom -> Unset autoscroll mode
                                autoScrollMode = false;
                            }
                        }

                        // Content scroll event : autoscroll eventually
                        if (autoScrollMode && e.ExtentHeightChange != 0)
                        {   // Content changed and autoscroll mode set -> Autoscroll
                            sv.ScrollToVerticalOffset(sv.ExtentHeight);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write("ScrollChanged error:" + ex.Message);
            }
        }


        private void PgCamera_Load(object sender, RoutedEventArgs e)
        {
            this.modelSet();
            this.LoadRegion();
            this.RoiShowCheck();

            AddeviceCam();
            showCamDevice();
            try
            {
                Mat srcDisplay1 = Cv2.ImRead("temp1.bmp", ImreadModes.Color);
            }
            catch (Exception ex)
            {
                logger.Create("ReadTemp1 Image Err" + ex.Message, LogLevel.Error);
            }

            this.Image = (imgView.Source as BitmapSource).ToMat();
            if(Image.Channels() < 3)
            {
                Cv2.CvtColor(Image, Image, ColorConversionCodes.GRAY2BGR);
            }    
            else if(Image.Channels() > 3)
            {
                Cv2.CvtColor(Image, Image, ColorConversionCodes.BGRA2BGR);
            }
            this.Dispatcher.Invoke(() =>
            {
                var visionModel = UiManager.currentModel.VisionModel;
                this.dblMatchingRateMin.Value = visionModel.MatchingRateMin;
                this.intWhitePixel.Value = visionModel.WhitePixels;
                this.intBlackPixel.Value = visionModel.BlackPixels;
                this.dblMatchingRate.Value = visionModel.MatchingRate;
                this.intThreshol.Value = visionModel.Threshol;
                this.intThresholBl.Value = visionModel.ThresholBl;
                this.rdnCirWh.IsChecked = visionModel.CirWhCntEnb;
                this.rdnRoiWh.IsChecked = visionModel.RoiWhCntEnb;

                this.txtAlphasld.Text = visionModel.AlphaSld.ToString("F1");
                this.txtBetasld.Text = visionModel.BetaSld.ToString("F1");
                this.txtGammasld.Text = visionModel.GammaSld.ToString("F1");

                //Save Image
                var imageLogger = visionModel.ImageLogger;
                this.txtFolderPath = imageLogger.folderPath;
                this.txtFileName = imageLogger.fileName;
                this.IsAddDateTime = imageLogger.isAddDateTime;
                this.IsUseFolderOK = imageLogger.isUseFolderOK;
                this.IsUseFolderNG = imageLogger.isUseFolderNG;
                this.ImageFormatSelected = imageLogger.imageFormat;
                this.NumUDImageStorage = imageLogger.imageStorage;
                //Save Image
                FileQM = new FileQueueManager(new string[]
                        { txtFolderPath, Path.Combine(txtFolderPath, "OK"), Path.Combine(txtFolderPath, "NG") }, NumUDImageStorage);
            });
        }
        private void Cam1ExposeTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.Cam1ExposeTime.Value == null)
                return;
            UiManager.appSetting.connection.camSettings.ExposeTime = (int)this.Cam1ExposeTime.Value;
            UiManager.SaveAppSetting();
            UiManager.Cam1.SetExposeTime((int)UiManager.appSetting.connection.camSettings.ExposeTime);
        }
        private void Cam2ExposeTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.Cam2ExposeTime.Value == null)
                return;
            UiManager.appSetting.connection.camSettings.ExposeTime = (int)this.Cam2ExposeTime.Value;
            UiManager.SaveAppSetting();
            UiManager.Cam2.SetExposeTime((int)UiManager.appSetting.connection.camSettings.ExposeTime);
        }

        #endregion

        #region Vision Event
        private void BtnVSJob_Click(object sender, RoutedEventArgs e)
        {
            ShapeEditorControl.ReleaseElement();
            cbxShowRoi.IsChecked = false;
            VisionAL vision;
            if (cbxCameraCh.SelectedValue.ToString() == "CH1")
            {
                vision = vision1;
            }
            else
            {
                vision = vision2;
            }
            if (this.Image != null)
            {
                try
                {
                    vision.Image1 = this.Image.Clone();
                    List<OpenCvSharp.Rect> OpencvRectLst = new List<OpenCvSharp.Rect>();
                    for (int i = 0; i < RectLst.Count; i++)
                    {
                        OpencvRectLst.Add(new OpenCvSharp.Rect((int)Canvas.GetLeft(RectLst[i]), (int)Canvas.GetTop(RectLst[i]), (int)RectLst[i].ActualWidth, (int)RectLst[i].ActualHeight));
                    }
                    //vision.visionCheck(OpencvRectLst);
                    vision.VisionCheck(OpencvRectLst);
                    this.Dispatcher.Invoke(() =>
                    {
                        imgView.Source = vision.Image1.ToWriteableBitmap(PixelFormats.Bgr24);
                    });
                }
                catch (Exception ex)
                {
                    logger.Create("Vision MAnual Job Err: " + ex.Message, LogLevel.Error);
                }

            }
            stopCamera = true;
        }
        #endregion

        #region CameraFuntion


        Boolean AddeviceCam()
        {
            treeViewGige.Items.Clear();
            treeViewUSB.Items.Clear();
            txt_Camera1_name_device.Items.Clear();
            txt_Camera2_name_device.Items.Clear();

            //hikCamera.DeviceListAcq();

            MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList = UiManager.hikCamera.m_pDeviceList;
            for (int i = 0; i < m_pDeviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                    if (gigeInfo.chUserDefinedName != "")
                    {
                        string Caminfo = (String.Format("GigE: " + gigeInfo.chUserDefinedName + " (" + gigeInfo.chSerialNumber + ")"));
                        updateCbx(Caminfo, i);

                    }
                    else
                    {
                        string Caminfo = String.Format(("GigE: " + gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" + gigeInfo.chSerialNumber + ")"));
                        updateCbx(Caminfo, i);
                    }
                }
                else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stUsb3VInfo, 0);
                    MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));

                    if (usbInfo.chUserDefinedName != "")
                    {
                        string Caminfo = String.Format(("USB: " + usbInfo.chUserDefinedName + " (" + usbInfo.chSerialNumber + ")"));
                        updateCbx(Caminfo, i);
                    }
                    else
                    {
                        string Caminfo = String.Format(("USB: " + usbInfo.chManufacturerName + " " + usbInfo.chModelName + " (" + usbInfo.chSerialNumber + ")"));
                        updateCbx(Caminfo, i);
                    }
                }
            }
            return true;

        }
        void showCamDevice()
        {


            MyCamera.MV_CC_DEVICE_INFO device = connectionSettings.camSettings.device;
            MyCamera.MV_CC_DEVICE_INFO device2 = connectionSettings.camSettings.device;

            //Cam1
            if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
            {
                IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                if (gigeInfo.chUserDefinedName != "")
                {
                    string Caminfo = (String.Format("GigE: " + gigeInfo.chUserDefinedName + " (" + gigeInfo.chSerialNumber + ")"));
                    txt_Camera1_name_device.SelectedValue = Caminfo;
                    //updateCbx(Caminfo);
                }
                else
                {
                    string Caminfo = String.Format(("GigE: " + gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" + gigeInfo.chSerialNumber + ")"));
                    txt_Camera1_name_device.SelectedValue = Caminfo;

                }
            }
            else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
            {
                IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stUsb3VInfo, 0);
                MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));

                if (usbInfo.chUserDefinedName != "")
                {
                    string Caminfo = String.Format(("USB: " + usbInfo.chUserDefinedName + " (" + usbInfo.chSerialNumber + ")"));
                    txt_Camera1_name_device.SelectedValue = Caminfo;
                }
                else
                {
                    string Caminfo = String.Format(("USB: " + usbInfo.chManufacturerName + " " + usbInfo.chModelName + " (" + usbInfo.chSerialNumber + ")"));
                    txt_Camera1_name_device.SelectedValue = Caminfo;
                }
            }

            //Cam2
            if (device2.nTLayerType == MyCamera.MV_GIGE_DEVICE)
            {
                IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device2.SpecialInfo.stGigEInfo, 0);
                MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                if (gigeInfo.chUserDefinedName != "")
                {
                    string Caminfo = (String.Format("GigE: " + gigeInfo.chUserDefinedName + " (" + gigeInfo.chSerialNumber + ")"));
                    txt_Camera2_name_device.SelectedValue = Caminfo;

                }
                else
                {
                    string Caminfo = String.Format(("GigE: " + gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" + gigeInfo.chSerialNumber + ")"));
                    txt_Camera2_name_device.SelectedValue = Caminfo;
                    //updateCbx(Caminfo);
                }
            }
            else if (device2.nTLayerType == MyCamera.MV_USB_DEVICE)
            {
                IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device2.SpecialInfo.stUsb3VInfo, 0);
                MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));

                if (usbInfo.chUserDefinedName != "")
                {
                    string Caminfo = String.Format(("USB: " + usbInfo.chUserDefinedName + " (" + usbInfo.chSerialNumber + ")"));
                    txt_Camera2_name_device.SelectedValue = Caminfo;
                }
                else
                {
                    string Caminfo = String.Format(("USB: " + usbInfo.chManufacturerName + " " + usbInfo.chModelName + " (" + usbInfo.chSerialNumber + ")"));
                    txt_Camera2_name_device.SelectedValue = Caminfo;
                }
            }

            //Show Expose Time
            try
            {
                this.Cam1ExposeTime.Value = (int)UiManager.appSetting.connection.camSettings.ExposeTime;
                this.Cam2ExposeTime.Value = (int)UiManager.appSetting.connection.camSettings.ExposeTime;
            }
            catch (Exception ex)
            {
                logger.Create("Can not show expose Time: " + ex.Message, LogLevel.Error);
            }

        }

        private void updateCbx(string CamInfor, int index)
        {
            TreeViewItem newChild = new TreeViewItem();
            newChild.Header = CamInfor;

            newChild.MouseRightButtonUp += newChild_MouseRightButtonDown;
            newChild.MouseDoubleClick += newChild_MouseDoubleClicked;

            if (CamInfor.Contains("GigE"))
            {
                treeViewGige.Items.Add(newChild);
                newChild.Name = String.Format("Device{0}", index.ToString().PadLeft(3, '0').ToUpper());
            }
            else if (CamInfor.Contains("USB"))
            {
                treeViewUSB.Items.Add(newChild);
                newChild.Name = String.Format("Device{0}", index.ToString().PadLeft(3, '0').ToUpper());
            }
            var cbi1 = new ComboBoxItem();
            cbi1.Content = CamInfor;
            this.txt_Camera1_name_device.Items.Add(cbi1);

            var cbi2 = new ComboBoxItem();
            cbi2.Content = CamInfor;
            this.txt_Camera2_name_device.Items.Add(cbi2);
        }

        void newChild_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            ContextMenu cm = this.FindResource("cmButton") as ContextMenu;
            cm.PlacementTarget = sender as ContextMenu;
            cm.IsOpen = true;

        }
        void newChild_MouseDoubleClicked(object sender, MouseEventArgs e)
        {
            ContextMenu cm = this.FindResource("cmButton") as ContextMenu;
            cm.PlacementTarget = sender as ContextMenu;
            cm.IsOpen = true;
            TreeViewItem item = sender as TreeViewItem;
            string name = ((string)item.Name);
            int index = Convert.ToInt32(name.Substring(6, 3));
            //int index = 0;
            MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(UiManager.hikCamera.m_pDeviceList.pDeviceInfo[Convert.ToInt32(index)], typeof(MyCamera.MV_CC_DEVICE_INFO));

            int nRet = m_pOperator.Open(ref device);
            if (MyCamera.MV_OK != nRet)
            {
                MessageBox.Show("Device open fail!");
                return;
            }
            item.Background = System.Windows.Media.Brushes.DarkOrange;
        }
        private void btnCamSave_Clicked(object sender, RoutedEventArgs e)
        {
            SaveData();
        }

        private void CommonCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        #endregion

        #region Tabar
        private void BtnCameraZoomOut_Click(object sender, RoutedEventArgs e)
        {
            var transform = myCanvas.RenderTransform as MatrixTransform;
            var matrix = transform.Matrix;
            var scale = 1.1; // choose appropriate scaling factor
            matrix.ScaleAtPrepend(scale, scale, 0.5, 0.5);
            myCanvas.RenderTransform = new MatrixTransform(matrix);
        }
        private void BtnCameraZoomIn_Click(object sender, RoutedEventArgs e)
        {
            var transform = myCanvas.RenderTransform as MatrixTransform;
            var matrix = transform.Matrix;
            var scale = 1.0 / 1.1; // choose appropriate scaling factor
            matrix.ScaleAtPrepend(scale, scale, 0.5, 0.5);
            myCanvas.RenderTransform = new MatrixTransform(matrix);
        }


        #endregion

        #region menu Item tree View
        void cmCamStopAcqui_Clicked(object sender, RoutedEventArgs e)
        {
            searchCam.Close();
            searchCam.DisPose();

        }
        void cmCamAcqui_Clicked(object sender, RoutedEventArgs e)
        {
            searchCam.Close();
            TreeViewItem Item = treeViewDevice.SelectedItem as TreeViewItem;
            string HeadDev = Item.Header as string;
            string name = ((string)Item.Name);
            int index = Convert.ToInt32(name.Substring(6, 3));
            //int index = 0;
            MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(UiManager.hikCamera.m_pDeviceList.pDeviceInfo[Convert.ToInt32(index)], typeof(MyCamera.MV_CC_DEVICE_INFO));

            int nRet = searchCam.Open(device, HikCam.AquisMode.AcquisitionMode);
            if (MyCamera.MV_OK != nRet)
            {
                MessageBox.Show("Device open fail!");
                return;
            }
            Item.Background = System.Windows.Media.Brushes.DarkOrange;
        }
        #endregion

        #region Config Canvas
        void SourceChangedHandler(object sender, EventArgs e)
        {
            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();
            myCanvas.Width = imgView.Source.Width;
            myCanvas.Height = imgView.Source.Height;
            //Cross X
            LineCreossX.X1 = 0;
            LineCreossX.Y1 = this.imgView.Source.Height / 2;
            LineCreossX.X2 = this.imgView.Source.Width;
            LineCreossX.Y2 = this.LineCreossX.Y1;
            //Cross Y
            LineCreossY.X1 = this.imgView.Source.Width / 2;
            LineCreossY.Y1 = 0;
            LineCreossY.X2 = this.LineCreossY.X1;
            LineCreossY.Y2 = this.imgView.Source.Height;

            rect.Width = 100;
            rect.Height = 100;
            Canvas.SetLeft(rect, myCanvas.Width / 2 - rect.Width / 2);
            Canvas.SetTop(rect, myCanvas.Height / 2 - rect.Height / 2);

        }
        private void Container_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                var element = sender as UIElement;
                var position = e.GetPosition(element);
                var transform = element.RenderTransform as MatrixTransform;
                var matrix = transform.Matrix;
                var scale = e.Delta >= 0 ? 1.1 : (1.0 / 1.1); // choose appropriate scaling factor
                matrix.ScaleAtPrepend(scale, scale, 0.5, 0.5);
                element.RenderTransform = new MatrixTransform(matrix);
            }
        }
        private void MyCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2 && e.LeftButton == MouseButtonState.Pressed)
            {
                var element = sender as UIElement;
                //var transform = element.RenderTransform as MatrixTransform;
                //var matrix = transform.Matrix;
            }
        }
        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            myCanvas.CaptureMouse();
            //Store click position relation to Parent of the canvas
            if (e.ClickCount >= 2 && e.LeftButton == MouseButtonState.Pressed)
            {
                var element = sender as UIElement;
                var position = e.GetPosition(element);
                var transform = element.RenderTransform as MatrixTransform;
                var matrix = transform.Matrix;
                matrix.ScaleAtPrepend(1.0 / 1.1, 1.0 / 1.1, 0.5, 0.5);
                double a = matrix.M11;
                element.RenderTransform = new MatrixTransform(matrix);
                // example 0
                double top = (double)myCanvas.GetValue(Canvas.TopProperty);
                double left = (double)myCanvas.GetValue(Canvas.LeftProperty);
            }
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //var draggableControl = sender as Shape;
            //originTT = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
            //isDragging = true;
            //clickPosition = e.GetPosition(this);
            //draggableControl.CaptureMouse();

            ////Release Mouse Capture
            myCanvas.ReleaseMouseCapture();
            ////Set cursor by default
            Mouse.OverrideCursor = null;
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            //Check object Canvas
            var draggableControl = sender as Shape;
            if (isDragging && draggableControl != null)
            {
                System.Windows.Point currentPosition = e.GetPosition(this);
                var transform = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                transform.X = originTT.X + (currentPosition.X - clickPosition.X);
                transform.Y = originTT.Y + (currentPosition.Y - clickPosition.Y);
                draggableControl.RenderTransform = new TranslateTransform(transform.X, transform.Y);
            }
        }

        #endregion

        #region Open Cam
        private bool InnitialCamera1()
        {
            return true;
            //MyCamera.MV_CC_DEVICE_INFO device = UiManager.appSettings.connection.camera1.device;
            //int ret = Cam1.Open(device, HikCam.AquisMode.AcquisitionMode);
            //Cam1.SetExposeTime((int)UiManager.appSettings.connection.camera1.ExposeTime);
            //Thread.Sleep(2);
            //if (ret == MyCamera.MV_OK)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }
        private bool InnitialCamera2()
        {
            return true;
            //MyCamera.MV_CC_DEVICE_INFO device = UiManager.appSettings.connection.camera2.device;
            //int ret = Cam2.Open(device, HikCam.AquisMode.AcquisitionMode);
            //Cam2.SetExposeTime((int)UiManager.appSettings.connection.camera2.ExposeTime);
            //Thread.Sleep(2);
            //if (ret == MyCamera.MV_OK)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }
        private void callThreadStartLoop()
        {
            try
            {
                Thread startThread = new Thread(new ThreadStart(waitTrigger));
                startThread.IsBackground = true;
                startThread.Start();
            }
            catch (Exception ex)
            {
                logger.Create("Start thread Auto loop Err : " + ex.ToString(), LogLevel.Error);
            }

        }
        private void waitTrigger()
        {
            TriggerCameraCH1();
            if (stopCamera)
            {
                return;
            }
            callThreadStartLoop();
            Thread.Sleep(1);
        }
        private void TriggerCameraCH1()
        {
            lock (cameraTrigger)
            {
                OpenCvSharp.Mat src1 = new Mat();
                OpenCvSharp.Mat src2 = new Mat();
                //OpenCvSharp.Mat srcDisplay1 = new Mat();
                //OpenCvSharp.Mat srcDisplay2 = new Mat();
                try
                {
                    string channel = "";
                    this.Dispatcher.Invoke(() => {
                        channel = this.cbxCameraCh.SelectedValue.ToString();
                    });
                    if (channel == "CH1")
                    {
                        if(src1 != null)
                        {
                            src1 = UiManager.Cam1.CaptureImage();
                            Thread.Sleep(10);
                            src1.SaveImage("temp1.bmp");
                            src1 = Cv2.ImRead("temp1.bmp", ImreadModes.Color);
                            this.Image = src1.Clone();
                            var out1 = vision1.AdjustContrastBrightness(src1, UiManager.currentModel.VisionModel.AlphaSld, UiManager.currentModel.VisionModel.BetaSld);
                            var out2 = vision1.AdjustGamma(out1, UiManager.currentModel.VisionModel.GammaSld);
                            //srcDisplay1 = out2.Clone();
                            src1 = out2.Clone();
                        }
                        else
                        {
                            logger.Create("Camera Trigger Err - Have no Data from camera - Image is null", LogLevel.Error);
                            stopCamera = true;
                            return;
                        }


                    }
                    else if (channel == "CH2")
                    {
                        src2 = UiManager.Cam2.CaptureImage();
                        if (src2 != null)
                        {
                            src2.SaveImage("temp2.bmp");
                            src2 = Cv2.ImRead("temp2.bmp", ImreadModes.Color);
                            this.Image = src2.Clone();
                            var out1 = vision1.AdjustContrastBrightness(src1, UiManager.currentModel.VisionModel.AlphaSld, UiManager.currentModel.VisionModel.BetaSld);
                            var out2 = vision1.AdjustGamma(out1, UiManager.currentModel.VisionModel.GammaSld);
                            //srcDisplay2 = out2.Clone();
                            src2 = out2.Clone();
                        }
                        else
                        {
                            logger.Create("Camera Trigger Err - Have no Data from camera - Image is null", LogLevel.Error);
                            stopCamera = true;
                            return;
                        }

                    }
                    Thread.Sleep(1);
                    this.Dispatcher.Invoke(() =>
                    {
                        if (channel == "CH1")
                        {
                            imgView.Source = src1.ToWriteableBitmap(PixelFormats.Bgr24);
                            GC.Collect();
                        }
                        else if (channel == "CH2")
                        {
                            imgView.Source = src2.ToWriteableBitmap(PixelFormats.Bgr24);
                            GC.Collect();
                        }
                        //Img_Main_process_2.Source = vision1.Image1.ToWriteableBitmap(PixelFormats.Bgr24);
                    });
                    return;
                }
                catch (Exception ex)
                {
                    logger.Create(ex.Message.ToString(), LogLevel.Error);
                }
            }

        }

        #endregion

        #region SetModel
        private void CbxCamera_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowImage();
            if (cbxCameraCh.SelectedValue == null)
                return;
            string CH = "";
            if (cbxCameraCh.SelectedValue.ToString() == "CH1")
            {
                CH = "Chanel2";
            }
            else
            {
                CH = "Chanel1";
            }
            MessageBoxResult result = MessageBox.Show(String.Format("Do you want to Save Data For {0}?", CH), "Confirmation", MessageBoxButton.YesNo);
            imgView.Focus();
            if (result == MessageBoxResult.Yes)
            {
                SaveData();
            }
            this.DeleteAllRegion();
            this.modelSet();
            this.LoadRegion();
            this.RoiShowCheck();


            this.intWhitePixel.Value = UiManager.currentModel.VisionModel.WhitePixels;
            this.intBlackPixel.Value = UiManager.currentModel.VisionModel.BlackPixels;
            this.dblMatchingRate.Value = UiManager.currentModel.VisionModel.MatchingRate;
            this.dblMatchingRateMin.Value = UiManager.currentModel.VisionModel.MatchingRateMin;
            this.intThreshol.Value = UiManager.currentModel.VisionModel.Threshol;
            this.intThresholBl.Value = UiManager.currentModel.VisionModel.ThresholBl;
            this.rdnCirWh.IsChecked = UiManager.currentModel.VisionModel.CirWhCntEnb;
            this.rdnRoiWh.IsChecked = UiManager.currentModel.VisionModel.RoiWhCntEnb;
            this.cbxEnableOffset.IsChecked = UiManager.currentModel.VisionModel.OffSetJigEnb;

        }
        void modelSet()
        {
           

        }
        #endregion

        #region Show Matrix Real Time
        public void SaveData()
        {
            //UiManager.hikCamera.DeviceListAcq();
            if (txt_Camera1_name_device.SelectedValue != null)
            {
                MyCamera.MV_CC_DEVICE_INFO device1 = new MyCamera.MV_CC_DEVICE_INFO();
                try
                {
                    //47
                    device1 = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(UiManager.hikCamera.m_pDeviceList.pDeviceInfo[txt_Camera1_name_device.SelectedIndex], typeof(MyCamera.MV_CC_DEVICE_INFO));


                    if (UiManager.Cam1.GetserialNumber() != UiManager.hikCamera.GetserialNumber(device1))
                    {
                        connectionSettings.camSettings.device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(UiManager.hikCamera.m_pDeviceList.pDeviceInfo[txt_Camera1_name_device.SelectedIndex], typeof(MyCamera.MV_CC_DEVICE_INFO));
                        UiManager.Cam1.Close();
                        UiManager.Cam1.DisPose();
                        UiManager.ConectCamera();
                        logger.Create("Change Camera1 Setting" + connectionSettings.camSettings.device.SpecialInfo.stCamLInfo.ToString(), LogLevel.Error);
                    }

                }
                catch (Exception ex)
                {
                    logger.Create("Ptr Device Camera1 Err" + ex.ToString() + UiManager.Cam1.GetserialNumber() + " " + UiManager.hikCamera.GetserialNumber(device1), LogLevel.Error);

                }
            }
            if (txt_Camera2_name_device.SelectedValue != null)
            {
                MyCamera.MV_CC_DEVICE_INFO device2 = new MyCamera.MV_CC_DEVICE_INFO();
                try
                {

                    //54
                    device2 = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(UiManager.hikCamera.m_pDeviceList.pDeviceInfo[txt_Camera2_name_device.SelectedIndex], typeof(MyCamera.MV_CC_DEVICE_INFO));
                    if (UiManager.Cam2.GetserialNumber() != UiManager.hikCamera.GetserialNumber(device2))
                    {
                        connectionSettings.camSettings.device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(UiManager.hikCamera.m_pDeviceList.pDeviceInfo[txt_Camera2_name_device.SelectedIndex], typeof(MyCamera.MV_CC_DEVICE_INFO));
                        UiManager.Cam2.Close();
                        UiManager.Cam2.DisPose();
                        UiManager.ConectCamera();
                        logger.Create("Change Camera2 Setting" + connectionSettings.camSettings.device.SpecialInfo.stCamLInfo.ToString(), LogLevel.Error);

                    }
                }
                catch (Exception ex)
                {
                    logger.Create("Ptr Device Camera2 Err" + ex.ToString() + UiManager.Cam2.GetserialNumber() + " " + UiManager.hikCamera.GetserialNumber(device2), LogLevel.Error);
                }
            }

            //Vision AL setup 
            if (this.intWhitePixel.Value != null)
            {
                UiManager.currentModel.VisionModel.WhitePixels = Convert.ToInt32(this.intWhitePixel.Value);
            }
            if (this.intBlackPixel.Value != null)
            {
                UiManager.currentModel.VisionModel.BlackPixels = Convert.ToInt32(this.intBlackPixel.Value);
            }
            if (this.dblMatchingRate.Value != null)
            {
                UiManager.currentModel.VisionModel.MatchingRate = Convert.ToInt32(this.dblMatchingRate.Value);
            }
            if (this.dblMatchingRateMin.Value != null)
            {
                UiManager.currentModel.VisionModel.MatchingRateMin = Convert.ToInt32(this.dblMatchingRateMin.Value);
            }
            if (this.intThreshol.Value != null)
            {
                UiManager.currentModel.VisionModel.Threshol = Convert.ToInt32(this.intThreshol.Value);
            }
            if (this.intThresholBl.Value != null)
            {
                UiManager.currentModel.VisionModel.ThresholBl = Convert.ToInt32(this.intThresholBl.Value);
            }
            if ((bool)this.cbxEnableOffset.IsChecked)
                UiManager.currentModel.VisionModel.OffSetJigEnb = true;
            else
            {
                UiManager.currentModel.VisionModel.OffSetJigEnb = false;
            }

            //Image Logger 
            UiManager.currentModel.VisionModel.ImageLogger.isAddDateTime = IsAddDateTime;
            UiManager.currentModel.VisionModel.ImageLogger.isUseFolderOK = IsUseFolderOK;
            UiManager.currentModel.VisionModel.ImageLogger.isUseFolderNG = IsUseFolderNG;
            UiManager.currentModel.VisionModel.ImageLogger.fileName = txtFileName;
            UiManager.currentModel.VisionModel.ImageLogger.folderPath = txtFolderPath;
            UiManager.currentModel.VisionModel.ImageLogger.imageFormat = ImageFormatSelected;
            UiManager.currentModel.VisionModel.ImageLogger.imageStorage = NumUDImageStorage;

            UiManager.currentModel.VisionModel.CirWhCntEnb = (bool)rdnCirWh.IsChecked;
            UiManager.currentModel.VisionModel.RoiWhCntEnb = (bool)rdnRoiWh.IsChecked;


            //Save ROI
            this.ShapeEditorControl.ReleaseElement();
            UiManager.currentModel.VisionModel.ROI.listRectangle = new List<OpenCvSharp.Rect> { };
            UiManager.currentModel.VisionModel.ROI.listRectangle.Clear();

            for (int i = 0; i < RectLst.Count; i++)
            {
                OpenCvSharp.Rect rec = new OpenCvSharp.Rect((int)Canvas.GetLeft(RectLst[i]), (int)Canvas.GetTop(RectLst[i]), (int)RectLst[i].ActualWidth, (int)RectLst[i].ActualHeight);
                UiManager.currentModel.VisionModel.ROI.listRectangle.Add(rec);
            }
            UiManager.SaveCurrentModelSettings();
            UiManager.SaveAppSetting();
            MessageBox.Show("Saving Success...");
            if (this.Image == null)
                return;

        }
        #endregion

        #region ROI Enable Edit
        public Rectangle rectCur;
        public Rectangle rectCoppy;
        public bool coppy = false;
        public List<System.Windows.Shapes.Rectangle> RectLst = new List<System.Windows.Shapes.Rectangle> { };

        public List<Label> LabelLst = new List<Label> { };
        private void CbxShowRoi_Click(object sender, RoutedEventArgs e)
        {
            ShapeEditorControl.ReleaseElement();
            RoiShowCheck();
        }
        private void RoiShowCheck()
        {
            this.cbxRoiManual.IsEnabled = (bool)(cbxShowRoi.IsChecked);
            this.cbxRoiMtrix.IsEnabled = (bool)(cbxShowRoi.IsChecked);
            this.lbRoiMatrix.IsEnabled = (bool)(cbxShowRoi.IsChecked);
            this.lbRoiMAnual.IsEnabled = (bool)(cbxShowRoi.IsChecked);
            this.btnROIUp.IsEnabled = (bool)(cbxShowRoi.IsChecked);
            this.btnROIDown.IsEnabled = (bool)(cbxShowRoi.IsChecked);
            this.btnROILeft.IsEnabled = (bool)(cbxShowRoi.IsChecked);
            this.btnROIRight.IsEnabled = (bool)(cbxShowRoi.IsChecked);
            this.btnCreatRegion.IsEnabled = (bool)(cbxShowRoi.IsChecked);
            this.cbxAutoIndexRoi.IsEnabled = (bool)(cbxShowRoi.IsChecked);
            this.cbxManualIndexRoi.IsEnabled = (bool)(cbxShowRoi.IsChecked);
            this.intROIIndex.IsEnabled = ((bool)(cbxShowRoi.IsChecked) && (bool)(cbxManualIndexRoi.IsChecked));
            this.btnDeleteRegionAll.IsEnabled = (bool)(cbxShowRoi.IsChecked);
            if ((Boolean)cbxShowRoi.IsChecked)
            {
                ShowImage();
                lbCreatROI.Foreground = Brushes.DarkOrange;
            }
            else
            {
                lbCreatROI.Foreground = Brushes.Gray;
            }
        }
        private void CbxRoiManual_Click(object sender, RoutedEventArgs e)
        {
            this.cbxRoiMtrix.IsChecked = !(bool)cbxRoiManual.IsChecked;
            if ((bool)cbxRoiMtrix.IsChecked)
            {
                this.btnCreatRegion.Visibility = Visibility.Hidden;
            }
            else
            {
                this.btnCreatRegion.Visibility = Visibility.Visible;
            }
        }

        private void CbxRoiMtrix_Click(object sender, RoutedEventArgs e)
        {
            this.cbxRoiManual.IsChecked = !(bool)cbxRoiMtrix.IsChecked;
            if ((bool)cbxRoiMtrix.IsChecked)
            {
                this.btnCreatRegion.Visibility = Visibility.Hidden;
            }
            else
            {
                this.btnCreatRegion.Visibility = Visibility.Visible;
            }
        }
        private void CbxManualIndexRoi_Click(object sender, RoutedEventArgs e)
        {
            this.cbxAutoIndexRoi.IsChecked = !(bool)cbxManualIndexRoi.IsChecked;
            intROIIndex.IsEnabled = !(bool)cbxAutoIndexRoi.IsChecked;
        }

        private void CbxAutoIndexRoi_Click(object sender, RoutedEventArgs e)
        {
            this.cbxManualIndexRoi.IsChecked = !(bool)cbxAutoIndexRoi.IsChecked;
            intROIIndex.IsEnabled = !(bool)cbxAutoIndexRoi.IsChecked;
        }
        private void LoadRegion()
        {
            if (UiManager.currentModel.VisionModel.ROI.listRectangle == null)
                return;
            for (int i = 0; i < UiManager.currentModel.VisionModel.ROI.listRectangle.Count; i++)
            {
                Name = String.Format("R{0}", i + 1);
                var converter = new BrushConverter();
                CreatRect(UiManager.currentModel.VisionModel.ROI.listRectangle[i].X, UiManager.currentModel.VisionModel.ROI.listRectangle[i].Y, UiManager.currentModel.VisionModel.ROI.listRectangle[i].Width, UiManager.currentModel.VisionModel.ROI.listRectangle[i].Height, new SolidColorBrush(Colors.Red), (Brush)converter.ConvertFromString("#40DC143C"), Name);
            }
        }
        private void BtnCreatRoi_Click(object sender, RoutedEventArgs e)
        {
            if (cbxShowRoi.IsChecked == false)
                return;

            String Name = "";
            var converter = new BrushConverter();
            if (RectLst.Count == 0)
            {

                Name = "R1";

            }
            else if (RectLst.Count > 0)
            {
                if ((bool)cbxAutoIndexRoi.IsChecked)
                {
                    int b = 1;
                    for (int i = 0; i < RectLst.Count; i++)
                    {
                        int temp = Convert.ToInt32(RectLst[i].Name.Replace("R", String.Empty));
                        if (b - temp < 0)
                        {
                            Name = String.Format("R{0}", b);
                            break;
                        }
                        else
                        {
                            b++;
                        }

                    }
                    if (b - 1 == Convert.ToInt32(RectLst[RectLst.Count - 1].Name.Replace("R", String.Empty)))
                    {
                        var recName = RectLst[RectLst.Count - 1].Name.Replace("R", String.Empty);
                        Name = String.Format("R{0}", Convert.ToInt32(recName) + 1);
                    }
                }
                else if ((bool)cbxManualIndexRoi.IsChecked)
                {
                    int ret = RectLst.FindIndex(a => a.Name == String.Format("R{0}", (int)(intROIIndex.Value)));
                    if (ret >= 0)
                    {
                        MessageBox.Show(String.Format("R{0} đã tồn tại trong List ROI.\r R{1} already exists ", (int)(intROIIndex.Value), (int)(intROIIndex.Value)));
                        return;
                    }

                    Name = String.Format("R{0}", (int)(intROIIndex.Value));
                }

            }


            CreatRect(10, 10, 200, 200, new SolidColorBrush(Colors.Red), (Brush)converter.ConvertFromString("#40DC143C"), Name);
        }
        private void BtnCreatRegion_Click(object sender, RoutedEventArgs e)
        {
            if (cbxShowRoi.IsChecked == false)
                return;

            String Name = "";
            var converter = new BrushConverter();
            if (RectLst.Count == 0)
            {

                Name = "R1";

            }
            else if (RectLst.Count > 0)
            {
                if ((bool)cbxAutoIndexRoi.IsChecked)
                {
                    int b = 1;
                    for (int i = 0; i < RectLst.Count; i++)
                    {
                        int temp = Convert.ToInt32(RectLst[i].Name.Replace("R", String.Empty));
                        if (b - temp < 0)
                        {
                            Name = String.Format("R{0}", b);
                            break;
                        }
                        else
                        {
                            b++;
                        }

                    }
                    if (b - 1 == Convert.ToInt32(RectLst[RectLst.Count - 1].Name.Replace("R", String.Empty)))
                    {
                        var recName = RectLst[RectLst.Count - 1].Name.Replace("R", String.Empty);
                        Name = String.Format("R{0}", Convert.ToInt32(recName) + 1);
                    }
                }
                else if ((bool)cbxManualIndexRoi.IsChecked)
                {
                    int ret = RectLst.FindIndex(a => a.Name == String.Format("R{0}", (int)(intROIIndex.Value)));
                    if (ret >= 0)
                    {
                        MessageBox.Show(String.Format("R{0} đã tồn tại trong List ROI.\r R{1} already exists ", (int)(intROIIndex.Value), (int)(intROIIndex.Value)));
                        return;
                    }

                    Name = String.Format("R{0}", (int)(intROIIndex.Value));
                }

            }


            CreatRect(10, 10, 200, 200, new SolidColorBrush(Colors.Red), (Brush)converter.ConvertFromString("#40DC143C"), Name);
        }

        private void DeleteRegion()
        {
            do
            {
                ShapeEditorControl.ReleaseElement();
                int index = RectLst.FindIndex(rec => rec.Name == rectCur.Name);
                if (index == -1)
                {
                    break;
                }
                for (int i = 0; i < myCanvas.Children.Count; i++)
                {
                    Label a = myCanvas.Children[i] as Label;
                    {
                        if (index >= 0 && a != null)
                        {
                            if ((string)a.Name == RectLst[index].Name)
                            {
                                myCanvas.Children.RemoveAt(i);
                            }
                        }
                    }

                }
                int b = 1;
                for (int i = 0; i < RectLst.Count; i++)
                {
                    int temp = Convert.ToInt32(RectLst[i].Name.Replace("R", String.Empty));
                    if (b - temp < 0)
                    {
                        intROIIndex.Value = b + 1;
                    }
                    else
                    {
                        b = temp;
                    }

                }

                myCanvas.Children.Remove(RectLst[index]);
                RectLst.RemoveAt(index);
                LabelLst.RemoveAt(index);
            }
            while (false);

        }
        private void CreatRect(int left, int top, int width, int height, Brush stroke, Brush Fill, string name)
        {

            var rect = new System.Windows.Shapes.Rectangle()
            {
                Width = width,
                Height = height,
                Stroke = stroke,
                Fill = Fill,
                Name = name,
                StrokeThickness = UiManager.appSetting.RoiProperty.StrokeThickness,
            };

            rect.MouseLeftButtonDown += Shape_MouseLeftButtonDown;
            Canvas.SetLeft(rect, left);
            Canvas.SetTop(rect, top);

            int index = Convert.ToInt32(name.Replace("R", string.Empty));
            if (index > RectLst.Count)
            {
                RectLst.Add(rect);
            }
            else
            {
                RectLst.Insert(index - 1, rect);
            }

            myCanvas.Children.Add(rect);
            intROIIndex.Value = Convert.ToInt32(name.Replace("R", String.Empty)) + 1;

            Label lb = new Label();
            lb.Name = name;
            lb.Content = name.Replace("R", String.Empty);
            lb.FontSize = UiManager.appSetting.RoiProperty.labelFontSize;
            lb.Foreground = System.Windows.Media.Brushes.Aqua;
            Canvas.SetLeft(lb, left);
            Canvas.SetTop(lb, top);
            LabelLst.Add(lb);
            myCanvas.Children.Add(lb);
        }
        private void BtnDeleteRegionAll_Click(object sender, RoutedEventArgs e)
        {
            ShapeEditorControl.ReleaseElement();
            imgView.Source = this.Image.ToWriteableBitmap(PixelFormats.Bgr24);
            int index = myCanvas.Children.Count;
            DeleteAllRegion();
        }
        public void DeleteAllRegion()
        {
            ShapeEditorControl.ReleaseElement();
            int index = myCanvas.Children.Count;
            for (int i = 0; i < index - 5; i++)
            {
                myCanvas.Children.RemoveAt(index - i - 1);
            }
            RectLst.Clear();
        }
        private void Shape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (cbxShowRoi.IsChecked == false)
                return;
            rectCur = sender as Rectangle;
            ShapeEditorControl.CaptureElement(sender as FrameworkElement, e);
            e.Handled = true;
        }
        private void PgCamera1_KeyDown(object sender, KeyEventArgs e)
        {
            if (rectCur == null)
                return;
            if (e.Key == Key.Delete)
            {
                DeleteRegion();
            }

            if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                coppy = true;
                rectCoppy = rectCur;
            }

            if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (coppy == true)

                {
                    var converter = new BrushConverter();

                    if ((bool)cbxAutoIndexRoi.IsChecked)
                    {
                        int b = 1;
                        for (int i = 0; i < RectLst.Count; i++)
                        {
                            int temp = Convert.ToInt32(RectLst[i].Name.Replace("R", String.Empty));
                            if (b - temp < 0)
                            {
                                Name = String.Format("R{0}", b);
                                break;
                            }
                            else
                            {
                                b++;
                            }

                        }
                        if (b - 1 == Convert.ToInt32(RectLst[RectLst.Count - 1].Name.Replace("R", String.Empty)))
                        {
                            var recName = RectLst[RectLst.Count - 1].Name.Replace("R", String.Empty);
                            Name = String.Format("R{0}", Convert.ToInt32(recName) + 1);
                        }
                    }
                    CreatRect((int)Canvas.GetLeft(rectCur) + 100,
                        (int)Canvas.GetTop(rectCur), (int)rectCoppy.ActualWidth, (int)rectCoppy.ActualHeight, rectCoppy.Stroke, rectCoppy.Fill, Name);

                }
            }

        }
        private void imgView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            rectCur = null;
            ShapeEditorControl.ReleaseElement();
        }
        private void MenuItemCreatMatrix_Click(object sender, RoutedEventArgs e)
        {
            var Point = Mouse.GetPosition(this);
            RegionCreatMatrix.MatrixData matrix = new RegionCreatMatrix().DoConfirmMatrix(new System.Windows.Point(Point.X, Point.Y - 200));
            ShapeEditorControl.ReleaseElement();
            {
                var converter = new BrushConverter();
                string Name = "";


                for (int i = 0; i < matrix.Row; i++)
                {
                    for (int j = 0; j < matrix.Colum; j++)
                    {
                        if (!(i == 0 && j == 0))
                        {
                            if (RectLst.Count >= 0)
                            {
                                Name = String.Format("R{0}", RectLst.Count + 1);
                            }

                            CreatRect((int)(Canvas.GetLeft(rectCur) + j * matrix.ColumPitch), (int)Canvas.GetTop(rectCur) + i * matrix.RowPitch, (int)rectCur.ActualWidth, (int)rectCur.ActualHeight, rectCur.Stroke, rectCur.Fill, Name);
                        }

                    }
                }
            }

        }
        private void MenuItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            DeleteRegion();
        }
        private void Property_Click(object sender, RoutedEventArgs e)
        {
            var Point = Mouse.GetPosition(this);
            new RegionProperty().DoConfirmMatrix(new System.Windows.Point(Point.X, Point.Y - 200));
            UpdateProperty();
        }

        private void UpdateProperty()
        {
            List<System.Windows.Shapes.Rectangle> RectLstCoppy = new List<System.Windows.Shapes.Rectangle> { };
            ShapeEditorControl.ReleaseElement();
            int index = myCanvas.Children.Count;
            for (int i = 0; i < index - 5; i++)
            {
                myCanvas.Children.RemoveAt(index - i - 1);
            }
            if (this.RectLst == null)
                return;
            for (int i = 0; i < RectLst.Count; i++)
            {
                RectLstCoppy.Add(RectLst[i]);
            }
            RectLst.Clear();
            for (int i = 0; i < RectLstCoppy.Count; i++)
            {
                Name = String.Format("R{0}", i + 1);
                var converter = new BrushConverter();
                CreatRect((int)Canvas.GetLeft(RectLstCoppy[i]), (int)Canvas.GetTop(RectLstCoppy[i]), (int)RectLstCoppy[i].ActualWidth, (int)RectLstCoppy[i].ActualHeight, new SolidColorBrush(Colors.Red), (Brush)converter.ConvertFromString("#40DC143C"), Name);
            }


        }
        private void MenuItemTemplate_Click(object sender, RoutedEventArgs e)
        {
            //Update Position Template
            ShapeEditorControl.ReleaseElement();
            if(rectCur == null) return;
            int index = RectLst.FindIndex(rec => rec.Name == rectCur.Name);
            var src = this.Image.Clone();
            var out1 = vision1.AdjustContrastBrightness(src, UiManager.currentModel.VisionModel.AlphaSld, UiManager.currentModel.VisionModel.BetaSld);
            var out2 = vision1.AdjustGamma(out1, UiManager.currentModel.VisionModel.GammaSld);
            Mat roi = new Mat(out2, new OpenCvSharp.Rect((int)(Canvas.GetLeft(RectLst[index])), (int)(Canvas.GetTop(RectLst[index])), (int)RectLst[index].ActualWidth, (int)RectLst[index].ActualHeight));

            var fileName = String.Format("{0}Template.png", this.cbxCameraCh.SelectedValue.ToString());
            var folder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", UiManager.currentModel.modelName.ToString());
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var filePath = System.IO.Path.Combine(folder, fileName);

            try
            {
                roi.SaveImage(filePath);
                //MessageBox.Show(String.Format("Save Template Sucessfull For {0} {1}", this.cbxCameraCh.SelectedValue.ToString(), this.model.Name));

                var result = MessageBox.Show(
                    String.Format("Save Template Successful For {0} {1}.\r Do you Open Folded ?", this.cbxCameraCh.SelectedValue.ToString(), UiManager.currentModel.modelName),
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {

                    if (System.IO.File.Exists(filePath))
                    {

                        Process.Start("explorer.exe", $"/select,\"{filePath}\"");
                    }
                    else
                    {

                        MessageBox.Show("File không tồn tại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show("Save Template NG");
            }

        }
        private void MenuItemSetPLCBit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ShapeEditorControl.lb.Content.ToString()))
                return;
            bool res = int.TryParse((string)ShapeEditorControl.lb.Content, out int indexROI);
            System.Windows.Point Point = Mouse.GetPosition(this);
            new SetupPLCBitROI(indexROI).DoConfirmMatrix(new System.Windows.Point(Point.X, Point.Y - 200));
        }
        private void BtnROILeft_Click(object sender, RoutedEventArgs e)
        {
            ShapeEditorControl.ReleaseElement();
            int index = myCanvas.Children.Count;
            for (int i = 4; i < index; i++)
            {
                Canvas.SetLeft(myCanvas.Children[i], Canvas.GetLeft(myCanvas.Children[i]) - 2);
            }
        }

        private void BtnROIRight_Click(object sender, RoutedEventArgs e)
        {
            ShapeEditorControl.ReleaseElement();
            int index = myCanvas.Children.Count;
            for (int i = 4; i < index; i++)
            {
                Canvas.SetLeft(myCanvas.Children[i], Canvas.GetLeft(myCanvas.Children[i]) + 2);
            }
        }

        private void BtnROIDown_Click(object sender, RoutedEventArgs e)
        {
            ShapeEditorControl.ReleaseElement();
            int index = myCanvas.Children.Count;
            for (int i = 4; i < index; i++)
            {
                Canvas.SetTop(myCanvas.Children[i], Canvas.GetTop(myCanvas.Children[i]) + 2);
            }
        }

        private void BtnROIUp_Click(object sender, RoutedEventArgs e)
        {
            ShapeEditorControl.ReleaseElement();
            int index = myCanvas.Children.Count;
            for (int i = 4; i < index; i++)
            {
                Canvas.SetTop(myCanvas.Children[i], Canvas.GetTop(myCanvas.Children[i]) - 2);
            }
        }


        #endregion

        #region Image Processing
        private void TxtBetasld_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key != Key.Enter) return;
                this.sldBeta.Value = Convert.ToDouble(txtBetasld.Text);
                UiManager.SaveCurrentModelSettings();
                UiManager.SaveAppSetting();
            }
            catch (Exception ex)
            {

            }
        }

        private void TxtAlphasld_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key != Key.Enter) return;
                this.sldAlpha.Value = Convert.ToDouble(txtAlphasld.Text);
                UiManager.SaveCurrentModelSettings();
                UiManager.SaveAppSetting();
            }
            catch (Exception ex)
            {

            }
        }

        private void TxtGammasld_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key != Key.Enter) return;
                this.sldGamma.Value = Convert.ToDouble(txtGammasld.Text);
                UiManager.SaveCurrentModelSettings();
                UiManager.SaveAppSetting();
            }
            catch (Exception ex)
            {

            }
        }

        private void BtnEnableGamma_Click(object sender, RoutedEventArgs e)
        {
          

        }

        private void SldBeta_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.txtBetasld.Text = sldBeta.Value.ToString("F1");
            {
                UiManager.currentModel.VisionModel.BetaSld = sldBeta.Value;
            }
            UiManager.SaveCurrentModelSettings();
            UiManager.SaveAppSetting();
            if (stopCamera)
            {
                ShowImage();
            }

        }
        private void SldAlpha_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.txtAlphasld.Text = sldAlpha.Value.ToString("F1");
            {
                UiManager.currentModel.VisionModel.AlphaSld = sldAlpha.Value;
            }
            UiManager.SaveCurrentModelSettings();
            UiManager.SaveAppSetting();
            if (stopCamera)
            {
                ShowImage();
            }

        }
        private void SldGamma_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.txtGammasld.Text = sldGamma.Value.ToString("F1");
            {
                UiManager.currentModel.VisionModel.GammaSld = sldGamma.Value;
            }
            UiManager.SaveAppSetting();
            UiManager.SaveCurrentModelSettings();
            if (stopCamera)
            {
                ShowImage();
            }
           
        }
        private void ShowImage()
        {
            GC.Collect();
            var src = this.Image.Clone();
            var out1 = vision1.AdjustContrastBrightness(src, UiManager.currentModel.VisionModel.AlphaSld, UiManager.currentModel.VisionModel.BetaSld);
            var out2 = vision1.AdjustGamma(out1, UiManager.currentModel.VisionModel.GammaSld);
            imgView.Source = out2.ToWriteableBitmap(PixelFormats.Bgr24);
        }
        #endregion

        #region Setup Image Logger
        private void BtnChooseFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.Description = "Select Folder";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    txtFolderPath = dialog.SelectedPath;
                }
            }
        }
        public void BtnCheckDisk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    if (string.IsNullOrEmpty(txtFolderPath))
                    {
                        MessageBox.Show("Folder Path is Empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!IsValidPath(txtFolderPath))
                    {
                        MessageBox.Show("FolderPath Error Syntax!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(txtFolderPath) || !IsValidPath(txtFolderPath))
                        return;
                }
                string drive = System.IO.Path.GetPathRoot(txtFolderPath);
                DriveInfo driveInfo = new DriveInfo(drive);
                if (driveInfo.IsReady)
                {
                    long totalFree = driveInfo.AvailableFreeSpace;
                    txtFreeDisk = FormatBytes(totalFree, out string tempSize);
                    this.DiskSize = tempSize;
                    MaxImageStorage = double.Parse(txtFreeDisk) - 1d;
                }
                else
                {
                    MessageBox.Show("Disk C is not Ready!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private string FormatBytes(long bytes, out string size)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            try
            {
                //Bỏ qua TB
                while (len >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    len /= 1024;
                }
            }
            catch (Exception ex)
            {
                logger.Create("Format Bytes Error: " + ex.Message, LogLevel.Error);
            }
            //return $"{len:0.##} {sizes[order]}";
            size = sizes[order];
            return $"{len:0.##}";
        }
        private bool IsValidPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            char[] invalidChars = System.IO.Path.GetInvalidPathChars();
            return !path.Any(c => invalidChars.Contains(c));
        }
        public void SaveImage(Mat src, int judgeVal = 1)
        {
            if (src == null || src.Empty())
            {
                logger.Create("Image Save NULL or Error", LogLevel.Error);
                return;
            }    
            try
            {
                // Kiểm tra folder tồn tại
                if (!IsValidPath(txtFolderPath))
                {
                    logger.Create("FolderPath Error Syntax!", LogLevel.Error);
                    return;
                }
                string folderJudge = "", folderPath = "";
                switch (judgeVal)
                {
                    case 1:
                        if (!IsUseFolderOK)
                            break;
                        folderJudge = "OK";
                        folderPath = System.IO.Path.Combine(txtFolderPath, folderJudge);
                        break;
                    case 2:
                        if (!IsUseFolderNG)
                            break;
                        folderJudge = "NG";
                        folderPath = System.IO.Path.Combine(txtFolderPath, folderJudge);
                        break;
                }
                if (!IsUseFolderOK && !IsUseFolderNG)
                    folderPath = txtFolderPath;
                if (!string.IsNullOrEmpty(folderPath))
                {
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    // Xử lý tên file
                    string timestamp = IsAddDateTime
                        ? $"{DateTime.Now:yyyy-MM-dd HH-mm-ss-fff}"
                        : "";
                    if (string.IsNullOrEmpty(txtFileName))
                        txtFileName = "Default";
                    if (string.IsNullOrEmpty(ImageFormatSelected))
                        ImageFormatSelected = "BMP";
                    string finalFileName = $"{txtFileName}{folderJudge} {timestamp}.{ImageFormatSelected.ToLower()}";
                    string fullPath = System.IO.Path.Combine(folderPath, finalFileName);

                    // Lưu ảnh
                    if (!Cv2.ImWrite(fullPath, src))
                    {
                        logger.Create("Save Image Fail!", LogLevel.Error);
                        return;
                    }
                    //Kiểm tra và xóa file ảnh cũ nhất trong trường hợp folder đầy dung lượng cho phép
                    FileQM.AddFiles(fullPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Create("Save Image Error: " + ex.Message, LogLevel.Error);
            }
        }
        #endregion
    }

}
