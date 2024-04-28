using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CGame.Editor
{
    public sealed class DialogueNode : NodeBase
    {
        public override NodeAssetBase NodeAsset { get; }
        
        public DialogueNode(DialogueNodeAsset nodeAsset = null)
        {
            AddToClassList("dialogueNode");
            
            NodeAsset = nodeAsset != null ? nodeAsset : ScriptableObject.CreateInstance<DialogueNodeAsset>();
            
            title = "Dialogue";
            
            AddInputPort("In", Orientation.Horizontal, Port.Capacity.Multi);
            AddOutputPort("Out");
            
            var serializedObject = new SerializedObject(NodeAsset);
            
            var authorTextField = new TextField { label = "Author : ", multiline = true };
            authorTextField.labelElement.style.minWidth = 0;
            authorTextField.BindProperty(serializedObject.FindProperty("author"));
            mainContainer.Add(authorTextField);

            var contentTextField = new TextField { label = "Content : ", multiline = true };
            contentTextField.labelElement.style.minWidth = 0;
            contentTextField.BindProperty(serializedObject.FindProperty("content"));
            mainContainer.Add(contentTextField);
        }
    }
}