using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public LayerMask unwalkableMask;
    //define the area 
    public Vector3 gridSize;
    //area of each node
    public float nodeRadius;
    //grid
    Node[, ,] grid;
    float nodeDiameter;
    int gridSizeX;
    int gridSizeY;
    int gridSizeZ;
    //player pos
    public Transform player;
    void Start()
    {
        //to find # of nodes in the grid
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridSize.z / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY, gridSizeZ];
        //left edge of the world
        Vector3 worldBotLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.y / 2 - Vector3.up * gridSize.z/2;
        //x values
        for(int x = 0; x<gridSizeX; x++)
        {
            //y values
            for (int y = 0; y < gridSizeY; y++)
            {
                //z values
                for (int z = 0; z < gridSizeZ; z++)
                {
                    //increment by diameter until other end is met
                    Vector3 worldPoint = worldBotLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius) + Vector3.up * (z * nodeDiameter +nodeRadius);
                    //check for collision
                    bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                    //add to the grid
                    grid[x, y, z] = new Node(walkable, worldPoint);
                }
            }

        }
    }

    //to get the playerPos in the grid
    public Node NodePoint(Vector3 worldPos)
    {
        //to get how far along the grid is the node
        float percentX = (worldPos.x + gridSize.x / 2) / gridSize.x;
        float percentY = (worldPos.z + gridSize.y / 2) / gridSize.y;
        //(diameter) to get the current node, worldpOs.y gets the one above 
        float percentZ = (worldPos.y-(nodeRadius*2)) / gridSize.z;


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
    //to draw the gizmo
    void OnDrawGizmos()
    {
        // z and y are swapped because z represents up in 3d space
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.z, gridSize.y));

        //draw grid
        if(grid !=null)
        {
            Node playerNode = NodePoint(player.position);

            foreach(Node n in grid)
            {
                //if walkable then white, if not then red
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                if(playerNode == n)
                {
                    Gizmos.color = Color.blue;
                }
                //draw cube to represent grid with a buffer 5
                //DO NOT CHANGE THE BUFFER
                Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiameter - 3f));

            }
        }
    }
}
