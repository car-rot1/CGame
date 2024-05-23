using System;
using UnityEngine;

namespace CGame
{
    public enum RoomType
    {
        Start,
        Other,
    }
    
    [Serializable]
    public sealed class RoomInfo
    {
        public Vector2Int position;
        public RoomType type;
        public int depth;
        public Direction lastDirection;
        public Direction connectDirection;
        
        public RoomInfo(Vector2Int position, RoomType type, int depth = 0, Direction lastDirection = Direction.None, Direction connectDirection = Direction.None)
        {
            this.position = position;
            this.type = type;
            this.depth = depth;
            this.lastDirection = lastDirection;
            this.connectDirection = connectDirection;
        }
    }
}