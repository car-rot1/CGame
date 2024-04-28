using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CGame.Editor
{
    public class DialogueSearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        private NodeGraphEditorWindow _editorWindow;
        private NodeGraphView _graphView;

        public void Init(NodeGraphEditorWindow editorWindow, NodeGraphView graphView)
        {
            _editorWindow = editorWindow;
            _graphView = graphView;
        }
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry> { new SearchTreeGroupEntry(new GUIContent("Create Node")) };
            foreach (var type in TypeCache.GetTypesDerivedFrom<NodeBase>().Where(type => !type.IsAbstract && type != typeof(RootNode)))
                entries.Add(new SearchTreeEntry(new GUIContent(type.Name)) { level = 1, userData = type });

            return entries;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var rootVisualElement = _editorWindow.rootVisualElement;
            var mousePosition = rootVisualElement.ChangeCoordinatesTo(rootVisualElement.parent, context.screenMousePosition - _editorWindow.position.position);
            var graphViewMousePosition = _graphView.contentContainer.WorldToLocal(mousePosition);
            graphViewMousePosition -= (Vector2)_graphView.viewTransform.position;
            
            var type = (Type)searchTreeEntry.userData;
            var node = (NodeBase)Activator.CreateInstance(type, new object[] { null });
            node.SetPosition(new Rect(graphViewMousePosition, node.GetPosition().size));
            _graphView.AddNode(node);
            return true;
        }
    }
}