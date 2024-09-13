using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CGame.Editor
{
    [CustomPropertyDrawer(typeof(NewValueAttribute))]
    public class NewValueAttributeDrawer : PropertyDrawer
    {
        private Type[] _allType;
        private string[] _allName;
        private Dictionary<string, object> _allTypeValue;
        
        private bool _foldout;

        private float _height;

        private bool _init;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (_init)
                return base.CreatePropertyGUI(property);

            _init = true;
            Init();
            
            return base.CreatePropertyGUI(property);
        }

        private void Init()
        {
            _allType = TypeCache.GetTypesDerivedFrom(fieldInfo.FieldType).ToArray();
            _allName = new string[_allType.Length + 1];
            _allName[0] = "Null";
            for (var i = 1; i < _allName.Length; i++)
            {
                _allName[i] = _allType[i - 1].Name;
            }
            _allTypeValue = new Dictionary<string, object> { { "Null", null } };
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!_init)
            {
                _init = true;
                Init();
            }
            
            var rect = new Rect(position.position, new Vector2(position.width, 18));

            var horizontalRect = rect.HorizontalSplit(EditorGUIUtility.labelWidth + 2.0f, -1);
            var fieldValue = fieldInfo.GetValue(property.serializedObject.targetObject);
            if (fieldValue != null)
                _foldout = EditorGUI.Foldout(horizontalRect[0], _foldout, new GUIContent(label.text + $" (Type : {fieldValue.GetType().Name}) "), true);
            else
                EditorGUI.LabelField(horizontalRect[0], label);
            if (GUI.Button(horizontalRect[1], "New Value"))
            {
                var menu = new GenericMenu();
                for (var i = 0; i < _allName.Length; i++)
                {
                    var index = i;
                    var name = _allName[index];
                    menu.AddItem(new GUIContent(name), false, () =>
                    {
                        if (!_allTypeValue.TryGetValue(name, out var value))
                        {
                            value = index == 0 ? null : Activator.CreateInstance(_allType[index - 1]);
                            _allTypeValue.Add(name, value);
                        }
                        Undo.RegisterCompleteObjectUndo(property.serializedObject.targetObject, property.serializedObject.targetObject.name);
                        fieldInfo.SetValue(property.serializedObject.targetObject, value);
                    });
                }
                menu.ShowAsContext();
            }
            
            _height = 0;
            if (_foldout)
            {
                var lastHeight = 18f;
                while (property.NextVisible(true))
                {
                    var scriptAttributeUtilityType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ScriptAttributeUtility");
                    var propertyHandler = scriptAttributeUtilityType.GetMethod("GetHandler", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, new object[]
                    {
                        property
                    });
                    var height = (float)propertyHandler.GetType().GetMethod("GetHeight")!.Invoke(propertyHandler, new object[]
                    {
                        property,
                        new GUIContent(property.name),
                        false
                    });
                    
                    _height += EditorGUIExtension.ControlVerticalSpacing + height;
                    position.y += EditorGUIExtension.ControlVerticalSpacing + lastHeight;
                    var propertyRect = new Rect(position.position, new Vector2(position.width, height));
                    propertyRect.xMin += 18;

                    lastHeight = height;
                    
                    if (EditorGUI.PropertyField(propertyRect, property, new GUIContent(property.name)))
                        property.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 18 + _height;
        }
    }
}