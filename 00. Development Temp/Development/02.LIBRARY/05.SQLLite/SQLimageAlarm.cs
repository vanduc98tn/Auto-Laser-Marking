using ITM_Semiconductor;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    class SQLimageAlarm
    {
        private const String dbFileName = "01 SQLImageAlarm.db";
        private static MyLogger logger = new MyLogger("SQLimageAlarm");
      
        public static SQLiteConnection GetConnection()
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "01.ImageAlarm");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string dbPath = Path.Combine(folderPath, dbFileName);

       
            var dbConnectionString = String.Format("Data Source={0};Mode=ReadWriteCreate;", dbPath);
            var conn = new SQLiteConnection(dbConnectionString)
            {
                DefaultTimeout = 10
            };
            return conn;
        }

        public static void createDatabaseIfNotExisted()
        {
            try
            {
                string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "01.ImageAlarm");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                string dbPath = Path.Combine(folderPath, dbFileName);

                if (!File.Exists(dbPath))
                {
                    logger.Create(" -> Database File SQLite Not Existed -> Create New!", LogLevel.Information);
                    SQLiteConnection.CreateFile(dbPath);
                    createTableAlarmImage();

                }
                else
                {
                    logger.Create(" -> Database File SQLite Already Existed!", LogLevel.Information);
                }
            }
            catch (Exception ex)
            {
                logger.Create("CreateDatabaseIfNotExisted Error: " + ex.Message, LogLevel.Error);
            }
        }
        private static void createTableAlarmImage()
        {
            try
            {
                logger.Create("Create TableAlarmImage: ", LogLevel.Information);
                using (var conn = GetConnection())
                {

                    var sql = $"CREATE TABLE IF NOT EXISTS AlarmImage_log (id INTEGER NOT NULL, NameImage TEXT NOT NULL, PRIMARY KEY(id AUTOINCREMENT))";
                    using (var sqlCmd = conn.CreateCommand())
                    {
                        sqlCmd.CommandText = sql;
                        conn.Open();
                        sqlCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Create("Create TableUserLog Error: " + ex.Message, LogLevel.Error);
            }
        }
        public static bool createAlarmImage(int id, string alarm)
        {
            var ret = false;
            using (var conn = GetConnection())
            {
                var sql = @"INSERT INTO AlarmImage_log (id, NameImage) VALUES (@id, @solution) ON CONFLICT(id) DO UPDATE SET NameImage = excluded.NameImage;";

                using (var sqlCmd = conn.CreateCommand())
                {
                    try
                    {
                        sqlCmd.CommandText = sql;
                        sqlCmd.Parameters.AddWithValue("@id", id);
                        sqlCmd.Parameters.AddWithValue("@solution", alarm.ToString());

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
        public static string ReadAlarmImage(int id)
        {
            string nameImage = null;
            using (var conn = GetConnection())
            {
                var sql = "SELECT NameImage FROM AlarmImage_log WHERE id = @id";
                using (var sqlCmd = conn.CreateCommand())
                {
                    try
                    {
                        sqlCmd.CommandText = sql;
                        sqlCmd.Parameters.AddWithValue("@id", id);

                        conn.Open();
                        using (var reader = sqlCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                nameImage = reader.GetString(0); // Lấy giá trị cột NameImage từ kết quả truy vấn
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Create("ReadAlarmImage Error: " + ex.Message, LogLevel.Error);
                    }
                }
            }
            return nameImage;
        }

    }
}
