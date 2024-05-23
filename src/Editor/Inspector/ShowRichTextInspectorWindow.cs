using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CGame.Editor
{
    [InitializeOnLoad]
    public static class ShowRichTextInspectorWindow
    {
        private static readonly Type InspectorWindowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        private static EditorWindow _currentInspectorWindow;

        static ShowRichTextInspectorWindow()
        {
            EditorApplication.update += OnUpdate;
            Selection.selectionChanged += () => _isDraw = false;
        }
        
        private static bool _isDraw;
        
        private static void OnUpdate()
        {
            if (_currentInspectorWindow == null)
            {
                _isDraw = false;
                var inspectorWindows = Resources.FindObjectsOfTypeAll(InspectorWindowType);
                _currentInspectorWindow = inspectorWindows.Length > 0 ? (EditorWindow)inspectorWindows[0] : null;
            }
            
            if (!_isDraw)
            {
                RebuildInspector();
            }
        }
        
        private static void RebuildInspector()
        {
            if (_currentInspectorWindow != null)
            {
                var className = (string)typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.PropertyEditor")
                    .GetField("s_EditorListClassName", BindingFlags.NonPublic | BindingFlags.Static)
                    !.GetValue(_currentInspectorWindow);
                var inspectorEditorsListElement = _currentInspectorWindow.rootVisualElement.Q(null, className);
                
                if (inspectorEditorsListElement.childCount <= 0)
                    return;
                
                for (var i = 1; i < inspectorEditorsListElement.childCount; i++)
                {
                    var editorElement = inspectorEditorsListElement[i];
                    
                    if (editorElement.childCount < 3)
                        return;
                    
                    if (editorElement[0] is not IMGUIContainer headerIMGUIContainer ||
                        editorElement[1] is not InspectorElement inspectorElement)
                        return;
                    
                    var serializedObject = (SerializedObject)inspectorElement.GetType()
                        .GetProperty("boundObject", BindingFlags.NonPublic | BindingFlags.Instance)
                        !.GetValue(inspectorElement);
                    
                    if (SerializedPropertyInfoUtility.Get(serializedObject).Any(serializedPropertyInfo =>
                            serializedPropertyInfo.fieldInfo != null &&
                            serializedPropertyInfo.fieldInfo.FieldType == typeof(string) &&
                            serializedPropertyInfo.fieldInfo.GetCustomAttribute<ShowRichTextAttribute>() != null))
                    {
                        _isDraw = false;
                        var action = headerIMGUIContainer.onGUIHandler;
                        headerIMGUIContainer.onGUIHandler = () =>
                        {
                            var rect = headerIMGUIContainer.localBound;
                            rect.x = rect.width - 80;
                            rect.y = 0;
                            rect.width = 80;
                            rect.height = 22;

                            var current = Event.current;
                            if (rect.Contains(current.mousePosition) && current.type is EventType.MouseUp)
                            {
                                EditorStyles.textField.richText = !EditorStyles.textField.richText;
                                EditorStyles.textArea.richText = EditorStyles.textField.richText;
                                current.Use();
                                GUIUtility.hotControl = 0;
                            }
                            
                            action?.Invoke();
                            EditorGUI.Toggle(rect, EditorStyles.textField.richText);
                            _isDraw = true;
                        };
                    }
                }
            }
        }
    }
}