using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CGame.Localization.Editor
{
    [InitializeOnLoad]
    public static class LocalizationEditorUtility
    {
        private static string _currentLanguage;
        private static readonly LocalizationSystem _localizationSystem;
        
        static LocalizationEditorUtility()
        {
            _localizationSystem = LocalizationSystem.Instance;
            EditorApplication.playModeStateChanged += PlayModeStateChange;
        }

        private static void PlayModeStateChange(PlayModeStateChange state)
        {
            switch (state)
            {
                case UnityEditor.PlayModeStateChange.EnteredPlayMode:
                    _currentLanguage = _localizationSystem.Language;
                    break;
                case UnityEditor.PlayModeStateChange.ExitingPlayMode:
                    _localizationSystem.Language = _currentLanguage;
                    break;
            }
        }
        
        public static List<MonoBehaviour> GetCurrentAllText()
        {
            var result = new List<MonoBehaviour>();
            result.AddRange(StageUtility.GetCurrentStageHandle().FindComponentsOfType<Text>());
#if UNITY_TEXTMESHPRO
            result.AddRange(StageUtility.GetCurrentStageHandle().FindComponentsOfType<TextMeshPro>());
            result.AddRange(StageUtility.GetCurrentStageHandle().FindComponentsOfType<TextMeshProUGUI>());
#endif
            return result;
        }
        
        public static List<MonoBehaviour> GetCurrentAllImage()
        {
            var result = new List<MonoBehaviour>();
            result.AddRange(StageUtility.GetCurrentStageHandle().FindComponentsOfType<Image>());
            result.AddRange(StageUtility.GetCurrentStageHandle().FindComponentsOfType<RawImage>());
            return result;
        }
    }
}