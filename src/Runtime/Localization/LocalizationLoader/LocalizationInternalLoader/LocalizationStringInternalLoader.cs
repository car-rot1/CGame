using System;
using System.Collections.Generic;
using UnityEngine;

namespace CGame.Localization
{
    [Serializable]
    public class LocalizationStringInternalLoader
    {
        [field: SerializeField] public string InternalPath { get; private set; } = "Language/String";
        [field: SerializeField] public InternalLoadType InternalLoadType { get; private set; }
        
        protected Dictionary<string, string> AllResource { get; private set; } = new();
        public string GetValue(string id) => AllResource.TryGetValue(id, out var value) ? value : id;
        
        public void RefreshAllResource(string language)
        {
            AllResource.Clear();

            switch (InternalLoadType)
            {
                case InternalLoadType.Resource:
                {
                    var path = InternalPath + '/' + language;
                    var textSO = Resources.Load<LanguageStringSO>(path);
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
    }
}