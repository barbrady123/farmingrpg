using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public const int UnitDistance = 10;

    public const int DiagonalUnitDistance = 14;

    [Header("Tiles & Tilemap References")]
    [Header("Options")]
    [SerializeField]
    private bool _observeMovementPenalties = true;

    [Range(0, 20)]
    [SerializeField]
    private int _pathMovementPenalty = 0;

    [Range(0, 20)]
    [SerializeField]
    private int _defaultMovementPenalty = 0;

    private GridNodes _gridNodes;

    private Node _startNode;

    private Node _targetNode;

    private int _gridWidth;

    private int _gridHeight;

    private int _originX;

    private int _originY;

    private List<Node> _openNodeList;

    private HashSet<Node> _closedNodeList;

    private bool _pathFound = false;

    public bool BuildPath(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
    {
        if (!PopulateGridNodesFromGridPropertiesDictionary(sceneName, startGridPosition, endGridPosition))
            return false;

        if (!FindShortestPath())
            return false;

        UpdatePathOnMovementStepStack(sceneName, npcMovementStepStack);
        return true;
    }

    private bool PopulateGridNodesFromGridPropertiesDictionary(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition)
    {
        if (!GridPropertiesManager.Instance.GameObjectSave.SceneData.TryGetValue(sceneName.ToString(), out var sceneSave))
            return false;

        if (sceneSave.GridPropertyDetailsDictionary == null)
            return false;

        (var gridDimensions, var gridOrigin) = GridPropertiesManager.Instance.GetGridDimensions(sceneName);

        // Create nodes grid based on grid properties dictionary
        _gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
        _gridWidth = gridDimensions.x;
        _gridHeight = gridDimensions.y;
        _originX = gridOrigin.x;
        _originY = gridOrigin.y;

        _openNodeList = new List<Node>();
        _closedNodeList = new HashSet<Node>();

        // Populate start node
        _startNode = _gridNodes.GetGridNode(startGridPosition.x - _originX, startGridPosition.y - _originY);

        // Populate target node
        _targetNode = _gridNodes.GetGridNode(endGridPosition.x - _originX, endGridPosition.y - _originY);

        // Populate obstacle and path info for grid
        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                var gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(x + _originX, y + _originY, sceneSave.GridPropertyDetailsDictionary);
                if (gridPropertyDetails == null)
                    continue;

                // If NPC Obstacle
                if (gridPropertyDetails.IsNPCObstacle)
                {
                    _gridNodes.GetGridNode(x, y).IsObstacle = true;
                }
                else if (gridPropertyDetails.IsPath)
                {
                    _gridNodes.GetGridNode(x, y).MovementPenalty = _pathMovementPenalty;
                }
                else
                {
                    _gridNodes.GetGridNode(x, y).MovementPenalty = _defaultMovementPenalty;
                }
            }
        }

        return true;
    }

    private bool FindShortestPath()
    {
        // Add start node to open list
        _openNodeList.Add(_startNode);

        // Loop through open node list until empty
        while (_openNodeList.Any())
        {
            _openNodeList.Sort();

            // First will be the lowest fCost
            var currentNode = _openNodeList.First();
            _openNodeList.RemoveAt(0);

            _closedNodeList.Add(currentNode);

            if (currentNode == _targetNode)
            {
                _pathFound = true;
                break;
            }

            EvaluateCurrentNodeNeighbors(currentNode);
        }

        return _pathFound;
    }

    private void EvaluateCurrentNodeNeighbors(Node currentNode)
    {
        var currentNodeGridPosition = currentNode.GridPosition;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if ((x == 0) && (y == 0))
                    continue;

                var validNeighborNode = GetValidNode(currentNodeGridPosition.x + x, currentNodeGridPosition.y + y);
                if (validNeighborNode == null)
                    continue;

                // Calculate new gcost for neighbor
                int newCostToNeighbor = currentNode.GCost + GetDistance(currentNode, validNeighborNode);
                if (_observeMovementPenalties)
                    newCostToNeighbor += validNeighborNode.MovementPenalty;

                bool isValidNeighborNodeInOpenList = _openNodeList.Contains(validNeighborNode);

                if ((newCostToNeighbor < validNeighborNode.GCost) || (!isValidNeighborNodeInOpenList))
                {
                    validNeighborNode.GCost = newCostToNeighbor;
                    validNeighborNode.HCost = GetDistance(validNeighborNode, _targetNode);
                    validNeighborNode.ParentNode = currentNode;

                    if (!isValidNeighborNodeInOpenList)
                    {
                        _openNodeList.Add(validNeighborNode);
                    }
                }
            }
        }
    }

    // Returns the node if it's valid
    private Node GetValidNode(int xPos, int yPos)
    {
        if ((xPos >= _gridWidth) || (yPos >= _gridHeight) || (xPos < 0) || (yPos < 0))
            return null;

        var node = _gridNodes.GetGridNode(xPos, yPos);

        if (node.IsObstacle || _closedNodeList.Contains(node))
            return null;

        return node;
    }

    private int GetDistance(Node node1, Node node2)
    {
        int xDiff = Mathf.Abs(node1.GridPosition.x - node2.GridPosition.x);
        int yDiff = Mathf.Abs(node1.GridPosition.y - node2.GridPosition.y);

        // 14 = rough diagonal distance approximation
        // Go "diagonal" (14) for short axis distance, then "straight" (10) for the longer axis distance
        return
            (xDiff > yDiff) ?
            (yDiff * DiagonalUnitDistance) + (UnitDistance * (xDiff - yDiff)) :
            (xDiff * DiagonalUnitDistance) + (UnitDistance * (yDiff - xDiff));
    }

    private void UpdatePathOnMovementStepStack(SceneName sceneName, Stack<NPCMovementStep> npcMovementStepStack)
    {
        var nextNode = _targetNode;

        while (nextNode != null)
        {
            npcMovementStepStack.Push(
                new NPCMovementStep(
                    sceneName,
                    new Vector2Int(
                        nextNode.GridPosition.x + _originX,
                        nextNode.GridPosition.y + _originY
                    )));

            nextNode = nextNode.ParentNode;
        }
    }
}
