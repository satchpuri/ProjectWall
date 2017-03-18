using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    #region Attributes
    //to see if the node is walkable or not
    public bool walkable;
    //get the position
    public Vector3 worldPos;
    public int gridX;
    public int gridY;
    public int gridZ;

    //f(h) = g(h) + h(h);
    public int gVal;
    public int hVal;
    public Node parent;
    #endregion

    #region Properties
    public int fVal
    {
        get
        {
            return gVal + hVal;
        }
    }
    #endregion

    #region Helper Methods
    public Node(bool walk, Vector3 pos, int x, int y, int z)
    {
        gridX = x;
        gridY = y;
        gridZ = z;
        walkable = walk;
        worldPos = pos;
    }
    #endregion
}

