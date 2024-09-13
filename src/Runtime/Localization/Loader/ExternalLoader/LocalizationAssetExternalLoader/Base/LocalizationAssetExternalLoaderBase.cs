using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CGame.Localization
{
    [Serializable]
    public abstract class LocalizationAssetExternalLoaderBase
    {
        [field: SerializeField] public string ExternalPath { get; protected set; }
        
        public abstract string Key { get; }
        protected Dictionary<string, Object> AllResource { get; private set; } = new();
        public Object GetValue(string id) => AllResource.TryGetValue(id, out var value) ? value : null;
        
        public void RefreshAllResource(string language)
        {
            AllResource.Clear();
            RefreshExternalResource(language);
        }

        protected abstract void RefreshExternalResource(string language);
    }
}