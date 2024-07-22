using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CGame.Localization
{
    [Serializable]
    public class LocalizationAssetInternalLoader
    {
        [field: SerializeField] public string InternalPath { get; private set; } = "Language/Asset";
        [field: SerializeField] public InternalLoadType InternalLoadType { get; private set; }
        
        protected Dictionary<string, Object> AllResource { get; private set; } = new();
        public Object GetValue(string id) => AllResource.TryGetValue(id, out var value) ? value : null;
        
        public void RefreshAllResource(string language)
        {
            AllResource.Clear();
            
            switch (InternalLoadType)
            {
                case InternalLoadType.Resource:
                {
                    var path = InternalPath + '/' + language;
                    var assetSo = Resources.Load<LanguageAssetSO>(path);
                    if (assetSo == null)
                        break;
                    foreach (var languageAssetInfo in assetSo.languageAssetInfos)
                    {
                        AllResource[languageAssetInfo.id] = languageAssetInfo.asset;
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
}