using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Development
{
    
    public static class ManagerLogin
    {
        static int result = 0;
        public static int DoCheck()
        {
            if(UiManager.managerSetting.assignSystem.LoginMes)
            {
                WndLoginMES loginMES = new WndLoginMES();
                var ResultLogin = loginMES.DoCheck();
                result = ResultLogin;

            }
            else
            {
                WndLogin Login = new WndLogin();
                var ResultLogin = Login.DoCheck();
                result = ResultLogin;
            }
            return result;
        }

    }
}
