using System;
using UnityEngine;

namespace CGame
{
    public static class DirectionExtension
    {
        public static Direction ToDirection(this Vector2Int self)
        {
            return self switch
            {
                { x: 0, y: > 0 } => Direction.Up,
                { x: > 0, y: > 0 } => Direction.RightUp,
                { x: > 0, y: 0 } => Direction.Right,
                { x: > 0, y: < 0 } => Direction.RightDown,
                { x: 0, y: < 0 } => Direction.Down,
                { x: < 0, y: < 0 } => Direction.LeftDown,
                { x: < 0, y: 0 } => Direction.Left,
                { x: < 0, y: > 0 } => Direction.LeftUp,
                _ => Direction.None
            };
        }
        
        public static Vector2Int ToVector(this Direction self)
        {
            return self switch
            {
                Direction.Up => Vector2Int.up,
                Direction.RightUp => new Vector2Int(1, 1),
                Direction.Right => Vector2Int.right,
                Direction.RightDown => new Vector2Int(1, -1),
                Direction.Down => Vector2Int.down,
                Direction.LeftDown => new Vector2Int(-1, -1),
                Direction.Left => Vector2Int.left,
                Direction.LeftUp => new Vector2Int(-1, 1),
                _ => Vector2Int.zero
            };
        }

        public static Direction Reverse(this Direction self)
        {
            return self switch
            {
                Direction.Up => Direction.Down,
                Direction.RightUp => Direction.LeftDown,
                Direction.Right => Direction.Left,
                Direction.RightDown => Direction.LeftUp,
                Direction.Down => Direction.Up,
                Direction.LeftDown => Direction.RightUp,
                Direction.Left => Direction.Right,
                Direction.LeftUp => Direction.RightDown,
                _ => Direction.None,
            };
        }
    }
}