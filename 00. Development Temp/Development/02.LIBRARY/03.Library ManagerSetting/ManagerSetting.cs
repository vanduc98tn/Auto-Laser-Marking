using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class ManagerSetting
    {
        public const string SETTING_FILE_NAME = "02.ManagerSetting.json";
       

        public SettingFTPUpdate settingFTPUpdate;
        public AssignSystem assignSystem;
        public LoginApp loginApp;
       


        public ManagerSetting()
        {
            settingFTPUpdate = new SettingFTPUpdate();
            assignSystem = new AssignSystem();
            loginApp = new LoginApp();
           
        }
        public string TOJSON()
        {
            string retValue = "";
            retValue = JsonConvert.SerializeObject(this);
            return retValue;
        }
        public static ManagerSetting FromJSON(String json)
        {

            var J = JsonConvert.DeserializeObject<ManagerSetting>(json);
            if (J.settingFTPUpdate == null)
            {
                J.settingFTPUpdate = new SettingFTPUpdate();
            }
            if (J.assignSystem == null)
            {
                J.assignSystem = new AssignSystem();
            }
            if (J.loginApp == null)
            {
                J.loginApp = new LoginApp();
            }
        
            return J;
        }
    }
}
