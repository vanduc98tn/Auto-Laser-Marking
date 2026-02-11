using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
namespace Development
{
    class AppSetting
    {
        public const string SETTING_FILE_NAME = "01.AppSetting.json";
        public SettingDevice settingDevice;
        public SaveDevice selectDevice;
        public LotInData LotinData;
        public MESSetting MESSettings;
        public RUNMachine RUN;


        public string currentModel; // Machine Run Model
        public ROIProperty RoiProperty;
        public ConnectionSettings connection;
        public AppSetting()
        {
            this.settingDevice = new SettingDevice();
            this.selectDevice = SaveDevice.Mitsubishi_MC_Protocol_Binary_TCP;
            this.LotinData = new LotInData();
            this.MESSettings = new MESSetting();
            this.RUN = new RUNMachine();

            this.currentModel = ModelSettings.DEFAULT_MODEL_NAME;
            this.RoiProperty = new ROIProperty();
            this.connection = new ConnectionSettings();
        }
        public string TOJSON()
        {
            string retValue = "";
            retValue = JsonConvert.SerializeObject(this , Formatting.Indented);
            return retValue;
        }
        public static AppSetting FromJSON(String json)
        {

            var _appSettings = JsonConvert.DeserializeObject<AppSetting>(json);


            if (_appSettings.settingDevice == null)
            {
                _appSettings.settingDevice = new SettingDevice();
            }
            if (_appSettings.LotinData == null)
            {
                _appSettings.LotinData = new LotInData();
            }
            if (_appSettings.MESSettings == null)
            {
                _appSettings.MESSettings = new MESSetting();
            }
            if (_appSettings.RUN == null)
            {
                _appSettings.RUN = new RUNMachine();
            }
            if(_appSettings.currentModel == null)
            {
                _appSettings.currentModel = ModelSettings.DEFAULT_MODEL_NAME;
            }  
            if(_appSettings.RoiProperty == null)
            {
                _appSettings.RoiProperty = new ROIProperty();
            }
            if(_appSettings.connection == null)
            {
                _appSettings.connection = new ConnectionSettings();
            }
            return _appSettings;
        }
    }
}
