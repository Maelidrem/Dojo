namespace MazeSolving.Types
{
    using System;

    public class SolverStats
    {
        public SolverType SolverType { get; private set; }
        public TimeSpan SolvingTime { get; internal set; }
        public int NumberOfNodeInSolution { get; internal set; }
        public int PathLength { get; internal set; }
        public TimeSpan ResultImageBuildTime { get; internal set; }

        public SolverStats(SolverType solverType)
        {
            this.SolverType = solverType;
        }
    }
}
