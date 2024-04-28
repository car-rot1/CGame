using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CGame.Editor
{
    public sealed class EventNode : NodeBase
    {
        public override NodeAssetBase NodeAsset { get; }

        public EventNode(EventNodeAsset nodeAsset = null)
        {
            NodeAsset = nodeAsset != null ? nodeAsset : ScriptableObject.CreateInstance<EventNodeAsset>();

            title = "Event";
            
            AddOutputPort("Out", Orientation.Horizontal, Port.Capacity.Multi);
            AddInputPort("In");

            var serializedObject = new SerializedObject(NodeAsset);
            var imguiContainer = new IMGUIContainer(() =>
            {
                serializedObject.Update();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("unityEvent"));
                serializedObject.ApplyModifiedProperties();
            });
            mainContainer.Add(imguiContainer);
        }
    }
}