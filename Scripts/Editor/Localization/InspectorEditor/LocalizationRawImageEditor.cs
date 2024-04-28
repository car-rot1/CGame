using UnityEditor;
using UnityEditor.UI;

namespace CGame.Localization.Editor
{
    [CustomEditor(typeof(LocalizationRawImage))]
    public class LocalizationRawImageEditor : RawImageEditor
    {
        private SerializedProperty id;
        private SerializedProperty customSprites;
        private SerializedProperty autoSetNativeSize;

        protected override void OnEnable()
        {
            base.OnEnable();
            id = serializedObject.FindProperty("id");
            customSprites = serializedObject.FindProperty("customSprites");
            autoSetNativeSize = serializedObject.FindProperty("autoSetNativeSize");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(id);
            EditorGUILayout.PropertyField(customSprites);
            EditorGUILayout.PropertyField(autoSetNativeSize);
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}