using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CGame.Editor
{
    public sealed class NodeGraphEditorWindow : EditorWindow
    {
        private string _title;
        private string _undoTitle;
        private readonly Stack<List<CommandBase>> _commands = new();

        public void AddCommand(List<CommandBase> commands)
        {
            titleContent.text = _undoTitle;
            _commands.Push(commands);
        }

        public void Undo()
        {
            if (!_commands.TryPop(out var commands))
                return;
                
            for (var i = commands.Count - 1; i >= 0; i--)
                commands[i].Undo();
                
            if (_commands.Count <= 0)
                titleContent.text = _title;
        }
        
        private NodeGraphView _graphView;
        private GraphViewData _graphViewData;

        private static NodeGraphEditorWindow _instance;

        private VisualElement _inspectorView;
        private UnityEditor.Editor _currentEditor;

        private string graphViewDataText;
        private string graphViewDataPath;
        
        public static void Open(GraphViewData graphViewData)
        {
            if (_instance == null)
            {
                _instance = GetWindow<NodeGraphEditorWindow>();
                _instance._title = ObjectNames.NicifyVariableName(nameof(NodeGraphEditorWindow));
                _instance._undoTitle = ObjectNames.NicifyVariableName(nameof(NodeGraphEditorWindow)) + '*';
                _instance._graphViewData = graphViewData;
                
                _instance.AddGraphView();
                _instance.AddToolbar();
                _instance.AddInspectorView();
            }
            else
            {
                _instance._graphViewData = graphViewData;
                GraphViewUtility.LoadGraphViewData(_instance._graphView, _instance._graphViewData);
            }

            if (graphViewData != null)
            {
                _instance.graphViewDataPath = AssetDatabaseExtension.GetAssetAbsolutePath(graphViewData);
                _instance.graphViewDataText = File.ReadAllText(_instance.graphViewDataPath);
            }
        }

        private void AddGraphView()
        {
            _graphView = new NodeGraphView(this);
            _graphView.styleSheets.Add(Resources.Load<StyleSheet>("GraphViewUSSFile"));
            
            _graphView.StretchToParentSize();
            _graphView.OnViewTransformChange += OnViewTransformChange;
            _graphView.OnSelectNode += node => _currentEditor = UnityEditor.Editor.CreateEditor(node.NodeAsset);
            _graphView.OnUnSelectNode += node => _currentEditor = null;
            
            rootVisualElement.Add(_graphView);
            GraphViewUtility.LoadGraphViewData(_graphView, _graphViewData);
            _commands.Clear();
            _instance.titleContent = new GUIContent(_instance._title);
        }

        private void OnViewTransformChange(ITransform viewTransform)
        {
            if (_graphViewData == null)
                return;
            
            _graphViewData.position = viewTransform.position;
            _graphViewData.scale = viewTransform.scale;
        }

        private void AddToolbar()
        {
            var toolbar = new Toolbar();
            toolbar.styleSheets.Add(Resources.Load<StyleSheet>("ToolbarUSSFile"));
            
            var isHide = false;
            var hideInspectorViewButton = new Button { text = "显示属性面板" };
            hideInspectorViewButton.AddToClassList("showInspectorViewButton");
            
            hideInspectorViewButton.clicked += () =>
            {
                isHide = !isHide;
                if (isHide)
                {
                    hideInspectorViewButton.RemoveFromClassList("showInspectorViewButton");
                    hideInspectorViewButton.AddToClassList("hideInspectorViewButton");
                    _inspectorView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                }
                else
                {
                    hideInspectorViewButton.RemoveFromClassList("hideInspectorViewButton");
                    hideInspectorViewButton.AddToClassList("showInspectorViewButton");
                    _inspectorView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                }
            };
            toolbar.Add(hideInspectorViewButton);
            
            var targetObjectField = new ObjectField { value = _graphViewData, objectType = typeof(GraphViewData) };
            targetObjectField.RegisterValueChangedCallback(changeValue =>
            {
                _graphViewData = (GraphViewData)changeValue.newValue;
                GraphViewUtility.LoadGraphViewData(_graphView, _graphViewData);

                if (_graphViewData != null)
                {
                    graphViewDataPath = AssetDatabaseExtension.GetAssetAbsolutePath(_graphViewData);
                    graphViewDataText = File.ReadAllText(graphViewDataPath);
                }
            });
            toolbar.Add(targetObjectField);

            var saveButton = new Button(() =>
            {
                if (_graphViewData == null)
                {
                    var path = EditorUtility.SaveFilePanelInProject("保存",
                        ObjectNames.NicifyVariableName(nameof(GraphViewData)), "asset", "请选择保存的路径");
                    if (string.IsNullOrWhiteSpace(path))
                        return;

                    _graphViewData = CreateInstance<GraphViewData>();
                    _graphViewData.nodesData = new List<NodeData>();
                    _graphViewData.edgesData = new List<EdgeData>();
                    targetObjectField.value = _graphViewData;
                    AssetDatabase.CreateAsset(_graphViewData, path);
                }
                GraphViewUtility.SaveGraphViewData(_graphView, _graphViewData);
                graphViewDataText = File.ReadAllText(graphViewDataPath);
            }) { text = "保存" };
            toolbar.Add(saveButton);

            var undoButton = new Button(Undo) { text = "撤销" };
            toolbar.Add(undoButton);
            
            rootVisualElement.Add(toolbar);
        }

        private void AddInspectorView()
        {
            _inspectorView = new VisualElement
            {
                style =
                {
                    minWidth = 200,
                    width = 250,
                }
            };
            _inspectorView.styleSheets.Add(Resources.Load<StyleSheet>("InspectorViewUSSFile"));
            _inspectorView.AddToClassList("inspectorView");

            var imguiContainerContainer= new VisualElement();
            imguiContainerContainer.AddToClassList("imguiContainerContainer");
            
            var imguiContainer = new IMGUIContainer(() =>
            {
                if (_currentEditor != null)
                {
                    EditorGUIUtility.labelWidth = 60;
                    _currentEditor.OnInspectorGUI();
                }
            });
            imguiContainer.AddToClassList("imguiContainer");
            
            imguiContainerContainer.Add(imguiContainer);
            _inspectorView.Add(imguiContainerContainer);

            var splitLine = new VisualElement();
            splitLine.AddToClassList("splitLine");
            splitLine.RegisterCallback<MouseUpEvent>(_ => { splitLine.ReleaseMouse(); });
            splitLine.RegisterCallback<MouseDownEvent>(_ => { splitLine.CaptureMouse(); });
            splitLine.RegisterCallback<MouseMoveEvent>(evt =>
            {
                if (splitLine.HasMouseCapture())
                    _inspectorView.style.width = _inspectorView.style.width.value.value + evt.mouseDelta.x;
            });
            _inspectorView.Add(splitLine);

            rootVisualElement.Add(_inspectorView);
        }

        private void OnDisable()
        {
            if (_graphViewData != null)
            {
                File.WriteAllText(graphViewDataPath, graphViewDataText);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_graphViewData));
            }
            
            _instance = null;
        }
    }
}