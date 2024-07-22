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
    
    [Serializable]
    public class LanguageStringSO : ScriptableObject
    {
        public List<LanguageTextInfo> languageTextInfos = new();
    }
}