using System;
using System.Collections.Generic;
using UnityEngine;

namespace CGame.Localization
{
    [Serializable]
    public class LocalizationStringInternalLoader
    {
        [SerializeField] private string internalPath = "Language/String";
        public string InternalPath => internalPath;

        [SerializeField] private InternalLoadType internalLoadType;
        public InternalLoadType InternalLoadType => internalLoadType;
        
        protected Dictionary<string, string> AllResource { get; private set; } = new();
        public string GetValue(string id) => AllResource.TryGetValue(id, out var value) ? value : id;
        
        public void RefreshAllResource(string language)
        {
            AllResource.Clear();

            switch (internalLoadType)
            {
                case InternalLoadType.Resource:
                {
                    var path = internalPath + '/' + language;
                    var textSo = Resources.Load<LocalizationStringSO>(path);
                    if (textSo == null)
                        break;
                    foreach (var languageTextInfo in textSo.localizationTextInfos)
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
    }
}