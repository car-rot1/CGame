using System;
using UnityEngine.Events;

namespace CGame
{
    public sealed class EventNodeAsset : NodeAssetBase
    {
        public UnityEvent<Action> unityEvent;
        
        public override bool Finish { get; protected set; }

        public override void Execute()
        {
            unityEvent?.Invoke(() => Finish = true);
        }
    }
}