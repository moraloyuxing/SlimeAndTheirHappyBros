using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathFinding : MonoBehaviour {


    PathRequestManager requestManager;
    PathFindGrid grid;

    void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<PathFindGrid>();
    }


    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        startNode.parent = startNode;

        bool hasTarget = false;
        if (!targetNode.walkable)
        {
            foreach (Node neighbour in grid.GetNeighbours(targetNode))
            {
                if (neighbour.walkable)
                {
                    targetNode = neighbour;
                    hasTarget = true;
                    break;
                }
            }
        }
        else hasTarget = true;

        if (hasTarget) {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);
                //Debug.Log(currentNode.gridX + "   " + currentNode.gridY);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    //Debug.Log("neighbor" + neighbour.gridX + "   " + neighbour.gridY);
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        //Debug.Log("continue  (" + neighbour.gridX + "," + neighbour.gridY + ")");

                        if (neighbour == targetNode) //!neighbour.walkable && 
                        {
                            pathSuccess = true;
                            targetNode = currentNode;
                            break;
                        }
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
            yield return null;
        }

        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
            //Debug.Log(waypoints);
            requestManager.FinishedProcessingPath(waypoints, pathSuccess);
        }
        else {
            Debug.Log("find path fail");
            //Vector3[] temp = new Vector3[1];
            requestManager.FinishedProcessingPath(null, pathSuccess);
        }
    }


    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        Vector3[] waypoints;
        //Debug.Log(endNode.worldPosition);
        if (startNode == endNode)
        {
            waypoints = new Vector3[1] { endNode.worldPosition };
            return waypoints;
        }
        else {
            while (currentNode != startNode)
            {
                
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
        }
        waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        //Debug.Log("gg" + waypoints.Length);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        //Debug.Log("path length" + path.ToArray().Length);
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        if (path.Count > 1)
        {
            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
                if (directionNew != directionOld)
                {
                    waypoints.Add(path[i - 1].worldPosition);
                }
                directionOld = directionNew;
            }
        }
        else {
            waypoints.Add(path[0].worldPosition);
        }
        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

}
