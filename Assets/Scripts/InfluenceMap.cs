using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InfluenceNode
{
    public float red,green;
    public Vector3 worldPos;
    public Team winner;

    public InfluenceNode(float red, float green, Vector3 worldPos)
    {
        this.red = red;
        this.green = green;
        this.worldPos = worldPos;
        this.winner = Team.None;
    }
    
    public Team GetStrongest()
    {
        float total = red + green;

        float percentRed = red / total;
        float percentGreen = green / total;

        winner = (percentRed > 0.5f) ? Team.Red : winner;
        winner = (percentGreen > 0.5f) ? Team.Green : winner;

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
    public bool continuousUpdates = false;

    //All units that can effect the influence map
    Influencer[] influencers;

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
    }

    // Update is called once per frame
    void Update () {
        
        // Updates the map once
        if (Input.GetKeyUp(KeyCode.I))
            MakeInfluenceGrid();

        // Toggles continuous map updates
        if (Input.GetKeyUp(KeyCode.O))
            continuousUpdates = !continuousUpdates;

        //Toggles grid visibility
        if (Input.GetKeyUp(KeyCode.P))
            gridIsVisible = !gridIsVisible;

        // If continuously updating... do that.
        if (continuousUpdates)
            MakeInfluenceGrid();
    }

    //Makes an influence map with no calculated influences
    void MakeInfluenceGrid()
    {
        //Get all current units on the field
        influencers = Object.FindObjectsOfType<Influencer>();

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
                grid[x, z] = new InfluenceNode(0, 0, nodePos);

                //Calculate white influence (Strength: 1)
                foreach (Influencer infer in influencers)
                {
                    if (Vector3.Distance(nodePos, infer.transform.position) < influenceDistance)
                    {
                        float falloff = (influenceDistance - Vector3.Distance(nodePos, infer.transform.position)) / influenceDistance;
                        if(infer.team == Team.Red) grid[x, z].red += infer.strength * falloff;
                        if(infer.team == Team.Green) grid[x, z].green += infer.strength * falloff;
                    }
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
                Color nodeColor = Color.gray;
                if (n.winner == Team.Red) nodeColor = Color.red;
                if (n.winner == Team.Green) nodeColor = Color.green;

                nodeColor.a = 0.8f;
                Gizmos.color = nodeColor;

                //draw cube to represent grid with a buffer 5
                Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiameter - 3f));
            }
        }
    }
}
