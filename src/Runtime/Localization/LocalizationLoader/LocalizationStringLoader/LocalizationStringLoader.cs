using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class LocalizationStringLoader
    {
        [field: SerializeField] public string InternalPath { get; private set; } = "Language/String";
        [field: SerializeField] public InternalLoadType InternalLoadType { get; private set; }
        [field: SerializeField] public string ExternalPath { get; private set; } = "../Language/Text";
        [field: SerializeField] public CsvFileInfo CsvFileInfo { get; set; } = new()
        {
            fileExtension = ".local.csv",
            ignoreHead = true,
            separator = ',',
            linefeed = '\n'
        };
        [field: SerializeField] public ExcelFileInfo ExcelFileInfo { get; set; } = new()
        {
            fileExtension = ".local.xlsx"
        };
        [field: SerializeField] public JsonFileInfo JsonFileInfo { get; set; } = new()
        {
            fileExtension = ".local.json"
        };

        protected Dictionary<string, string> AllResource { get; private set; } = new();
        public string GetValue(string id) => AllResource.TryGetValue(id, out var value) ? value : id;
        
        public void RefreshAllResource(LocalizationSystem localizationSystem, string language)
        {
            AllResource.Clear();
            var csvPath = ExternalPath + '/' + language + CsvFileInfo.fileExtension;
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
                
            var excelPath = ExternalPath + '/' + language + ExcelFileInfo.fileExtension;
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
                
            var jsonPath = ExternalPath + '/' + language + JsonFileInfo.fileExtension;
            if (File.Exists(jsonPath))
            {
                var jsonValue = NewJsonFileController.GetValue<LanguageTextJsonInfo>(jsonPath);
                foreach (var languageTextJsonInfo in jsonValue)
                {
                    AllResource[languageTextJsonInfo.id] = languageTextJsonInfo.text;
                }
                return;
            }

            switch (InternalLoadType)
            {
                case InternalLoadType.Resource:
                {
                    var path = InternalPath + '/' + language;
                    var textSO = Resources.Load<LanguageTextSO>(path);
                    if (textSO == null)
                        break;
                    foreach (var languageTextInfo in textSO.languageTextInfos)
                    {
                        AllResource[languageTextInfo.id] = languageTextInfo.text;
                    }
                    break;
                }
                case InternalLoadType.Addressable:
                    break;
                case InternalLoadType.Yooasset:
                    break;
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