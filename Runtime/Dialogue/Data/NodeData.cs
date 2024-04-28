using System;
using UnityEngine;

namespace CGame
{
    [Serializable]
    public struct NodeData
    {
        public NodeAssetBase nodeAsset;
        public string nodeTypeName;
        public Rect nodePosition;
    }
}