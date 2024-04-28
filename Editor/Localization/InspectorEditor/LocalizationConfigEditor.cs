using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CGame.Localization.Editor
{
    [CustomEditor(typeof(LocalizationConfig))]
    public class LocalizationConfigEditor : UnityEditor.Editor
    {
        private string[] _allLanguage;
        private int _defaultLanguageIndex;
        
        private LocalizationConfig _target;

        private SerializedProperty _defaultLanguage;
        private SerializedProperty _internalLoadType;
        private SerializedProperty _languageTextFolderInternalPath;
        private SerializedProperty _languageImageFolderInternalPath;
        private SerializedProperty _languageTextFolderExternalPath;
        private SerializedProperty _languageImageFolderExternalPath;
        private SerializedProperty _csvFileInfo;
        private SerializedProperty _excelFileInfo;
        private SerializedProperty _jsonFileInfo;

        private void OnEnable()
        {
            _target = (LocalizationConfig)target;
            
            _allLanguage = GetAllLanguage();
            
            _defaultLanguage = serializedObject.FindProperty("defaultLanguage");
            _internalLoadType = serializedObject.FindProperty("internalLoadType");
            _languageTextFolderInternalPath = serializedObject.FindProperty("languageTextFolderInternalPath");
            _languageImageFolderInternalPath = serializedObject.FindProperty("languageImageFolderInternalPath");
            _languageTextFolderExternalPath = serializedObject.FindProperty("languageTextFolderExternalPath");
            _languageImageFolderExternalPath = serializedObject.FindProperty("languageImageFolderExternalPath");
            _csvFileInfo = serializedObject.FindProperty("csvFileInfo");
            _excelFileInfo = serializedObject.FindProperty("excelFileInfo");
            _jsonFileInfo = serializedObject.FindProperty("jsonFileInfo");
            
            int i;
            for (i = 0; i < _allLanguage.Length; i++)
            {
                var language = _allLanguage[i];
                if (language.Equals(_defaultLanguage.stringValue))
                {
                    _defaultLanguageIndex = i;
                    break;
                }
            }
            if (i >= _allLanguage.Length)
            {
                _defaultLanguageIndex = 0;
                _defaultLanguage.stringValue = _allLanguage.Length > 0 ? _allLanguage[0] : "";
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject(target as ScriptableObject), typeof(ScriptableObject), false);
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginChangeCheck();
            _defaultLanguageIndex = EditorGUILayout.Popup(_defaultLanguage.displayName, _defaultLanguageIndex, _allLanguage);
            if (EditorGUI.EndChangeCheck())
                _defaultLanguage.stringValue = _allLanguage[_defaultLanguageIndex];
            
            EditorGUILayout.PropertyField(_internalLoadType);
            EditorGUILayout.PropertyField(_languageTextFolderInternalPath);
            EditorGUILayout.PropertyField(_languageImageFolderInternalPath);
            EditorGUILayout.PropertyField(_languageTextFolderExternalPath);
            EditorGUILayout.PropertyField(_languageImageFolderExternalPath);
            EditorGUILayout.PropertyField(_csvFileInfo);
            EditorGUILayout.PropertyField(_excelFileInfo);
            EditorGUILayout.PropertyField(_jsonFileInfo);

            serializedObject.ApplyModifiedProperties();
        }

        private string[] GetAllLanguage()
        {
            switch (_target.InternalLoadType)
            {
                case InternalLoadType.Resource:
                    return Resources.LoadAll<LanguageTextSO>(_target.LanguageTextFolderInternalPath)
                        .Select(languageTextSo => Path.GetFileNameWithoutExtension(languageTextSo.name))
                        .ToArray();
                case InternalLoadType.Addressable:
                    return new string[] { };
                case InternalLoadType.Yooasset:
                    return new string[] { };
                default:
                    return new string[] { };
            }
        }
    }
}