using System;
using System.Reflection;
using UnityEditor;

namespace CGame.Editor
{
    public static class ScriptAttributeUtilityExtension
    {
        private static readonly Type ScriptAttributeUtilityType;

        static ScriptAttributeUtilityExtension()
        {
            ScriptAttributeUtilityType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ScriptAttributeUtility");
        }
        
        private static MethodInfo _getDrawerTypeForPropertyAndTypeMethodInfo;
        private static MethodInfo _getHandlerMethodInfo;
        
        public static Type GetDrawerTypeForPropertyAndType(SerializedProperty property, Type type)
        {
            if (_getDrawerTypeForPropertyAndTypeMethodInfo == null)
                _getDrawerTypeForPropertyAndTypeMethodInfo = ScriptAttributeUtilityType.GetMethod("GetDrawerTypeForPropertyAndType", BindingFlags.Static | BindingFlags.NonPublic);
            return (Type)_getDrawerTypeForPropertyAndTypeMethodInfo!.Invoke(null, new object[] { property, type });
        }

        public static PropertyHandlerExtension GetHandler(SerializedProperty property)
        {
            if (_getHandlerMethodInfo == null)
                _getHandlerMethodInfo = ScriptAttributeUtilityType.GetMethod("GetHandler", BindingFlags.Static | BindingFlags.NonPublic);
            return new PropertyHandlerExtension(_getHandlerMethodInfo!.Invoke(null, new object[] { property }));
        }
    }
}