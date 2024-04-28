using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CGame.Localization
{
    public enum InternalLoadType
    {
        Resource,
        Addressable,
        Yooasset,
    }

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
    
    public class LocalizationConfig : ScriptableObject
    {
        [SerializeField] private string defaultLanguage = "简体中文";
        public string DefaultLanguage => defaultLanguage;

        [SerializeField] private InternalLoadType internalLoadType = InternalLoadType.Resource;
        public InternalLoadType InternalLoadType => internalLoadType;

        [SerializeField] private string languageTextFolderInternalPath = "Language/Text";
        public string LanguageTextFolderInternalPath => languageTextFolderInternalPath;
        
        [SerializeField] private string languageImageFolderInternalPath = "Language/Image";
        public string LanguageImageFolderInternalPath => languageImageFolderInternalPath;
        
        [SerializeField] private string languageTextFolderExternalPath = "../Language/Text";
        public string LanguageTextFolderExternalPath => Application.dataPath + '/' + languageTextFolderExternalPath;
        
        [SerializeField] private string languageImageFolderExternalPath = "../Language/Image";
        public string LanguageImageFolderExternalPath => Application.dataPath + '/' +  languageImageFolderExternalPath;

        [SerializeField] private CsvFileInfo csvFileInfo = new()
        {
            fileExtension = ".local.csv",
            ignoreHead = true,
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

        private static LocalizationConfig _instance;
        public static LocalizationConfig Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Resources.Load<LocalizationConfig>("Language/" + nameof(LocalizationConfig));
#if UNITY_EDITOR
                if (_instance == null)
                {
                    CreateConfig();
                    _instance = Resources.Load<LocalizationConfig>("Language/" + nameof(LocalizationConfig));
                }
#endif
                return _instance;
            }
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        [MenuItem("Localization/Create Config")]
        private static void CreateConfig()
        {
            var config = Resources.Load<LocalizationConfig>("Language/" + nameof(LocalizationConfig));
            if (config != null)
                return;

            if (!Directory.Exists(Application.dataPath + "/Resources/Language"))
                Directory.CreateDirectory(Application.dataPath + "/Resources/Language");
            
            config = CreateInstance<LocalizationConfig>();
            AssetDatabase.CreateAsset(config, "Assets/Resources/Language/LocalizationConfig.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = config;
        }
#endif

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

        public string GetImageFileNameWithoutExtension(string path) => IsImageFile(path) ? Path.GetFileNameWithoutExtension(path) : null;
        public bool IsImageFile(string path) => Path.GetExtension(path) is ".jpg" or ".png" or ".bmp";
    }
}
