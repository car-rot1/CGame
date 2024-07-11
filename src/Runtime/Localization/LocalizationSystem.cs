using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace CGame.Localization
{
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
                RefreshAllResource(value);
                OnLanguageChange?.Invoke(_language);
            }
        }
        public event Action<string> OnLanguageChange;

        private readonly LocalizationConfig _config = LocalizationConfig.Instance;
        private readonly Dictionary<string, LocalizationAssetLoaderBase> _localizationAssetLoaders = new();

        protected override void Init()
        {
            foreach (var localizationAssetLoader in _config.localizationAssetLoaders)
                _localizationAssetLoaders.Add(localizationAssetLoader.Key, localizationAssetLoader);
            Language = _config.DefaultLanguage;
        }

        private void RefreshAllResource(string language)
        {
            _config.localizationStringLoader.RefreshAllResource(this, language);
            foreach (var localizationAssetLoader in _config.localizationAssetLoaders)
                localizationAssetLoader.RefreshAllResource(this, language);
        }

        public string GetString(string id)
        {
            return _config.localizationStringLoader.GetValue(id);
        }

        public Object GetAsset(string key, string id)
        {
            return _localizationAssetLoaders.TryGetValue(key, out var loader) ? loader.GetValue(id) : null;
        }
        
        public T GetAsset<T>(string key, string id) where T : Object
        {
            return _localizationAssetLoaders.TryGetValue(key, out var loader) ? (T)loader.GetValue(id) : null;
        }
    }
}
