using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CGame.Editor
{
    public sealed class DialogueOptionNode : NodeBase
    {
        public override NodeAssetBase NodeAsset { get; }

        public DialogueOptionNode(DialogueOptionNodeAsset nodeAsset = null)
        {
            AddToClassList("dialogueOptionNode");
            
            if (nodeAsset == null)
            {
                nodeAsset = ScriptableObject.CreateInstance<DialogueOptionNodeAsset>();
                nodeAsset.optionNodes = new List<OptionNodeAsset>();
            }
            NodeAsset = nodeAsset;
            
            title = "DialogueOption";
            
            AddInputPort("In", Orientation.Horizontal, Port.Capacity.Multi);
            AddOutputPort("Out", Orientation.Horizontal, Port.Capacity.Multi);
            
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