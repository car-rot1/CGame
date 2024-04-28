using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CGame.Localization.Editor
{
    [CustomEditor(typeof(LocalizationTextComponent))]
    public class LocalizationTextComponentEditor : UnityEditor.Editor
    {
        private SerializedProperty id;
        private SerializedProperty customTexts;
        private SerializedProperty customFonts;
        
        private Text _targetText;
        
        protected void OnEnable()
        {
            id = serializedObject.FindProperty("id");
            customTexts = serializedObject.FindProperty("customTexts");
            customFonts = serializedObject.FindProperty("customFonts");

            _targetText = (Text)target.GetType().GetProperty("TargetText")!.GetValue(target);
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(target as MonoBehaviour), typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();
            
            if (!_targetText.supportRichText)
            {
                EditorGUILayout.PropertyField(id);
                EditorGUILayout.PropertyField(customTexts);
            }
            EditorGUILayout.PropertyField(customFonts);

            serializedObject.ApplyModifiedProperties();
        }
    }
}