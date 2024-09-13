using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CGame.Localization
{
    [Serializable]
    public struct LocalizationAssetInfo
    {
        public string id;
        public Object asset;
    }
    
    [CreateAssetMenu(menuName = "Localization/ScriptableObject/" + nameof(LocalizationAssetSO), fileName = nameof(LocalizationAssetSO))]
    [Serializable]
    public class LocalizationAssetSO : ScriptableObject
    {
        public List<LocalizationAssetInfo> localizationAssetInfos = new();
    }
}