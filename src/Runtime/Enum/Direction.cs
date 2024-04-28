using System;

namespace CGame
{
    [Flags]
    public enum Direction
    {
        None = 0,
        All = (1 << 8) - 1,
        Up = 1 << 0,
        RightUp = 1 << 1,
        Right = 1 << 2,
        RightDown = 1 << 3,
        Down = 1 << 4,
        LeftDown = 1 << 5,
        Left = 1 << 6,
        LeftUp = 1 << 7,
    }
}