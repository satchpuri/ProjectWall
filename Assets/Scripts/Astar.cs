using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Astar : MonoBehaviour
{
    #region Attributes
    //start and end pos
    public Transform seeker;
    public Transform target;
    Grid grid;
    #endregion

    #region Inbuilt Methods
    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    void Update()
    {
        //give positions of the seeker and the target
        PathFinding(seeker.position, target.position);
    }
    #endregion

    #region helperMethods
    void PathFinding(Vector3 startPos, Vector3 targetPos)
    {
        //get grid positions of start and target node
        Node startNode = grid.NodePoint(startPos);
        Node targetNode = grid.NodePoint(targetPos);

        //list of nodes to be checked
        List<Node> checkSet = new List<Node>();
        //nodes already checked
        HashSet<Node> checkedSet = new HashSet<Node>();
        //add starting position to the list of nodes to be checked
        checkSet.Add(startNode);

        //loop until empty
        while (checkSet.Count > 0)
        {
            //current node
            Node currentNode = checkSet[0];
            for (int i = 1; i < checkSet.Count; i++)
            {
                //find lowest fVal and assign to currentNode
                if (checkSet[i].fVal < currentNode.fVal || checkSet[i].fVal == currentNode.fVal)
                {
                    //if (fVal is same then check for hVal
                    if (checkSet[i].hVal < currentNode.hVal)
                    {
                        currentNode = checkSet[i];
                    }
                }
            }
            //remove from open
            checkSet.Remove(currentNode);
            //add to closed
            checkedSet.Add(currentNode);

            //if on target
            if (currentNode == targetNode)
            {
                DrawPath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                //check if neighbour is not walkable or already checked
                if (!neighbour.walkable || checkedSet.Contains(neighbour))
                {
                    //go to the next one
                    continue;
                }
                
                //set new fVal (fVal = gval + hval)
                int neighbourCost = currentNode.gVal + GetDistance(currentNode, neighbour);
                if (neighbourCost < neighbour.gVal || !checkSet.Contains(neighbour))
                {
                    neighbour.gVal = neighbourCost;
                    neighbour.hVal = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;
                    //if not added to check list, add it
                    if (!checkSet.Contains(neighbour))
                    {
                        checkSet.Add(neighbour);
                    }
                }
            }
        }
    }
    
    //to get the distance
    int GetDistance(Node nodeA, Node nodeB)
    {
        //get distance for each axis
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        int distZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);

        //to calculate distance in 3D. 
        //for 2d 14y + 10(x-y) or 14x + 10(y-x)
        if (distX > distZ)
        {
            return (14 * distZ + 10 * (distX - distZ) + 10*distY);
        }
        return (14 * distX + 10 * (distZ - distX) + 10 * distY);
    }

    //to draw the path
    void DrawPath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        //get path in reverse
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        //2 -ves make a +ve
        path.Reverse();
        grid.path = path;
    }
    #endregion
}


