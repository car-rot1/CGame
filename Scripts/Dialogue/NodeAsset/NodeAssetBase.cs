using UnityEngine;

namespace CGame
{
    public abstract class NodeAssetBase : ScriptableObject
    {
        public virtual NodeAssetBase NextNode { get; set; }
        public abstract bool Finish { get; protected set; }
        public abstract void Execute();
    }
}