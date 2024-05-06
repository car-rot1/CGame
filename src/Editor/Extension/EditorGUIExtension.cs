using System;
using System.Reflection;
using UnityEditor;

namespace CGame
{
    [InitializeOnLoad]
    public static class EditorGUIExtension
    {
        private static readonly Type EditorGUIType;
        
        public static readonly float ControlVerticalSpacing;
        public static readonly float VerticalSpacingMultiField;
        public static readonly float IndentPerLevel;
        
        private static readonly MethodInfo setExpandedRecurseMethodInfo;
        
        static EditorGUIExtension()
        {
            EditorGUIType = typeof(EditorGUI);
            
            var kControlVerticalSpacing = EditorGUIType
                .GetField("kControlVerticalSpacing", BindingFlags.Static | BindingFlags.NonPublic)!
                .GetValue(null);
            var valueProperty = kControlVerticalSpacing
                .GetType()
                .GetProperty("value")!;
            ControlVerticalSpacing = (float)valueProperty.GetValue(kControlVerticalSpacing);
            
            var kVerticalSpacingMultiField = EditorGUIType
                .GetField("kVerticalSpacingMultiField", BindingFlags.Static | BindingFlags.NonPublic)!
                .GetValue(null);
            VerticalSpacingMultiField = (float)valueProperty.GetValue(kVerticalSpacingMultiField);

            IndentPerLevel = (float)EditorGUIType.GetField("kIndentPerLevel", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);

            setExpandedRecurseMethodInfo = EditorGUIType.GetMethod("SetExpandedRecurse", BindingFlags.NonPublic | BindingFlags.Static)!;
        }
        
        public static void SetExpandedRecurse(SerializedProperty property, bool expanded)
        {
            setExpandedRecurseMethodInfo.Invoke(null, new object[] { property, expanded });
        }
    }
}