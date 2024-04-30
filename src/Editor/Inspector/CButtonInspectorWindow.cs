using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace CGame.Editor
{
    [InitializeOnLoad]
    public static class CButtonInspectorWindow
    {
        private static readonly Type InspectorWindowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        private static EditorWindow _currentInspectorWindow;

        static CButtonInspectorWindow()
        {
            EditorApplication.update += OnUpdate;
            Selection.selectionChanged += SelectionChanged;
        }

        private static async void OnUpdate()
        {
            if (Selection.count > 1)
                return;
            
            if (_currentInspectorWindow == null)
            {
                var inspectorWindows = Resources.FindObjectsOfTypeAll(InspectorWindowType);
                _currentInspectorWindow = inspectorWindows.Length > 0 ? (EditorWindow)inspectorWindows[0] : null;
                
                // 这种方式有可能会重新创建新的window窗体
                // _currentInspectorWindow = EditorWindow.GetWindow(InspectorWindowType);

                // if (_currentInspectorWindow != null)
                // {
                //     var editorWindow = (EditorWindow)_currentInspectorWindow;
                //     var test = editorWindow.rootVisualElement.Q("unity-content-container");
                //
                //     // .tracker.activeEditors
                //     var track = editorWindow.GetType().GetProperty("tracker").GetValue(editorWindow) as ActiveEditorTracker;
                //     foreach (var trackActiveEditor in track.activeEditors)
                //     {
                //         Debug.Log(trackActiveEditor);
                //     }
                //     
                //     test.Insert(0, new Button
                //     {
                //         text = "哈哈哈哈哈"
                //     });
                // }

                await Task.Delay(1000);
                SelectionChanged();
            }
        }

        private static void SelectionChanged()
        {
            // if (Selection.count > 1)
            //     return;
            
            if (_currentInspectorWindow != null)
            {
                // var editorElementUpdater = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.PropertyEditor")
                //     .GetField("m_EditorElementUpdater", BindingFlags.NonPublic | BindingFlags.Instance)
                //     !.GetValue(_currentInspectorWindow);
                //
                // var editorElements = (IList)editorElementUpdater.GetType()
                //     .GetField("m_EditorElements", BindingFlags.NonPublic | BindingFlags.Instance)
                //     !.GetValue(editorElementUpdater);
                
                var className = (string)typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.PropertyEditor")
                    .GetField("s_EditorListClassName", BindingFlags.NonPublic | BindingFlags.Static)
                    !.GetValue(_currentInspectorWindow);
                var inspectorEditorsListElement = _currentInspectorWindow.rootVisualElement.Q(null, className);
                
                // for (var i = 1; i < editorElements.Count; i++)
                for (var i = 1; i < inspectorEditorsListElement.childCount; i++)
                {
                    var element = inspectorEditorsListElement[i];
                    if (element.childCount < 3)
                        continue;
                    
                    if (element[1] is not InspectorElement inspectorElement)
                        continue;
                    
                    var serializedObject = (SerializedObject)inspectorElement.GetType()
                        .GetProperty("boundObject", BindingFlags.NonPublic | BindingFlags.Instance)
                        !.GetValue(inspectorElement);
                    
                    var myElement = new VisualElement
                    {
                        name = "MyElement",
                        style =
                        {
                            marginLeft = 18,
                            marginRight = 3,
                        }
                    };
                    
                    DrawMethodButton(serializedObject.targetObject.GetType(), serializedObject.targetObject, myElement);
                    foreach (var serializedPropertyInfo in SerializedPropertyInfo.Get(serializedObject))
                    {
                        if (serializedPropertyInfo.fieldInfo != null)
                            DrawMethodButton(serializedPropertyInfo.fieldInfo.FieldType, serializedPropertyInfo.Value, myElement);
                    }
                    
                    if (myElement.childCount > 0)
                        inspectorElement.Add(myElement);
                }
            }
        }

        private static void DrawMethodButton(Type type, object value, VisualElement element)
        {
            var method = type.GetMethod("GetEnumerator", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (method != null)
            {
                var result = (IEnumerator)method.Invoke(value, new object[] { });
                while (result.MoveNext())
                {
                    if (result.Current == null)
                        continue;
                    foreach (var methodInfo in result.Current.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                                                 BindingFlags.Instance | BindingFlags.Static))
                    {
                        var cButtonAttribute = methodInfo.GetCustomAttribute<CButtonAttribute>();
                        if (cButtonAttribute != null)
                        {
                            Type unSupportType = null;

                            var parameters = new List<object>();
                            var methodInfoParameters = methodInfo.GetParameters();
                            for (var j = 0; j < methodInfoParameters.Length; j++)
                            {
                                var index = j;
                                var parameterInfo = methodInfoParameters[index];

                                if (parameterInfo.ParameterType == typeof(int))
                                {
                                    parameters.Add(0);

                                    var integerField = new IntegerField
                                    {
                                        label = parameterInfo.Name,
                                        value = 0
                                    };
                                    integerField.RegisterValueChangedCallback(changeEvent =>
                                    {
                                        parameters[index] = changeEvent.newValue;
                                    });
                                    element.Add(integerField);
                                }
                                else if (parameterInfo.ParameterType == typeof(float))
                                {
                                    parameters.Add(0f);
                                    var floatField = new FloatField()
                                    {
                                        label = parameterInfo.Name,
                                        value = 0f
                                    };
                                    floatField.RegisterValueChangedCallback(changeEvent =>
                                    {
                                        parameters[index] = changeEvent.newValue;
                                    });
                                    element.Add(floatField);
                                }
                                else if (parameterInfo.ParameterType == typeof(double))
                                {
                                    parameters.Add(0d);
                                    var doubleField = new DoubleField()
                                    {
                                        label = parameterInfo.Name,
                                        value = 0d
                                    };
                                    doubleField.RegisterValueChangedCallback(changeEvent =>
                                    {
                                        parameters[index] = changeEvent.newValue;
                                    });
                                    element.Add(doubleField);
                                }
                                else if (parameterInfo.ParameterType == typeof(string))
                                {
                                    parameters.Add("");
                                    var textField = new TextField()
                                    {
                                        label = parameterInfo.Name,
                                        value = ""
                                    };
                                    textField.RegisterValueChangedCallback(changeEvent =>
                                    {
                                        parameters[index] = changeEvent.newValue;
                                    });
                                    element.Add(textField);
                                }
                                else if (parameterInfo.ParameterType.IsEnum)
                                {
                                    parameters.Add(Enum.GetValues(parameterInfo.ParameterType).GetValue(0));
                                    var enumField =
                                        new EnumField(
                                            Enum.GetValues(parameterInfo.ParameterType).GetValue(0) as Enum)
                                        {
                                            label = parameterInfo.Name,
                                        };
                                    enumField.RegisterValueChangedCallback(changeEvent =>
                                    {
                                        parameters[index] = changeEvent.newValue;
                                    });
                                    element.Add(enumField);
                                }
                                else if (parameterInfo.ParameterType.IsSubclassOf(typeof(Object)))
                                {
                                    parameters.Add(null);
                                    var objectField = new ObjectField()
                                    {
                                        label = parameterInfo.Name,
                                        objectType = parameterInfo.ParameterType,
                                        value = null
                                    };
                                    objectField.RegisterValueChangedCallback(changeEvent =>
                                    {
                                        parameters[index] = changeEvent.newValue;
                                    });
                                    element.Add(objectField);
                                }
                                else
                                {
                                    unSupportType = parameterInfo.ParameterType;
                                    break;
                                }
                            }

                            if (unSupportType != null)
                            {
                                element.Add(new Label($"存在暂不支持的参数类型：{unSupportType.Name}"));
                            }
                            else
                            {
                                var go = result.Current;
                                element.Add(new Button(() =>
                                {
                                    methodInfo.Invoke(go, parameters.Count > 0 ? parameters.ToArray() : null);
                                })
                                {
                                    text = string.IsNullOrEmpty(cButtonAttribute.name)
                                        ? methodInfo.Name
                                        : cButtonAttribute.name
                                });
                            }
                        }
                    }
                }
                return;
            }

            foreach (var methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                var cButtonAttribute = methodInfo.GetCustomAttribute<CButtonAttribute>();
                if (cButtonAttribute != null)
                {
                    Type unSupportType = null;

                    var parameters = new List<object>();
                    var methodInfoParameters = methodInfo.GetParameters();
                    for (var j = 0; j < methodInfoParameters.Length; j++)
                    {
                        var index = j;
                        var parameterInfo = methodInfoParameters[index];

                        if (parameterInfo.ParameterType == typeof(int))
                        {
                            parameters.Add(0);

                            var integerField = new IntegerField
                            {
                                label = parameterInfo.Name,
                                value = 0
                            };
                            integerField.RegisterValueChangedCallback(changeEvent =>
                            {
                                parameters[index] = changeEvent.newValue;
                            });
                            element.Add(integerField);
                        }
                        else if (parameterInfo.ParameterType == typeof(float))
                        {
                            parameters.Add(0f);
                            var floatField = new FloatField()
                            {
                                label = parameterInfo.Name,
                                value = 0f
                            };
                            floatField.RegisterValueChangedCallback(changeEvent =>
                            {
                                parameters[index] = changeEvent.newValue;
                            });
                            element.Add(floatField);
                        }
                        else if (parameterInfo.ParameterType == typeof(double))
                        {
                            parameters.Add(0d);
                            var doubleField = new DoubleField()
                            {
                                label = parameterInfo.Name,
                                value = 0d
                            };
                            doubleField.RegisterValueChangedCallback(changeEvent =>
                            {
                                parameters[index] = changeEvent.newValue;
                            });
                            element.Add(doubleField);
                        }
                        else if (parameterInfo.ParameterType == typeof(string))
                        {
                            parameters.Add("");
                            var textField = new TextField()
                            {
                                label = parameterInfo.Name,
                                value = ""
                            };
                            textField.RegisterValueChangedCallback(changeEvent =>
                            {
                                parameters[index] = changeEvent.newValue;
                            });
                            element.Add(textField);
                        }
                        else if (parameterInfo.ParameterType.IsEnum)
                        {
                            parameters.Add(Enum.GetValues(parameterInfo.ParameterType).GetValue(0));
                            var enumField =
                                new EnumField(
                                    Enum.GetValues(parameterInfo.ParameterType).GetValue(0) as Enum)
                                {
                                    label = parameterInfo.Name,
                                };
                            enumField.RegisterValueChangedCallback(changeEvent =>
                            {
                                parameters[index] = changeEvent.newValue;
                            });
                            element.Add(enumField);
                        }
                        else if (parameterInfo.ParameterType.IsSubclassOf(typeof(Object)))
                        {
                            parameters.Add(null);
                            var objectField = new ObjectField()
                            {
                                label = parameterInfo.Name,
                                objectType = parameterInfo.ParameterType,
                                value = null
                            };
                            objectField.RegisterValueChangedCallback(changeEvent =>
                            {
                                parameters[index] = changeEvent.newValue;
                            });
                            element.Add(objectField);
                        }
                        else
                        {
                            unSupportType = parameterInfo.ParameterType;
                            break;
                        }
                    }

                    if (unSupportType != null)
                    {
                        element.Add(new Label($"存在暂不支持的参数类型：{unSupportType.Name}"));
                    }
                    else
                    {
                        element.Add(new Button(() =>
                        {
                            methodInfo.Invoke(value, parameters.Count > 0 ? parameters.ToArray() : null);
                        })
                        {
                            text = string.IsNullOrEmpty(cButtonAttribute.name)
                                ? methodInfo.Name
                                : cButtonAttribute.name
                        });
                    }
                }
            }
        }
    }
}
