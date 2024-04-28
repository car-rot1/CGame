using System.IO;
using Excel;
using OfficeOpenXml;
using UnityEditor;

namespace CGame
{
    public static class ExcelUtility
    {
        public static object[,] ReadExcel(string path, int tableIndex = 0)
        {
            var stream = File.Open(path, FileMode.Open, FileAccess.Read);
            var excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            var dataSet = excelReader.AsDataSet();
            var table = dataSet.Tables[tableIndex];

            var rowCount = table.Rows.Count;
            var columnCount = table.Columns.Count;
            var result = new object[rowCount, columnCount];

            for (var i = 0; i < rowCount; i++)
            {
                for (var j = 0; j < columnCount; j++)
                {
                    result[i, j] = table.Rows[i][j];
                }
            }

            return result;
        }

        public static void WriteExcel(string path, string[,] value, string sheetName = "sheet1")
        {
            var fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
                fileInfo = new FileInfo(path);
            }
            using var package = new ExcelPackage(fileInfo);
            using var sheet = package.Workbook.Worksheets.Add(sheetName);
            
            for (int i = 0, length0 = value.GetLength(0); i < length0; i++)
            {
                for (int j = 0, length1 = value.GetLength(1); j < length1; j++)
                {
                    sheet.SetValue(i + 1, j + 1, value[i, j]);
                }
            }
            package.Save();
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
    }
}
