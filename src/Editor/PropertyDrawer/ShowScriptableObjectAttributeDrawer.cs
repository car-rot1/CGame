using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CGame.Editor
{
    [CustomPropertyDrawer(typeof(ShowScriptableObjectAttribute))]
    public class ShowScriptableObjectAttributeDrawer : PropertyDrawer
    {
        private float _height;

        // private bool _init;
        private MethodInfo _getHandler;
        // private readonly Dictionary<SerializedProperty, object> _propertyHandleDic = new();
        
        // public override VisualElement CreatePropertyGUI(SerializedProperty property)
        // {
        //     if (_init)
        //         return base.CreatePropertyGUI(property);
        //
        //     _init = true;
        //     Init();
        //     
        //     return base.CreatePropertyGUI(property);
        // }
        //
        // private void Init()
        // {
        //     var scriptAttributeUtilityType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ScriptAttributeUtility");
        //     _getHandler = scriptAttributeUtilityType.GetMethod("GetHandler", BindingFlags.Static | BindingFlags.NonPublic);
        //     _propertyHandleDic.Clear();
        // }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // if (!_init)
            // {
            //     _init = true;
            //     Init();
            // }

            if (_getHandler == null)
            {
                var scriptAttributeUtilityType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ScriptAttributeUtility");
                _getHandler = scriptAttributeUtilityType.GetMethod("GetHandler", BindingFlags.Static | BindingFlags.NonPublic);
            }
            
            _height = 0;
            
            var rect = position;
            rect.height = 18;
            EditorGUI.PropertyField(rect, property, label);
            if (property.objectReferenceValue == null)
                return;
            rect.yMin += 20;
            _height += 20;
            var obj = new SerializedObject(property.objectReferenceValue);
            
            EditorGUI.BeginChangeCheck();
            obj.UpdateIfRequiredOrScript();
            SerializedProperty iterator = obj.GetIterator();
            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                {
                    var handle = _getHandler!.Invoke(null, new object[] { iterator });
                    var h = (float)handle!.GetType().GetMethod("GetHeight")?.Invoke(handle, new object[]
                    {
                        iterator, null, true
                    })!;
                    rect.height = h;
                    EditorGUI.PropertyField(rect, iterator, true);
                    rect.yMin += h + 2;
                    _height += h + 2;
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