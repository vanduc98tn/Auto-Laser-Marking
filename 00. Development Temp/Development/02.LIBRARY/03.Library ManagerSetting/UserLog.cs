using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public enum UserActions
    {

        APP_START,
        APP_BUTTON_MAIN,
        APP_BUTTON_MENU,
        APP_BUTTON_IO,
        APP_BUTTON_LASTJAM,
        APP_BUTTON_SHUTDOWN,
        MAIN_ENTER,
        MAIN_EXIT,

        MAIN_BUTTON_START,
        MAIN_BUTTON_PAUSE,
        MAIN_BUTTON_HOME,
        MAIN_BUTTON_STOP,
        MAIN_BUTTON_RESET,
        MAIN_BUTTON_CONVEY,
        MAIN_BUTTON_INIT,
        MAIN_BUTTON_LOT_END,
        MAIN_BUTTON_LOTIN,

        PAGE_STATUS_LOG_LOADED,
        PAGE_STATUS_LOG_UNLOAED,
        PAGE_STATUS_LOG_BUTTON_ALARM_FIRST,
        PAGE_STATUS_LOG_BUTTON_ALARM_PRE_PAGE,
        PAGE_STATUS_LOG_BUTTON_ALARM_PREVIOUS,
        PAGE_STATUS_LOG_BUTTON_ALARM_CURRENT,
        PAGE_STATUS_LOG_BUTTON_ALARM_NEXT,
        PAGE_STATUS_LOG_BUTTON_ALARM_NEXT_PAGE,
        PAGE_STATUS_LOG_BUTTON_ALARM_LAST,
        PAGE_STATUS_LOG_BUTTON_EVENT_PREVIOUS,
        PAGE_STATUS_LOG_BUTTON_EVENT_LOG,
        PAGE_STATUS_LOG_BUTTON_TODAY,
        PAGE_STATUS_LOG_BUTTON_PRE_PAGE,
        PAGE_STATUS_LOG_BUTTON_NEXT_PAGE,
        PAGE_STATUS_LOG_CHANGE_DATE,


        PAGE_MANUAL_OPERATION,
        PAGE_MANUAL_OPERATION1,
        PAGE_MANUAL_OPERATION2,
        PAGE_MANUAL_OPERATION3,

        PAGE_MECHANICAL_MENU,
        PAGE_MECHANICAL_MENU1,
        PAGE_MECHANICAL_MENU2,
        PAGE_MECHANICAL_MENU3,
        PAGE_MECHANICAL_SETUP_TCP_PLC,
        PAGE_MECHANICAL_SETUP_TCP_SCANNER,


        LOGON_SHOW,
        LOGON_BUTTON_ENTER,
        LOGON_BUTTON_CANCEL,
        LOGON_BUTTON_CHANGE_PASSWORD,
        LOGON_CHANGE_PASS_SUCCESS,
        LOGIN_BUTTON_SIGNIN,
        LOGIN_BUTTON_MANAGER,
        LOGIN_BUTTON_AUTOTEAMS,
        LOGIN_BUTTON_OPERATOR,

        CHANGEPASS_SHOW,
        CHANGEPASS_BUTTON_OK,
        CHANGEPASS_BUTTON_CANCEL,

        IO_ENTER,
        IO_EXIT,
        IO_INPUT,
        IO_OUTPUT,
        LASTJAM_ENTER,
        LASTJAM_EXIT,
        LASTJAM_SELECT,

        WND_MAIN,
        WND_MENU,
        WND_IO,
        WND_ALARM,
        WND_CLOSE,

        CLICK_BTN

        
    };
    class UserLog
    {
        public int Id { get; set; }
        public String Username { get; set; }
        public String CreatedTime { get; set; }
        public int Action { get; set; }
        public String Message { get; set; }

        public UserLog() { }

        public UserLog(UserActions action)
        {
            this.Id = 0;
            this.Username = UiManager.managerSetting.loginApp.UseName;
            this.CreatedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
            this.Action = (int)action;
            this.Message = getActionMessage(action);
        }

        private static String getActionMessage(UserActions action)
        {
            var ret = action.ToString();

            return ret;
        }
    }
}
