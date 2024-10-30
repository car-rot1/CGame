/*  存在严重bug，会导致Update死循环
using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

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
            Selection.selectionChanged += () => _isDraw = false;
        }
        
        private static bool _isDraw;

        private static void OnUpdate()
        {
            // if (Selection.count > 1)
            //     return;
            
            if (_currentInspectorWindow == null)
            {
                _isDraw = false;
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
            }
            
            if (!_isDraw)
                RebuildInspector();
        }

        private static void RebuildInspector()
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
                
                if (inspectorEditorsListElement.childCount <= 0)
                    return;
                
                // for (var i = 1; i < editorElements.Count; i++)
                for (var i = 1; i < inspectorEditorsListElement.childCount; i++)
                {
                    var element = inspectorEditorsListElement[i];
                    if (element.childCount < 3)
                        return;
                    
                    if (element[1] is not InspectorElement { childCount: > 2 } inspectorElement)
                        return;
                    
                    var serializedObject = (SerializedObject)inspectorElement.GetType()
                        .GetProperty("boundObject", BindingFlags.NonPublic | BindingFlags.Instance)
                        !.GetValue(inspectorElement);
                    
                    if (serializedObject == null || serializedObject.targetObject == null)
                        continue;
                    
                    var myElement = new VisualElement
                    {
                        name = "CButtonElement",
                        style =
                        {
                            marginLeft = 18,
                            marginRight = 3,
                        }
                    };
                    
                    DrawMethodButton(serializedObject.targetObject, serializedObject.targetObject.GetType(), myElement);
                    foreach (var serializedPropertyInfo in SerializedPropertyInfoUtility.Get(serializedObject))
                    {
                        if (serializedPropertyInfo.fieldInfo != null)
                            DrawMethodButton(serializedPropertyInfo.Value, serializedPropertyInfo.fieldInfo.FieldType, myElement);
                    }
                    
                    if (myElement.childCount > 0)
                        inspectorElement.Add(myElement);
                }

                _isDraw = true;
            }
        }

        private static void DrawMethodButton(object value, Type valueType, VisualElement containerElement)
        {
            if (value != default && !value.Equals(default))
            {
                var method = valueType.GetMethod("GetEnumerator", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (method != null)
                {
                    var result = (IEnumerator)method.Invoke(value, new object[] { });
                    while (result.MoveNext())
                    {
                        if (result.Current == null)
                            continue;
                        var go = result.Current;
                        var listElement = ButtonMethodDrawer.DrawElement(go, go.GetType());
                        if (listElement.childCount > 0)
                            containerElement.Add(listElement);
                    }
                    return;
                }
            }

            var element = ButtonMethodDrawer.DrawElement(value, valueType);
            if (element.childCount > 0)
                containerElement.Add(element);
        }
    }
}
*/
