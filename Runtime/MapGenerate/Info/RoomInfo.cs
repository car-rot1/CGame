using UnityEngine;

namespace CGame
{
    public sealed class RoomInfo
    {
        public Vector2Int position;
        public Color color;
        public int depth;
        public Direction lastDirection;
        public Direction connectDirection;
        
        public RoomInfo(Vector2Int position, Color color, int depth = 0, Direction lastDirection = Direction.None, Direction connectDirection = Direction.None)
        {
            this.position = position;
            this.color = color;
            this.depth = depth;
            this.lastDirection = lastDirection;
            this.connectDirection = connectDirection;
        }
    }
}