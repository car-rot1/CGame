using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CGame.Editor
{
    public class NodeGraphView : GraphView
    {
        public const float MinScale = 0.25f;
        public const float MaxScale = 3f;
        
        public event Action<ITransform> OnViewTransformChange;
        public event Action<NodeBase> OnSelectNode;
        public event Action<NodeBase> OnUnSelectNode;
        
        public RootNode RootNode { get; set; }
        private readonly NodeGraphEditorWindow _editorWindow;
        
        public NodeGraphView(NodeGraphEditorWindow editorWindow)
        {
            _editorWindow = editorWindow;
            
            SetupZoom(MinScale, MaxScale);
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            Insert(0, new GridBackground());
            
            var searchWindowProvider = ScriptableObject.CreateInstance<DialogueSearchWindowProvider>();
            searchWindowProvider.Init(_editorWindow, this);
            
            nodeCreationRequest += context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindowProvider);
            };

            viewTransformChanged += graphView => OnViewTransformChange?.Invoke(graphView.viewTransform);
            graphViewChanged += OnGraphViewChanged;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.movedElements != null)
            {
                OnMoveElements(graphViewChange.movedElements, graphViewChange.moveDelta);
            }
            
            if (graphViewChange.edgesToCreate != null)
            {
                OnCreateEdges(graphViewChange.edgesToCreate);
            }
            
            if (graphViewChange.elementsToRemove != null)
            {
                OnRemoveElements(graphViewChange.elementsToRemove);
            }
        
            return graphViewChange;
        }

        private void OnMoveElements(IEnumerable<GraphElement> elements, Vector2 moveDelta)
        {
            _editorWindow.AddCommand(new List<CommandBase>
            {
                new MoveElementCommand(elements, moveDelta)
            });
        }

        private void OnCreateEdges(IReadOnlyCollection<Edge> edges)
        {
            var connectNodes = new ConnectNodesCommand(edges.ToDictionary(edge => edge, edge => (edge.output, edge.input)));
            connectNodes.Execute();
            _editorWindow.AddCommand(new List<CommandBase>
            {
                new CreateElementsCommand(this, edges),
                connectNodes,
            });
        }

        private void OnRemoveElements(List<GraphElement> elements)
        {
            var removeElements = new HashSet<GraphElement>(elements);
            foreach (var element in elements)
            {
                switch (element)
                {
                    case Edge edge:
                    {
                        var outputNodeAsset = ((NodeBase)edge.output.node).NodeAsset;
                        var inputNodeAsset = ((NodeBase)edge.input.node).NodeAsset;

                        if (outputNodeAsset is DialogueOptionNodeAsset dialogueOptionNodeAsset)
                            dialogueOptionNodeAsset.optionNodes.Remove((OptionNodeAsset)inputNodeAsset);
                        else
                            outputNodeAsset.NextNode = null;
                        break;
                    }
                    case NodeBase node:
                    {
                        var removeEdges = edges.Where(edge => edge.output.node == node || edge.input.node == node);
                        foreach (var removeEdge in removeEdges)
                        {
                            removeElements.Add(removeEdge);
                            var outputNodeAsset = ((NodeBase)removeEdge.output.node).NodeAsset;
                            var inputNodeAsset = ((NodeBase)removeEdge.input.node).NodeAsset;

                            if (outputNodeAsset is DialogueOptionNodeAsset dialogueOptionNodeAsset)
                                dialogueOptionNodeAsset.optionNodes.Remove((OptionNodeAsset)inputNodeAsset);
                            else
                                outputNodeAsset.NextNode = null;
                            RemoveElement(removeEdge);
                        }
                        break;
                    }
                }
            }
            _editorWindow.AddCommand(new List<CommandBase>
            {
                new DisconnectNodesCommand(removeElements.OfType<Edge>().ToDictionary(edge => edge, edge => (edge.output, edge.input))),
                new RemoveElementsCommand(this, removeElements),
            });
        }

        public void AddNode(NodeBase node)
        {
            AddElement(node);
            node.OnSelect += OnSelectNode;
            node.OnUnSelect += OnUnSelectNode;
            OnAddNode(node);
        }

        private void OnAddNode(NodeBase node)
        {
            _editorWindow.AddCommand(new List<CommandBase>
            {
                new CreateElementsCommand(this, new[] { node }),
            });
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return startPort.node switch
            {
                DialogueOptionNode => ports.ToList()
                    .Where(port => port.node is OptionNode && startPort.direction != port.direction)
                    .ToList(),
                _ => ports.ToList()
                    .Where(port => port.node is not OptionNode && startPort.node != port.node && startPort.direction != port.direction)
                    .ToList()
            };
        }
    }
}