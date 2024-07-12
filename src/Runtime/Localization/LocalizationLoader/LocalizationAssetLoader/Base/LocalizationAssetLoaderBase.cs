using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CGame.Localization
{
    public enum InternalLoadType
    {
        Resource,
        Addressable,
        Yooasset,
    }
    
    [Serializable]
    public abstract class LocalizationAssetLoaderBase
    {
        [field: SerializeField] public string InternalPath { get; protected set; }
        [field: SerializeField] public InternalLoadType InternalLoadType { get; protected set; }
        [field: SerializeField] public string ExternalPath { get; protected set; }
        
        public abstract string Key { get; }
        protected Dictionary<string, Object> AllResource { get; private set; } = new();
        public Object GetValue(string id) => AllResource.TryGetValue(id, out var value) ? value : null;
        
        public void RefreshAllResource(LocalizationSystem localizationSystem, string language)
        {
            AllResource.Clear();
            if (RefreshExternalResource(localizationSystem, language))
                return;
            RefreshInternalResource(localizationSystem, language);
        }

        protected virtual bool RefreshExternalResource(LocalizationSystem localizationSystem, string language) => false;
        protected virtual bool RefreshInternalResource(LocalizationSystem localizationSystem, string language)
        {
            switch (InternalLoadType)
            {
                case InternalLoadType.Resource:
                {
                    var path = InternalPath + '/' + language;
                    var imageSo = Resources.Load<LanguageSpriteSO>(path);
                    if (imageSo == null)
                        break;
                    foreach (var languageImageInfo in imageSo.languageSpriteInfos)
                        AllResource[languageImageInfo.id] = languageImageInfo.sprite;
                    return true;
                }
                case InternalLoadType.Addressable:
                    return true;
                case InternalLoadType.Yooasset:
                    return true;
            }
            return false;
        }
    }
}