using UnityEditor;
using UnityEngine;

namespace CGame.Editor
{
    [CustomPropertyDrawer(typeof(RequiredInterfaceMonoAttribute))]
    public class RequiredInterfaceMonoAttributeDrawer : PropertyDrawer
    {
        private RequiredInterfaceMonoAttribute Attribute => (RequiredInterfaceMonoAttribute)attribute;

        private bool _isError;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyRect = position;
            propertyRect.height = 18;
            if (EditorGUI.PropertyField(propertyRect, property, label))
                property.serializedObject.ApplyModifiedProperties();

            var obj = property.objectReferenceValue;
            if (obj == default || obj.Equals(default))
                return;
            
            _isError = !Attribute.interfaceType.IsInstanceOfType(obj);

            if (_isError)
            {
                var errorRect = position;
                // errorRect.xMin += EditorGUIUtility.labelWidth + 2;
                errorRect.y += 18 + EditorGUIExtension.ControlVerticalSpacing;
                errorRect.height = 18;
                EditorGUI.HelpBox(errorRect, $"{obj.GetType().Name}类并未实现{Attribute.interfaceType.Name}接口", MessageType.Error);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = base.GetPropertyHeight(property, label);
            if (_isError)
                height += EditorGUIExtension.ControlVerticalSpacing + 18;
            return height;
        }
    }
}