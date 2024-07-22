using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CGame.Localization
{
    [Serializable]
    public struct LanguageAssetInfo
    {
        public string id;
        public Object asset;
    }
    
    [Serializable]
    public class LanguageAssetSO : ScriptableObject
    {
        public List<LanguageAssetInfo> languageAssetInfos = new();
    }
}