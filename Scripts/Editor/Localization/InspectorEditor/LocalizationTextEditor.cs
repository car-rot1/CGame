using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;

namespace CGame.Localization.Editor
{
    [CustomEditor(typeof(LocalizationText))]
    public class LocalizationTextEditor : GraphicEditor
    {
        private SerializedProperty m_Text;
        private SerializedProperty m_FontData;
        private SerializedProperty id;
        private SerializedProperty customTexts;
        private SerializedProperty customFonts;

        private Text _targetText;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Text = serializedObject.FindProperty("m_Text");
            m_FontData = serializedObject.FindProperty("m_FontData");
            id = serializedObject.FindProperty("id");
            customTexts = serializedObject.FindProperty("customTexts");
            customFonts = serializedObject.FindProperty("customFonts");

            _targetText = (Text)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_Text);

            if (!_targetText.supportRichText)
            {
                EditorGUILayout.PropertyField(id);
                EditorGUILayout.PropertyField(customTexts);
            }

            EditorGUILayout.PropertyField(customFonts);
            EditorGUILayout.PropertyField(m_FontData);

            AppearanceControlsGUI();
            RaycastControlsGUI();
            MaskableControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
