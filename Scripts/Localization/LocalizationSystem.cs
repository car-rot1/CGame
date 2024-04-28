using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CGame.Localization
{
    [Serializable]
    public struct LanguageTextJsonInfo
    {
        public string id;
        public string text;
    }
    
    public class LocalizationSystem : SingletonClass<LocalizationSystem>
    {
        private string _language;
        public string Language
        {
            get => _language;
            set
            {
                if (_language == value)
                    return;
                _language = value;
                RefreshAllText(value);
                RefreshAllImage(value);
                OnLanguageChange?.Invoke(_language);
            }
        }
        public event Action<string> OnLanguageChange;

        public readonly List<string> internalLanguages = new();
        public readonly List<string> externalLanguages = new();
        
        public void RefreshAllLanguage()
        {
            internalLanguages.Clear();
            switch (_config.InternalLoadType)
            {
                case InternalLoadType.Resource:
                    foreach (var languageTextSo in Resources.LoadAll<LanguageTextSO>(_config.LanguageTextFolderInternalPath))
                        internalLanguages.Add(Path.GetFileNameWithoutExtension(languageTextSo.name));
                    break;
                case InternalLoadType.Addressable:
                    break;
                case InternalLoadType.Yooasset:
                    break;
            }
            internalLanguages.Sort();
            
            externalLanguages.Clear();
            if (Directory.Exists(_config.LanguageTextFolderExternalPath))
            {
                foreach (var file in Directory.GetFiles(_config.LanguageTextFolderExternalPath))
                {
                    if (_config.IsCsvFile(file))
                        externalLanguages.Add('*' + _config.GetCsvFileNameWithoutExtension(file));
                    else if (_config.IsExcelFile(file))
                        externalLanguages.Add('*' + _config.GetExcelFileNameWithoutExtension(file));
                    else if (_config.IsJsonFile(file))
                        externalLanguages.Add('*' + _config.GetJsonFileNameWithoutExtension(file));
                }
            }
            externalLanguages.Sort();
        }
        
        private readonly Dictionary<string, string> _allText = new();
        private void RefreshAllText(string language)
        {
            _allText.Clear();
            if (externalLanguages.Contains(language))
            {
                language = language[1..];
                var csvPath = _config.LanguageTextFolderExternalPath + '/' + language + _config.CsvFileInfo.fileExtension;
                if (File.Exists(csvPath))
                {
                    var csvValue = CsvFileController.GetValue<(string id, string text)>(csvPath,
                        _config.CsvFileInfo.ignoreHead,
                        _config.CsvFileInfo.separator,
                        _config.CsvFileInfo.linefeed);
                    foreach (var (id, text) in csvValue)
                        _allText[id] = text;
                    return;
                }
                
                var excelPath = _config.LanguageTextFolderExternalPath + '/' + language + _config.ExcelFileInfo.fileExtension;
                if (File.Exists(excelPath))
                {
                    var excelValue = ExcelUtility.ReadExcel(excelPath);
                    for (var i = 0; i < excelValue.GetLength(0); i++)
                    {
                        if (string.IsNullOrWhiteSpace(excelValue[i, 0].ToString()))
                            continue;
                        _allText[excelValue[i, 0].ToString()] = excelValue[i, 1].ToString();
                    }
                    return;
                }
                
                var jsonPath = _config.LanguageTextFolderExternalPath + '/' + language + _config.JsonFileInfo.fileExtension;
                if (File.Exists(jsonPath))
                {
                    var jsonValue = NewJsonFileController.GetValue<LanguageTextJsonInfo>(jsonPath);
                    foreach (var languageTextJsonInfo in jsonValue)
                    {
                        _allText[languageTextJsonInfo.id] = languageTextJsonInfo.text;
                    }
                    return;
                }
            }
            else
            {
                switch (_config.InternalLoadType)
                {
                    case InternalLoadType.Resource:
                    {
                        var path = _config.LanguageTextFolderInternalPath + '/' + language;
                        var textSO = Resources.Load<LanguageTextSO>(path);
                        if (textSO == null)
                            break;
                        foreach (var languageTextInfo in textSO.languageTextInfos)
                        {
                            _allText[languageTextInfo.id] = languageTextInfo.text;
                        }
                        break;
                    }
                    case InternalLoadType.Addressable:
                        break;
                    case InternalLoadType.Yooasset:
                        break;
                }
            }
        }
        
        private readonly Dictionary<string, Sprite> _allImage = new();
        private void RefreshAllImage(string language)
        {
            _allImage.Clear();
            if (externalLanguages.Contains(language))
            {
                language = language[1..];
                var folderPath = _config.LanguageImageFolderExternalPath + '/' + language;
                if (Directory.Exists(folderPath))
                {
                    foreach (var file in Directory.GetFiles(folderPath))
                    {
                        if (!_config.IsImageFile(file))
                            continue;
                        
                        var imgByte = File.ReadAllBytes(file);
                        var texture = new Texture2D(128, 128);
                        texture.LoadImage(imgByte);
                        _allImage[Path.GetFileNameWithoutExtension(file)] = texture.ToSprite();
                    }
                }
            }
            else
            {
                switch (_config.InternalLoadType)
                {
                    case InternalLoadType.Resource:
                    {
                        var path = _config.LanguageImageFolderInternalPath + '/' + language;
                        var imageSo = Resources.Load<LanguageImageSO>(path);
                        if (imageSo == null)
                            break;
                        foreach (var languageImageInfo in imageSo.languageImageInfos)
                            _allImage[languageImageInfo.id] = languageImageInfo.sprite;
                        break;
                    }
                    case InternalLoadType.Addressable:
                        break;
                    case InternalLoadType.Yooasset:
                        break;
                }
            }
        }

        private readonly LocalizationConfig _config = LocalizationConfig.Instance;

        protected override void Init()
        {
            Language = _config.DefaultLanguage;
            Refresh();
        }

        public void Refresh()
        {
            RefreshAllLanguage();
            RefreshAllText(Language);
            RefreshAllImage(Language);
        }

        public string GetText(string id)
        {
            return _allText.TryGetValue(id, out var value) ? value : id;
        }

        public Sprite GetSprite(string id)
        {
            return _allImage.TryGetValue(id, out var sprite) ? sprite : null;
        }
        
        public Texture2D GetTexture(string id)
        {
            var sprite = GetSprite(id);
            return sprite ? sprite.GetPartTexture() : null;
        }
    }
}
