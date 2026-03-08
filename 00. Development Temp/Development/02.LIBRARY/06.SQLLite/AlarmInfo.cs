using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class AlarmInfo
    {
        #region Choose Mode Auto/ Manual
        public const int MODE_AUTO = 0;
        public const int MODE_MANUAL = 1;
        #endregion

        private static Dictionary<int, String> alarmMessageMap = new Dictionary<int, string>()
        {

        };
        private static Dictionary<int, String> alarmSolutionMap = new Dictionary<int, string>()
        {


        };

        public static string getMessage(int alarmType)
        {
            string ret;
            if (!alarmMessageMap.TryGetValue(alarmType, out ret))
            {
                ret = "-";
            }
            return ret;
        }

        public static String getSolution(int alarmType)
        {
            String ret;
            if (!alarmSolutionMap.TryGetValue(alarmType, out ret))
            {
                ret = "-";
            }
            return ret;
        }
        public int id { get; set; }
        public int alarmCode { get; set; }
        public DateTime createdTime { get; set; }
        public int mode { get; set; }
        public string message { get; set; }
        public string solution { get; set; }

        public String getMode()
        {
            if (mode == MODE_AUTO)
            {
                return "AUTO";
            }
            return "MANUAL";
        }
        public AlarmInfo() { }

        public AlarmInfo(int code, String msg, String sol)
        {
            // this.id = 0;
            // this.mode = mode;
            this.alarmCode = code;
            this.createdTime = DateTime.Now;
            this.message = msg;
            this.solution = sol;
        }
    }
}
