using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CGame.Localization
{
    public class LocalizationConfig : ScriptableObject
    {
        [SerializeField] private List<string> languages;
        public IEnumerable<string> Languages => languages;
        [SerializeField] private string defaultLanguage;
        public string DefaultLanguage => defaultLanguage;
        
        public LocalizationStringInternalLoader stringInternalLoader;
        public LocalizationAssetInternalLoader assetInternalLoader;

        public LocalizationStringExternalLoader stringExternalLoader;
        [SerializeReference] public List<LocalizationAssetExternalLoaderBase> assetExternalLoaders;

        private static LocalizationConfig _instance;
        public static LocalizationConfig Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Resources.Load<LocalizationConfig>("Language/" + nameof(LocalizationConfig));
#if UNITY_EDITOR
                if (_instance == null)
                {
                    CreateConfig();
                    _instance = Resources.Load<LocalizationConfig>("Language/" + nameof(LocalizationConfig));
                }
#endif
                return _instance;
            }
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        [MenuItem("Localization/Create Config")]
        private static void CreateConfig()
        {
            var config = Resources.Load<LocalizationConfig>("Language/" + nameof(LocalizationConfig));
            if (config != null)
                return;

            if (!Directory.Exists(Application.dataPath + "/Resources/Language"))
                Directory.CreateDirectory(Application.dataPath + "/Resources/Language");
            
            config = CreateInstance<LocalizationConfig>();
            AssetDatabase.CreateAsset(config, "Assets/Resources/Language/LocalizationConfig.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = config;
        }
#endif
    }
}