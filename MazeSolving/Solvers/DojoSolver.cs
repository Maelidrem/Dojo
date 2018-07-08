namespace MazeSolving.Solvers
{
    using System;
    using System.Collections.Generic;
    using Maze;
    using MazeSolving.Types;

    internal class DojoSolver : ISolver
    {
        public List<int> Solve(IEnumerable<Cell> tree)
        {
            return new List<int>();
        }

        public SolverType GetSolverType()
        {
            return SolverType.DojoSolver;
        }
    }
}