using System;
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

                await Task.Delay(100);
                SelectionChanged();
            }
        }

        private static void SelectionChanged()
        {
            if (Selection.count > 1)
                return;
            
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

                    
                    // var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    // var iterator = serializedObject.GetIterator();
                    // while (iterator.Next(true))
                    // {
                    //     foreach (var assembly in assemblies)
                    //     {
                    //         var type = assembly.GetType(iterator.type);
                    //         if (type != null)
                    //         {
                    //             Debug.Log(type);
                    //             break;
                    //         }
                    //     }
                    // }
                    
                    var myElement = new VisualElement
                    {
                        name = "MyElement",
                        style =
                        {
                            marginLeft = 18,
                            marginRight = 3,
                        }
                    };
                    
                    foreach (var methodInfo in serializedObject.targetObject.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                    {
                        var testButtonAttribute = methodInfo.GetCustomAttribute<CButtonAttribute>();
                        if (testButtonAttribute != null)
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
                                    myElement.Add(integerField);
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
                                    myElement.Add(floatField);
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
                                    myElement.Add(doubleField);
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
                                    myElement.Add(textField);
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
                                    myElement.Add(enumField);
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
                                    myElement.Add(objectField);
                                }
                                else
                                {
                                    unSupportType = parameterInfo.ParameterType;
                                    break;
                                }
                            }

                            if (unSupportType != null)
                            {
                                myElement.Add(new Label($"存在暂不支持的参数类型：{unSupportType.Name}"));
                            }
                            else
                            {
                                myElement.Add(new Button(() =>
                                {
                                    methodInfo.Invoke(serializedObject.targetObject,
                                        parameters.Count > 0 ? parameters.ToArray() : null);
                                })
                                {
                                    text = string.IsNullOrEmpty(testButtonAttribute.name)
                                        ? methodInfo.Name
                                        : testButtonAttribute.name
                                });
                            }
                        }
                    }
                    
                    if (myElement.childCount > 0)
                        inspectorElement.Add(myElement);
                }
            }
        }
    }
}
