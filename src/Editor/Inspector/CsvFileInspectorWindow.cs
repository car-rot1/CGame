using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CGame.Localization.Editor
{
    [InitializeOnLoad]
    public static class CsvFileInspectorWindow
    {
        private static readonly Type InspectorWindowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        private static EditorWindow _currentInspectorWindow;
        private static readonly List<List<string>> _rowInfos = new();

        static CsvFileInspectorWindow()
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

                await Task.Delay(100);
                SelectionChanged();
            }
        }
        
        private static void SelectionChanged()
        {
            if (Selection.count > 1)
                return;
            
            var selectObject = Selection.activeObject;
            var localTextFilePath = AssetDatabaseExtension.GetAssetAbsolutePath(selectObject);
            
            if (!Path.GetExtension(localTextFilePath).Equals(".csv"))
                return;

            var csvContent = File.ReadAllText(localTextFilePath, Encoding.UTF8);

            CsvUtility.CsvContentToDataNonAlloc(csvContent, _rowInfos, LocalizationConfig.Instance.CsvFileInfo.separator, LocalizationConfig.Instance.CsvFileInfo.linefeed);
            
            if (_currentInspectorWindow != null)
            {
                var className = (string)typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.PropertyEditor")
                    .GetField("s_EditorListClassName", BindingFlags.NonPublic | BindingFlags.Static)
                    !.GetValue(_currentInspectorWindow);
                var inspectorEditorsListElement = _currentInspectorWindow.rootVisualElement.Q(null, className);
                
                var element = inspectorEditorsListElement[1];
                
                var myElement = new VisualElement
                {
                    name = "MyElement",
                    style =
                    {
                        height = _rowInfos.Count * 18
                    }
                };

                var table = new VisualElement
                {
                    name = "Table",
                    style =
                    {
                        height = _rowInfos.Count * 18,
                        flexDirection = FlexDirection.Row,
                        flexWrap = Wrap.Wrap,
                    }
                };
                var rows = _rowInfos.Count;
                var columns = _rowInfos.Max(rowInfo => rowInfo.Count);
                var itemWidth = new StyleLength(new Length(100f / columns, LengthUnit.Percent));
                for (var i = 0; i < rows; i++)
                {
                    for (var j = 0; j < _rowInfos[i].Count; j++)
                    {
                        var label = new Label(_rowInfos[i][j])
                        {
                            style =
                            {
                                width = itemWidth,
                                unityTextAlign = TextAnchor.MiddleCenter,
                            }
                        };
                        table.Add(label);
                    }
                }
                myElement.Add(table);

                // var imgui = new IMGUIContainer()
                // {
                //     style =
                //     {
                //         height = _rowInfos.Count * 18,
                //     }
                // };
                // imgui.onGUIHandler += () =>
                // {
                //     var rect = EditorGUILayout.GetControlRect();
                //     rect.height = _rowInfos.Count * 18;
                //
                //     var vRects = rect.VerticalEquallySplit(18f);
                //     var hRectsLength = _rowInfos.Max(rowInfo => rowInfo.Count);
                //     
                //     var defaultColor = GUI.color;
                //     for (var i = 0; i < _rowInfos.Count; i++)
                //     {
                //         var rowInfo = _rowInfos[i];
                //         var hRects = vRects[i].HorizontalEquallySplit(hRectsLength);
                //
                //         GUI.color = i % 2 == 0 ? new Color(0.6f, 0.6f, 0.6f) : new Color(0.9f, 0.9f, 0.9f);
                //         for (var j = 0; j < hRectsLength; j++)
                //         {
                //             var r = hRects[j];
                //             r.xMin += 3;
                //             r.xMax -= 3;
                //             r.yMin += 1;
                //             r.yMax -= 1;
                //             GUI.Box(r, "");
                //         }
                //         GUI.color = defaultColor;
                //         for (var j = 0; j < rowInfo.Count; j++)
                //         {
                //             EditorGUI.LabelField(hRects[j], rowInfo[j], new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
                //         }
                //     }
                // };
                // myElement.Add(imgui);
                element.Insert(1, myElement);
            }
        }
    }
}
