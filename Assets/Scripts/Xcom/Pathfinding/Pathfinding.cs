using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding
{

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    [HideInInspector] public MyGrid<PathNode> grid;
    private List<PathNode> openList, closedList;

    public Pathfinding(int width, int height, Vector3 location, float gridSize) 
    { 
        grid = new MyGrid<PathNode>(width, height, gridSize, location, (MyGrid<PathNode> grid, int x, int y) => new PathNode(grid, x, y));
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++) 
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue; 
                if (!neighbourNode.isWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }

                }
            }

        }

        //out of nodes on the open list
        return null;

    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {

        List<PathNode > neighbourList = new List<PathNode>();

        //left
        neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
        //left down
        neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
        //left up
        neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));

        //right
        neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
        //right down
        neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
        //right up
        neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));

        //up
        neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));
        //down
        neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));

        neighbourList = neighbourList.Where(l => l != null).ToList();

        return neighbourList;

    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {

        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null) 
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }

        path.Reverse();

        return path;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetNode (int x, int y)
    {

        if (x > grid.GetWidth() || x < 0)
        {
            return null;
        }

        if (y > grid.GetHeight() || y < 0)
        {
            return null;
        }

        return grid.GetGridObject(x, y);
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }

        return lowestFCostNode;
    }

    public MyGrid<PathNode> GetGrid()
    {
        return grid;
    }

}
