using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CGame
{
    public static class CsvFileController
    {
        public const string Extension = ".csv";
        
        private static bool CheckFilePath(string path)
        {
            path = Path.ChangeExtension(path, Extension);
            return !path.GetPathState().ContainsAll(PathState.Invalid, PathState.Directory);
        }
        
        public static List<T> GetValue<T>(string path, bool ignoreHead = true, char separator = ',', char linefeed = '\n')
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return null;
            }
            
            if (path.GetPathState().HasFlag(PathState.Exist))
            {
                return CsvUtility.FromCsv<T>(File.ReadAllText(path, Encoding.UTF8), ignoreHead, separator, linefeed);
            }
            
            Debug.LogError("文件不存在！");
            return null;
        }
        
        public static async Task<List<T>> GetValueAsync<T>(string path, bool ignoreHead = true, char separator = ',', char linefeed = '\n')
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return null;
            }
            
            if (path.GetPathState().HasFlag(PathState.Exist))
            {
                return CsvUtility.FromCsv<T>(await File.ReadAllTextAsync(path, Encoding.UTF8), ignoreHead, separator, linefeed);
            }
            
            Debug.LogError("文件不存在！");
            return null;
        }
        
        public static bool GetValueNonAlloc<T>(string path, List<T> values, bool ignoreHead = true, char separator = ',', char linefeed = '\n')
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return false;
            }
            
            if (path.GetPathState().HasFlag(PathState.Exist))
            {
                return CsvUtility.FromCsvNonAlloc(File.ReadAllText(path, Encoding.UTF8), values, ignoreHead, separator, linefeed);
            }
            
            Debug.LogError("文件不存在！");
            return false;
        }
        
        public static async Task<bool> GetValueNonAllocAsync<T>(string path, List<T> values, bool ignoreHead = true, char separator = ',', char linefeed = '\n')
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return false;
            }
            
            if (path.GetPathState().HasFlag(PathState.Exist))
            {
                return CsvUtility.FromCsvNonAlloc(await File.ReadAllTextAsync(path, Encoding.UTF8), values, ignoreHead, separator, linefeed);
            }
            
            Debug.LogError("文件不存在！");
            return false;
        }
        
        public static int GetValueNonAlloc<T>(string path, T[] values, bool ignoreHead = true, char separator = ',', char linefeed = '\n')
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return -1;
            }
            
            if (path.GetPathState().HasFlag(PathState.Exist))
            {
                return CsvUtility.FromCsvNonAlloc(File.ReadAllText(path, Encoding.UTF8), values, ignoreHead, separator, linefeed);
            }
            
            Debug.LogError("文件不存在！");
            return -1;
        }
        
        public static async Task<int> GetValueNonAllocAsync<T>(string path, T[] values, bool ignoreHead = true, char separator = ',', char linefeed = '\n')
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return -1;
            }
            
            if (path.GetPathState().HasFlag(PathState.Exist))
            {
                return CsvUtility.FromCsvNonAlloc(await File.ReadAllTextAsync(path, Encoding.UTF8), values, ignoreHead, separator, linefeed);
            }
            
            Debug.LogError("文件不存在！");
            return -1;
        }
        
        public static List<List<string>> GetCsvData(string path, char separator = ',', char linefeed = '\n')
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return null;
            }
            
            if (path.GetPathState().HasFlag(PathState.Exist))
            {
                return CsvUtility.CsvContentToData(File.ReadAllText(path, Encoding.UTF8), separator, linefeed);
            }
            
            Debug.LogError("文件不存在！");
            return null;
        }
        
        public static async Task<List<List<string>>> GetCsvDataAsync(string path, char separator = ',', char linefeed = '\n')
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return null;
            }
            
            if (path.GetPathState().HasFlag(PathState.Exist))
            {
                return CsvUtility.CsvContentToData(await File.ReadAllTextAsync(path, Encoding.UTF8), separator, linefeed);
            }
            
            Debug.LogError("文件不存在！");
            return null;
        }
        
        public static bool GetCsvDataNonAlloc(string path, List<List<string>> values, char separator = ',', char linefeed = '\n')
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return false;
            }
            
            if (path.GetPathState().HasFlag(PathState.Exist))
            {
                CsvUtility.CsvContentToDataNonAlloc(File.ReadAllText(path, Encoding.UTF8), values, separator, linefeed);
                return true;
            }
            
            Debug.LogError("文件不存在！");
            return false;
        }
        
        public static async Task<bool> GetCsvDataNonAllocAsync(string path, List<List<string>> values, char separator = ',', char linefeed = '\n')
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return false;
            }
            
            if (path.GetPathState().HasFlag(PathState.Exist))
            {
                CsvUtility.CsvContentToDataNonAlloc(await File.ReadAllTextAsync(path, Encoding.UTF8), values, separator, linefeed);
                return true;
            }
            
            Debug.LogError("文件不存在！");
            return false;
        }
        
        public static (int length0, int length1) GetCsvDataNonAlloc(string path, string[,] values, char separator = ',', char linefeed = '\n')
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return (-1, -1);
            }
            
            if (path.GetPathState().HasFlag(PathState.Exist))
            {
                return CsvUtility.CsvContentToDataNonAlloc(File.ReadAllText(path, Encoding.UTF8), values, separator, linefeed);
            }
            
            Debug.LogError("文件不存在！");
            return (-1, -1);
        }
        
        public static async Task<(int length0, int length1)> GetCsvDataNonAllocAsync(string path, string[,] values, char separator = ',', char linefeed = '\n')
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return (-1, -1);
            }
            
            if (path.GetPathState().HasFlag(PathState.Exist))
            {
                return CsvUtility.CsvContentToDataNonAlloc(await File.ReadAllTextAsync(path, Encoding.UTF8), values, separator, linefeed);
            }
            
            Debug.LogError("文件不存在！");
            return (-1, -1);
        }

        public static void SetValue<T>(string path, List<T> values, bool ignoreHead = true, char separator = ',', char linefeed = '\n', params string[] customHeads)
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return;
            }
            
            File.WriteAllText(path, CsvUtility.ToCsv(values, ignoreHead, separator, linefeed, customHeads), Encoding.UTF8);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
        
        public static async Task SetValueAsync<T>(string path, List<T> values, bool ignoreHead = true, char separator = ',', char linefeed = '\n', params string[] customHeads)
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return;
            }
            
            await File.WriteAllTextAsync(path, CsvUtility.ToCsv(values, ignoreHead, separator, linefeed, customHeads), Encoding.UTF8);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
        
        public static void SetValue<T>(string path, T[] values, bool ignoreHead = true, char separator = ',', char linefeed = '\n', params string[] customHeads)
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return;
            }
            
            File.WriteAllText(path, CsvUtility.ToCsv(values, ignoreHead, separator, linefeed, customHeads), Encoding.UTF8);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
        
        public static async Task SetValueAsync<T>(string path, T[] values, bool ignoreHead = true, char separator = ',', char linefeed = '\n', params string[] customHeads)
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return;
            }
            
            await File.WriteAllTextAsync(path, CsvUtility.ToCsv(values, ignoreHead, separator, linefeed, customHeads), Encoding.UTF8);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
        
        public static void SetCsvData(string path, List<List<string>> data, char separator = ',', char linefeed = '\n')
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return;
            }
            
            File.WriteAllText(path, CsvUtility.DataToCsvContent(data, separator, linefeed), Encoding.UTF8);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
        
        public static async Task SetCsvDataAsync(string path, List<List<string>> data, char separator = ',', char linefeed = '\n')
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return;
            }
            
            await File.WriteAllTextAsync(path, CsvUtility.DataToCsvContent(data, separator, linefeed), Encoding.UTF8);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
        
        public static void SetCsvData(string path, string[,] data, char separator = ',', char linefeed = '\n')
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return;
            }
            
            File.WriteAllText(path, CsvUtility.DataToCsvContent(data, separator, linefeed), Encoding.UTF8);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
        
        public static async Task SetCsvDataAsync(string path, string[,] data, char separator = ',', char linefeed = '\n')
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return;
            }
            
            await File.WriteAllTextAsync(path, CsvUtility.DataToCsvContent(data, separator, linefeed), Encoding.UTF8);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
    }
}