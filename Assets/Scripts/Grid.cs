using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    #region Attributes
    public LayerMask unwalkableMask;
    //define the area 
    public Vector3 gridSize;
    //area of each node
    public float nodeRadius;
    //grid
    Node[,,] grid;

    float nodeDiameter;

    int gridSizeX;
    int gridSizeY;
    int gridSizeZ;

    //player pos
    public Transform player;
    public List<Node> path;
    #endregion

    #region Inbuilt Methods
    void Awake()
    {
        //to find # of nodes in the grid
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridSize.z / nodeDiameter);
        CreateGrid();
    }

    //to draw the gizmo
    void OnDrawGizmos()
    {
        // z and y are swapped because z represents up in 3d space
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.z, gridSize.y));

        //draw grid
        if (grid != null)
        {
            Node playerNode = NodePoint(player.position);

            foreach (Node n in grid)
            {
                //if walkable then white, if not then red
                Gizmos.color = (n.walkable) ? Color.white : Color.cyan;
                //path color
                if (path != null)
                {
                    if (path.Contains(n))
                    {
                        Gizmos.color = Color.red;
                    }
                }
                //player color
                if (playerNode == n)
                {
                    Gizmos.color = Color.blue;
                }
                //draw cube to represent grid with a buffer 5
                //DO NOT CHANGE THE BUFFER
                Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiameter - 3f));
            }
        }
    }

    #endregion

    #region Helper Methods
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY, gridSizeZ];
        //left edge of the world
        Vector3 worldBotLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.y / 2 - Vector3.up * gridSize.z / 2;
        //x values
        for (int x = 0; x < gridSizeX; x++)
        {
            //y values
            for (int y = 0; y < gridSizeY; y++)
            {
                //z values
                for (int z = 0; z < gridSizeZ; z++)
                {
                    //increment by diameter until other end is met
                    Vector3 worldPoint = worldBotLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius) + Vector3.up * (z * nodeDiameter + nodeRadius);
                    //check for collision
                    bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                    //add to the grid
                    grid[x, y, z] = new Node(walkable, worldPoint, x, y, z);
                }
            }

        }
    }

    //to return a list of neibhors
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbour = new List<Node>();

        //3x3x3 neibhor check
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    //skip center
                    if (x == 0 && y == 0 && z == 0)
                    {
                        continue;
                    }

                    int checkX = node.gridX + y;
                    int checkY = node.gridY + y;
                    int checkZ = node.gridZ + z;
                    //add neibhors
                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && checkZ >= 0 && checkZ < gridSizeZ)
                    {
                        neighbour.Add(grid[checkX, checkY, checkZ]);
                    }
                }
            }
        }

        return neighbour;
    }

    //to get the playerPos in the grid
    public Node NodePoint(Vector3 worldPos)
    {
        //to get how far along the grid is the node
        float percentX = (worldPos.x + gridSize.x / 2) / gridSize.x;
        float percentY = (worldPos.z + gridSize.y / 2) / gridSize.y;
        //(diameter) to get the current node, worldpOs.y gets the one above 
        float percentZ = (worldPos.y - gridSize.z/(nodeRadius * 2)) / gridSize.z;

        //clamping to avoid going outside the bounds
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        percentZ = Mathf.Clamp01(percentZ);

        //get x y z idecies 
        //gridsize -1 because arrays are 0 based
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);

        //return grid pos
        return grid[x, y, z];
    }
    #endregion
}
