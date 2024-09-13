using System;
using System.Collections.Generic;
using UnityEngine;

namespace CGame.Localization
{
    [Serializable]
    public struct LocalizationTextInfo
    {
        public string id;
        public string text;
    }
    
    [CreateAssetMenu(menuName = "Localization/ScriptableObject/" + nameof(LocalizationStringSO), fileName = nameof(LocalizationStringSO))]
    [Serializable]
    public class LocalizationStringSO : ScriptableObject
    {
        public List<LocalizationTextInfo> localizationTextInfos = new();
    }
}