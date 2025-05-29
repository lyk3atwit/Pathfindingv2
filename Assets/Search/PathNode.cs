using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public Tile tile;
    public PathNode parent;
    public float gCost;
    public float hCost;
    public float FCost => gCost + hCost;
}