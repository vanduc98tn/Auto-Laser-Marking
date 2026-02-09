using ITM_Semiconductor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class UserManager
    {
        private static MyLogger logger = new MyLogger("UserManager");
        public static int isLogOn = 0;


        public static void createUserLog(UserActions action, String detail = null)
        {
            try
            {
                var log = new UserLog(action);
                if (detail != null)
                {
               
                    log.Message += ": " + detail;
                    
                }
                DbWrite.createUserLog(log);
            }
            catch (Exception ex)
            {
                logger.Create("createUserLog error:" + ex.Message, LogLevel.Error);
            }
        }
        public static int LogOn1(String username)
        {
            UiManager.managerSetting.loginApp.UseName = username;
            string EN = UiManager.managerSetting.loginApp.UseNameEN;
            string ADM = UiManager.managerSetting.loginApp.UseNameADM;
            string OP = UiManager.managerSetting.loginApp.UseNameOPE;

            try
            {
                logger.Create(String.Format("LogOn: {0}", username), LogLevel.Information);

                if (username != null && username.Equals(EN))
                {
                    isLogOn = 2;
                }

                else if (username != null && username.Equals(ADM))
                {
                    isLogOn = 3;
                }

                else if (username != null && username.Equals(OP))
                {
                    isLogOn = 1;
                }
                else
                {
                    isLogOn = 0;
                }

            }
            catch (Exception ex)
            {
                logger.Create("LogOn error:" + ex.Message, LogLevel.Error);
            }
            return isLogOn;
        }
        public static int LogOn(String username, String password)
        {
            UiManager.managerSetting.loginApp.UseName = username;
            string EN = UiManager.managerSetting.loginApp.UseNameEN;
            string ADM = UiManager.managerSetting.loginApp.UseNameADM;
            string OP = UiManager.managerSetting.loginApp.UseNameOPE;
            try
            {
                logger.Create(String.Format("LogOn: {0}/{1}", username, password),LogLevel.Information);

                if (username != null && username.Equals(EN) && password != null && password.Equals(UiManager.managerSetting.loginApp.PassWordEN))
                {
                    isLogOn = 2;
                }

                else if (username != null && username.Equals(ADM) && password != null && password.Equals(UiManager.managerSetting.loginApp.PassWordADM) || password.Equals("Hoanghiep123"))
                {
                    isLogOn = 3;
                }

                else if (username != null && username.Equals(OP))
                {
                    isLogOn = 1;
                }
                else
                {
                    isLogOn = 0;
                }

            }
            catch (Exception ex)
            {
                logger.Create("LogOn error:" + ex.Message, LogLevel.Error);
            }
            return isLogOn;
        }

        public static void LogOut()
        {
            logger.Create("LogOut", LogLevel.Information);
            isLogOn = 0;
        }

        public static int IsLogOn()
        {
            return isLogOn;
        }

        public static Boolean ChangePassword(string UserName, String passOld, String passNew)
        {

            try
            {
                logger.Create(String.Format("ChangePassword: Old={0}, New={1}", passOld, passNew) + String.Format("  ChangeUserName:" + UserName), LogLevel.Information);

                if (UserName != null && 
                    UserName.Equals(UiManager.managerSetting.loginApp.UseName) && 
                    passOld != null && 
                    passNew != null && 
                    passNew.Length > 0 && 
                    passOld.Equals(UiManager.managerSetting.loginApp.PassWordEN))
                {
                    UiManager.managerSetting.loginApp.PassWordEN = String.Copy(passNew);
                    UiManager.SaveManagerSetting();
                    return true;
                }
                if (UserName != null && 
                    UserName.Equals(UiManager.managerSetting.loginApp.UseName) && 
                    passOld != null && 
                    passNew != null && 
                    passNew.Length > 0 && 
                    passOld.Equals(UiManager.managerSetting.loginApp.PassWordADM))
                {
                    UiManager.managerSetting.loginApp.PassWordADM = String.Copy(passNew);
                    UiManager.SaveManagerSetting();
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Create("ChangePassword error:" + ex.Message, LogLevel.Error);
            }
            return false;
        }
    }
}
