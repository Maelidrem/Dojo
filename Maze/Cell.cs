namespace Maze
{
    using System.Collections.Generic;
    using System.Drawing;

    public class Cell
    {
        public Cell()
        {
            this.Neighbours = new Dictionary<CardinalPoint, Neighbour>();
            this.Visited = false;
        }

        public bool Visited { get; set; }
        public int Identifier { get; set; }
        public CellType Type { get; set; }
        public Point Position { get; set; }
        public Dictionary<CardinalPoint, Neighbour> Neighbours { get; set; }
    }
}
