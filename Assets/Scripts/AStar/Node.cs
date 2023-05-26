using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector2Int GridPosition { get; set; }

    // Distance from starting node
    public int GCost { get; set; }

    // Distance from finishing node
    public int HCost { get; set; }

    public bool IsObstacle { get; set; }

    public int MovementPenalty { get; set; }

    public Node ParentNode { get; set; }

    public int FCost => this.GCost + this.HCost;

    public Node(Vector2Int gridPosition)
    {
        this.GridPosition = gridPosition;
    }

    public int CompareTo(Node other)
    {
        int compare = FCost.CompareTo(other.FCost);
        return (compare != 0) ? compare : HCost.CompareTo(other.HCost);
    }
}
