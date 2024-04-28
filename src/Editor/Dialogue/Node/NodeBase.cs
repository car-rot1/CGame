using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CGame.Editor
{
    [Serializable]
    public abstract class NodeBase : Node
    {
        protected static StyleSheet NodeStyleSheet = Resources.Load<StyleSheet>("NodeUSSFile");
        
        public event Action<NodeBase> OnSelect; 
        public event Action<NodeBase> OnUnSelect;

        public override bool expanded => false;

        public abstract NodeAssetBase NodeAsset { get; }
        
        public sealed override string title { get => base.title; set => base.title = value; }

        protected NodeBase()
        {
            styleSheets.Add(NodeStyleSheet);
        }

        protected Port AddInputPort(string portName, Orientation orientation = Orientation.Horizontal, Port.Capacity capacity = Port.Capacity.Single)
        {
            var inputPort = Port.Create<Edge>(orientation, UnityEditor.Experimental.GraphView.Direction.Input, capacity, typeof(float));
            inputPort.portName = portName;
            inputContainer.Add(inputPort);

            return inputPort;
        }
        
        protected Port AddOutputPort(string portName, Orientation orientation = Orientation.Horizontal, Port.Capacity capacity = Port.Capacity.Single)
        {
            var outPort = Port.Create<Edge>(orientation, UnityEditor.Experimental.GraphView.Direction.Output, capacity, typeof(float));
            outPort.portName = portName;
            outputContainer.Add(outPort);

            return outPort;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnSelect?.Invoke(this);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            OnUnSelect?.Invoke(this);
        }
    }
}