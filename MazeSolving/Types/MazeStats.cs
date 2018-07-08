namespace MazeSolving.Types
{
    using Maze;
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public class MazeStats
    {
        public MazeStats()
        {
            SolverStats = new List<SolverStats>();
        }

        public MazeType MazeType {get; set;}
        public Size MazeSize { get; internal set; }
        public int CorridorSpaces { get; internal set; }
        public float MazeProportionOfCorridor { get; internal set; }
        public TimeSpan TreeBuildTime { get; internal set; }
        public int TreeNumberOfCells { get; internal set; }
        public List<SolverStats> SolverStats { get; internal set; }
        public TimeSpan ConvertingImageToBoolArrayTime { get; internal set; }
        public TimeSpan TreeImageBuildTime { get; internal set; }

        public override string ToString()
        {
            List<string> stats = new List<string>();
            stats.Add(string.Empty);
            stats.Add("==== Maze name : " + MazeType.ToString() + " ====");
            stats.Add("- maze conversion time : " + ConvertingImageToBoolArrayTime.ToString("mm':'ss'.'fff"));
            stats.Add("- size : " + MazeSize.Width + "x" + MazeSize.Height + "px (" + MazeSize.Width * MazeSize.Height + "px)");
            stats.Add("- corridor : " + CorridorSpaces + " (" + MazeProportionOfCorridor.ToString() + "% of maze size)");
            stats.Add("---- Tree ----");
            stats.Add("- number of cells : " + TreeNumberOfCells + " (" + 100 * TreeNumberOfCells / CorridorSpaces + "% of corridor)");
            stats.Add("- build time : " + TreeBuildTime.ToString("mm':'ss'.'fff"));
            stats.Add("- image building time : " + TreeImageBuildTime);
            foreach(SolverStats solverStats in SolverStats)
            {
                stats.Add("---- Solver : " + solverStats.SolverType + " ----");
                stats.Add("- number of nodes : " + solverStats.NumberOfNodeInSolution);
                stats.Add("- path length : " + solverStats.PathLength);
                stats.Add("- solving time : " + solverStats.SolvingTime.ToString("mm':'ss'.'fff"));
                stats.Add("- image build time : " + solverStats.ResultImageBuildTime.ToString("mm':'ss'.'fff"));
            }

            return string.Join(Environment.NewLine, stats);
        }
    }
}
