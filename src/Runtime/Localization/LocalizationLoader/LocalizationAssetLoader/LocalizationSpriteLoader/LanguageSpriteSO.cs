using System;
using System.Collections.Generic;
using UnityEngine;

namespace CGame.Localization
{
    [Serializable]
    public struct LanguageSpriteInfo
    {
        public string id;
        public Sprite sprite;
    }
    
    public class LanguageSpriteSO : ScriptableObject
    {
        public List<LanguageSpriteInfo> languageSpriteInfos = new();
    }
}