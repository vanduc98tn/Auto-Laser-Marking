using Newtonsoft.Json.Linq;
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
using System.Xml.Linq;

namespace Development
{
    /// <summary>
    /// Interaction logic for PgModel.xaml
    /// </summary>
    public partial class PgModel : Page
    {
        private static MyLogger logger = new MyLogger("Page Model");
        private int SelectNumberModel = 1;
        public PgModel()
        {
            InitializeComponent();
            this.btSelectModel01.Click += BtSelectModel01_Click;
            this.btSelectModel02.Click += BtSelectModel02_Click;
            this.btSelectModel03.Click += BtSelectModel03_Click;
            this.btSelectModel04.Click += BtSelectModel04_Click;
            this.btSelectModel05.Click += BtSelectModel05_Click;
            this.btSelectModel06.Click += BtSelectModel06_Click;
            this.btSelectModel07.Click += BtSelectModel07_Click;
            this.btSelectModel08.Click += BtSelectModel08_Click;
            this.btSelectModel09.Click += BtSelectModel09_Click;
            this.btSelectModel10.Click += BtSelectModel10_Click;

            this.Loaded += PgModel_Loaded;

            this.btSaveModel.Click += BtSaveModel_Click;
            this.btLoadModel.Click += BtLoadModel_Click;

            this.BtnDeleteModel.Click += BtnDeleteModel_Click;
        }

        private void BtnDeleteModel_Click(object sender, RoutedEventArgs e)
        {
            WndComfirm wndcf = new WndComfirm();
            WndMessenger wndms = new WndMessenger();

            UiManager.Instance.PLC.device.ReadBit(DeviceCode.M, 21, out bool IL);
            if (!IL)
            {
                wndms.MessengerShow($"PLC Interlock, cannot delete Model {SelectNumberModel}!!!", Window.GetWindow(this));
                return;
            } 

            if (wndcf.DoComfirmYesNo($"Do you want to Delete Model {SelectNumberModel}\n\r" +
                                     $"Sẽ xoá toàn bộ dữ liệu Model {SelectNumberModel}, dữ liệu hiện tại của máy đang chạy không ảnh hưởng"))
            {
                ModelSettings modelDelete = null;
                switch (this.SelectNumberModel)
                {
                    case 0:
                        wndms.MessengerShow("Please Choose Model !!!", Window.GetWindow(this));
                        return;
                    case 1:
                        if (this.txbModel1.Text == "Model Null")
                        {
                            wndms.MessengerShow("Model Null!!!", Window.GetWindow(this));
                            return;
                        }
                        modelDelete = ModelStore.GetModelSettings(this.txbModel1.Text);
                        break;
                    case 2:
                        if (this.txbModel2.Text == "Model Null")
                        {
                            wndms.MessengerShow("Model Null!!!", Window.GetWindow(this));
                            return;
                        }
                        modelDelete = ModelStore.GetModelSettings(this.txbModel2.Text);
                        break;
                    case 3:
                        if (this.txbModel3.Text == "Model Null")
                        {
                            wndms.MessengerShow("Model Null!!!", Window.GetWindow(this));
                            return;
                        }
                        modelDelete = ModelStore.GetModelSettings(this.txbModel3.Text);
                        break;
                    case 4:
                        if (this.txbModel4.Text == "Model Null")
                        {
                            wndms.MessengerShow("Model Null!!!", Window.GetWindow(this));
                            return;
                        }
                        modelDelete = ModelStore.GetModelSettings(this.txbModel4.Text);
                        break;
                    case 5:
                        if (this.txbModel5.Text == "Model Null")
                        {
                            wndms.MessengerShow("Model Null!!!", Window.GetWindow(this));
                            return;
                        }
                        modelDelete = ModelStore.GetModelSettings(this.txbModel5.Text);
                        break;
                    case 6:
                        if (this.txbModel6.Text == "Model Null")
                        {
                            wndms.MessengerShow("Model Null!!!", Window.GetWindow(this));
                            return;
                        }
                        modelDelete = ModelStore.GetModelSettings(this.txbModel6.Text);
                        break;
                    case 7:
                        if (this.txbModel7.Text == "Model Null")
                        {
                            wndms.MessengerShow("Model Null!!!", Window.GetWindow(this));
                            return;
                        }
                        modelDelete = ModelStore.GetModelSettings(this.txbModel7.Text);
                        break;
                    case 8:
                        if (this.txbModel8.Text == "Model Null")
                        {
                            wndms.MessengerShow("Model Null!!!", Window.GetWindow(this));
                            return;
                        }
                        modelDelete = ModelStore.GetModelSettings(this.txbModel8.Text);
                        break;
                    case 9:
                        if (this.txbModel9.Text == "Model Null")
                        {
                            wndms.MessengerShow("Model Null!!!", Window.GetWindow(this));
                            return;
                        }
                        modelDelete = ModelStore.GetModelSettings(this.txbModel9.Text);
                        break;
                    case 10:
                        if (this.txbModel10.Text == "Model Null")
                        {
                            wndms.MessengerShow("Model Null!!!", Window.GetWindow(this));
                            return;
                        }
                        modelDelete = ModelStore.GetModelSettings(this.txbModel10.Text);
                        break;
                }
                if (modelDelete == null)
                {
                    wndms.MessengerShow($"Cannot find Model {modelDelete.modelName}!!!", Window.GetWindow(this));
                    return;
                }
                if (String.Equals(this.SelectNumberModel.ToString("D2"), this.lblModelNo.Content.ToString()))
                {
                    wndms.MessengerShow("You Cannot Delete Current Model !!!", Window.GetWindow(this));
                    return;
                }
                ModelStore.DeleteModel(modelDelete.modelName);
                this.PgModel_Loaded(null, null);

                UiManager.Instance.PLC.device.WriteDoubleWord(DeviceCode.R, 10600, SelectNumberModel);
                Thread.Sleep(10);
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 804, true);
                Thread.Sleep(20);
                UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 804, false);
                logger.Create($"Delete Model {SelectNumberModel} ok!", LogLevel.Information);
                wndms.MessengerShow($"Delete of Model {SelectNumberModel} ok!", Window.GetWindow(this));
            }
        }

        private void BtLoadModel_Click(object sender, RoutedEventArgs e)
        {
            WndComfirm wndcf = new WndComfirm();
            WndMessenger wndms = new WndMessenger();

            UiManager.Instance.PLC.device.ReadBit(DeviceCode.M, 21, out bool IL);
            if (!IL)
            {
                wndms.MessengerShow($"PLC Interlock, cannot load Model {SelectNumberModel}!!!", Window.GetWindow(this));
                return;
            }

            if (wndcf.DoComfirmYesNo($"Do you want to Load Model {SelectNumberModel}\n\r" +
                                    $"Sẽ ghi đè toàn bộ dữ liệu Model {SelectNumberModel} vào dữ liệu hiện tại của máy đang chạy"))
            {
                try
                {
                    ModelSettings loadedModel = null;
                    switch (this.SelectNumberModel)
                    {
                        case 0:
                            wndms.MessengerShow("Please Choose Model !!!", Window.GetWindow(this));
                            return;
                        case 1:
                            if (this.txbModel1.Text == "Model Null")
                                return;
                            loadedModel = ModelStore.GetModelSettings(this.txbModel1.Text);
                            break;
                        case 2:
                            if (this.txbModel2.Text == "Model Null")
                                return;
                            loadedModel = ModelStore.GetModelSettings(this.txbModel2.Text);
                            break;
                        case 3:
                            if (this.txbModel3.Text == "Model Null")
                                return;
                            loadedModel = ModelStore.GetModelSettings(this.txbModel3.Text);
                            break;
                        case 4:
                            if (this.txbModel4.Text == "Model Null")
                                return;
                            loadedModel = ModelStore.GetModelSettings(this.txbModel4.Text);
                            break;
                        case 5:
                            if (this.txbModel5.Text == "Model Null")
                                return;
                            loadedModel = ModelStore.GetModelSettings(this.txbModel5.Text);
                            break;
                        case 6:
                            if (this.txbModel6.Text == "Model Null")
                                return;
                            loadedModel = ModelStore.GetModelSettings(this.txbModel6.Text);
                            break;
                        case 7:
                            if (this.txbModel7.Text == "Model Null")
                                return;
                            loadedModel = ModelStore.GetModelSettings(this.txbModel7.Text);
                            break;
                        case 8:
                            if (this.txbModel8.Text == "Model Null")
                                return;
                            loadedModel = ModelStore.GetModelSettings(this.txbModel8.Text);
                            break;
                        case 9:
                            if (this.txbModel9.Text == "Model Null")
                                return;
                            loadedModel = ModelStore.GetModelSettings(this.txbModel9.Text);
                            break;
                        case 10:
                            if (this.txbModel10.Text == "Model Null")
                                return;
                            loadedModel = ModelStore.GetModelSettings(this.txbModel10.Text);
                            break;
                    }

                    if (loadedModel != null)
                    {
                        UiManager.ReplaceModel(loadedModel);
                        lblModelName.Content = loadedModel.modelName;
                        lblModelNo.Content = $"{loadedModel.indexModel:D2}";

                        UiManager.Instance.PLC.device.WriteDoubleWord(DeviceCode.R, 10600, SelectNumberModel);
                        Thread.Sleep(10);
                        UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 802, true);
                        Thread.Sleep(20);
                        UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 802, false);
                        logger.Create($"Load Model {SelectNumberModel} ok!", LogLevel.Information);
                        wndms.MessengerShow($"Load data from Model {SelectNumberModel} ok!", Window.GetWindow(this));
                    }
                    else
                    {
                        wndms.MessengerShow("Please Choose Model !!!", Window.GetWindow(this));
                    }
                }
                catch (Exception ex)
                {
                    logger.Create(String.Format("Load Model Error: ") + ex.Message, LogLevel.Error);
                }
            }
        }

        private void BtSaveModel_Click(object sender, RoutedEventArgs e)
        {
            WndComfirm wndcf = new WndComfirm();
            WndMessenger wndms = new WndMessenger();

            //UiManager.Instance.PLC.device.ReadBit(DeviceCode.M, 21, out bool IL);
            //if (!IL)
            //{
            //    wndms.MessengerShow($"PLC Interlock, cannot save Model {SelectNumberModel}!!!", Window.GetWindow(this));
            //    return;
            //}

            if (wndcf.DoComfirmYesNo($"Do you want to Save Model {SelectNumberModel}\n\r" +
                                    $"Sẽ ghi đè toàn bộ dữ liệu hiện tại của máy đang chạy vào Model {SelectNumberModel}"))
            {
                this.SaveModel();
            }
        }
        private void SaveModel()
        {
            WndMessenger wndms = new WndMessenger();
            // Save:
            switch (SelectNumberModel)
            {
                case 0:
                    wndms.MessengerShow("Please Choose Model !!!", Window.GetWindow(this));
                    return;
                case 1:
                    if (this.txbModel1.Text == "Model Null")
                    {
                        wndms.MessengerShow("Please input Model name !!!", Window.GetWindow(this));
                        return;
                    }
                    else
                    {
                        UiManager.currentModel.modelName = this.txbModel1.Text;
                        UiManager.currentModel.indexModel = SelectNumberModel;
                        UiManager.currentModel.updatedTime = DateTime.Now;
                        ModelStore.UpdateModelSettings(UiManager.currentModel.Appsetting());
                    }
                    break;
                case 2:
                    if (this.txbModel2.Text == "Model Null")
                    {
                        wndms.MessengerShow("Please input Model name !!!", Window.GetWindow(this));
                        return;
                    }
                    else
                    {
                        UiManager.currentModel.modelName = this.txbModel2.Text;
                        UiManager.currentModel.indexModel = SelectNumberModel;
                        UiManager.currentModel.updatedTime = DateTime.Now;
                        ModelStore.UpdateModelSettings(UiManager.currentModel.Appsetting());
                    }
                    break;
                case 3:
                    if (this.txbModel3.Text == "Model Null")
                    {
                        wndms.MessengerShow("Please input Model name !!!", Window.GetWindow(this));
                        return;
                    }
                    else
                    {
                        UiManager.currentModel.modelName = this.txbModel3.Text;
                        UiManager.currentModel.indexModel = SelectNumberModel;
                        UiManager.currentModel.updatedTime = DateTime.Now;
                        ModelStore.UpdateModelSettings(UiManager.currentModel.Appsetting());
                    }
                    break;
                case 4:
                    if (this.txbModel4.Text == "Model Null")
                    {
                        wndms.MessengerShow("Please input Model name !!!", Window.GetWindow(this));
                        return;
                    }
                    else
                    {
                        UiManager.currentModel.modelName = this.txbModel4.Text;
                        UiManager.currentModel.indexModel = SelectNumberModel;
                        UiManager.currentModel.updatedTime = DateTime.Now;
                        ModelStore.UpdateModelSettings(UiManager.currentModel.Appsetting());
                    }
                    break;
                case 5:
                    if (this.txbModel5.Text == "Model Null")
                    {
                        wndms.MessengerShow("Please input Model name !!!", Window.GetWindow(this));
                        return;
                    }
                    else
                    {
                        UiManager.currentModel.modelName = this.txbModel5.Text;
                        UiManager.currentModel.indexModel = SelectNumberModel;
                        UiManager.currentModel.updatedTime = DateTime.Now;
                        ModelStore.UpdateModelSettings(UiManager.currentModel.Appsetting());
                    }
                    break;
                case 6:
                    if (this.txbModel6.Text == "Model Null")
                    {
                        wndms.MessengerShow("Please input Model name !!!", Window.GetWindow(this));
                        return;
                    }
                    else
                    {
                        UiManager.currentModel.modelName = this.txbModel6.Text;
                        UiManager.currentModel.indexModel = SelectNumberModel;
                        UiManager.currentModel.updatedTime = DateTime.Now;
                        ModelStore.UpdateModelSettings(UiManager.currentModel.Appsetting());
                    }
                    break;
                case 7:
                    if (this.txbModel7.Text == "Model Null")
                    {
                        wndms.MessengerShow("Please input Model name !!!", Window.GetWindow(this));
                        return;
                    }
                    else
                    {
                        UiManager.currentModel.modelName = this.txbModel7.Text;
                        UiManager.currentModel.indexModel = SelectNumberModel;
                        UiManager.currentModel.updatedTime = DateTime.Now;
                        ModelStore.UpdateModelSettings(UiManager.currentModel.Appsetting());
                    }
                    break;
                case 8:
                    if (this.txbModel8.Text == "Model Null")
                    {
                        wndms.MessengerShow("Please input Model name !!!", Window.GetWindow(this));
                        return;
                    }
                    else
                    {
                        UiManager.currentModel.modelName = this.txbModel8.Text;
                        UiManager.currentModel.indexModel = SelectNumberModel;
                        UiManager.currentModel.updatedTime = DateTime.Now;
                        ModelStore.UpdateModelSettings(UiManager.currentModel.Appsetting());
                    }
                    break;
                case 9:
                    if (this.txbModel9.Text == "Model Null")
                    {
                        wndms.MessengerShow("Please input Model name !!!", Window.GetWindow(this));
                        return;
                    }
                    else
                    {
                        UiManager.currentModel.modelName = this.txbModel9.Text;
                        UiManager.currentModel.indexModel = SelectNumberModel;
                        UiManager.currentModel.updatedTime = DateTime.Now;
                        ModelStore.UpdateModelSettings(UiManager.currentModel.Appsetting());
                    }
                    break;
                case 10:
                    if (this.txbModel10.Text == "Model Null")
                    {
                        wndms.MessengerShow("Please input Model name !!!", Window.GetWindow(this));
                        return;
                    }
                    else
                    {
                        UiManager.currentModel.modelName = this.txbModel10.Text;
                        UiManager.currentModel.indexModel = SelectNumberModel;
                        UiManager.currentModel.updatedTime = DateTime.Now;
                        ModelStore.UpdateModelSettings(UiManager.currentModel.Appsetting());
                    }
                    break;

            }

            UiManager.Instance.PLC.device.WriteDoubleWord(DeviceCode.R, 10600, SelectNumberModel);
            Thread.Sleep(10);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 800, true);
            Thread.Sleep(20);
            UiManager.Instance.PLC.device.WriteBit(DeviceCode.L, 800, false);
            logger.Create($"Save Model {SelectNumberModel} ok!", LogLevel.Information);
            wndms.MessengerShow($"Save data to Model {SelectNumberModel} ok!", Window.GetWindow(this));

        }

        private void PgModel_Loaded(object sender, RoutedEventArgs e)
        {
            this.lblModelNo.Content = UiManager.appSetting.currentModelNo.ToString("D2");
            this.lblModelName.Content = UiManager.appSetting.currentModelName;
            SelectModel();

            this.txbModel1.Text = "Model Null";
            this.txbModel2.Text = "Model Null";
            this.txbModel3.Text = "Model Null";
            this.txbModel4.Text = "Model Null";
            this.txbModel5.Text = "Model Null";
            this.txbModel6.Text = "Model Null";
            this.txbModel7.Text = "Model Null";
            this.txbModel8.Text = "Model Null";
            this.txbModel9.Text = "Model Null";
            this.txbModel10.Text = "Model Null";

            //ModelSettings currentModel = ModelStore.GetModelSettings(UiManager.appSetting.currentModelName);
            //if(currentModel == null)
            //{
            //    logger.Create("Cannot find current model settings: " + UiManager.appSetting.currentModelName, LogLevel.Error);
            //    return;
            //}

            List<ModelInfo> modelInfoList = ModelStore.GetModelInfoList();
            for (int i = 0; i < modelInfoList.Count; i++)
            {
                switch (modelInfoList[i].Index)
                {
                    case 1:
                        this.txbModel1.Text = modelInfoList[i].Name;
                        break;
                    case 2:
                        this.txbModel2.Text = modelInfoList[i].Name;
                        break;
                    case 3:
                        this.txbModel3.Text = modelInfoList[i].Name;
                        break;
                    case 4:
                        this.txbModel4.Text = modelInfoList[i].Name;
                        break;
                    case 5:
                        this.txbModel5.Text = modelInfoList[i].Name;
                        break;
                    case 6:
                        this.txbModel6.Text = modelInfoList[i].Name;
                        break;
                    case 7:
                        this.txbModel7.Text = modelInfoList[i].Name;
                        break;
                    case 8:
                        this.txbModel8.Text = modelInfoList[i].Name;
                        break;
                    case 9:
                        this.txbModel9.Text = modelInfoList[i].Name;
                        break;
                    case 10:
                        this.txbModel10.Text = modelInfoList[i].Name;
                        break;
                }
            }
        }
        private void BtSelectModel01_Click(object sender, RoutedEventArgs e)
        {
            this.SelectNumberModel = 1;
            this.SelectModel();
        }
        private void BtSelectModel02_Click(object sender, RoutedEventArgs e)
        {
            this.SelectNumberModel = 2;
            this.SelectModel();
        }
        private void BtSelectModel03_Click(object sender, RoutedEventArgs e)
        {
            this.SelectNumberModel = 3;
            this.SelectModel();
        }
        private void BtSelectModel04_Click(object sender, RoutedEventArgs e)
        {
            this.SelectNumberModel = 4;
            this.SelectModel();
        }
        private void BtSelectModel05_Click(object sender, RoutedEventArgs e)
        {
            this.SelectNumberModel = 5;
            this.SelectModel();
        }
        private void BtSelectModel06_Click(object sender, RoutedEventArgs e)
        {
            this.SelectNumberModel = 6;
            this.SelectModel();
        }
        private void BtSelectModel07_Click(object sender, RoutedEventArgs e)
        {
            this.SelectNumberModel = 7;
            this.SelectModel();
        }
        private void BtSelectModel08_Click(object sender, RoutedEventArgs e)
        {
            this.SelectNumberModel = 8;
            this.SelectModel();
        }
        private void BtSelectModel09_Click(object sender, RoutedEventArgs e)
        {
            this.SelectNumberModel = 9;
            this.SelectModel();
        }
        private void BtSelectModel10_Click(object sender, RoutedEventArgs e)
        {
            this.SelectNumberModel = 10;
            this.SelectModel();
        }
        private void SelectModel()
        {
            this.txtSelectModel.Text = SelectNumberModel.ToString();

            Button[] buttons = {
                                    btSelectModel01, btSelectModel02, btSelectModel03, btSelectModel04, btSelectModel05,
                                    btSelectModel06, btSelectModel07, btSelectModel08, btSelectModel09, btSelectModel10
                                };
            var bg = (Brush)new BrushConverter().ConvertFromString("#333333");
            var border = (Brush)new BrushConverter().ConvertFromString("#1ba1e2");
            var fg = (Brush)new BrushConverter().ConvertFromString("#ffffff");
            foreach (var btn in buttons)
            {
                btn.Background = bg;
                btn.BorderBrush = border;
                btn.Foreground = fg;
            }

            var bg1 = (Brush)new BrushConverter().ConvertFromString("#1ba1e2");
            var border1 = (Brush)new BrushConverter().ConvertFromString("#1ba1e2");
            var fg1 = (Brush)new BrushConverter().ConvertFromString("#ffffff");
            switch (txtSelectModel.Text)
            {
                case "1":
                    btSelectModel01.Background = bg1;
                    btSelectModel01.BorderBrush = border1;
                    btSelectModel01.Foreground = fg1;
                    break;
                case "2":
                    btSelectModel02.Background = bg1;
                    btSelectModel02.BorderBrush = border1;
                    btSelectModel02.Foreground = fg1;
                    break;
                case "3":
                    btSelectModel03.Background = bg1;
                    btSelectModel03.BorderBrush = border1;
                    btSelectModel03.Foreground = fg1;
                    break;
                case "4":
                    btSelectModel04.Background = bg1;
                    btSelectModel04.BorderBrush = border1;
                    btSelectModel04.Foreground = fg1;
                    break;
                case "5":
                    btSelectModel05.Background = bg1;
                    btSelectModel05.BorderBrush = border1;
                    btSelectModel05.Foreground = fg1;
                    break;
                case "6":
                    btSelectModel06.Background = bg1;
                    btSelectModel06.BorderBrush = border1;
                    btSelectModel06.Foreground = fg1;
                    break;
                case "7":
                    btSelectModel07.Background = bg1;
                    btSelectModel07.BorderBrush = border1;
                    btSelectModel07.Foreground = fg1;
                    break;
                case "8":
                    btSelectModel08.Background = bg1;
                    btSelectModel08.BorderBrush = border1;
                    btSelectModel08.Foreground = fg1;
                    break;
                case "9":
                    btSelectModel09.Background = bg1;
                    btSelectModel09.BorderBrush = border1;
                    btSelectModel09.Foreground = fg1;
                    break;
                case "10":
                    btSelectModel10.Background = bg1;
                    btSelectModel10.BorderBrush = border1;
                    btSelectModel10.Foreground = fg1;
                    break;
            }
        }
    }
}
