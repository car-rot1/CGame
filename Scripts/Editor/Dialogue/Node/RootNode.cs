using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CGame.Editor
{
    public sealed class RootNode : NodeBase
    {
        public override NodeAssetBase NodeAsset { get; }

        public RootNode(RootNodeAsset nodeAsset = null)
        {
            NodeAsset = nodeAsset != null ? nodeAsset : ScriptableObject.CreateInstance<RootNodeAsset>();
            
            title = "Root";
            
            capabilities -= Capabilities.Deletable;
            
            AddOutputPort("Out");
        }
    }
}