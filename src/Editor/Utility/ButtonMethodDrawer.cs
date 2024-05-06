using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace CGame.Editor
{
    public static class ButtonMethodDrawer
    {
        private const BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        public static void DrawGUI(Vector2 position, object value, Type valueType, Vector2 buttonSize)
        {
            var rect = new Rect(position, buttonSize);
            foreach (var methodInfo in valueType.GetMethods(All))
            {
                var cButtonAttribute = methodInfo.GetCustomAttribute<CButtonAttribute>();
                if (cButtonAttribute != null)
                {
                    var parameters = new List<object>();

                    var methodInfoParameters = methodInfo.GetParameters();
                    for (var i = 0; i < methodInfoParameters.Length; i++)
                    {
                        var parameterInfo = methodInfoParameters[i];
                        
                        parameters.Add(parameterInfo.ParameterType.IsValueType
                            ? Activator.CreateInstance(parameterInfo.ParameterType)
                            : null);
                        
                        if (parameterInfo.ParameterType == typeof(int))
                        {
                            parameters[i] = EditorGUI.IntField(rect, parameterInfo.Name, (int)parameters[i]);
                        }
                        else if (parameterInfo.ParameterType == typeof(float))
                        {
                            parameters[i] = EditorGUI.FloatField(rect, parameterInfo.Name, (float)parameters[i]);
                        }
                        else if (parameterInfo.ParameterType == typeof(double))
                        {
                            parameters[i] = EditorGUI.DoubleField(rect, parameterInfo.Name, (double)parameters[i]);
                        }
                        else if (parameterInfo.ParameterType == typeof(string))
                        {
                            parameters[i] = EditorGUI.TextField(rect, parameterInfo.Name, (string)parameters[i]);
                        }
                        else if (parameterInfo.ParameterType.IsEnum)
                        {
                            parameters[i] = EditorGUI.EnumPopup(rect, parameterInfo.Name, (Enum)parameters[i]);
                        }
                        else if (parameterInfo.ParameterType.IsSubclassOf(typeof(Object)))
                        {
                            parameters[i] = EditorGUI.ObjectField(rect, parameterInfo.Name, (Object)parameters[i], parameterInfo.ParameterType, true);
                        }
                        else
                        {
                            EditorGUI.HelpBox(rect, $"暂不支持该类型参数 : {parameterInfo.ParameterType}", MessageType.Warning);
                        }
                        
                        rect.y += buttonSize.y;
                        rect.height += buttonSize.y;
                    }

                    if (GUI.Button(rect, string.IsNullOrEmpty(cButtonAttribute.name) ? methodInfo.Name : cButtonAttribute.name))
                    {
                        methodInfo.Invoke(value, parameters.Count > 0 ? parameters.ToArray() : null);
                    }
                }
            }
        }
        
        private static Rect DrawIMGUIField(Rect rect, Func<object> valueGetter, Action<object> valueSetter, Type valueType, string label, bool foldout)
        {
            var value = valueGetter.Invoke();
            if (value is int intValue)
            {
                valueSetter.Invoke(EditorGUI.IntField(rect, label, intValue));
                return rect;
            }
            if (value is float floatValue)
            {
                valueSetter.Invoke(EditorGUI.FloatField(rect, label, floatValue));
                return rect;
            }
            if (value is double doubleValue)
            {
                valueSetter.Invoke(EditorGUI.DoubleField(rect, label, doubleValue));
                return rect;
            }
            if (valueType == typeof(string))
            {
                valueSetter.Invoke(EditorGUI.TextField(rect, label, (string)value));
                return rect;
            }
            if (valueType.IsEnum)
            {
                valueSetter.Invoke(EditorGUI.EnumPopup(rect, label, (Enum)value));
                return rect;
            }
            if (value is Vector2 vector2Value)
            {
                valueSetter.Invoke(EditorGUI.Vector2Field(rect, label, vector2Value));
                return rect;
            }
            if (value is Vector2Int vector2IntValue)
            {
                valueSetter.Invoke(EditorGUI.Vector2IntField(rect, label, vector2IntValue));
                return rect;
            }
            if (value is Vector3 vector3Value)
            {
                valueSetter.Invoke(EditorGUI.Vector3Field(rect, label, vector3Value));
                return rect;
            }
            if (value is Vector3Int vector3IntValue)
            {
                valueSetter.Invoke(EditorGUI.Vector3IntField(rect, label, vector3IntValue));
                return rect;
            }
            if (value is Vector4 vector4Value)
            {
                valueSetter.Invoke(EditorGUI.Vector4Field(rect, label, vector4Value));
                return rect;
            }
            if (value is Rect rectValue)
            {
                valueSetter.Invoke(EditorGUI.RectField(rect, label, rectValue));
                return rect;
            }
            if (value is RectInt rectIntValue)
            {
                valueSetter.Invoke(EditorGUI.RectIntField(rect, label, rectIntValue));
                return rect;
            }
            if (valueType.IsSubclassOf(typeof(Object)))
            {
                valueSetter.Invoke(EditorGUI.ObjectField(rect, label, (Object)value, valueType, true));
                return rect;
            }

            foldout = EditorGUI.Foldout(rect, foldout, label);
            if (foldout)
            {
                var height = rect.height;
                foreach (var fieldInfo in valueType.GetFields(All))
                {
                    rect.y += height + 2;
                    rect.height = height;
                    
                    if (value == null)
                    {
                        valueSetter.Invoke(FormatterServices.GetUninitializedObject(valueType));
                        value = valueGetter.Invoke();
                    }

                    var newValue = value;
                    DrawIMGUIField(rect, () => fieldInfo.GetValue(newValue), obj => fieldInfo.SetValue(newValue, obj), fieldInfo.FieldType, fieldInfo.Name, false);
                }
            }

            return rect;
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

                    var button = new Button(() => methodInfo.Invoke(value, parameters.Count > 0 ? parameters.ToArray() : null))
                    {
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