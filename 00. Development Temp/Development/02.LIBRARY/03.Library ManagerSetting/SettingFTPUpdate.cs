using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
     public class SettingFTPUpdate
    {
        public String Url { get; set; }
        public String UseLogin { get; set; }
        public String UserName { get; set; }
        public String PassWord { get; set; }
        public String NameAppOpen { get; set; }

        public SettingFTPUpdate()
        {
            this.Url = "ftp://192.168.54.99:38/01.TestServer/";
            this.UseLogin = "N";
            this.UserName = "No Name";
            this.PassWord = "NoPassWord";
            this.NameAppOpen = "Development.exe";
        }

    }
}
