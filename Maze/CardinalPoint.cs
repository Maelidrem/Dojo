namespace Maze
{
    using System;

    [Flags]
    public enum CardinalPoint
    {
        None = 0,
        West = 1,
        North = 2,
        East = 4,
        South = 8
    }
}
