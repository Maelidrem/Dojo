namespace MazeSolving.Solvers
{
    using System;
    using System.Collections.Generic;
    using Maze;
    using MazeSolving.Types;

    internal class LeftTurn : ISolver
    {
        public List<int> Solve(IEnumerable<Cell> tree)
        {
            Dictionary<int, Cell> treeDict = new Dictionary<int, Cell>();
            foreach (Cell cell in tree)
            {
                treeDict.Add(cell.Identifier, cell);
            }

            this.GetLeft(treeDict, 0, CardinalPoint.South);
            return solution;
        }

        private readonly List<int> visited = new List<int>();
        private readonly List<int> solution = new List<int>();
        private bool endFound = false;

        private void GetLeft(Dictionary<int, Cell> tree, int currentCellIdentifier, CardinalPoint direction)
        {
            tree[currentCellIdentifier].Visited = true;
            visited.Add(currentCellIdentifier);
            solution.Add(currentCellIdentifier);
            List<CardinalPoint> order = this.GetOrderFromDirection(direction);
            if (tree[currentCellIdentifier].Type == CellType.End)
            {
                endFound = true;
                return;
            }

            foreach(CardinalPoint newDirection in order)
            {
                Cell tmpCell = tree[currentCellIdentifier];
                if (tmpCell.Neighbours.ContainsKey(newDirection) && 
                    !tree[tmpCell.Neighbours[newDirection].Identifier].Visited)
                {
                    this.GetLeft(tree, tmpCell.Neighbours[newDirection].Identifier, newDirection);
                    if (endFound)
                    {
                        return;
                    }
                }
            }

            solution.RemoveAt(solution.Count - 1);
        }

        private List<CardinalPoint> GetOrderFromDirection(CardinalPoint direction)
        {
            List<CardinalPoint> order = new List<CardinalPoint>();
            switch (direction)
            {
                case CardinalPoint.West:
                    order.Add(CardinalPoint.South);
                    order.Add(CardinalPoint.West);
                    order.Add(CardinalPoint.North);
                    break;
                case CardinalPoint.North:
                    order.Add(CardinalPoint.West);
                    order.Add(CardinalPoint.North);
                    order.Add(CardinalPoint.East);
                    break;
                case CardinalPoint.East:
                    order.Add(CardinalPoint.North);
                    order.Add(CardinalPoint.East);
                    order.Add(CardinalPoint.South);
                    break;
                case CardinalPoint.South:
                    order.Add(CardinalPoint.East);
                    order.Add(CardinalPoint.South);
                    order.Add(CardinalPoint.West);
                    break;
            }

            return order;
        }

        public SolverType GetSolverType()
        {
            return SolverType.TurnLeft;
        }
    }
}