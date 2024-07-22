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
        private readonly Dictionary<string, LocalizationAssetExternalLoaderBase> _assetExternalLoaders = new();

        protected override void Init()
        {
            foreach (var assetExternalLoader in _config.assetExternalLoaders)
                _assetExternalLoaders.Add(assetExternalLoader.Key, assetExternalLoader);
            Language = _config.DefaultLanguage;
        }

        private void RefreshAllResource(string language)
        {
            _config.stringInternalLoader.RefreshAllResource(language);
            _config.assetInternalLoader.RefreshAllResource(language);
            
            _config.stringExternalLoader.RefreshAllResource(language);
            foreach (var assetExternalLoader in _config.assetExternalLoaders)
                assetExternalLoader.RefreshAllResource(language);
        }

        public string GetString(string id)
        {
            var result = _config.stringExternalLoader.GetValue(id);
            return string.IsNullOrWhiteSpace(result) ? _config.stringInternalLoader.GetValue(id) : result;
        }

        public Object GetAsset(string key, string id)
        {
            return _assetExternalLoaders.TryGetValue(key, out var externalLoader) ? externalLoader.GetValue(id) : _config.assetInternalLoader.GetValue(id);
        }
        
        public T GetAsset<T>(string key, string id) where T : Object
        {
            var asset = GetAsset(key, id);
            if (asset == null)
                return null;
            return (T)asset;
        }
    }
}
