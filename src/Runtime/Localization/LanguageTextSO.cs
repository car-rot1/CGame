using System;
using System.Collections.Generic;
using UnityEngine;

namespace CGame.Localization
{
    [Serializable]
    public struct LanguageTextInfo
    {
        public string id;
        public string text;
    }
    
    public class LanguageTextSO : ScriptableObject
    {
        public List<LanguageTextInfo> languageTextInfos = new();
    }
}
