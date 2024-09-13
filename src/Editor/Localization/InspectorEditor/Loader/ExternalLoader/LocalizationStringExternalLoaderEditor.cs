using System.Collections;
using System.Collections.Generic;
using System.IO;
using CGame.Editor;
using UnityEditor;
using UnityEngine;

namespace CGame.Localization.Editor
{
    [CustomPropertyDrawer(typeof(LocalizationStringExternalLoader))]
    public class LocalizationStringExternalLoaderEditor : PropertyDrawer
    {
        private bool _foldout;
        private bool _csvFoldout;
        private bool _excelFoldout;
        private bool _jsonFoldout;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 18;
            _foldout = EditorGUI.Foldout(position, _foldout, label, true);
            if (_foldout)
            {
                position.xMin += EditorGUIExtension.IndentPerLevel;
                EditorGUIUtility.labelWidth -= EditorGUIExtension.IndentPerLevel;
                
                position.y += position.height + EditorGUIExtension.ControlVerticalSpacing;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("externalPath"));

                position.y += position.height + EditorGUIExtension.ControlVerticalSpacing;
                _csvFoldout = EditorGUI.Foldout(position, _csvFoldout, new GUIContent("Csv File Info"), true);
                if (_csvFoldout)
                {
                    var csvFileInfo = property.FindPropertyRelative("csvFileInfo");
                    position.xMin += EditorGUIExtension.IndentPerLevel;
                    EditorGUIUtility.labelWidth -= EditorGUIExtension.IndentPerLevel;
                    
                    position.y += position.height + EditorGUIExtension.ControlVerticalSpacing;
                    EditorGUI.PropertyField(position, csvFileInfo.FindPropertyRelative("fileExtension"));
                    
                    position.y += position.height + EditorGUIExtension.ControlVerticalSpacing;
                    EditorGUI.PropertyField(position, csvFileInfo.FindPropertyRelative("ignoreHead"));
                    
                    position.y += position.height + EditorGUIExtension.ControlVerticalSpacing;
                    EditorGUI.PropertyField(position, csvFileInfo.FindPropertyRelative("separator"));
                    
                    position.y += position.height + EditorGUIExtension.ControlVerticalSpacing;
                    EditorGUI.PropertyField(position, csvFileInfo.FindPropertyRelative("linefeed"));
                    
                    position.xMin -= EditorGUIExtension.IndentPerLevel;
                    EditorGUIUtility.labelWidth += EditorGUIExtension.IndentPerLevel;
                }
                
                position.y += position.height + EditorGUIExtension.ControlVerticalSpacing;
                _excelFoldout = EditorGUI.Foldout(position, _excelFoldout, new GUIContent("Excel File Info"), true);
                if (_excelFoldout)
                {
                    var csvFileInfo = property.FindPropertyRelative("csvFileInfo");
                    position.xMin += EditorGUIExtension.IndentPerLevel;
                    EditorGUIUtility.labelWidth -= EditorGUIExtension.IndentPerLevel;
                    
                    position.y += position.height + EditorGUIExtension.ControlVerticalSpacing;
                    EditorGUI.PropertyField(position, csvFileInfo.FindPropertyRelative("fileExtension"));
                    
                    position.xMin -= EditorGUIExtension.IndentPerLevel;
                    EditorGUIUtility.labelWidth += EditorGUIExtension.IndentPerLevel;
                }
                
                position.y += position.height + EditorGUIExtension.ControlVerticalSpacing;
                _jsonFoldout = EditorGUI.Foldout(position, _jsonFoldout, new GUIContent("Json File Info"), true);
                if (_jsonFoldout)
                {
                    var csvFileInfo = property.FindPropertyRelative("csvFileInfo");
                    position.xMin += EditorGUIExtension.IndentPerLevel;
                    EditorGUIUtility.labelWidth -= EditorGUIExtension.IndentPerLevel;
                    
                    position.y += position.height + EditorGUIExtension.ControlVerticalSpacing;
                    EditorGUI.PropertyField(position, csvFileInfo.FindPropertyRelative("fileExtension"));
                    
                    position.xMin -= EditorGUIExtension.IndentPerLevel;
                    EditorGUIUtility.labelWidth += EditorGUIExtension.IndentPerLevel;
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = base.GetPropertyHeight(property, label);

            if (_foldout)
                height += EditorGUIExtension.ControlVerticalSpacing + 18 + 
                          EditorGUIExtension.ControlVerticalSpacing + 18 + 
                          EditorGUIExtension.ControlVerticalSpacing + 18 + 
                          EditorGUIExtension.ControlVerticalSpacing + 18;

            if (_csvFoldout)
                height += EditorGUIExtension.ControlVerticalSpacing + 18 +
                          EditorGUIExtension.ControlVerticalSpacing + 18 +
                          EditorGUIExtension.ControlVerticalSpacing + 18 +
                          EditorGUIExtension.ControlVerticalSpacing + 18;

            if (_excelFoldout)
                height += EditorGUIExtension.ControlVerticalSpacing + 18;
            
            if (_jsonFoldout)
                height += EditorGUIExtension.ControlVerticalSpacing + 18;
            
            return height;
        }
    }
}
