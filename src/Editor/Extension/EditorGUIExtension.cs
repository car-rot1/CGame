using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CGame.Editor
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
        
        public static void DrawSolidRect(Rect rect, Color color, bool usePlaymodeTint = true)
        {
            if (Event.current.type != EventType.Repaint)
                return;
            
            if (usePlaymodeTint)
                EditorGUI.DrawRect(rect, color);
            else
            {
                var currentColor = GUI.color;
                GUI.color = color;
                GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
                GUI.color = currentColor;
            }
        }
        
        public static void DrawBorders(Rect rect, float borderWidth, Color color, bool usePlaymodeTint = true) => DrawBorders(rect, borderWidth, borderWidth, borderWidth, borderWidth, color, usePlaymodeTint);
        
        public static void DrawBorders(Rect rect, float left, float right, float top, float bottom, Color color, bool usePlaymodeTint = true)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            if (left > 0)
                DrawSolidRect(new Rect(rect) { width = left }, color, usePlaymodeTint);
            if (top > 0)
                DrawSolidRect(new Rect(rect) { height = top }, color, usePlaymodeTint);
            if (right > 0)
                DrawSolidRect(new Rect(rect) { x = rect.x + rect.width - right, width = right }, color, usePlaymodeTint);
            if (bottom > 0)
                DrawSolidRect(new Rect(rect) { y = rect.y + rect.height - bottom, height = bottom }, color, usePlaymodeTint);
        }

        public static void DrawGridsForSize(Rect rect, float lineWidth, Color color, float width, float height, ToIntType toIntType = ToIntType.Floor, bool usePlaymodeTint = true)
        {
            rect.xMax -= lineWidth;
            rect.yMax -= lineWidth;

            var column = toIntType switch
            {
                ToIntType.Round => Mathf.RoundToInt(rect.width / width),
                ToIntType.Ceil => Mathf.CeilToInt(rect.width / width),
                ToIntType.Floor => Mathf.FloorToInt(rect.width / width),
                _ => 0
            };
            
            var row = toIntType switch
            {
                ToIntType.Round => Mathf.RoundToInt(rect.height / height),
                ToIntType.Ceil => Mathf.CeilToInt(rect.height / height),
                ToIntType.Floor => Mathf.FloorToInt(rect.height / height),
                _ => 0
            };

            for (var i = 0; i <= column; i++)
            {
                DrawSolidRect(new Rect(rect) { x = rect.xMin + i * width, height = row * height, width = lineWidth }, color, usePlaymodeTint);
            }
            
            for (var i = 0; i <= row; i++)
            {
                DrawSolidRect(new Rect(rect) { y = rect.yMin + i * height, height = lineWidth, width = column * width }, color, usePlaymodeTint);
            }
        }
    }
}