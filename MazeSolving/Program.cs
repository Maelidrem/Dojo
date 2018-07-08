namespace MazeSolving
{
    using Maze;
    using System.Drawing;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Drawing.Imaging;
    using System.Diagnostics;
    using MazeSolving.Types;
    using MazeSolving.Solvers;

    public class Program
    {
        static Stopwatch stw = new Stopwatch();
        static void Main(string[] args)
        {
            IEnumerable<MazeType> types = new List<MazeType>()
            {
                MazeType.Tiny,
                MazeType.Small,
                MazeType.Normal,
            };
            types = Enum.GetValues(typeof(MazeType)).Cast<MazeType>();

            foreach (MazeType type in types)
            {
                Bitmap maze = Tree.GetMaze(type);

                stw.Restart();
                List<List<bool>> convertedMaze = Tree.ConvertMazeToBool(maze, type);
                stw.Stop();

                MazeStats tmpStats = new MazeStats
                {
                    ConvertingImageToBoolArrayTime = stw.Elapsed,
                    MazeType = type,
                    MazeSize = new Size(convertedMaze.Count, convertedMaze[0].Count)
            };

                for (int y = 0; y < convertedMaze.Count; y++)
                {
                    for (int x = 0; x < convertedMaze[y].Count; x++)
                    {
                        if (convertedMaze[y][x])
                        {
                            tmpStats.CorridorSpaces++;
                        }
                    }
                }

                tmpStats.MazeProportionOfCorridor = 100 * tmpStats.CorridorSpaces / (maze.Width * maze.Width);

                stw.Restart();
                List<Cell> tree = Tree.BuildTree(convertedMaze);
                stw.Stop();
                tmpStats.TreeBuildTime = stw.Elapsed;
                tmpStats.TreeNumberOfCells = tree.Count;

                stw.Restart();
                Bitmap nodeMaze = Tree.CreateNodeMazeBitmapClassic(convertedMaze, tree);
                stw.Stop();
                tmpStats.TreeImageBuildTime = stw.Elapsed;
                
                nodeMaze.Save(@"C:\Users\90017522\Pictures\" + type.ToString() + "nodes.png", ImageFormat.Png);

                foreach (SolverType solverType in Enum.GetValues(typeof(MazeType)))
                {
                    ISolver solver = new DummySolver();
                    switch (solverType)
                    {
                        case SolverType.TurnLeft:
                            solver = new LeftTurn();
                            break;
                        case SolverType.DojoSolver:
                            solver = new DojoSolver();
                            break;
                    }

                    if (solver != null && solver.GetSolverType() != SolverType.Dummy)
                    {
                        stw.Restart();
                        List<int> mazeSolved = solver.Solve(tree);
                        stw.Stop();

                        SolverStats tmpSolverStats = new SolverStats(solver.GetSolverType())
                        {
                            SolvingTime = stw.Elapsed,
                            NumberOfNodeInSolution = mazeSolved.Count,
                            PathLength = Tree.GetPathLength(tree, mazeSolved)
                        };

                        stw.Restart();
                        Bitmap solvedMaze = Tree.CreateSolvedMazeBitmapClassic(convertedMaze, tree, mazeSolved);
                        stw.Stop();
                        tmpSolverStats.ResultImageBuildTime = stw.Elapsed;

                        solvedMaze.Save(@"C:\Users\90017522\Pictures\" + type.ToString() + "Solved" + solver.GetSolverType().ToString() + ".png", ImageFormat.Png);

                        tmpStats.SolverStats.Add(tmpSolverStats);
                    }
                }

                Console.Write(tmpStats.ToString());
                var input = Console.ReadLine();
            }
        }
    }
}
