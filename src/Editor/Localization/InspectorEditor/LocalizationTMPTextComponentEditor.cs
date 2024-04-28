#if UNITY_TEXTMESHPRO
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace CGame.Localization.Editor
{
    [CustomEditor(typeof(LocalizationTMPTextComponent))]
    public class LocalizationTMPTextComponentEditor : UnityEditor.Editor
    {
        private SerializedProperty id;
        private SerializedProperty customTexts;
        private SerializedProperty customFonts;
        
        private TextMeshProUGUI _targetText;
        
        protected void OnEnable()
        {
            id = serializedObject.FindProperty("id");
            customTexts = serializedObject.FindProperty("customTexts");
            customFonts = serializedObject.FindProperty("customFonts");

            _targetText = (TextMeshProUGUI)target.GetType().GetProperty("TargetText")!.GetValue(target);
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(target as MonoBehaviour), typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();
            
            if (!_targetText.richText)
            {
                EditorGUILayout.PropertyField(id);
                EditorGUILayout.PropertyField(customTexts);
            }
            EditorGUILayout.PropertyField(customFonts);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif