using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CGame.Editor
{
    public static class GraphViewUtility
    {
        public static void LoadGraphViewData(NodeGraphView nodeGraphView, GraphViewData data)
        {
            var removeNodes = nodeGraphView.nodes;
            var removeEdges = nodeGraphView.edges;
            foreach (var removeNode in removeNodes)
                nodeGraphView.RemoveElement(removeNode);
            foreach (var removeEdge in removeEdges)
                nodeGraphView.RemoveElement(removeEdge);
            
            if (data == null || data.nodesData.Count <= 0)
            {
                var rootNode = new RootNode();
                
                var x = nodeGraphView.parent.parent.contentRect.width * 0.3f;
                var y = nodeGraphView.parent.parent.contentRect.height * 0.1f;
                rootNode.SetPosition(new Rect(new Vector2(x, y), rootNode.GetPosition().size));
                nodeGraphView.RootNode = rootNode;
                nodeGraphView.AddNode(rootNode);

                if (data != null)
                {
                    nodeGraphView.viewTransform.position = data.position;
                    nodeGraphView.viewTransform.scale = data.scale;
                }
                
                return;
            }
            
            nodeGraphView.viewTransform.position = data.position;
            nodeGraphView.viewTransform.scale = data.scale;
            
            foreach (var nodeData in data.nodesData)
            {
                var nodeType = Type.GetType(nodeData.nodeTypeName)!;
                var node = (NodeBase)Activator.CreateInstance(nodeType, new object[] { nodeData.nodeAsset });
                node.SetPosition(nodeData.nodePosition);
                if (node is RootNode rootNode)
                    nodeGraphView.RootNode = rootNode;
                nodeGraphView.AddNode(node);
            }

            var nodes = nodeGraphView.nodes.OfType<NodeBase>().ToList();
            foreach (var edgeData in data.edgesData)
            {
                var edge = new Edge();
                var outputNode = nodes.First(node => node.NodeAsset == edgeData.outputNodeAsset);
                var inputNode = nodes.First(node => node.NodeAsset ==  edgeData.inputNodeAsset);
                edge.output = outputNode.outputContainer.Q<Port>();
                edge.input = inputNode.inputContainer.Q<Port>();
                nodeGraphView.AddElement(edge);
            }
        }

        public static void SaveGraphViewData(NodeGraphView nodeGraphView, GraphViewData data)
        {
            data.position = nodeGraphView.viewTransform.position;
            data.scale = nodeGraphView.viewTransform.scale;
            
            foreach (var nodeAsset in AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(data)))
                AssetDatabase.RemoveObjectFromAsset(nodeAsset);
            data.nodesData.Clear();
            data.edgesData.Clear();

            data.rootNodeAsset = (RootNodeAsset)nodeGraphView.RootNode.NodeAsset;
            foreach (var node in nodeGraphView.nodes.OfType<NodeBase>())
            {
                data.nodesData.Add(new NodeData
                {
                    nodeAsset = node.NodeAsset,
                    nodePosition = node.GetPosition(),
                    nodeTypeName = node.GetType().FullName,
                });
                AssetDatabase.AddObjectToAsset(node.NodeAsset, data);
            }
            
            foreach (var edge in nodeGraphView.edges)
            {
                data.edgesData.Add(new EdgeData
                {
                    inputNodeAsset = ((NodeBase)edge.input.node).NodeAsset,
                    outputNodeAsset = ((NodeBase)edge.output.node).NodeAsset,
                });
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}