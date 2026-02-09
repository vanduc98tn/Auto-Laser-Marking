
using Development;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITM_Semiconductor
{
    class DbWrite
    {
        private static MyLogger logger = new MyLogger("DbWrite");
        
        public static bool createAlarm(AlarmInfo alarm)
        {
            var ret = false;
            using (var conn = Dba.GetConnection())
            {
                var sql = "INSERT INTO alarm_log (created_time, alarm_code, message, solution) VALUES (@time, @code, @message, @solution)";
               // var sql = "INSERT INTO alarm_log (created_time, alarm_code, message, solution, mode) VALUES (@time, @code, @message, @solution, @mode)";
                using (var sqlCmd = conn.CreateCommand())
                {
                    try
                    {
                        sqlCmd.CommandText = sql;
                        sqlCmd.Parameters.AddWithValue("@time", alarm.createdTime.ToString("yyyy-MM-dd HH:mm:ss.ff"));
                        sqlCmd.Parameters.AddWithValue("@code", alarm.alarmCode);
                        sqlCmd.Parameters.AddWithValue("@message", alarm.message);
                        sqlCmd.Parameters.AddWithValue("@solution", alarm.solution);
                        //sqlCmd.Parameters.AddWithValue("@mode", alarm.mode);

                        conn.Open();
                        ret = sqlCmd.ExecuteNonQuery() > 0;
                    }
                    catch (Exception ex)
                    {
                        logger.Create("CreateAlarm Error: " + ex.Message, LogLevel.Error);
                    }
                }
            }
            return ret;
        }
        public static bool createEventLog(string message, string type)
        {
            var ret = false;
            using (var conn = Dba.GetConnection())
            {
                var sql = "INSERT INTO event_log (created_time, message,event_type) VALUES (@createdd_time, @message, @event_type)";
                using (var sqlCmd = conn.CreateCommand())
                {
                    try
                    {
                        string UseName = UiManager.managerSetting.loginApp.UseName;
                        sqlCmd.CommandText = sql;
                        sqlCmd.Parameters.AddWithValue("@createdd_time",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff"));
                        sqlCmd.Parameters.AddWithValue("@message",$"[{UseName}]:{message}");
                        sqlCmd.Parameters.AddWithValue("@event_type",type);
                      

                        conn.Open();
                        ret = sqlCmd.ExecuteNonQuery() > 0;
                    }
                    catch (Exception ex)
                    {
                        logger.Create("CreateUserLog Error:" + ex.Message, LogLevel.Error);
                    }
                }
            }
            return ret;
        }
        public static bool createUserLog(UserLog log)
        {
            var ret = false;
            if (log.Username == null)
            {
                log.Username = "Operator";
            }
            if (string.IsNullOrEmpty(log.Username) || log.Action == null)
            {
                logger.Create("CreateUserLog Error: Username or Action is null", LogLevel.Error);
                return false;
            }

            using (var conn = Dba.GetConnection())
            {
                var sql = "INSERT INTO user_log (username, created_time, action, message) VALUES (@username, @time, @action, @message)";
                using (var sqlCmd = conn.CreateCommand())
                {
                    try
                    {
                        sqlCmd.CommandText = sql;
                        sqlCmd.Parameters.AddWithValue("@username", log.Username);

                        // Parse the string to DateTime and format it
                        if (DateTime.TryParse(log.CreatedTime, out DateTime createdTime))
                        {
                            sqlCmd.Parameters.AddWithValue("@time", createdTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        else
                        {
                            logger.Create("CreateUserLog Error: Invalid CreatedTime format", LogLevel.Error);
                            return false;
                        }

                        sqlCmd.Parameters.AddWithValue("@action", log.Action);
                        sqlCmd.Parameters.AddWithValue("@message", log.Message ?? string.Empty);

                        conn.Open();
                        ret = sqlCmd.ExecuteNonQuery() > 0;
                    }
                    catch (Exception ex)
                    {
                        logger.Create("CreateUserLog Error: " + ex.Message, LogLevel.Error);
                    }
                }
            }
            return ret;
        }
        public static bool DeleteHistoryAlarm()
        {
            var ret = false;
            using (var conn = Dba.GetConnection())
            {
                var sql = "DELETE FROM alarm_log";
                using (var sqlCmd = conn.CreateCommand())
                {
                    try
                    {
                        sqlCmd.CommandText = sql;

                        conn.Open();
                        ret = sqlCmd.ExecuteNonQuery() > 0;
                    }
                    catch (Exception ex)
                    {
                        logger.Create("Delete History Alarm Error: " + ex.Message, LogLevel.Error);
                    }
                }
            }
            return ret;
        }
        public static bool DeleteHistory()
        {
            var ret = false;
            using (var conn = Dba.GetConnection())
            {
                var sql = "DELETE FROM result_Check";
                using (var sqlCmd = conn.CreateCommand())
                {
                    try
                    {
                        sqlCmd.CommandText = sql;

                        conn.Open();
                        ret = sqlCmd.ExecuteNonQuery() > 0;
                    }
                    catch (Exception ex)
                    {
                        logger.Create("Delete History Alarm Error: " + ex.Message, LogLevel.Error);
                    }
                }
            }
            return ret;
        }
        
    }
}
