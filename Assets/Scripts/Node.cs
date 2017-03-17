using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    //to see if the node is walkable or not
    public bool walkable;
    //get the position
    public Vector3 worldPos;

    public Node(bool walk, Vector3 pos)
    {
        walkable = walk;
        worldPos = pos;
    }
}
