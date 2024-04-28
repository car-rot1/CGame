using UnityEngine;
using UnityEngine.Serialization;

namespace CGame
{
    public class NodeAssetController : MonoBehaviour
    {
        public GraphViewData graphViewData;

        private NodeAssetBase currentNode;

        private void Awake()
        {
            if (currentNode == null)
                return;
            
            currentNode = graphViewData.rootNodeAsset;
            currentNode.Execute();
        }

        private void Update()
        {
            if (currentNode == null || currentNode.NextNode == null)
                return;
            
            if (currentNode.Finish)
            {
                currentNode = currentNode.NextNode;
                currentNode.Execute();
            }
        }
    }
}
