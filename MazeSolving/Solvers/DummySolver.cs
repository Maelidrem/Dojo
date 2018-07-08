namespace MazeSolving.Solvers
{
    using System.Collections.Generic;
    using Maze;
    using MazeSolving.Types;

    internal class DummySolver : ISolver
    {
        public List<int> Solve(IEnumerable<Cell> tree)
        {
            return new List<int>();
        }

        public SolverType GetSolverType()
        {
            return SolverType.Dummy;
        }
    }
}