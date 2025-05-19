using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] private Vector2Int startCords;
    public Vector2Int StartCords
    {
        get { return startCords; }
    }

    [SerializeField] private Vector2Int targetCords;
    public Vector2Int TargetCords
    {
        get { return targetCords; }
    }

    private Node startNode;
    private Node targetNode;
    private Node currentNode;

    private Queue<Node> frontier = new Queue<Node>();
    private Dictionary<Vector2Int, Node> reached = new Dictionary<Vector2Int, Node>();

    private GridScript gridScript;
    private Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

    private Vector2Int[] searchOrder = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };

    private void Awake()
    {
        gridScript = FindObjectOfType<GridScript>();
        if (gridScript != null)
        {
            grid = gridScript.Grid;
        }
    }

    public List<Node> GetNewPath()
    {
        return GetNewPath(startCords);
    }

    public List<Node> GetNewPath(Vector2Int coordinates)
    {
        gridScript.ResetNode();

        BreadthFirstSearch(coordinates);
        return BuildPath();
    }

    void BreadthFirstSearch(Vector2Int coordinates)
    {
        startNode.walkable = true;
        targetNode.walkable = true;
        
        frontier.Clear();
        reached.Clear();

        bool isRunning = true;
        
        frontier.Enqueue(grid[coordinates]);
        reached.Add(coordinates, grid[coordinates]);

        while (frontier.Count > 0 && isRunning == true)
        {
            currentNode = frontier.Dequeue();
            currentNode.explored = true;
            ExploreNeighbors();
            if (currentNode.cords == targetCords)
            {
                isRunning = false;
                currentNode.walkable = false;
            }
        }
    }

    private void ExploreNeighbors()
    {
        List<Node> neighbors = new List<Node>();

        foreach (Vector2Int direction in searchOrder)
        {
            Vector2Int neighborCords = currentNode.cords + direction;

            if (grid.ContainsKey(neighborCords))
            {
                neighbors.Add(grid[neighborCords]);
            }
        }

        foreach (Node neighbor in neighbors)
        {
            if (!reached.ContainsKey(neighbor.cords) && neighbor.walkable)
            {
                neighbor.connectTo = currentNode;
                reached.Add(neighbor.cords, neighbor);
                frontier.Enqueue(neighbor);
            }
        }
    }

    List<Node> BuildPath()
    {
        List<Node> path = new List<Node>();
        Node currentNode = targetNode;
        
        path.Add(currentNode);
        currentNode.path = true;
        while (currentNode.connectTo != null)
        {
            currentNode = currentNode.connectTo;
            path.Add(currentNode);
            currentNode.path = true;
        }
        
        path.Reverse();
        return path;
    }

    public void NotifyReceivers()
    {
        BroadcastMessage("RecalculatePath",false,SendMessageOptions.DontRequireReceiver);
    }

    public void SetNewDestination(Vector2Int startCords, Vector2Int targetCords)
    {
        this.startCords = startCords;
        this.targetCords = targetCords;
        startNode = grid[this.startCords];
        targetNode = grid[this.targetCords];
        GetNewPath();
    }
}
