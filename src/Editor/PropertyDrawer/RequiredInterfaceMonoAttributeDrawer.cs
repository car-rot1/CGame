using UnityEditor;
using UnityEngine;

namespace CGame.Editor
{
    [CustomPropertyDrawer(typeof(RequiredInterfaceMonoAttribute))]
    public class RequiredInterfaceMonoAttributeDrawer : PropertyDrawer
    {
        private new RequiredInterfaceMonoAttribute attribute => (RequiredInterfaceMonoAttribute)base.attribute;

        private bool _isError;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyRect = position;
            propertyRect.height = 18;
            if (EditorGUI.PropertyField(propertyRect, property, label))
                property.serializedObject.ApplyModifiedProperties();

            var obj = property.objectReferenceValue;
            if (obj != null && attribute.interfaceType.IsInstanceOfType(obj))
                _isError = false;
            else
                _isError = true;

            if (_isError)
            {
                var errorRect = position;
                errorRect.xMin += EditorGUIUtility.labelWidth;
                errorRect.y += 20;
                errorRect.height = 18;
                EditorGUI.HelpBox(errorRect, $"{obj.GetType().Name}类并未实现{attribute.interfaceType.Name}接口", MessageType.Error);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = base.GetPropertyHeight(property, label);
            if (_isError)
                height += 18 + 2;
            return height;
        }
    }
}