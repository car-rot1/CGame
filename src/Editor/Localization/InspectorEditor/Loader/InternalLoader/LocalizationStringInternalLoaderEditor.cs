using System.IO;
using CGame.Editor;
using UnityEditor;
using UnityEngine;

namespace CGame.Localization.Editor
{
    [CustomPropertyDrawer(typeof(LocalizationStringInternalLoader))]
    public class LocalizationStringInternalLoaderEditor : PropertyDrawer
    {
        private bool _foldout;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 18;
            _foldout = EditorGUI.Foldout(position, _foldout, label, true);
            if (_foldout)
            {
                position.xMin += EditorGUIExtension.IndentPerLevel;
                EditorGUIUtility.labelWidth -= EditorGUIExtension.IndentPerLevel;
                
                position.y += position.height + EditorGUIExtension.ControlVerticalSpacing;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("internalPath"));
                
                position.y += position.height + EditorGUIExtension.ControlVerticalSpacing;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("internalLoadType"));
            
                position.y += position.height + EditorGUIExtension.ControlVerticalSpacing;
                if (GUI.Button(position, "Create Directory"))
                {
                    var stringInternalLoader = (LocalizationStringInternalLoader)fieldInfo.GetValue(property.serializedObject.targetObject);
                    
                    var path = stringInternalLoader.InternalLoadType switch
                    {
                        InternalLoadType.Resource => Application.dataPath + "/Resources/" + stringInternalLoader.InternalPath,
                        InternalLoadType.Addressable => Application.dataPath + "/" + stringInternalLoader.InternalPath,
                        InternalLoadType.Yooasset => Application.dataPath + "/" + stringInternalLoader.InternalPath,
                        _ => ""
                    };
                    if (Directory.Exists(path))
                        return;
                    
                    Directory.CreateDirectory(path);
                    AssetDatabase.Refresh();
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_foldout)
                return 18 + EditorGUIExtension.ControlVerticalSpacing + 18 + EditorGUIExtension.ControlVerticalSpacing +
                       18 + EditorGUIExtension.ControlVerticalSpacing + 18;
            return base.GetPropertyHeight(property, label);
        }
    }
}
