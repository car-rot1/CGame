using System.Reflection;
using UnityEditor;

namespace CGame.Editor
{
    public class SerializedPropertyInfo
    {
        public readonly SerializedProperty serializedProperty;
        public readonly FieldInfo fieldInfo;
        // public object Value => fieldInfo.GetValue(Owner);
        // public object Owner => parent != null ? parent.Value : serializedProperty.serializedObject.targetObject;
        public object Value => fieldInfo.GetValue(owner);
        public readonly object owner;

        public SerializedPropertyInfo parent;
        
        public SerializedPropertyInfo(SerializedProperty serializedProperty, FieldInfo fieldInfo, object owner, SerializedPropertyInfo parent)
        {
            this.serializedProperty = serializedProperty;
            this.fieldInfo = fieldInfo;
            this.owner = owner;
            this.parent = parent;
        }
    }
}