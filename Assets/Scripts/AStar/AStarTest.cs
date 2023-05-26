using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(AStar))]
public class AStarTest : MonoBehaviour
{
    private AStar _aStar;

    [SerializeField]
    private Vector2Int _startPosition;

    [SerializeField]
    private Vector2Int _finishPosition;

    [SerializeField]
    private Tilemap _tileMapToDisplayPathOn = null;

    [SerializeField]
    private TileBase _tileToUseToDisplayPath = null;

    [SerializeField]
    private bool _displayStartAndFinish = false;

    [SerializeField]
    private bool _displayPath = false;

    private Stack<NPCMovementStep> _npcMovementSteps;

    private void Awake()
    {
        _aStar = GetComponent<AStar>();
        _npcMovementSteps = new Stack<NPCMovementStep>();
    }

    private void Update()
    {
        if ((_startPosition == null) || (_finishPosition == null) || (_tileMapToDisplayPathOn == null) || (_tileToUseToDisplayPath == null))
            return;

        // Display/Hide start and finish tiles
        _tileMapToDisplayPathOn.SetTile(_startPosition.ToVector3Int(), _displayStartAndFinish ? _tileToUseToDisplayPath : null);
        _tileMapToDisplayPathOn.SetTile(_finishPosition.ToVector3Int(), _displayStartAndFinish ? _tileToUseToDisplayPath : null);

        if (_displayPath)
        {
            Enum.TryParse<SceneName>(SceneManager.GetActiveScene().name, out SceneName sceneName);

            // Build path
            _aStar.BuildPath(sceneName, _startPosition, _finishPosition, _npcMovementSteps);

            foreach (var step in _npcMovementSteps)
            {
                _tileMapToDisplayPathOn.SetTile(step.GridCoordinate.ToVector3Int(), _tileToUseToDisplayPath);
            }
        }
        else
        {
            if (_npcMovementSteps.Any())
            {
                foreach (var step in _npcMovementSteps)
                {
                    _tileMapToDisplayPathOn.SetTile(step.GridCoordinate.ToVector3Int(), null);
                }

                _npcMovementSteps.Clear();
            }
        }
    }
}
