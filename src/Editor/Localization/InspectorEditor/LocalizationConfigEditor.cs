using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CGame.Localization.Editor
{
    [CustomEditor(typeof(LocalizationConfig))]
    public class LocalizationConfigEditor : UnityEditor.Editor
    {
        private int _defaultLanguageIndex;
        private List<string> _allLanguage;

        private IEnumerable<Type> _allLocalizationAssetExternalLoaderType;

        private SerializedProperty _languages;
        private SerializedProperty _defaultLanguage;
        private SerializedProperty _stringInternalLoader;
        private SerializedProperty _assetInternalLoader;

        private SerializedProperty _stringExternalLoader;
        private SerializedProperty _assetExternalLoaders;
        
        private void OnEnable()
        {
            _languages = serializedObject.FindProperty("languages");
            _defaultLanguage = serializedObject.FindProperty("defaultLanguage");
            _stringInternalLoader = serializedObject.FindProperty("stringInternalLoader");
            _assetInternalLoader = serializedObject.FindProperty("assetInternalLoader");

            _stringExternalLoader = serializedObject.FindProperty("stringExternalLoader");
            _assetExternalLoaders = serializedObject.FindProperty("assetExternalLoaders");
            
            int i;
            var languagesSize = _languages.arraySize;
            for (i = 0; i < languagesSize; i++)
            {
                var language = _languages.GetArrayElementAtIndex(i).stringValue;
                if (language.Equals(_defaultLanguage.stringValue))
                {
                    _defaultLanguageIndex = i;
                    break;
                }
            }
            if (i >= _languages.arraySize)
            {
                _defaultLanguageIndex = 0;
                _defaultLanguage.stringValue = languagesSize > 0 ? _languages.GetArrayElementAtIndex(0).stringValue : "";
            }

            _allLanguage = new List<string>();
            _allLocalizationAssetExternalLoaderType = TypeCache.GetTypesDerivedFrom<LocalizationAssetExternalLoaderBase>().Where(type => !type.IsAbstract);
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject(target as ScriptableObject), typeof(ScriptableObject), false);
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.PropertyField(_languages);
            
            _allLanguage.Clear();
            for (var i = 0; i < _languages.arraySize; i++)
            {
                _allLanguage.Add(_languages.GetArrayElementAtIndex(i).stringValue);
            }
            
            EditorGUI.BeginChangeCheck();
            _defaultLanguageIndex = EditorGUILayout.Popup(_defaultLanguage.displayName, _defaultLanguageIndex, _allLanguage.ToArray());
            if (EditorGUI.EndChangeCheck())
                _defaultLanguage.stringValue = _allLanguage[_defaultLanguageIndex];
            
            EditorGUILayout.PropertyField(_stringInternalLoader);
            EditorGUILayout.PropertyField(_assetInternalLoader);
            
            EditorGUILayout.PropertyField(_stringExternalLoader);
            EditorGUILayout.PropertyField(_assetExternalLoaders);
            
            if (_allLocalizationAssetExternalLoaderType.Count() == _assetExternalLoaders.arraySize)
                return;
            
            if (GUILayout.Button("AddLoader"))
            {
                var menu = new GenericMenu();
                foreach (var type in _allLocalizationAssetExternalLoaderType)
                {
                    int i;
                    for (i = 0; i < _assetExternalLoaders.arraySize; i++)
                    {
                        if (_assetExternalLoaders.GetArrayElementAtIndex(i).managedReferenceValue.GetType() == type)
                            break;
                    }
                    if (i < _assetExternalLoaders.arraySize)
                        continue;
                    menu.AddItem(new GUIContent(type.Name), false, () =>
                    {
                        var loader = Activator.CreateInstance(type);
                        var length = _assetExternalLoaders.arraySize;
                        _assetExternalLoaders.InsertArrayElementAtIndex(length);
                        _assetExternalLoaders.GetArrayElementAtIndex(length).managedReferenceValue = loader;
                        _assetExternalLoaders.serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.ShowAsContext();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}