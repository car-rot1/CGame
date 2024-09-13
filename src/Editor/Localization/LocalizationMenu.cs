using System.Collections.Generic;
using System.IO;
using System.Linq;
using CGame.Editor;
#if UNITY_TEXTMESHPRO
using TMPro;
#endif
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace CGame.Localization.Editor
{
    public static class LocalizationMenu
    {
        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is LocalizationStringSO languageStringSo)
            {
                LanguageTextSOEditorWindow.Open(languageStringSo);
                return true;
            }
            
            if (EditorUtility.InstanceIDToObject(instanceID) is LocalizationAssetSO languageAssetSo)
            {
                LanguageAssetSOEditorWindow.Open(languageAssetSo);
                return true;
            }

            return false;
        }
        
        [MenuItem("Localization/Text/Generate LanguageTextSO", false, 0)]
        public static void GenerateLanguageTextSo()
        {
            GenerateLanguageTextSOEditorWindow.OpenWindow();
        }

        [MenuItem("Localization/Text/Replace Component", false, 1)]
        public static void ReplaceTextComponent()
        {
            ReplaceTextComponentEditorWindow.OpenWindow();
        }

        [MenuItem("CONTEXT/Text/Replace LocalizationText")]
        public static void ReplaceLocalizationText(MenuCommand command)
        {
            ComponentUtility.ReplaceComponent<Text, LocalizationText>((Text)command.context);
        }
        
#if UNITY_TEXTMESHPRO
        [MenuItem("CONTEXT/TextMeshPro/Replace LocalizationTextMeshPro")]
        public static void ReplaceLocalizationTextMeshPro(MenuCommand command)
        {
            ComponentUtility.ReplaceComponent<TextMeshPro, LocalizationTextMeshPro>((TextMeshPro)command.context);
            Debug.Log("替换成功，运行游戏后生效");
        }
        
        [MenuItem("CONTEXT/TextMeshProUGUI/Replace LocalizationTextMeshProUGUI")]
        public static void ReplaceLocalizationTextMeshProUGUI(MenuCommand command)
        {
            ComponentUtility.ReplaceComponent<TextMeshProUGUI, LocalizationTextMeshProUGUI>((TextMeshProUGUI)command.context);
            Debug.Log("替换成功，运行游戏后生效");
        }
#endif

        [MenuItem("GameObject/UI/Legacy/Localization Text", false, 2065)]
        public static void CreateLocalizationText(MenuCommand command)
        {
            if (command.context == null)
            {
                var gameObjects = Selection.gameObjects
                    .Where(go => go.GetComponentInParent<Canvas>() != null)
                    .ToList();

                if (gameObjects.Count <= 0)
                {
                    var localText = CreateComponent<LocalizationText>("LocalText (Legacy)", GetCanvas().gameObject);
                    var transform = localText.GetComponent<RectTransform>();
                    transform.sizeDelta = new Vector2(160, 30);
                    localText.text = "New Text";
                    return;
                }
                
                foreach (var gameObject in gameObjects)
                {
                    var localText = CreateComponent<LocalizationText>("LocalText (Legacy)", gameObject);
                    var transform = localText.GetComponent<RectTransform>();
                    transform.sizeDelta = new Vector2(160, 30);
                    localText.text = "New Text";
                }
            }
            else
            {
                var localText = CreateComponent<LocalizationText>("LocalText (Legacy)", (GameObject)command.context);
                var transform = localText.GetComponent<RectTransform>();
                    transform.sizeDelta = new Vector2(160, 30);
                localText.text = "New Text";
            }
        }
        
#if UNITY_TEXTMESHPRO
        [MenuItem("GameObject/3D Object/Localization TextMeshPro", false, 29)]
        public static void CreateLocalizationTextMeshPro(MenuCommand command)
        {
            if (command.context == null)
            {
                var gameObjects = Selection.gameObjects.ToList();

                if (gameObjects.Count <= 0)
                {
                    var localText = CreateComponent<LocalizationTextMeshPro>("LocalText (TMP)");
                    var transform = localText.GetComponent<RectTransform>();
                    transform.sizeDelta = new Vector2(20, 5);
                    localText.text = "Sample text";
                    return;
                }
                
                foreach (var gameObject in gameObjects)
                {
                    var localText = CreateComponent<LocalizationTextMeshPro>("LocalText (TMP)", gameObject);
                    var transform = localText.GetComponent<RectTransform>();
                    transform.sizeDelta = new Vector2(20, 5);
                    localText.text = "Sample text";
                }
            }
            else
            {
                var localText = CreateComponent<LocalizationTextMeshPro>("LocalText (TMP)", (GameObject)command.context);
                var transform = localText.GetComponent<RectTransform>();
                transform.sizeDelta = new Vector2(20, 5);
                localText.text = "Sample text";
            }
        }
        
        [MenuItem("GameObject/UI/Localization TextMeshPro", false, 1901)]
        public static void CreateLocalizationTextMeshProUGUI(MenuCommand command)
        {
            if (command.context == null)
            {
                var gameObjects = Selection.gameObjects
                    .Where(go => go.GetComponentInParent<Canvas>() != null)
                    .ToList();

                if (gameObjects.Count <= 0)
                {
                    var localText = CreateComponent<LocalizationTextMeshProUGUI>("LocalText (TMP)", GetCanvas().gameObject);
                    localText.text = "New Text";
                    return;
                }
                
                foreach (var gameObject in gameObjects)
                {
                    var localText = CreateComponent<LocalizationTextMeshProUGUI>("LocalText (TMP)", gameObject);
                    localText.text = "New Text";
                }
            }
            else
            {
                var localText = CreateComponent<LocalizationTextMeshProUGUI>("LocalText (TMP)", (GameObject)command.context);
                localText.text = "New Text";
            }
        }
#endif

        private static Canvas GetCanvas()
        {
            var canvas = StageUtility.GetCurrentStageHandle().FindComponentsOfType<Canvas>().FirstOrDefault();
            if (canvas != null)
                return canvas;
            
            EditorApplication.ExecuteMenuItem("GameObject/UI/Canvas");
            canvas = StageUtility.GetCurrentStageHandle().FindComponentsOfType<Canvas>().First();

            return canvas;
        }
        
        private static T CreateComponent<T>(string name, GameObject parent = null) where T : Component
        {
            var go = new GameObject(name);
            var component = go.AddComponent<T>();
            GameObjectUtility.SetParentAndAlign(go, parent);
            Undo.RegisterCreatedObjectUndo(go, name);
            Selection.activeObject = go;

            return component;
        }
    }
}