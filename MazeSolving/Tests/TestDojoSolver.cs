namespace MazeSolving.Test
{
    using Maze;
    using MazeSolving.Solvers;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestFixture]
    public class TestDojoSolver
    {
        [Test]
        public void Test1()
        {
            List<Cell> tree = Tree.GetTree(MazeType.Tiny);
            ISolver solver = new DojoSolver();
            List<int> solution = solver.Solve(tree);

            Assert.Pass();
        }
    }
}
