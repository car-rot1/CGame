using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace CGame.Editor
{
    public class DisconnectNodesCommand : CommandBase
    {
        private readonly Dictionary<Edge, (Port outputPort, Port inputPort)> _connectDic;

        public DisconnectNodesCommand(IDictionary<Edge, (Port outputPort, Port inputPort)> connectDic)
        {
            _connectDic = new Dictionary<Edge, (Port outputPort, Port inputPort)>(connectDic);
        }
        
        public override void Execute()
        {
            foreach (var (edge, (outputPort, inputPort)) in _connectDic)
            {
                edge.output = null;
                edge.input = null;
                
                var outputNodeAsset = ((NodeBase)outputPort.node).NodeAsset;
                var inputNodeAsset = ((NodeBase)inputPort.node).NodeAsset;
            
                if (outputNodeAsset is DialogueOptionNodeAsset dialogueOptionNodeAsset)
                    dialogueOptionNodeAsset.optionNodes.Remove((OptionNodeAsset)inputNodeAsset);
                else
                    outputNodeAsset.NextNode = null;
            }
        }

        public override void Undo()
        {
            foreach (var (edge, (outputPort, inputPort)) in _connectDic)
            {
                edge.output = outputPort;
                edge.input = inputPort;
                
                var outputNodeAsset = ((NodeBase)outputPort.node).NodeAsset;
                var inputNodeAsset = ((NodeBase)inputPort.node).NodeAsset;
            
                if (outputNodeAsset is DialogueOptionNodeAsset dialogueOptionNodeAsset)
                    dialogueOptionNodeAsset.optionNodes.Add((OptionNodeAsset)inputNodeAsset);
                else
                    outputNodeAsset.NextNode = inputNodeAsset;
            }
        }
    }
}