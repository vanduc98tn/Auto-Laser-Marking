using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    class StatusMachine
    {

        public static Dictionary<int, string> _StatusList = new Dictionary<int, string>();
      

        public static void LoadStatus()
        {
            string Alarmpath = AppDomain.CurrentDomain.BaseDirectory + Properties.Settings.Default.StatusMachine;
            if (!File.Exists(Alarmpath))
                return;

           
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(Alarmpath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; 
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++) 
                {
                    int StatusKey;
                    if (int.TryParse(worksheet.Cells[row, 1].Text, out StatusKey))
                    {
                        string StatusMessage = worksheet.Cells[row, 2].Text.Replace("\\n", Environment.NewLine);

                        _StatusList[StatusKey] = StatusMessage;

                    }
                }
            }
        }
        public static void LoadStatus(string language = "vi")
        {
            string statusPath = AppDomain.CurrentDomain.BaseDirectory + Properties.Settings.Default.StatusMachine;
            if (!File.Exists(statusPath))
                return;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(statusPath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

             
                int headerRow = 2; 
                for (int r = 1; r <= Math.Min(10, rowCount); r++)
                {
                    if (worksheet.Cells[r, 2].Text.Trim().ToLower().Contains("messenger"))
                    {
                        headerRow = r;
                        break;
                    }
                }

              
                int statusColIndex = -1;

                for (int col = 1; col <= 10; col++)
                {
                    string header = worksheet.Cells[headerRow, col].Text.Trim().ToLower();

                    if (header == $"messenger {language}" ||
                        header.Contains(language)) 
                    {
                        statusColIndex = col;
                        break;
                    }
                }

               

                
                for (int row = headerRow + 1; row <= rowCount; row++)
                {
                    if (int.TryParse(worksheet.Cells[row, 1].Text.Trim(), out int statusKey))
                    {
                        string statusMessage = worksheet.Cells[row, statusColIndex].GetValue<string>()?
                                               .Replace("\\n", Environment.NewLine).Trim() ?? "";

                        _StatusList[statusKey] = statusMessage;
                    }
                }
            }
        }

        public static string GetMes(int StatusKey)
        {
            if (!_StatusList.ContainsKey(StatusKey))
                return "";
            return _StatusList[StatusKey];
        }

      
    }
}
