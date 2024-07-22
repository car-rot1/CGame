using UnityEngine;
using Object = UnityEngine.Object;

namespace CGame.Localization
{
    public abstract class LocalizationAssetBase<TAsset, TLoader> : MonoBehaviour where TAsset : Object where TLoader : LocalizationAssetExternalLoaderBase
    {
        public string id;
        private TAsset asset;
        private TAsset Asset
        {
            get => asset;
            set
            {
                if (asset == value)
                    return;
                asset = value;
                UpdateAsset(Asset);
            }
        }
        protected LocalizationSystem localizationSystem;

        private void Awake()
        {
            Init();
        }

        protected virtual void Init()
        {
            localizationSystem = LocalizationSystem.Instance;
            LanguageChange(localizationSystem.Language);
            localizationSystem.OnLanguageChange += LanguageChange;
        }
        
        private void LanguageChange(string language)
        {
            Asset = localizationSystem.GetAsset<TAsset>(typeof(TLoader).Name, id);
        }

        private void OnDestroy()
        {
            DeInit();
        }
        
        protected virtual void DeInit()
        {
            localizationSystem.OnLanguageChange -= LanguageChange;
        }

        protected abstract void UpdateAsset(TAsset asset);
    }
}