namespace Maze
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Runtime.InteropServices;

    public class Tree
    {
        public static readonly Color CoolColor = Color.FromArgb(255, 255, 255, 255);
        public static Bitmap GetMaze(MazeType type)
        {
            switch (type)
            {
                case MazeType.Tiny:
                    return Resource.tiny;
                case MazeType.Small:
                    return Resource.small;
                case MazeType.Normal:
                    return Resource.normal;
                case MazeType.Braid200:
                    return Resource.braid200;
                case MazeType.Braid2k:
                    return Resource.braid2k;
                case MazeType.Combo400:
                    return Resource.combo400;
                case MazeType.Combo6k:
                    return Resource.combo6k;
                case MazeType.Perfect10k:
                    return Resource.perfect10k;
                case MazeType.Perfect15k:
                    return Resource.perfect15k;
                case MazeType.Perfect2k:
                    return Resource.perfect2k;
                case MazeType.Perfect4k:
                    return Resource.perfect4k;
            }

            return null;
        }

        public static List<Cell> GetTree(MazeType type)
        {
            Bitmap maze = Tree.GetMaze(type);
            List<List<bool>> convertedMaze = Tree.ConvertMazeToBool(maze, type);
            return Tree.BuildTree(convertedMaze);
        }

        public static Bitmap CreateNodeMazeBitmap(List<List<bool>> maze, List<Cell> tree)
        {
            Cell cell;
            Bitmap nodeMaze = new Bitmap(maze[0].Count, maze.Count, PixelFormat.Format24bppRgb);
            using (Graphics grp = Graphics.FromImage(nodeMaze))
            {
                grp.FillRectangle(Brushes.White, 0, 0, nodeMaze.Width, nodeMaze.Height);
            }

            List<List<Color>> coloredMaze = new List<List<Color>>();
            for (int y = 0; y < maze.Count; y++)
            {
                coloredMaze.Add(new List<Color>());
                for (int x = 0; x < maze[y].Count; x++)
                {
                    cell = tree.Find(o => o.Position.X == x && o.Position.Y == y);
                    coloredMaze[y].Add(cell == null ? (maze[y][x] ? Color.White : Color.Black) : Color.Green);
                }
            }

            unsafe
            {
                BitmapData bitmapData = nodeMaze.LockBits(new Rectangle(0, 0, nodeMaze.Width, nodeMaze.Height), ImageLockMode.ReadWrite, nodeMaze.PixelFormat);
                int bytesPerPixel = Bitmap.GetPixelFormatSize(nodeMaze.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                for (int y = 0; y < heightInPixels; y++)
                {
                    byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        // calculate new pixel value
                        currentLine[x] = coloredMaze[y][x / 3].B;
                        currentLine[x + 1] = coloredMaze[y][x / 3].G;
                        currentLine[x + 2] = coloredMaze[y][x / 3].R;
                    }
                }

                nodeMaze.UnlockBits(bitmapData);
            }

            return nodeMaze;
        }

        public static Bitmap CreateSolvedMazeBitmap(List<List<bool>> maze, List<Cell> tree, List<int> mazeSolved)
        {
            Point nullPoint = new Point(0, 0);
            List<Point> path = BuildPath(tree, mazeSolved);
            List<Color> colorGradient = GetColorGradient(Color.Blue, Color.Red, path.Count);

            List<List<Color>> coloredMaze = new List<List<Color>>();
            for (int y = 0; y < maze.Count; y++)
            {
                coloredMaze.Add(new List<Color>());
                for (int x = 0; x < maze[y].Count; x++)
                {
                    Point point = path.Find(o => o.X == x && o.Y == y);
                    if (point == null || point == nullPoint)
                    {
                        coloredMaze[y].Add(maze[y][x] ? Color.White : Color.Black);
                    }
                    else
                    {
                        int i = path.IndexOf(point);
                        coloredMaze[y].Add(colorGradient[i]);
                    }
                }
            }

            Bitmap nodeMaze = new Bitmap(maze[0].Count, maze.Count, PixelFormat.Format24bppRgb);
            using (Graphics grp = Graphics.FromImage(nodeMaze))
            {
                grp.FillRectangle(Brushes.White, 0, 0, nodeMaze.Width, nodeMaze.Height);
            }

            unsafe
            {
                BitmapData bitmapData = nodeMaze.LockBits(new Rectangle(0, 0, nodeMaze.Width, nodeMaze.Height), ImageLockMode.ReadWrite, nodeMaze.PixelFormat);
                int bytesPerPixel = Bitmap.GetPixelFormatSize(nodeMaze.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                for (int y = 0; y < heightInPixels; y++)
                {
                    byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {                        
                        // calculate new pixel value
                        currentLine[x] = coloredMaze[y][x / 3].B;
                        currentLine[x + 1] = coloredMaze[y][x / 3].G;
                        currentLine[x + 2] = coloredMaze[y][x / 3].R;
                    }
                }

                nodeMaze.UnlockBits(bitmapData);
            }

            return nodeMaze;
        }

        private static List<Point> BuildPath(List<Cell> tree, List<int> mazeSolved)
        {
            List<Point> path = new List<Point>();
            Cell from;
            Cell to;
            for (int i = 1; i < mazeSolved.Count; i++)
            {
                from = tree.Find(o => o.Identifier == mazeSolved[i - 1]);
                to = tree.Find(o => o.Identifier == mazeSolved[i]);
                if (to.Position.X != from.Position.X)
                {
                    if (from.Position.X > to.Position.X)
                    {
                        for (int j = from.Position.X; j > to.Position.X; j--)
                        {
                            path.Add(new Point(j, from.Position.Y));
                        }
                    }
                    else if (from.Position.X < to.Position.X)
                    {
                        for (int j = from.Position.X; j < to.Position.X; j++)
                        {
                            path.Add(new Point(j, from.Position.Y));
                        }
                    }
                }
                else if (to.Position.Y != from.Position.Y)
                {
                    if (from.Position.Y > to.Position.Y)
                    {
                        for (int j = from.Position.Y; j > to.Position.Y; j--)
                        {
                            path.Add(new Point(from.Position.X, j));
                        }
                    }
                    else if(from.Position.Y < to.Position.Y)
                    {
                        for (int j = from.Position.Y; j < to.Position.Y; j++)
                        {
                            path.Add(new Point(from.Position.X, j));
                        }
                    }
                }
            }
            if (mazeSolved.Count > 0)
            {
                path.Add(tree.Find(o => o.Identifier == mazeSolved.Last()).Position);
            }

            return path;
        }

        private static List<Color> GetColorGradient(Color from, Color to, int steps)
        {
            List<Color> colorGradient = new List<Color>();
            for (int i = 0; i < steps; i++)
            {
                byte redStep = (byte)(from.R + ((to.R - from.R) * i / steps));
                byte blueStep = (byte)(from.B + ((to.B - from.B) * i / steps));
                byte greenStep = (byte)(from.G + ((to.G - from.G) * i / steps));
                colorGradient.Add(Color.FromArgb(redStep, greenStep, blueStep));
            }

            return colorGradient;
        }

        public static Point position;
        public static List<Cell> BuildTree(List<List<bool>> maze)
        {
            int identifier = 0;
            List<Cell> tree = new List<Cell>();
            for (int y = 0; y < maze.Count; y++)
            {
                for (int x = 0; x < maze[y].Count; x++)
                {
                    if (maze[y][x])
                    {
                        position = new Point(x, y);
                        if (y == 0)
                        {
                            tree.Add(new Cell()
                            {
                                Type = CellType.Start,
                                Position = position,
                                Identifier = identifier,
                            });

                            identifier++;
                        }
                        else if (y == maze.Count - 1)
                        {
                            Cell cell = new Cell()
                            {
                                Type = CellType.End,
                                Position = position,
                                Identifier = identifier,
                                Neighbours = FindNeighbours(position, CardinalPoint.North, tree)
                            };

                            foreach (CardinalPoint cardinal in cell.Neighbours.Keys)
                            {
                                tree.Find(o => o.Identifier == cell.Neighbours[cardinal].Identifier).Neighbours.Add(OppositeCardinal(cardinal), new Neighbour()
                                {
                                    Identifier = identifier,
                                    Position = position,
                                    Weight = cell.Neighbours[cardinal].Weight,
                                });
                            }

                            tree.Add(cell);
                            identifier++;
                        }
                        else if (IsInteresting(maze, position))
                        {
                            CardinalPoint cardinalPointsToSearch = GetCardinalPointsToSearch(maze, position);
                            Cell cell = new Cell()
                            {
                                Type = CellType.Corridor,
                                Position = position,
                                Identifier = identifier,
                                Neighbours = FindNeighbours(position, cardinalPointsToSearch, tree)
                            };

                            foreach (CardinalPoint cardinal in cell.Neighbours.Keys)
                            {
                                tree.Find(o => o.Identifier == cell.Neighbours[cardinal].Identifier).Neighbours.Add(OppositeCardinal(cardinal), new Neighbour()
                                {
                                    Identifier = identifier,
                                    Position = position,
                                    Weight = cell.Neighbours[cardinal].Weight,
                                });
                            }

                            tree.Add(cell);
                            identifier++;
                        }
                    }
                }
            }

            return tree;
        }

        private static CardinalPoint OppositeCardinal(CardinalPoint cardinal)
        {
            switch (cardinal)
            {
                case CardinalPoint.West:
                    return CardinalPoint.East;
                case CardinalPoint.North:
                    return CardinalPoint.South;
                case CardinalPoint.East:
                    return CardinalPoint.West;
                case CardinalPoint.South:
                    return CardinalPoint.North;
            }

            return CardinalPoint.None;
        }

        public static List<List<bool>> ConvertMazeToBool(Bitmap maze, MazeType type)
        {
            List<List<bool>> mazeBool = new List<List<bool>>();
            for (int y = 0; y < maze.Size.Height; y++)
            {
                mazeBool.Add(new List<bool>());
                if (type == MazeType.Braid2k)
                {
                    mazeBool[y].Add(false);
                }

                for (int x = 0; x < maze.Size.Width; x++)
                {
                    mazeBool[y].Add(maze.GetPixel(x, y) == CoolColor);
                }
            }

            return mazeBool;
        }

        public static int GetPathLength(List<Cell> tree, List<int> mazeSolved)
        {
            if (mazeSolved == null || mazeSolved.Count == 0)
            {
                return int.MaxValue;
            }

            int pathLength = 0;
            for (int i = 0; i < mazeSolved.Count - 1; i++)
            {
                pathLength += tree.Find(o => o.Identifier == mazeSolved[i])
                                  .Neighbours
                                  .Values
                                  .ToList()
                                  .Find(o => o.Identifier == mazeSolved[i + 1])
                                  .Weight;
            }

            return pathLength;
        }

        private static CardinalPoint GetCardinalPointsToSearch(List<List<bool>> maze, Point position)
        {
            CardinalPoint cardinalPoints = CardinalPoint.None;
            if (maze[position.Y][position.X - 1])
            {
                cardinalPoints = CardinalPoint.West;
            }

            if (maze[position.Y - 1][position.X])
            {
                cardinalPoints |= CardinalPoint.North;
            }

            return cardinalPoints;
        }

        private static bool IsInteresting(List<List<bool>> maze, Point position)
        {
            if (!maze[position.Y][position.X])
            {
                return false;
            }

            int pathCount = CountCoolColorAroundPoint(maze, position);
            if (pathCount == 0)
            {
                return false;
            }
            else if (pathCount == 1 || pathCount == 3 || pathCount == 4)
            {
                return true;
            }
            else if (pathCount == 2 &&
                     (maze[position.Y][position.X - 1] || maze[position.Y][position.X + 1]) &&
                     (maze[position.Y - 1][position.X] || maze[position.Y + 1][position.X]))
            {
                return true;
            }

            return false;
        }

        private static int CountCoolColorAroundPoint(List<List<bool>> maze, Point point)
        {
            int count = 0;
            count = maze[position.Y][position.X - 1] ? count + 1 : count;
            count = maze[position.Y][position.X + 1] ? count + 1 : count;
            count = maze[position.Y - 1][position.X] ? count + 1 : count;
            count = maze[position.Y + 1][position.X] ? count + 1 : count;
            return count;
        }

        private static Dictionary<CardinalPoint, Neighbour> FindNeighbours(Point position, CardinalPoint cardinal, IEnumerable<Cell> tree)
        {
            Dictionary<CardinalPoint, Neighbour> neighbours = new Dictionary<CardinalPoint, Neighbour>();
            foreach (CardinalPoint cardinalPoint in Enum.GetValues(typeof(CardinalPoint)))
            {
                Cell foundCell = null;
                int distance = int.MinValue;
                switch (cardinal & cardinalPoint)
                {
                    case CardinalPoint.West:
                        foundCell = tree.Where(o => o.Position.Y == position.Y).OrderByDescending(o => o.Position.X).FirstOrDefault();
                        distance = foundCell == null ? int.MinValue : position.X - foundCell.Position.X;
                        break;
                    case CardinalPoint.North:
                        foundCell = tree.Where(o => o.Position.X == position.X).OrderByDescending(o => o.Position.Y).FirstOrDefault();
                        distance = foundCell == null ? int.MinValue : position.Y - foundCell.Position.Y;
                        break;
                    case CardinalPoint.East:
                    case CardinalPoint.South:
                        throw new Exception("Search for neighbours unavailable to the south or east");
                }

                if (foundCell != null)
                {
                    neighbours.Add(cardinal & cardinalPoint, new Neighbour()
                    {
                        Identifier = foundCell.Identifier,
                        Position = foundCell.Position,
                        Weight = distance,
                    });
                }
            }

            return neighbours;
        }
    }
}
