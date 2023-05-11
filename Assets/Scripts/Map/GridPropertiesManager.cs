using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Video;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonobehavior<GridPropertiesManager>, ISaveable
{
    private Grid _grid;

    private Transform _cropParentTransform;

    private Tilemap _groundDecoration1;

    private Tilemap _groundDecoration2;

    private Dictionary<string, GridPropertyDetails> _gridPropertyDictionary;

    [SerializeField]
    private Tile[] _dugGround = null;

    [SerializeField]
    private Tile[] _wateredGround = null;

    [SerializeField]
    private SO_GridProperties[] _so_GridPropertiesArray = null;

    [SerializeField]
    private SO_CropDetailsList _so_CropDetailsList = null;

    private string _iSaveableUniqueID;

    public string ISaveableUniqueID { get => _iSaveableUniqueID; set => _iSaveableUniqueID = value; }

    private GameObjectSave _gameObjectSave;

    public GameObjectSave GameObjectSave { get => _gameObjectSave; set => _gameObjectSave = value; }

    protected override void Awake()
    {
        base.Awake();

        _iSaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        _gameObjectSave = new GameObjectSave();
    }

    private void Start()
    {
        InitializeGridProperties();
    }

    private void ClearDisplayGroundDecorations()
    {
        _groundDecoration1.ClearAllTiles();
        _groundDecoration2.ClearAllTiles();
    }

    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayGroundDecorations();
        ClearDisplayAllPlantedCrops();
    }

    private void ClearDisplayAllPlantedCrops()
    {
        foreach (var crop in FindObjectsOfType<Crop>())
        {
            Destroy(crop.gameObject);
        }
    }

    public void DisplayWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.DaysSinceWatered >= 0)
        {
            ConnectWateredGround(gridPropertyDetails);
        }
    }

    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.DaysSinceDug >= 0)
        {
            ConnectDugGround(gridPropertyDetails);
        }
    }

    private void DisplayGridPropertyDetails()
    {
        foreach (var item in _gridPropertyDictionary)
        {
            DisplayDugGround(item.Value);
            DisplayWateredGround(item.Value);
            DisplayPlantedCrop(item.Value);
        }
    }

    public void DisplayPlantedCrop(GridPropertyDetails gridPropertyDetails)
    {
        var cropDetails = _so_CropDetailsList.GetCropDetails(gridPropertyDetails.SeedItemCode);
        if (cropDetails == null)
            return;

        // Find the current growsth stage
        int currentStage = cropDetails.GetGrowthStageForDays(gridPropertyDetails.GrowthDays);

        var worldPosition = _groundDecoration2.CellToWorld(new Vector3Int(gridPropertyDetails.GridX, gridPropertyDetails.GridY, 0));
        worldPosition = new Vector3(worldPosition.x + Settings.GridCellSize / 2, worldPosition.y, worldPosition.z);

        Crop.Create(
            cropDetails.GrowthPrefab[currentStage],
            worldPosition,
            cropDetails.GrowthSprite[currentStage],
            _cropParentTransform,
            gridPropertyDetails.GridX,
            gridPropertyDetails.GridY);
    }

    private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
    {
        // Select tile based on surrounding dug tiles
        var dugTile0 = SetDugTile(gridPropertyDetails.GridX, gridPropertyDetails.GridY);
        _groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.GridX, gridPropertyDetails.GridY, 0), dugTile0);

        // Set 4 tiles if dug surrounding current tile - up, down, left, right now that this central tile has been dug
        var adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.GridX, gridPropertyDetails.GridY + 1);
        if (adjacentGridPropertyDetails?.DaysSinceDug >= 0)
        {
            var dugTile1 = SetDugTile(gridPropertyDetails.GridX, gridPropertyDetails.GridY + 1);
            _groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.GridX, gridPropertyDetails.GridY + 1, 0), dugTile1);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.GridX, gridPropertyDetails.GridY - 1);
        if (adjacentGridPropertyDetails?.DaysSinceDug >= 0)
        {
            var dugTile2 = SetDugTile(gridPropertyDetails.GridX, gridPropertyDetails.GridY - 1);
            _groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.GridX, gridPropertyDetails.GridY - 1, 0), dugTile2);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.GridX + 1, gridPropertyDetails.GridY);
        if (adjacentGridPropertyDetails?.DaysSinceDug >= 0)
        {
            var dugTile3 = SetDugTile(gridPropertyDetails.GridX + 1, gridPropertyDetails.GridY);
            _groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.GridX + 1, gridPropertyDetails.GridY, 0), dugTile3);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.GridX - 1, gridPropertyDetails.GridY);
        if (adjacentGridPropertyDetails?.DaysSinceDug >= 0)
        {
            var dugTile4 = SetDugTile(gridPropertyDetails.GridX - 1, gridPropertyDetails.GridY);
            _groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.GridX - 1, gridPropertyDetails.GridY, 0), dugTile4);
        }
    }

    private void ConnectWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        // Select tile based on surrounding watered tiles
        var wateredTile0 = SetWateredTile(gridPropertyDetails.GridX, gridPropertyDetails.GridY);
        _groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.GridX, gridPropertyDetails.GridY, 0), wateredTile0);

        // Set 4 tiles if dug surrounding current tile - up, down, left, right now that this central tile has been dug
        var adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.GridX, gridPropertyDetails.GridY + 1);
        if (adjacentGridPropertyDetails?.DaysSinceWatered >= 0)
        {
            var wateredTile1 = SetWateredTile(gridPropertyDetails.GridX, gridPropertyDetails.GridY + 1);
            _groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.GridX, gridPropertyDetails.GridY + 1, 0), wateredTile1);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.GridX, gridPropertyDetails.GridY - 1);
        if (adjacentGridPropertyDetails?.DaysSinceWatered >= 0)
        {
            var wateredTile2 = SetWateredTile(gridPropertyDetails.GridX, gridPropertyDetails.GridY - 1);
            _groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.GridX, gridPropertyDetails.GridY - 1, 0), wateredTile2);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.GridX + 1, gridPropertyDetails.GridY);
        if (adjacentGridPropertyDetails?.DaysSinceWatered >= 0)
        {
            var wateredTile3 = SetWateredTile(gridPropertyDetails.GridX + 1, gridPropertyDetails.GridY);
            _groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.GridX + 1, gridPropertyDetails.GridY, 0), wateredTile3);
        }

        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.GridX - 1, gridPropertyDetails.GridY);
        if (adjacentGridPropertyDetails?.DaysSinceWatered >= 0)
        {
            var wateredTile4 = SetWateredTile(gridPropertyDetails.GridX - 1, gridPropertyDetails.GridY);
            _groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.GridX - 1, gridPropertyDetails.GridY, 0), wateredTile4);
        }
    }

    private bool IsGridSquareDug(int gridX, int gridY) => GetGridPropertyDetails(gridX, gridY)?.DaysSinceDug >= 0;

    private bool IsGridSquareWatered(int gridX, int gridY) => GetGridPropertyDetails(gridX, gridY)?.DaysSinceWatered >= 0;

    private Tile SetDugTile(int gridX, int gridY)
    {
        // Get whether surrounding tiles (up, down, left, right) are dug or not
        bool dugUp = IsGridSquareDug(gridX, gridY + 1);
        bool dugDown = IsGridSquareDug(gridX, gridY - 1);
        bool dugRight = IsGridSquareDug(gridX + 1, gridY);
        bool dugLeft = IsGridSquareDug(gridX - 1, gridY);

        // Set appropriate tile based on whether surrounding tiles are dug or not
        if (!dugUp)
        {
            if (!dugDown)
            {
                if (!dugLeft)
                {
                    return (!dugRight) ? _dugGround[0] : _dugGround[13];
                }
                else
                {
                    return (!dugRight) ? _dugGround[15] : _dugGround[14];
                }
            }
            else
            {
                if (!dugLeft)
                {
                    return (!dugRight) ? _dugGround[4] : _dugGround[1];
                }
                else
                {
                    return (!dugRight) ? _dugGround[3] : _dugGround[2];
                }
            }
        }
        else
        {
            if (!dugDown)
            {
                if (!dugLeft)
                {
                    return (!dugRight) ? _dugGround[12] : _dugGround[9];
                }
                else
                {
                    return (!dugRight) ? _dugGround[11] : _dugGround[10];
                }
            }
            else
            {
                if (!dugLeft)
                {
                    return (!dugRight) ? _dugGround[8] : _dugGround[5];
                }
                else
                {
                    return (!dugRight) ? _dugGround[7] : _dugGround[6];
                }
            }
        }
    }

    private Tile SetWateredTile(int gridX, int gridY)
    {
        // Get whether surrounding tiles (up, down, left, right) are dug or not
        bool wateredUp = IsGridSquareWatered(gridX, gridY + 1);
        bool wateredDown = IsGridSquareWatered(gridX, gridY - 1);
        bool wateredRight = IsGridSquareWatered(gridX + 1, gridY);
        bool wateredLeft = IsGridSquareWatered(gridX - 1, gridY);

        // Set appropriate tile based on whether surrounding tiles are dug or not
        if (!wateredUp)
        {
            if (!wateredDown)
            {
                if (!wateredLeft)
                {
                    return (!wateredRight) ? _wateredGround[0] : _wateredGround[13];
                }
                else
                {
                    return (!wateredRight) ? _wateredGround[15] : _wateredGround[14];
                }
            }
            else
            {
                if (!wateredLeft)
                {
                    return (!wateredRight) ? _wateredGround[4] : _wateredGround[1];
                }
                else
                {
                    return (!wateredRight) ? _wateredGround[3] : _wateredGround[2];
                }
            }
        }
        else
        {
            if (!wateredDown)
            {
                if (!wateredLeft)
                {
                    return (!wateredRight) ? _wateredGround[12] : _wateredGround[9];
                }
                else
                {
                    return (!wateredRight) ? _wateredGround[11] : _wateredGround[10];
                }
            }
            else
            {
                if (!wateredLeft)
                {
                    return (!wateredRight) ? _wateredGround[8] : _wateredGround[5];
                }
                else
                {
                    return (!wateredRight) ? _wateredGround[7] : _wateredGround[6];
                }
            }
        }
    }

    private void InitializeGridProperties()
    {
        foreach (var so_GridProperties in _so_GridPropertiesArray)
        {
            var gridPropertyDictionary = new Dictionary<string, GridPropertyDetails>();

            foreach (var gridProperty in so_GridProperties.GridPropertyList)
            {
                var gridPropertyDetails = GetGridPropertyDetails(gridProperty.GridCoordinate.X, gridProperty.GridCoordinate.Y, gridPropertyDictionary) ?? new GridPropertyDetails();

                switch (gridProperty.GridBoolProperty)
                {
                    case GridBoolProperty.Diggable:
                        gridPropertyDetails.IsDiggable = gridProperty.GridBoolValue;
                        break;
                    case GridBoolProperty.CanDropItem:
                        gridPropertyDetails.CanDropItem = gridProperty.GridBoolValue;
                        break;
                    case GridBoolProperty.CanPlaceFurniture:
                        gridPropertyDetails.CanPlaceFurniture = gridProperty.GridBoolValue;
                        break;
                    case GridBoolProperty.IsPath:
                        gridPropertyDetails.IsPath = gridProperty.GridBoolValue;
                        break;
                    case GridBoolProperty.IsNPCObstacle:
                        gridPropertyDetails.IsNPCObstacle = gridProperty.GridBoolValue;
                        break;
                }

                SetGridPropertyDetails(gridProperty.GridCoordinate.X, gridProperty.GridCoordinate.Y, gridPropertyDetails, gridPropertyDictionary);
            }

            // Create scene save for this gameobject
            var sceneSave = new SceneSave(gridPropertyDictionary);

            // If starting scene set the gridPropertyDictionary member variable to the current iteration
            if (so_GridProperties.SceneName.ToString() == SceneControllerManager.Instance.StartSceneName.ToString())
            {
                _gridPropertyDictionary = gridPropertyDictionary;
            }

            // Add scene save to gameobject scene data
            GameObjectSave.SceneData.Add(so_GridProperties.SceneName.ToString(), sceneSave);
        }
    }

    private void OnEnable()
    {
        ISaveableRegister();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent += AdvanceGameDay;
    }

    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
        EventHandler.AdvanceGameDayEvent -= AdvanceGameDay;
    }

    private void AfterSceneLoaded()
    {
        _grid = GameObject.FindObjectOfType<Grid>();
        _groundDecoration1 = GameObject.FindGameObjectWithTag(Global.Tags.GroundDecoration1).GetComponent<Tilemap>();
        _groundDecoration2 = GameObject.FindGameObjectWithTag(Global.Tags.GroundDecoration2).GetComponent<Tilemap>();
        _cropParentTransform = GameObject.FindGameObjectWithTag(Global.Tags.CropsParentTransform)?.transform;
    }

    // This isn't that great as this will advance objects that possibly were set seconds prior to the "next day"...
    // Really objects need to track their datetimes and check for advancing on a more granular level...
    private void AdvanceGameDay(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        ClearDisplayGridPropertyDetails();

        foreach (var so_GridProperties in _so_GridPropertiesArray)
        {
            if (!GameObjectSave.SceneData.TryGetValue(so_GridProperties.SceneName.ToString(), out var sceneSave))
                continue;

            if (sceneSave?.GridPropertyDetailsDictionary == null)
                continue;

            foreach (var item in sceneSave.GridPropertyDetailsDictionary)
            {
                if (item.Value.GrowthDays >= 0)
                {
                    item.Value.GrowthDays++;
                }

                if (item.Value.DaysSinceWatered >= 0)
                {
                    item.Value.DaysSinceWatered = -1;
                }
            }
        }

        DisplayGridPropertyDetails();
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.SaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.SaveableObjectList.Remove(this);
    }

    /// <summary>
    /// This lookup should not be here...
    /// </summary>
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string, GridPropertyDetails> gridPropertyDictionary) =>
        gridPropertyDictionary.TryGetValue($"x{gridX}y{gridY}", out var gridPropertyDetails) ? gridPropertyDetails : null;

    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY) => GetGridPropertyDetails(gridX, gridY, _gridPropertyDictionary);

    public GridPropertyDetails GetGridPropertyDetails(Vector2Int pos) => GetGridPropertyDetails(pos.x, pos.y);

    public CropDetails GetCropDetails(int seedItemCode) => _so_CropDetailsList.GetCropDetails(seedItemCode);

    /// <summary>
    /// Should also not be here
    /// </summary>
    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        gridPropertyDetails.GridX = gridX;
        gridPropertyDetails.GridY = gridY;

        gridPropertyDictionary[$"x{gridX}y{gridY}"] = gridPropertyDetails;
    }

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails) => SetGridPropertyDetails(gridX, gridY, gridPropertyDetails, _gridPropertyDictionary);

    public void ISaveableStoreScene(string sceneName)
    {
        // Remove old scene save for gameObject if exists
        GameObjectSave.SceneData.Remove(sceneName);

        // Add scene save to game object scene data
        GameObjectSave.SceneData.Add(sceneName, new SceneSave(_gridPropertyDictionary));
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        if (!GameObjectSave.SceneData.TryGetValue(sceneName, out var sceneSave))
            return;

        if (sceneSave.GridPropertyDetailsDictionary != null)
        {
            // Get grid property details dictionary - it exists since we created it in initialize
            _gridPropertyDictionary = sceneSave.GridPropertyDetailsDictionary;
        }

        // If grid properties exist
        if (_gridPropertyDictionary.Any())
        {
            // Grid property details found for the current scene destroy existing ground decoration
            ClearDisplayGridPropertyDetails();

            // Instantiate grid property details for current scene
            DisplayGridPropertyDetails();
        }
    }

    public Crop GetCropObjectAtGridLocation(int gridX, int gridY)
    {
        var colliders = Physics2D.OverlapPointAll(_grid.GetCellCenterWorld(new Vector3Int(gridX, gridY, 0)));

        foreach (var collider in colliders)
        {
            var crop = collider.gameObject.GetComponentInParent<Crop>() ?? collider.gameObject.GetComponentInChildren<Crop>();
            if (crop != null)
                return crop;
        }

        return null;
    }
}
