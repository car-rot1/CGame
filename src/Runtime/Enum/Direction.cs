using System;

namespace CGame
{
    [Flags]
    public enum Direction
    {
        None = 0,
        All = Up | RightUp | Right | RightDown | Down | LeftDown | Left | LeftUp,
        MainFour = Up | Right | Down | Left,
        OtherFour = RightUp | RightDown | LeftDown | LeftUp, 
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