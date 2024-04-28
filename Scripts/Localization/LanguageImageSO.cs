using System;
using System.Collections.Generic;
using UnityEngine;

namespace CGame.Localization
{
    [Serializable]
    public struct LanguageImageInfo
    {
        public string id;
        public Sprite sprite;
    }
    
    public class LanguageImageSO : ScriptableObject
    {
        public List<LanguageImageInfo> languageImageInfos = new();
    }
}