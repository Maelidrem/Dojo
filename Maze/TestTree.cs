namespace Maze
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestFixture]
    public class TestTree
    {
        [TestCase(MazeType.Tiny)]
        public void MazeLoading(MazeType type)
        {
            Bitmap maze = Tree.GetMaze(type);
            Assert.Pass();
        }

        [TestCase(MazeType.Tiny)]
        public void MazeToTree(MazeType type)
        {
            Bitmap maze = Tree.GetMaze(type);
            List<List<bool>> convertedMaze = Tree.ConvertMazeToBool(maze, type);
            List<Cell> tree = Tree.BuildTree(convertedMaze);
            Assert.Pass();
        }
    }
}
