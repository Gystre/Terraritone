using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector2 worldPos;
    public int gridX;
    public int gridY;

    public int gCost; //distance from starting node
    public int hCost; //(heuristic cost) distance from end node
    public Node parent;
    int heapIndex;

    public Node(bool walkable, Vector2 worldPos, int x, int y)
    {
        this.walkable = walkable;
        this.worldPos = worldPos;
        this.gridX = x;
        this.gridY = y;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }

        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost); //returns 1 if higher
        }

        return -compare; //want to return 1 if lower so make negative
    }

    public override string ToString()
    {
        return "walkable: " + walkable + " worldPos: " + worldPos.X + "x " + worldPos.Y + "y" + " gridXY " + gridX + "x " + gridY + "y";
    }
}
