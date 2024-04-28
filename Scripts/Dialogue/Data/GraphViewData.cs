using System.Collections.Generic;
using UnityEngine;

namespace CGame
{
    [CreateAssetMenu(fileName = nameof(GraphViewData), menuName = "NodeGraphView/NodeGraphView Data")]
    public sealed class GraphViewData : ScriptableObject
    {
        public RootNodeAsset rootNodeAsset;
        
        [HideInInspector] public Vector3 position;
        [HideInInspector] public Vector3 scale = Vector3.one;
        [HideInInspector] public List<NodeData> nodesData;
        [HideInInspector] public List<EdgeData> edgesData;
    }
}