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
        private static LocalizationConfig _config;

        static LocalizationMenu()
        {
            _config = LocalizationConfig.Instance;
        }
        
        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is LanguageStringSO languageTextSo)
            {
                LanguageTextSOEditorWindow.Open(languageTextSo);
                return true;
            }
            
            if (EditorUtility.InstanceIDToObject(instanceID) is LanguageSpriteSO languageImageSo)
            {
                LanguageImageSOEditorWindow.Open(languageImageSo);
                return true;
            }

            return false;
        }

        [MenuItem("Assets/Create/Localization/" + nameof(LanguageStringSO), priority = 0)]
        public static void CreateLanguageTextSO()
        {
            var targets = Selection.objects;

            if (targets is not { Length: > 0 })
            {
                CreateLanguageTextSOFromSelectObject(null);
                return;
            }
            
            foreach (var target in targets)
            {
                CreateLanguageTextSOFromSelectObject(target);
            }
        }

        private static void CreateLanguageTextSOFromSelectObject(Object target)
        {
            var languageTextSo = ScriptableObject.CreateInstance<LanguageStringSO>();
            string newAssetFilePathWithName;
            
            if (target == null)
            {
                newAssetFilePathWithName = AssetDatabase.GenerateUniqueAssetPath("Assets/" + nameof(LanguageStringSO) + ".asset");
            }
            else
            {
                var path = AssetDatabase.GetAssetPath(target);
                
                if (path.GetPathState().ContainsAll(PathState.Directory | PathState.Exist))
                    newAssetFilePathWithName = AssetDatabase.GenerateUniqueAssetPath(path + '/' + nameof(LanguageStringSO) + ".asset");
                else
                {
                    if (_config.localizationStringLoader.IsCsvFile(path))
                    {
                        CsvFileController.GetValueNonAlloc(path, languageTextSo.languageTextInfos);
                        newAssetFilePathWithName = AssetDatabase.GenerateUniqueAssetPath(Path.GetDirectoryName(path) + '/' + _config.localizationStringLoader.GetCsvFileNameWithoutExtension(path) + ".asset");
                    }
                    else if (_config.localizationStringLoader.IsExcelFile(path))
                    {
                        var value = ExcelUtility.ReadExcel(path);
                        for (var i = 0; i < value.GetLength(0); i++)
                            languageTextSo.languageTextInfos.Add(new LanguageTextInfo { id = value[i, 0].ToString(), text = value[i, 1].ToString() });
                        newAssetFilePathWithName = AssetDatabase.GenerateUniqueAssetPath(Path.GetDirectoryName(path) + '/' + _config.localizationStringLoader.GetExcelFileNameWithoutExtension(path) + ".asset");
                    }
                    else if (_config.localizationStringLoader.IsJsonFile(path))
                    {
                        NewJsonFileController.GetValueNonAlloc(path, languageTextSo.languageTextInfos);
                        newAssetFilePathWithName = AssetDatabase.GenerateUniqueAssetPath(Path.GetDirectoryName(path) + '/' + _config.localizationStringLoader.GetJsonFileNameWithoutExtension(path) + ".asset");
                    }
                    else
                    {
                        newAssetFilePathWithName = AssetDatabase.GenerateUniqueAssetPath(Path.GetDirectoryName(path) + '/' + nameof(LanguageStringSO) + ".asset");
                    }
                }
            }
            Selection.activeObject = languageTextSo;
            
            AssetDatabase.CreateAsset(languageTextSo, newAssetFilePathWithName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        [MenuItem("Assets/Create/Localization/" + nameof(LanguageSpriteSO), priority = 1)]
        public static void CreateLanguageImageSO()
        {
            CreateLanguageImageSOFromSelectObject(Selection.objects);
        }

        private static void CreateLanguageImageSOFromSelectObject(Object[] targets)
        {
            var languageImageSos = new List<LanguageSpriteSO>();
            var newAssetFilePathWithNames = new List<string>();
            
            if (targets is not { Length: > 0 })
            {
                languageImageSos.Add(ScriptableObject.CreateInstance<LanguageSpriteSO>());
                newAssetFilePathWithNames.Add(AssetDatabase.GenerateUniqueAssetPath("Assets/" + nameof(LanguageSpriteSO) + ".asset"));
            }
            else
            {
                var foldTargets = targets
                    .Where(target => AssetDatabase.GetAssetPath(target).GetPathState().ContainsAll(PathState.Directory | PathState.Exist))
                    .ToList();
        
                if (foldTargets.Count <= 0)
                {
                    var path = AssetDatabase.GetAssetPath(targets[0]);
                    path = Path.GetDirectoryName(path);
                    languageImageSos.Add(ScriptableObject.CreateInstance<LanguageSpriteSO>());
                    newAssetFilePathWithNames.Add(AssetDatabase.GenerateUniqueAssetPath(path + '/' + nameof(LanguageSpriteSO) + ".asset"));
                }
                else
                {
                    foreach (var foldTarget in foldTargets)
                    {
                        var path = AssetDatabase.GetAssetPath(foldTarget);
                        languageImageSos.Add(ScriptableObject.CreateInstance<LanguageSpriteSO>());
                        newAssetFilePathWithNames.Add(AssetDatabase.GenerateUniqueAssetPath(path + '/' + nameof(LanguageSpriteSO) + ".asset"));
                    }
                }

                var spriteLoader = (LocalizationSpriteLoader)_config.localizationAssetLoaders.Find(loader => loader.Key == nameof(LocalizationSpriteLoader));
                foreach (var target in targets)
                {
                    if (!spriteLoader.IsImageFile(AssetDatabase.GetAssetPath(target)))
                        continue;
        
                    var path = AssetDatabase.GetAssetPath(target);
                    switch (target)
                    {
                        case Texture2D:
                        {
                            foreach (var languageImageSo in languageImageSos)
                                languageImageSo.languageSpriteInfos.Add(new LanguageSpriteInfo { id = spriteLoader.GetImageFileNameWithoutExtension(path), sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path)});
                            break;
                        }
                        case Sprite sprite:
                        {
                            foreach (var languageImageSo in languageImageSos)
                                languageImageSo.languageSpriteInfos.Add(new LanguageSpriteInfo { id = spriteLoader.GetImageFileNameWithoutExtension(path), sprite = sprite });
                            break;
                        }
                    }
                }
            }
            
            Selection.activeObject = languageImageSos[0];
            
            for (var i = 0; i < newAssetFilePathWithNames.Count; i++)
            {
                AssetDatabase.CreateAsset(languageImageSos[i], newAssetFilePathWithNames[i]);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        [MenuItem("Localization/Text/Generate LanguageTextSO", false, 0)]
        public static void GenerateLanguageTextSO()
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
        
        [MenuItem("Localization/Image/Generate LanguageImageSO", false, 2)]
        public static void GenerateLanguageImageSO()
        {
            GenerateLanguageImageSOEditorWindow.OpenWindow();
        }
        
        [MenuItem("Localization/Image/Replace Component", false, 3)]
        public static void ReplaceImageComponent()
        {
            ReplaceImageComponentEditorWindow.OpenWindow();
        }
        
        [MenuItem("CONTEXT/Image/Replace LocalizationImage")]
        public static void ReplaceLocalizationImage(MenuCommand command)
        {
            ComponentUtility.ReplaceComponent<Image, LocalizationImage>((Image)command.context);
        }
        
        [MenuItem("CONTEXT/RawImage/Replace LocalizationRawImage")]
        public static void ReplaceLocalizationRawImage(MenuCommand command)
        {
            ComponentUtility.ReplaceComponent<RawImage, LocalizationRawImage>((RawImage)command.context);
        }
        
        [MenuItem("GameObject/UI/Localization Image", false, 1901)]
        public static void CreateLocalizationImage(MenuCommand command)
        {
            if (command.context == null)
            {
                var gameObjects = Selection.gameObjects
                    .Where(go => go.GetComponentInParent<Canvas>() != null)
                    .ToList();

                if (gameObjects.Count <= 0)
                {
                    CreateComponent<LocalizationImage>("LocalImage", GetCanvas().gameObject);
                    return;
                }
                
                foreach (var gameObject in gameObjects)
                {
                    CreateComponent<LocalizationImage>("LocalImage", gameObject);
                }
            }
            else
            {
                CreateComponent<LocalizationImage>("LocalImage", (GameObject)command.context);
            }
        }
        
        [MenuItem("GameObject/UI/Localization Raw Image", false, 1902)]
        public static void CreateLocalizationRawImage(MenuCommand command)
        {
            if (command.context == null)
            {
                var gameObjects = Selection.gameObjects
                    .Where(go => go.GetComponentInParent<Canvas>() != null)
                    .ToList();

                if (gameObjects.Count <= 0)
                {
                    CreateComponent<LocalizationRawImage>("LocalRawImage", GetCanvas().gameObject);
                    return;
                }
                
                foreach (var gameObject in gameObjects)
                {
                    CreateComponent<LocalizationRawImage>("LocalRawImage", gameObject);
                }
            }
            else
            {
                CreateComponent<LocalizationRawImage>("LocalRawImage", (GameObject)command.context);
            }
        }
    }
}