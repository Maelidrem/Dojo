namespace MazeSolving.Solvers
{
    using Maze;
    using MazeSolving.Types;
    using System.Collections.Generic;

    internal interface ISolver
    {
        List<int> Solve(IEnumerable<Cell> tree);
        SolverType GetSolverType();
    }
}