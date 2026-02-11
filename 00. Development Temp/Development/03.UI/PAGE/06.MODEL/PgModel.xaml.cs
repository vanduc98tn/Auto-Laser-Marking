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
        private int SelectNumberModel = 0;
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
            WndComfirm wnd = new WndComfirm();
            if (wnd.DoComfirmYesNo($"Do you want to Delete Model {SelectNumberModel}"))
            {
                WndMessenger wndMes0 = new WndMessenger();
                ModelSettings modelDelete = null;
                switch (this.SelectNumberModel)
                {
                    case 1:
                        if (this.txbModel1.Text == "Model Null")
                        {
                            wndMes0.MessengerShow("Model Null!!!", Window.GetWindow(this));
                            return;
                        }
                        modelDelete = ModelStore.GetModelSettings(this.txbModel1.Text);
                        break;
                    case 2:
                        if (this.txbModel2.Text == "Model Null")
                        {
                            wndMes0.MessengerShow("Model Null!!!", Window.GetWindow(this));
                            return;
                        }
                        modelDelete = ModelStore.GetModelSettings(this.txbModel2.Text);
                        break;
                    case 3:
                        if (this.txbModel3.Text == "Model Null")
                        {
                            wndMes0.MessengerShow("Model Null!!!", Window.GetWindow(this));
                            return;
                        }
                        modelDelete = ModelStore.GetModelSettings(this.txbModel3.Text);
                        break;
                    case 4:
                        if (this.txbModel4.Text == "Model Null")
                        {
                            wndMes0.MessengerShow("Model Null!!!", Window.GetWindow(this));
                            return;
                        }
                        modelDelete = ModelStore.GetModelSettings(this.txbModel4.Text);
                        break;
                    case 5:
                        if (this.txbModel5.Text == "Model Null")
                        {
                            wndMes0.MessengerShow("Model Null!!!", Window.GetWindow(this));
                            return;
                        }
                        modelDelete = ModelStore.GetModelSettings(this.txbModel5.Text);
                        break;
                    case 6:
                        if (this.txbModel6.Text == "Model Null")
                        {
                            wndMes0.MessengerShow("Model Null!!!", Window.GetWindow(this));
                            return;
                        }
                        modelDelete = ModelStore.GetModelSettings(this.txbModel6.Text);
                        break;
                    case 7:
                        if (this.txbModel7.Text == "Model Null")
                        {
                            wndMes0.MessengerShow("Model Null!!!", Window.GetWindow(this));
                            return;
                        }
                        modelDelete = ModelStore.GetModelSettings(this.txbModel7.Text);
                        break;
                    case 8:
                        if (this.txbModel8.Text == "Model Null")
                        {
                            wndMes0.MessengerShow("Model Null!!!", Window.GetWindow(this));
                            return;
                        }
                        modelDelete = ModelStore.GetModelSettings(this.txbModel8.Text);
                        break;
                    case 9:
                        if (this.txbModel9.Text == "Model Null")
                        {
                            wndMes0.MessengerShow("Model Null!!!", Window.GetWindow(this));
                            return;
                        }
                        modelDelete = ModelStore.GetModelSettings(this.txbModel9.Text);
                        break;
                    case 10:
                        if (this.txbModel10.Text == "Model Null")
                        {
                            wndMes0.MessengerShow("Model Null!!!", Window.GetWindow(this));
                            return;
                        }
                        modelDelete = ModelStore.GetModelSettings(this.txbModel10.Text);
                        break;
                }
                if (modelDelete == null)
                {
                    wndMes0.MessengerShow($"Cannot find Model {modelDelete.modelName}!!!", Window.GetWindow(this));
                    return;
                }
                if (String.Equals(modelDelete.modelName, this.lblModel.Content.ToString()))
                {
                    wndMes0.MessengerShow("You Cannot Delete Current Model !!!", Window.GetWindow(this));
                    return;
                }
                ModelStore.DeleteModel(modelDelete.modelName);
                this.PgModel_Loaded(null, null);
            }
        }

        private void BtLoadModel_Click(object sender, RoutedEventArgs e)
        {
            WndComfirm wnd = new WndComfirm();
            if (wnd.DoComfirmYesNo($"Do you want to Load Model {SelectNumberModel}"))
            {
                try
                {
                    ModelSettings loadedModel = null;
                    switch (this.SelectNumberModel)
                    {
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
                        lblModel.Content = loadedModel.modelName;
                        lblModelNo.Content = $"{loadedModel.indexModel:D2}";
                    }
                    else
                    {
                        WndMessenger wnd1 = new WndMessenger();
                        wnd1.MessengerShow("Please Choose Model !!!", Window.GetWindow(this));
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
            WndComfirm wnd = new WndComfirm();
            if (wnd.DoComfirmYesNo("Do you want to Save Model"))
            {
                this.SaveModel();
            }
        }
        private void SaveModel()
        {
            // Save:
            switch (SelectNumberModel)
            {
                case 1:
                    if (this.txbModel1.Text == "Model Null")
                    {
                        this.txbModel1.Text = $"Model {SelectNumberModel}";
                        var model = UiManager.currentModel.Clone();
                        model.modelName = this.txbModel1.Text;
                        model.indexModel = SelectNumberModel;
                        model.updatedTime = DateTime.Now;
                        ModelStore.UpdateModelSettings(model);
                    }
                    else
                    {
                        UiManager.currentModel.updatedTime = DateTime.Now;
                        UiManager.currentModel.modelName = this.txbModel1.Text;
                        UiManager.currentModel.indexModel = SelectNumberModel;
                        ModelStore.UpdateModelSettings(UiManager.currentModel);
                    }
                    break;
                case 2:
                    if (this.txbModel2.Text == "Model Null")
                    {
                        this.txbModel2.Text = $"Model {SelectNumberModel}";
                        var model = UiManager.currentModel.Clone();
                        model.modelName = this.txbModel2.Text;
                        model.indexModel = SelectNumberModel;
                        model.updatedTime = DateTime.Now;
                        ModelStore.UpdateModelSettings(model);
                    }
                    else
                    {
                        UiManager.currentModel.updatedTime = DateTime.Now;
                        UiManager.currentModel.modelName = this.txbModel2.Text;
                        UiManager.currentModel.indexModel = SelectNumberModel;
                        ModelStore.UpdateModelSettings(UiManager.currentModel);
                    }
                    break;
                case 3:
                    if (this.txbModel3.Text == "Model Null")
                    {
                        this.txbModel3.Text = $"Model {SelectNumberModel}";
                        var model = UiManager.currentModel.Clone();
                        model.modelName = this.txbModel3.Text;
                        model.indexModel = SelectNumberModel;
                        model.updatedTime = DateTime.Now;
                        ModelStore.UpdateModelSettings(model);
                    }
                    else
                    {
                        UiManager.currentModel.updatedTime = DateTime.Now;
                        UiManager.currentModel.modelName = this.txbModel3.Text;
                        UiManager.currentModel.indexModel = SelectNumberModel;
                        ModelStore.UpdateModelSettings(UiManager.currentModel);
                    }
                    break;
                case 4:
                    if (this.txbModel4.Text == "Model Null")
                    {
                        this.txbModel4.Text = $"Model {SelectNumberModel}";
                        var model = UiManager.currentModel.Clone();
                        model.modelName = this.txbModel4.Text;
                        model.indexModel = SelectNumberModel;
                        model.updatedTime = DateTime.Now;
                        ModelStore.UpdateModelSettings(model);
                    }
                    else
                    {
                        UiManager.currentModel.updatedTime = DateTime.Now;
                        UiManager.currentModel.modelName = this.txbModel4.Text;
                        UiManager.currentModel.indexModel = SelectNumberModel;
                        ModelStore.UpdateModelSettings(UiManager.currentModel);
                    }
                    break;
                case 5:
                    if (this.txbModel5.Text == "Model Null")
                    {
                        this.txbModel5.Text = $"Model {SelectNumberModel}";
                        var model = UiManager.currentModel.Clone();
                        model.modelName = this.txbModel5.Text;
                        model.indexModel = SelectNumberModel;
                        model.updatedTime = DateTime.Now;
                        ModelStore.UpdateModelSettings(model);
                    }
                    else
                    {
                        UiManager.currentModel.updatedTime = DateTime.Now;
                        UiManager.currentModel.modelName = this.txbModel5.Text;
                        UiManager.currentModel.indexModel = SelectNumberModel;
                        ModelStore.UpdateModelSettings(UiManager.currentModel);
                    }
                    break;
                case 6:
                    if (this.txbModel6.Text == "Model Null")
                    {
                        this.txbModel6.Text = $"Model {SelectNumberModel}";
                        var model = UiManager.currentModel.Clone();
                        model.modelName = this.txbModel6.Text;
                        model.indexModel = SelectNumberModel;
                        model.updatedTime = DateTime.Now;
                        ModelStore.UpdateModelSettings(model);
                    }
                    else
                    {
                        UiManager.currentModel.updatedTime = DateTime.Now;
                        UiManager.currentModel.modelName = this.txbModel6.Text;
                        UiManager.currentModel.indexModel = SelectNumberModel;
                        ModelStore.UpdateModelSettings(UiManager.currentModel);
                    }
                    break;
                case 7:
                    if (this.txbModel7.Text == "Model Null")
                    {
                        this.txbModel7.Text = $"Model {SelectNumberModel}";
                        var model = UiManager.currentModel.Clone();
                        model.modelName = this.txbModel7.Text;
                        model.indexModel = SelectNumberModel;
                        model.updatedTime = DateTime.Now;
                        ModelStore.UpdateModelSettings(model);
                    }
                    else
                    {
                        UiManager.currentModel.updatedTime = DateTime.Now;
                        UiManager.currentModel.modelName = this.txbModel7.Text;
                        UiManager.currentModel.indexModel = SelectNumberModel;
                        ModelStore.UpdateModelSettings(UiManager.currentModel);
                    }
                    break;
                case 8:
                    if (this.txbModel8.Text == "Model Null")
                    {
                        this.txbModel8.Text = $"Model {SelectNumberModel}";
                        var model = UiManager.currentModel.Clone();
                        model.modelName = this.txbModel8.Text;
                        model.indexModel = SelectNumberModel;
                        model.updatedTime = DateTime.Now;
                        ModelStore.UpdateModelSettings(model);
                    }
                    else
                    {
                        UiManager.currentModel.updatedTime = DateTime.Now;
                        UiManager.currentModel.modelName = this.txbModel8.Text;
                        UiManager.currentModel.indexModel = SelectNumberModel;
                        ModelStore.UpdateModelSettings(UiManager.currentModel);
                    }
                    break;
                case 9:
                    if (this.txbModel9.Text == "Model Null")
                    {
                        this.txbModel9.Text = $"Model {SelectNumberModel}";
                        var model = UiManager.currentModel.Clone();
                        model.modelName = this.txbModel9.Text;
                        model.indexModel = SelectNumberModel;
                        model.updatedTime = DateTime.Now;
                        ModelStore.UpdateModelSettings(model);
                    }
                    else
                    {
                        UiManager.currentModel.updatedTime = DateTime.Now;
                        UiManager.currentModel.modelName = this.txbModel9.Text;
                        UiManager.currentModel.indexModel = SelectNumberModel;
                        ModelStore.UpdateModelSettings(UiManager.currentModel);
                    }
                    break;
                case 10:
                    if (this.txbModel10.Text == "Model Null")
                    {
                        this.txbModel10.Text = $"Model {SelectNumberModel}";
                        var model = UiManager.currentModel.Clone();
                        model.modelName = this.txbModel10.Text;
                        model.indexModel = SelectNumberModel;
                        model.updatedTime = DateTime.Now;
                        ModelStore.UpdateModelSettings(model);
                    }
                    else
                    {
                        UiManager.currentModel.updatedTime = DateTime.Now;
                        UiManager.currentModel.modelName = this.txbModel10.Text;
                        UiManager.currentModel.indexModel = SelectNumberModel;
                        ModelStore.UpdateModelSettings(UiManager.currentModel);
                    }
                    break;
            }
        }

        private void PgModel_Loaded(object sender, RoutedEventArgs e)
        {
            this.lblModel.Content = UiManager.appSetting.currentModel;
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

            ModelSettings currentModel = ModelStore.GetModelSettings(UiManager.appSetting.currentModel);
            if(currentModel == null)
            {
                logger.Create("Cannot find current model settings: " + UiManager.appSetting.currentModel, LogLevel.Error);
                return;
            }
            this.lblModelNo.Content = currentModel.indexModel.ToString("D2");

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
        }
    }
}
