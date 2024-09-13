using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CGame.Localization
{
    [Serializable]
    public struct CsvFileInfo
    {
        public string fileExtension;
        public bool ignoreHead;
        public char separator;
        public char linefeed;
    }
    
    [Serializable]
    public struct ExcelFileInfo
    {
        public string fileExtension;
    }
    
    [Serializable]
    public struct JsonFileInfo
    {
        public string fileExtension;
    }
    
    [Serializable]
    public struct LanguageTextJsonInfo
    {
        public string id;
        public string text;
    }
    
    [Serializable]
    public class LocalizationStringExternalLoader
    {
        [SerializeField] private string externalPath = "../Language/Text";
        public string ExternalPath => externalPath;

        [SerializeField] private CsvFileInfo csvFileInfo = new()
        {
            fileExtension = ".local.csv",
            ignoreHead = false,
            separator = ',',
            linefeed = '\n'
        };
        public CsvFileInfo CsvFileInfo => csvFileInfo;

        [SerializeField] private ExcelFileInfo excelFileInfo = new()
        {
            fileExtension = ".local.xlsx"
        };
        public ExcelFileInfo ExcelFileInfo => excelFileInfo;

        [SerializeField] private JsonFileInfo jsonFileInfo = new()
        {
            fileExtension = ".local.json"
        };
        public JsonFileInfo JsonFileInfo => jsonFileInfo;
        
        protected Dictionary<string, string> AllResource { get; private set; } = new();
        public string GetValue(string id) => AllResource.TryGetValue(id, out var value) ? value : null;
        
        public void RefreshAllResource(string language)
        {
            AllResource.Clear();
            var csvPath = Application.dataPath + '/' + externalPath + '/' + language + CsvFileInfo.fileExtension;
            if (File.Exists(csvPath))
            {
                var csvValue = CsvFileController.GetValue<(string id, string text)>(csvPath,
                    CsvFileInfo.ignoreHead,
                    CsvFileInfo.separator,
                    CsvFileInfo.linefeed);
                foreach (var (id, text) in csvValue)
                    AllResource[id] = text;
                return;
            }
                
            var excelPath = Application.dataPath + '/' + externalPath + '/' + language + ExcelFileInfo.fileExtension;
            if (File.Exists(excelPath))
            {
                var excelValue = ExcelUtility.ReadExcel(excelPath);
                for (var i = 0; i < excelValue.GetLength(0); i++)
                {
                    if (string.IsNullOrWhiteSpace(excelValue[i, 0].ToString()))
                        continue;
                    AllResource[excelValue[i, 0].ToString()] = excelValue[i, 1].ToString();
                }
                return;
            }
                
            var jsonPath = Application.dataPath + '/' + externalPath + '/' + language + JsonFileInfo.fileExtension;
            if (File.Exists(jsonPath))
            {
                var jsonValue = NewJsonFileController.GetValue<LanguageTextJsonInfo>(jsonPath);
                foreach (var languageTextJsonInfo in jsonValue)
                {
                    AllResource[languageTextJsonInfo.id] = languageTextJsonInfo.text;
                }
                return;
            }
        }
        
        public string GetCsvFileNameWithoutExtension(string path) => GetTextFileNameWithoutExtension(path, CsvFileInfo.fileExtension);
        public bool IsCsvFile(string path) => IsTextFile(path, CsvFileInfo.fileExtension);
        
        public string GetExcelFileNameWithoutExtension(string path) => GetTextFileNameWithoutExtension(path, ExcelFileInfo.fileExtension);
        public bool IsExcelFile(string path) => IsTextFile(path, ExcelFileInfo.fileExtension);
        
        public string GetJsonFileNameWithoutExtension(string path) => GetTextFileNameWithoutExtension(path, JsonFileInfo.fileExtension);
        public bool IsJsonFile(string path) => IsTextFile(path, JsonFileInfo.fileExtension);

        private string GetTextFileNameWithoutExtension(string path, string extension) => IsTextFile(path, extension) ? Path.GetFileName(path)[..^extension.Length] : null;
        private bool IsTextFile(string path, string extension)
        {
            var fileName = Path.GetFileName(path);
            var extensionLength = extension.Length;
            return fileName.Length >= extensionLength && fileName[^extensionLength..].Equals(extension);
        }
    }
}