using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    class AlarmList
    {
        public static Dictionary<int, string> _AlarmList = new Dictionary<int, string>();
        public static Dictionary<int, string> _SolutionList = new Dictionary<int, string>();

        public static Dictionary<int, string> _NameButton = new Dictionary<int, string>();
        public static Dictionary<int, string> _DeviceButton = new Dictionary<int, string>();
        public static Dictionary<int, string> _DeviceCodeButton = new Dictionary<int, string>();

        public static Dictionary<int, string> _DeviceLampButton = new Dictionary<int, string>();
        public static Dictionary<int, string> _DeviceCodeLampButton = new Dictionary<int, string>();


        public static void LoadAlarm()
        {
            string Alarmpath = AppDomain.CurrentDomain.BaseDirectory + Properties.Settings.Default.alarmList;
            if (!File.Exists(Alarmpath))
                return;

         
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(Alarmpath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; 
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    int alarmKey;
                    if (int.TryParse(worksheet.Cells[row, 1].Text, out alarmKey))
                    {
                        string alarmMessage = worksheet.Cells[row, 2].Text.Replace("\\n", Environment.NewLine);
                        string solutionMessage = worksheet.Cells[row, 3].Text.Replace("\\n", Environment.NewLine);

                        string NameButton = worksheet.Cells[row, 4].Text.Replace("\\n", Environment.NewLine);
                        string DeviceButton = worksheet.Cells[row, 6].Text.Replace("\\n", Environment.NewLine);
                        string DeviceCodeButton = worksheet.Cells[row, 5].Text.Replace("\\n", Environment.NewLine);

                        string DeviceLampButton = worksheet.Cells[row, 8].Text.Replace("\\n", Environment.NewLine);
                        string DeviceCodeLampButton = worksheet.Cells[row, 7].Text.Replace("\\n", Environment.NewLine);

                        _AlarmList[alarmKey] = alarmMessage;
                        _SolutionList[alarmKey] = solutionMessage;

                        _NameButton[alarmKey] = NameButton;
                        _DeviceButton[alarmKey] = DeviceButton;
                        _DeviceCodeButton[alarmKey] = DeviceCodeButton;

                        _DeviceLampButton[alarmKey] = DeviceLampButton;
                        _DeviceCodeLampButton[alarmKey] = DeviceCodeLampButton;
                    }
                }
            }
        }
        public static void LoadAlarm(string language = "vi")
        {
            string Alarmpath = AppDomain.CurrentDomain.BaseDirectory + Properties.Settings.Default.alarmList;
            if (!File.Exists(Alarmpath))
                return;

          
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(Alarmpath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

              
                int headerRow = 1;
                for (int r = 1; r <= Math.Min(10, rowCount); r++)
                {
                    if (worksheet.Cells[r, 1].Text.Trim().ToLower() == "mã lỗi")
                    {
                        headerRow = r;
                        break;
                    }
                }

               
                if (headerRow == 1) headerRow = 3;

            
                int alarmColIndex = -1;
                int solutionColIndex = -1;
               
                for (int col = 1; col <= colCount; col++)
                {
                    string header = worksheet.Cells[headerRow, col].Text.Trim();

                   
                    string cleanHeader = header.ToLower().Replace("  ", " ").Trim();

                    if (cleanHeader == $"messenger {language}")
                        alarmColIndex = col;
                    else if (cleanHeader == $"solution {language}")
                        solutionColIndex = col;
                }

              
                // nếu không ngôn ngữ phía trên thì thử tìm lại ngôn ngữ vi là tiếng việt - hiiep cmt
                if (alarmColIndex == -1 || solutionColIndex == -1)
                {
                    for (int col = 1; col <= colCount; col++)
                    {
                        string cleanHeader = worksheet.Cells[headerRow, col].Text.Trim().ToLower().Replace("  ", " ").Trim();
                        if (cleanHeader == "messenger vi") alarmColIndex = col;
                        else if (cleanHeader == "solution vi") solutionColIndex = col;
                    }
                }

                // Nếu không thấy dùng mặc định cột 2 va 3 (tiếng Việt)
                if (alarmColIndex == -1) alarmColIndex = 2;
                if (solutionColIndex == -1) solutionColIndex = 3;

             
                for (int row = headerRow + 1; row <= rowCount; row++)
                {
                    if (int.TryParse(worksheet.Cells[row, 1].Text.Trim(), out int alarmKey))
                    {
                        string alarmMessage = worksheet.Cells[row, alarmColIndex].GetValue<string>()?.Replace("\\n", Environment.NewLine) ?? "";
                        string solutionMessage = worksheet.Cells[row, solutionColIndex].GetValue<string>()?.Replace("\\n", Environment.NewLine) ?? "";

                        string NameButton = worksheet.Cells[row, 4].GetValue<string>()?.Replace("\\n", Environment.NewLine) ?? "";
                        string DeviceCodeButton = worksheet.Cells[row, 5].GetValue<string>()?.Replace("\\n", Environment.NewLine) ?? "";
                        string DeviceNameButton = worksheet.Cells[row, 6].GetValue<string>()?.Replace("\\n", Environment.NewLine) ?? "";
                        string DeviceCodeLampButton = worksheet.Cells[row, 7].GetValue<string>()?.Replace("\\n", Environment.NewLine) ?? "";
                        string DeviceLampButton = worksheet.Cells[row, 8].GetValue<string>()?.Replace("\\n", Environment.NewLine) ?? "";

                        _AlarmList[alarmKey] = alarmMessage;
                        _SolutionList[alarmKey] = solutionMessage;
                        _NameButton[alarmKey] = NameButton;
                        _DeviceButton[alarmKey] = DeviceNameButton;       
                        _DeviceCodeButton[alarmKey] = DeviceCodeButton;
                        _DeviceLampButton[alarmKey] = DeviceLampButton;
                        _DeviceCodeLampButton[alarmKey] = DeviceCodeLampButton;
                    }
                }
            }
        }


        public static string GetMes(int AlarmKey)
        {
            if (!_AlarmList.ContainsKey(AlarmKey))
                return "";
            return _AlarmList[AlarmKey];
        }

        public static string GetSolution(int AlarmKey)
        {
            if (!_SolutionList.ContainsKey(AlarmKey))
                return "";
            return _SolutionList[AlarmKey];
        }




        public static string GetNameButton(int AlarmKey)
        {
            if (!_NameButton.ContainsKey(AlarmKey))
                return "";
            return _NameButton[AlarmKey];
        }

        public static string GetDeviceButton(int AlarmKey)
        {
            if (!_DeviceButton.ContainsKey(AlarmKey))
                return "";
            return _DeviceButton[AlarmKey];
        }

        public static string GetDeviceCodeButton(int AlarmKey)
        {
            if (!_DeviceCodeButton.ContainsKey(AlarmKey))
                return "";
            return _DeviceCodeButton[AlarmKey];
        }



        public static string GetDeviceLampButton(int AlarmKey)
        {
            if (!_DeviceButton.ContainsKey(AlarmKey))
                return "";
            return _DeviceLampButton[AlarmKey];
        }

        public static string GetDeviceCodeLampButton(int AlarmKey)
        {
            if (!_DeviceCodeButton.ContainsKey(AlarmKey))
                return "";
            return _DeviceCodeLampButton[AlarmKey];
        }


    }
}
