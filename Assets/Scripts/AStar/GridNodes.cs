using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class GridNodes
{
    private int _width;

    private int _height;

    private Node[,] _gridNode;

    public GridNodes(int width, int height)
    {
        _width = width;
        _height = height;

        _gridNode = new Node[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _gridNode[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    public Node GetGridNode(int xPos, int yPos)
    {
        if ((xPos >= _width) || (yPos >= _height))
            return null;

        return _gridNode[xPos, yPos];
    }
}
