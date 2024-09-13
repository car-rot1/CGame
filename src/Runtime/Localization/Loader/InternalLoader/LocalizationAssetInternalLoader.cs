using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CGame.Localization
{
    [Serializable]
    public class LocalizationAssetInternalLoader
    {
        [SerializeField] private string internalPath = "Language/Asset";
        public string InternalPath => internalPath;

        [SerializeField] private InternalLoadType internalLoadType;
        public InternalLoadType InternalLoadType => internalLoadType;
        
        protected Dictionary<string, Object> AllResource { get; private set; } = new();
        public Object GetValue(string id) => AllResource.TryGetValue(id, out var value) ? value : null;
        
        public void RefreshAllResource(string language)
        {
            AllResource.Clear();
            
            switch (internalLoadType)
            {
                case InternalLoadType.Resource:
                {
                    var path = internalPath + '/' + language;
                    var assetSo = Resources.Load<LocalizationAssetSO>(path);
                    if (assetSo == null)
                        break;
                    foreach (var languageAssetInfo in assetSo.localizationAssetInfos)
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