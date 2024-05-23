using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace CGame.Editor
{
    public static class ButtonMethodDrawer
    {
        private const BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        public static float DrawGUI(Rect rect, object value, Type valueType, IList<bool> foldouts)
        {
            var height = 0f;
            foreach (var methodInfo in valueType.GetMethods(All))
            {
                var cButtonAttribute = methodInfo.GetCustomAttribute<CButtonAttribute>();
                if (cButtonAttribute != null)
                {
                    var parameters = new List<object>();

                    var methodInfoParameters = methodInfo.GetParameters();
                    for (var i = 0; i < methodInfoParameters.Length; i++)
                    {
                        var index = i;
                        var parameterInfo = methodInfoParameters[index];
                        
                        parameters.Add(parameterInfo.ParameterType.IsValueType ? Activator.CreateInstance(parameterInfo.ParameterType) : null);

                        height += DrawIMGUIField(rect, () => parameters[index], obj => parameters[index] = obj, parameterInfo.ParameterType, parameterInfo.Name, foldouts, 0);
                        
                        rect.y = height;
                    }

                    if (GUI.Button(rect, string.IsNullOrEmpty(cButtonAttribute.name) ? methodInfo.Name : cButtonAttribute.name))
                    {
                        methodInfo.Invoke(value, parameters.Count > 0 ? parameters.ToArray() : null);
                    }
                    height += EditorGUIExtension.ControlVerticalSpacing + 18;
                }
            }
            return height;
        }
        
        private static float DrawIMGUIField(Rect rect, Func<object> valueGetter, Action<object> valueSetter, Type valueType, string label, IList<bool> foldouts, int foldoutIndex)
        {
            var result = 0f;
            var value = valueGetter.Invoke();
            if (value is int intValue)
            {
                valueSetter.Invoke(EditorGUI.IntField(rect, label, intValue));
                return rect.height;
            }
            if (value is float floatValue)
            {
                valueSetter.Invoke(EditorGUI.FloatField(rect, label, floatValue));
                return rect.height;
            }
            if (value is double doubleValue)
            {
                valueSetter.Invoke(EditorGUI.DoubleField(rect, label, doubleValue));
                return rect.height;
            }
            if (valueType == typeof(string))
            {
                valueSetter.Invoke(EditorGUI.TextField(rect, label, (string)value));
                return rect.height;
            }
            if (valueType.IsEnum)
            {
                valueSetter.Invoke(EditorGUI.EnumPopup(rect, label, (Enum)value));
                return rect.height;
            }
            if (value is Vector2 vector2Value)
            {
                valueSetter.Invoke(EditorGUI.Vector2Field(rect, label, vector2Value));
                return rect.height;
            }
            if (value is Vector2Int vector2IntValue)
            {
                valueSetter.Invoke(EditorGUI.Vector2IntField(rect, label, vector2IntValue));
                return rect.height;
            }
            if (value is Vector3 vector3Value)
            {
                valueSetter.Invoke(EditorGUI.Vector3Field(rect, label, vector3Value));
                return rect.height;
            }
            if (value is Vector3Int vector3IntValue)
            {
                valueSetter.Invoke(EditorGUI.Vector3IntField(rect, label, vector3IntValue));
                return rect.height;
            }
            if (value is Vector4 vector4Value)
            {
                valueSetter.Invoke(EditorGUI.Vector4Field(rect, label, vector4Value));
                return rect.height;
            }
            if (value is Rect rectValue)
            {
                valueSetter.Invoke(EditorGUI.RectField(rect, label, rectValue));
                return rect.height;
            }
            if (value is RectInt rectIntValue)
            {
                valueSetter.Invoke(EditorGUI.RectIntField(rect, label, rectIntValue));
                return rect.height;
            }
            if (valueType.IsSubclassOf(typeof(Object)))
            {
                valueSetter.Invoke(EditorGUI.ObjectField(rect, label, (Object)value, valueType, true));
                return rect.height;
            }

            if (foldoutIndex >= foldouts.Count)
                foldouts.Add(false);
            foldouts[foldoutIndex] = EditorGUI.Foldout(rect, foldouts[foldoutIndex], label);
            result += rect.height;
            if (foldouts[foldoutIndex])
            {
                foldoutIndex++;
                rect.xMin += EditorGUIExtension.IndentPerLevel;
                EditorGUIUtility.labelWidth -= EditorGUIExtension.IndentPerLevel;
                var height = rect.height;
                foreach (var fieldInfo in valueType.GetFields(All))
                {
                    rect.y += height + EditorGUIExtension.ControlVerticalSpacing;
                    if (value == null)
                    {
                        valueSetter.Invoke(FormatterServices.GetUninitializedObject(valueType));
                        value = valueGetter.Invoke();
                    }

                    var newValue = value;
                    result += EditorGUIExtension.ControlVerticalSpacing;
                    result += DrawIMGUIField(rect, () => fieldInfo.GetValue(newValue), obj => fieldInfo.SetValue(newValue, obj), fieldInfo.FieldType, fieldInfo.Name, foldouts, foldoutIndex);
                }
            }

            return result;
        }
        
        public static VisualElement DrawElement(object value, Type valueType, Vector2 buttonSize = default)
        {
            var element = new VisualElement();
            foreach (var methodInfo in valueType.GetMethods(All))
            {
                var cButtonAttribute = methodInfo.GetCustomAttribute<CButtonAttribute>();
                if (cButtonAttribute != null)
                {
                    var box = new Box
                    {
                        style = { marginTop = 1f, marginBottom = 1f }
                    };
                    
                    var parameters = new List<object>();

                    var methodInfoParameters = methodInfo.GetParameters();
                    for (var i = 0; i < methodInfoParameters.Length; i++)
                    {
                        var index = i;
                        var parameterInfo = methodInfoParameters[index];

                        parameters.Add(parameterInfo.ParameterType.IsValueType ? Activator.CreateInstance(parameterInfo.ParameterType) : null);

                        var fieldElement = DrawUIElementField(() => parameters[index], obj => parameters[index] = obj, parameterInfo.ParameterType, parameterInfo.Name);
                        
                        box.Add(fieldElement);
                    }

                    var button = new Button(() =>
                    {
                        methodInfo.Invoke(value, parameters.Count > 0 ? parameters.ToArray() : null);
                        InternalEditorUtility.RepaintAllViews();
                    })
                    {
                        focusable = false,
                        text = string.IsNullOrEmpty(cButtonAttribute.name) ? methodInfo.Name : cButtonAttribute.name
                    };
                    
                    if (buttonSize != Vector2.zero)
                    {
                        button.style.width = buttonSize.x;
                        button.style.height = buttonSize.y;
                    }
                    box.Add(button);
                    element.Add(box);
                }
            }

            return element;
        }
        
        private static VisualElement DrawUIElementField(Func<object> valueGetter, Action<object> valueSetter, Type valueType, string label)
        {
            var value = valueGetter.Invoke();
            if (value is int intValue)
            {
                var fieldElement = new IntegerField { label = label, value = intValue };
                fieldElement.RegisterValueChangedCallback(changeEvent => { valueSetter.Invoke(changeEvent.newValue); });
                return fieldElement;
            }
            if (value is float floatValue)
            {
                var fieldElement = new FloatField { label = label, value = floatValue };
                fieldElement.RegisterValueChangedCallback(changeEvent => { valueSetter.Invoke(changeEvent.newValue); });
                return fieldElement;
            }
            if (value is double doubleValue)
            {
                var fieldElement = new DoubleField { label = label, value = doubleValue };
                fieldElement.RegisterValueChangedCallback(changeEvent => { valueSetter.Invoke(changeEvent.newValue); });
                return fieldElement;
            }
            if (valueType == typeof(string))
            {
                var fieldElement = new TextField { label = label, value = (string)value };
                fieldElement.RegisterValueChangedCallback(changeEvent => { valueSetter.Invoke(changeEvent.newValue); });
                return fieldElement;
            }
            if (valueType.IsEnum)
            {
                var fieldElement = new EnumField { label = label, value = (Enum)value };
                fieldElement.RegisterValueChangedCallback(changeEvent => { valueSetter.Invoke(changeEvent.newValue); });
                return fieldElement;
            }
            if (valueType == typeof(Vector2))
            {
                var fieldElement = new Vector2Field { label = label, value = (Vector2)value };
                fieldElement.RegisterValueChangedCallback(changeEvent => { valueSetter.Invoke(changeEvent.newValue); });
                return fieldElement;
            }
            if (valueType == typeof(Vector2Int))
            {
                var fieldElement = new Vector2IntField { label = label, value = (Vector2Int)value };
                fieldElement.RegisterValueChangedCallback(changeEvent => { valueSetter.Invoke(changeEvent.newValue); });
                return fieldElement;
            }
            if (valueType == typeof(Vector3))
            {
                var fieldElement = new Vector3Field { label = label, value = (Vector3)value };
                fieldElement.RegisterValueChangedCallback(changeEvent => { valueSetter.Invoke(changeEvent.newValue); });
                return fieldElement;
            }
            if (valueType == typeof(Vector3Int))
            {
                var fieldElement = new Vector3IntField { label = label, value = (Vector3Int)value };
                fieldElement.RegisterValueChangedCallback(changeEvent => { valueSetter.Invoke(changeEvent.newValue); });
                return fieldElement;
            }
            if (valueType == typeof(Vector4))
            {
                var fieldElement = new Vector4Field { label = label, value = (Vector4)value };
                fieldElement.RegisterValueChangedCallback(changeEvent => { valueSetter.Invoke(changeEvent.newValue); });
                return fieldElement;
            }
            if (valueType == typeof(Rect))
            {
                var fieldElement = new RectField { label = label, value = (Rect)value };
                fieldElement.RegisterValueChangedCallback(changeEvent => { valueSetter.Invoke(changeEvent.newValue); });
                return fieldElement;
            }
            if (valueType == typeof(RectInt))
            {
                var fieldElement = new RectIntField { label = label, value = (RectInt)value };
                fieldElement.RegisterValueChangedCallback(changeEvent => { valueSetter.Invoke(changeEvent.newValue); });
                return fieldElement;
            }
            if (valueType.IsSubclassOf(typeof(Object)))
            {
                var fieldElement = new ObjectField { label = label, objectType = valueType, value = (Object)value };
                fieldElement.RegisterValueChangedCallback(changeEvent => { valueSetter.Invoke(changeEvent.newValue); });
                return fieldElement;
            }

            var parentElement = new Foldout { text = label, value = false };
            parentElement.RegisterValueChangedCallback(callback =>
            {
                if (callback.target != parentElement)
                    return;
                
                if (callback.newValue)
                {
                    foreach (var fieldInfo in valueType.GetFields(All))
                    {
                        if (value == null)
                        {
                            valueSetter.Invoke(FormatterServices.GetUninitializedObject(valueType));
                            value = valueGetter.Invoke();
                        }

                        var newValue = value;
                        parentElement.Add(DrawUIElementField(() => fieldInfo.GetValue(newValue), obj => fieldInfo.SetValue(newValue, obj), fieldInfo.FieldType, fieldInfo.Name));
                    }
                }
                else
                {
                    parentElement.Clear();
                }
            });
            
            return parentElement;
        }
    }
}