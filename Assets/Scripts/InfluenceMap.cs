using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InfluenceNode
{
    public float black, blue, white, yellow;
    public Vector3 worldPos;
    public Color winner;

    public InfluenceNode(int black, int yellow, int blue, int white, Vector3 pos)
    {
        this.black = black;
        this.yellow = yellow;
        this.white = white;
        this.blue = blue;

        this.worldPos = pos;

        this.winner = Color.gray;
    }
    public Color GetStrongest()
    {
        float total = black + yellow + white + blue;

        float percentBlack = black / total;
        float percentYellow = yellow / total;
        float percentWhite = white / total;
        float percentBlue = blue / total;

        winner = (percentBlack > 0.5f) ? Color.black : winner;
        winner = (percentYellow > 0.5f) ? Color.yellow : winner;
        winner = (percentWhite > 0.5f) ? Color.white : winner;
        winner = (percentBlue > 0.5f) ? Color.blue : winner;

        return winner;
        
    }
}

public class InfluenceMap : MonoBehaviour {

    #region Attributes
    public LayerMask unwalkableMask;
    //define the area 
    public Vector3 gridSize;
    //area of each node
    public float nodeRadius;
    //grid
    InfluenceNode[,] grid;

    float nodeDiameter;
    public float influenceDistance = 10f;

    int gridSizeX;
    int gridSizeZ;

    public bool gridIsVisible = true;

    //All units that can effect the influence map
    GameObject[] blacks, blues, whites, yellows;

    #endregion

	// Use this for initialization
	void Start () {
		
	}

    void Awake()
    {
        //to find # of nodes in the grid
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        MakeInfluenceGrid();
    }

    // Update is called once per frame
    void Update () {
        MakeInfluenceGrid();
        if (Input.GetKeyUp(KeyCode.I))
        {
            Debug.Log("Recalculating influence map");
            MakeInfluenceGrid();
            Debug.Log("Finished");
        }
	}

    //Makes an influence map with no calculated influences
    void MakeInfluenceGrid()
    {
        //Get all current units on the field
        blacks = GameObject.FindGameObjectsWithTag("blackTeam");
        blues = GameObject.FindGameObjectsWithTag("blueTeam");
        whites = GameObject.FindGameObjectsWithTag("whiteTeam");
        yellows = GameObject.FindGameObjectsWithTag("yellowTeam");

        grid = new InfluenceNode[gridSizeX, gridSizeZ];

        //left edge of the world
        Vector3 worldBotLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.y / 2 - Vector3.up * gridSize.z / 2;
        
        //x values
        for (int x = 0; x < gridSizeX; x++)
        {
            //y values
            for (int z = 0; z < gridSizeZ; z++)
            {
                Vector3 nodePos = worldBotLeft
                    + Vector3.right * (x * nodeDiameter + nodeRadius)
                    + Vector3.forward * (z * nodeDiameter + nodeRadius);
                grid[x, z] = new InfluenceNode(0, 0, 0, 0, nodePos);

                //Calculate white influence (Strength: 1)
                foreach (GameObject white in whites)
                    if (Vector3.Distance(nodePos, white.transform.position) < influenceDistance)
                    {
                        float falloff = (influenceDistance - Vector3.Distance(nodePos, white.transform.position)) / influenceDistance;
                        grid[x, z].white += 1 * falloff;
                    }

                //Calculate blue influence (Strength: 2)
                foreach (GameObject blue in blues)
                    if (Vector3.Distance(nodePos, blue.transform.position) < influenceDistance)
                    {
                        float falloff = (influenceDistance - Vector3.Distance(nodePos, blue.transform.position)) / influenceDistance;
                        grid[x, z].blue += 2* falloff;
                    }

                //Calculate yellow influence (Strength: 3)
                foreach (GameObject yellow in yellows)
                    if (Vector3.Distance(nodePos, yellow.transform.position) < influenceDistance)
                    {
                        float falloff = (influenceDistance - Vector3.Distance(nodePos, yellow.transform.position)) / influenceDistance;
                        grid[x, z].yellow += 3 * falloff;
                    }

                //Calculate black influence (Strength: 4)
                foreach (GameObject black in blacks)
                    if (Vector3.Distance(nodePos, black.transform.position) < influenceDistance)
                    {
                        float falloff = (influenceDistance - Vector3.Distance(nodePos, black.transform.position)) / influenceDistance;
                        grid[x, z].black += 4 * falloff;
                    }

                grid[x, z].GetStrongest();

            }

        }
    }

    //to get the playerPos in the grid
    public InfluenceNode GetNodeFromWorldPos(Vector3 worldPos)
    {
        //to get how far along the grid is the node
        float percentX = (worldPos.x + gridSize.x / 2) / gridSize.x;
        float percentY = (worldPos.z + gridSize.y / 2) / gridSize.y;

        //clamping to avoid going outside the bounds
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        //get x y z idecies 
        //gridsize -1 because arrays are 0 based
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeZ - 1) * percentY);

        //return grid pos
        return grid[x, y];
    }

    //to draw the gizmo
    void OnDrawGizmos()
    {
        if (!gridIsVisible) return;

        // z and y are swapped because z represents up in 3d space
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.z, gridSize.y));

        //draw grid
        if (grid != null)
        {
            foreach (InfluenceNode n in grid)
            {
                //if walkable then white, if not then red
                Color translucent = n.winner;
                translucent.a = 0.8f;
                Gizmos.color = translucent;

                //draw cube to represent grid with a buffer 5
                Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiameter - 3f));
            }
        }
    }
}
