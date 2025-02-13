using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;

namespace CGame.Localization.Editor
{
    [CustomEditor(typeof(LocalizationTextMeshPro))]
    public class LocalizationTextMeshProEditor : TMP_EditorPanel
    {
        private SerializedProperty id;
        private SerializedProperty customTexts;
        private SerializedProperty customFonts;

        private TextMeshPro _targetText;

        protected override void OnEnable()
        {
            base.OnEnable();
            id = serializedObject.FindProperty("id");
            customTexts = serializedObject.FindProperty("customTexts");
            customFonts = serializedObject.FindProperty("customFonts");

            _targetText = (TextMeshPro)target;
        }

        public override void OnInspectorGUI()
        {
            // Make sure Multi selection only includes TMP Text objects.
            if (IsMixSelectionTypes()) return;

            serializedObject.Update();

            DrawTextInput();

            DrawLocalization();

            DrawMainSettings();

            DrawExtraSettings();

            EditorGUILayout.Space();

            if (serializedObject.ApplyModifiedProperties() || m_HavePropertiesChanged)
            {
                m_TextComponent.havePropertiesChanged = true;
                m_HavePropertiesChanged = false;
                EditorUtility.SetDirty(target);
            }
        }

        private void DrawLocalization()
        {
            EditorGUI.BeginChangeCheck();
            if (!_targetText.richText)
            {
                EditorGUILayout.PropertyField(id);
                EditorGUILayout.PropertyField(customTexts);
            }

            EditorGUILayout.PropertyField(customFonts);
            if (EditorGUI.EndChangeCheck())
                m_HavePropertiesChanged = true;
        }
    }
}