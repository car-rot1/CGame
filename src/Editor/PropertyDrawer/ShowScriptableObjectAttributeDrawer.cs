using UnityEditor;
using UnityEngine;

namespace CGame.Editor
{
    [CustomPropertyDrawer(typeof(ShowScriptableObjectAttribute))]
    public class ShowScriptableObjectAttributeDrawer : PropertyDrawer
    {
        private float _height;
        private bool _foldout;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _height = 0;
            
            var rect = position;
            rect.height = 18;
            EditorGUI.PropertyField(rect, property, label);
                
            if (property.objectReferenceValue == null || !property.objectReferenceValue.GetType().IsSubclassOf(typeof(ScriptableObject)))
                return;

            var obj = new SerializedObject(property.objectReferenceValue);
            var iterator = obj.GetIterator();
            
            var isExpanded = property.isExpanded;
            var style = DragAndDrop.activeControlID == -10 ? EditorStyles.foldoutPreDrop : EditorStyles.foldout;
            var expanded = EditorGUI.Foldout(rect, isExpanded, "", true, style);
            if (isExpanded != expanded)
            {
                property.isExpanded = expanded;
                if (Event.current.alt)
                    EditorGUIExtension.SetExpandedRecurse(iterator, expanded);
            }
            
            if (!expanded)
                return;
            
            rect.y += EditorGUIExtension.ControlVerticalSpacing + 18;
            _height += EditorGUIExtension.ControlVerticalSpacing + 18;
            
            EditorGUI.BeginChangeCheck();
            obj.UpdateIfRequiredOrScript();

            rect.xMin += EditorGUIExtension.IndentPerLevel;
            EditorGUIUtility.labelWidth -= EditorGUIExtension.IndentPerLevel;

            for (var enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                {
                    var h = ScriptAttributeUtilityExtension.GetHandler(iterator).GetHeight(iterator, null, true);
                    rect.height = h;
                    EditorGUI.PropertyField(rect, iterator, true);
                    rect.y += h + EditorGUIExtension.ControlVerticalSpacing;
                    _height += h + EditorGUIExtension.ControlVerticalSpacing;
                }
            }
            obj.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + _height;
        }
    }
}