using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    class MyLogger
    {
        private static Object objLock = new Object();

        private String prefix = "";

        public MyLogger(String prefix)
        {
            this.prefix = prefix;
        }

        public void Create(String content,LogLevel logLevel)
        {
            // Get FilePath:
            var currentDate = DateTime.Now.ToString("yyyy-MM");

            var fileName = String.Format("{0}.log", DateTime.Now.ToString("yyyy-MM-dd"));
            var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "02.Logs", "DebugLogs", currentDate);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var filePath = System.IO.Path.Combine(folder, fileName);

            lock (objLock)
            {
                try
                {
                    var log = String.Format("\r\n{0}-[{1}]-[{3}]:{2}", DateTime.Now.ToString("HH:mm:ss.ff"), this.prefix, content,logLevel.ToString());

                    System.Diagnostics.Debug.Write(log);

                    using (var strWriter = new StreamWriter(filePath, true))
                    {
                        strWriter.Write(log);
                        strWriter.Flush();
                    }
                }
                catch (Exception ex)
                {
                    Debug.Write("\r\nMyLoger.Create error:" + ex.Message);
                }
            }
        }

        public void CreateDataVision(String content, LogLevel logLevel)
        {
            // Get FilePath:
            var currentDate = DateTime.Now.ToString("yyyy-MM");

            var fileName = String.Format("{0}.log", DateTime.Now.ToString("yyyy-MM-dd"));
            var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "02.Logs", "DebugLogsVision", currentDate);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var filePath = System.IO.Path.Combine(folder, fileName);

            lock (objLock)
            {
                try
                {
                    var log = String.Format("\r\n{0}-[{1}]-[{3}]:{2}", DateTime.Now.ToString("HH:mm:ss.ff"), this.prefix, content,logLevel.ToString());

                    System.Diagnostics.Debug.Write(log);

                    using (var strWriter = new StreamWriter(filePath, true))
                    {
                        strWriter.Write(log);
                        strWriter.Flush();
                    }
                }
                catch (Exception ex)
                {
                    Debug.Write("\r\nMyLoger.Create error:" + ex.Message);
                }
            }
        }
        public void Create01(String content, LogLevel logLevel)
        {
            // Get FilePath:
            var currentDate = DateTime.Now.ToString("yyyy-MM");

            var fileName = String.Format("{0}.log", DateTime.Now.ToString("yyyy-MM-dd"));
            var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "02.Logs", "DebugTester", currentDate);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var filePath = System.IO.Path.Combine(folder, fileName);

            lock (objLock)
            {
                try
                {
                    var log = String.Format("\r\n{0}-[{1}]-[{3}]:{2}", DateTime.Now.ToString("HH:mm:ss.ff"), this.prefix, content, logLevel.ToString());

                    System.Diagnostics.Debug.Write(log);

                    using (var strWriter = new StreamWriter(filePath, true))
                    {
                        strWriter.Write(log);
                        strWriter.Flush();
                    }
                }
                catch (Exception ex)
                {
                    Debug.Write("\r\nMyLoger.Create error:" + ex.Message);
                }
            }
        }
        public void CreateLaser(String content, LogLevel logLevel)
        {
            // Get FilePath:
            var currentDate = DateTime.Now.ToString("yyyy-MM");

            var fileName = String.Format("{0}.log", DateTime.Now.ToString("yyyy-MM-dd"));
            var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "02.Logs", "DebugLaser", currentDate);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var filePath = System.IO.Path.Combine(folder, fileName);

            lock (objLock)
            {
                try
                {
                    var log = String.Format("\r\n{0}-[{1}]-[{3}]:{2}", DateTime.Now.ToString("HH:mm:ss.ff"), this.prefix, content, logLevel.ToString());

                    System.Diagnostics.Debug.Write(log);

                    using (var strWriter = new StreamWriter(filePath, true))
                    {
                        strWriter.Write(log);
                        strWriter.Flush();
                    }
                }
                catch (Exception ex)
                {
                    Debug.Write("\r\nMyLoger.Create error:" + ex.Message);
                }
            }
        }
        public void WriteLogSystem(Exception ex, string title)
        {
            try
            {
                string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "02.Logs", "LogSystem");
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

               
                string fileName = $"{DateTime.Now:yyyy-MM-dd}.log";
                string filePath = Path.Combine(logDirectory, fileName);

                
                string logContent = $"==================== {DateTime.Now:HH:mm:ss} ====================\n" +
                                    $"Title: {title}\n" +
                                    $"Message: {ex?.Message}\n" +
                                    $"StackTrace: {ex?.StackTrace}\n" +
                                    $"InnerException: {ex?.InnerException?.Message}\n" +
                                    "==============================================================\n\n";
                File.AppendAllText(filePath, logContent);
            }
            catch
            {
                
            }
        }

        public void UpdateLogMes(DataPCB Data)
        {
            try
            {

                // Check file existing:                    
                var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "MesLogData");
                folder = Path.Combine(folder, DateTime.Today.ToString("yyyy-MM-dd"));
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                var fileName = String.Format("[{0}]-MES.csv", DateTime.Today.ToString("yyyy-MM-dd"));
                var filePath = Path.Combine(folder, fileName);

                // Create Headers if file not existed:
                if (!File.Exists(filePath))
                {
                    using (var strWriter = new StreamWriter(filePath, false))
                    {
                        var header = "BARCODE, " +
                            "DATE," +
                            "RESULT CHECK," +
                            "CUR BIN CHAR," +
                            "PRE BIN CODE, " +
                            "WORK IN RESULT, " +
                            "WORK IN MSG, " +
                            "SEND CUR BIN MSG," +
                            "RECEVIE CUR BIN MSG," +
                            "WORK OUT RESULT," +
                            "WORK OUT MSG";
                        strWriter.WriteLine(header);
                    }
                }
                ;
                string result = string.Join("|", Data.RESULT_PCB);
                // Create log:
                var log = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                    Data.BARCODE_PCB,
                    Data.TRAN_TIME,
                    result,
                    Data.CUR_BIN_CHAR,
                    Data.PRE_BIN_CODE,
                    Data.WORK_IN_RESULT,
                    Data.WORK_IN_MSG,
                    Data.SEND_CUR_BIN_MSG,
                    Data.RECEVIE_CUR_BIN_MSG,
                    Data.WORK_OUT_RESULT,
                    Data.WORK_OUT_MSG
                    );
                using (var strWriter = new StreamWriter(filePath, true))
                {
                    strWriter.WriteLine(log);
                    strWriter.Flush();
                }
            }
            catch (Exception ex)
            {

                Create("AutoLogger CreateMesLog error:" + ex.Message, LogLevel.Error);
            }
        }
    }
}
