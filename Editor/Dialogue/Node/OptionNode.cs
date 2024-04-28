using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CGame.Editor
{
    public sealed class OptionNode : NodeBase
    {
        public override NodeAssetBase NodeAsset { get; }

        public OptionNode(OptionNodeAsset nodeAsset = null)
        {
            AddToClassList("optionNode");
            
            NodeAsset = nodeAsset != null ? nodeAsset : ScriptableObject.CreateInstance<OptionNodeAsset>();
            
            title = "Option";
            
            AddInputPort("In", Orientation.Horizontal, Port.Capacity.Multi);
            AddOutputPort("Out", Orientation.Horizontal, Port.Capacity.Multi);

            var serializedObject = new SerializedObject(NodeAsset);
            var optionTextField = new TextField { label = "Option : ", multiline = true };
            optionTextField.labelElement.style.minWidth = 0;
            optionTextField.BindProperty(serializedObject.FindProperty("option"));
            mainContainer.Add(optionTextField);
        }
    }
}