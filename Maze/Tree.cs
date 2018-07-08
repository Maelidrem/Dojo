﻿namespace Maze
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

        public static Bitmap CreateNodeMazeBitmapLockBitsMarshal(List<List<bool>> maze, IEnumerable<Cell> tree)
        {
            Bitmap nodeMaze = new Bitmap(maze[0].Count, maze.Count);
            Cell tmpCell = null;
            BitmapData bitmapData = nodeMaze.LockBits(new Rectangle(0, 0, nodeMaze.Width, nodeMaze.Height), ImageLockMode.ReadWrite, nodeMaze.PixelFormat);

            int bytesPerPixel = Bitmap.GetPixelFormatSize(nodeMaze.PixelFormat) / 8;
            int byteCount = bitmapData.Stride * nodeMaze.Height;
            byte[] pixels = new byte[byteCount];
            IntPtr ptrFirstPixel = bitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;

            for (int y = 0; y < heightInPixels; y++)
            {
                int currentLine = y * bitmapData.Stride;
                for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                {
                    tmpCell = tree.Where(o => o.Position.X == x && o.Position.Y == y).FirstOrDefault();
                    // calculate new pixel value
                    pixels[currentLine + x] = tmpCell == null ? (maze[y][x / bytesPerPixel] ? Color.White.B : Color.Black.B) : Color.Green.B;
                    pixels[currentLine + x + 1] = tmpCell == null ? (maze[y][x / bytesPerPixel] ? Color.White.G : Color.Black.G) : Color.Green.G;
                    pixels[currentLine + x + 2] = tmpCell == null ? (maze[y][x / bytesPerPixel] ? Color.White.R : Color.Black.R) : Color.Green.R;
                }
            }

            // copy modified bytes back
            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            nodeMaze.UnlockBits(bitmapData);
            return nodeMaze;
        }

        public static Bitmap CreateNodeMazeBitmapClassic(List<List<bool>> maze, IEnumerable<Cell> tree)
        {
            Bitmap nodeMaze = new Bitmap(maze[0].Count, maze.Count);
            Cell tmpCell = null;
            for (int y = 0; y < maze.Count; y++)
            {
                for (int x = 0; x < maze[y].Count; x++)
                {
                    tmpCell = tree.Where(o => o.Position == new Point(x, y)).FirstOrDefault();
                    nodeMaze.SetPixel(x, y, tmpCell == null ? (maze[y][x] ? Color.White : Color.Black) : Color.Green);
                }
            }

            return nodeMaze;
        }

        public static Bitmap CreateSolvedMazeBitmapClassic(List<List<bool>> maze, List<Cell> tree, List<int> mazeSolved)
        {
            List<Point> path = BuildPath(tree, mazeSolved);
            List<Color> colorGradient = GetColorGradient(Color.Blue, Color.Red, path.Count);

            Bitmap nodeMaze = new Bitmap(maze[0].Count, maze.Count);
            for (int y = 0; y < maze.Count; y++)
            {
                for (int x = 0; x < maze[y].Count; x++)
                {
                    nodeMaze.SetPixel(x, y, maze[y][x] ? Color.White : Color.Black);
                }
            }

            for(int i = 0; i < path.Count; i++)
            {
                nodeMaze.SetPixel(path[i].X, path[i].Y, colorGradient[i]);
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