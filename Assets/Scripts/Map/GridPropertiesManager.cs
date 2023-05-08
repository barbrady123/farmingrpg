using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonobehavior<GridPropertiesManager>, ISaveable
{
    public Grid Grid;

    private Dictionary<string, GridPropertyDetails> _gridPropertyDictionary;

    [SerializeField]
    private SO_GridProperties[] _so_GridPropertiesArray = null;

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
    }

    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
    }

    private void AfterSceneLoaded()
    {
        this.Grid = GameObject.FindObjectOfType<Grid>();
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

    /// <summary>
    /// Should also not be here
    /// </summary>
    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        gridPropertyDetails.GridX = gridX;
        gridPropertyDetails.GridY = gridY;

        gridPropertyDictionary[$"x{gridX}y{gridY}"] = gridPropertyDetails;
    }

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

        if (sceneSave.GridPropertyDetailsDictionary == null)
            return;

        // Scene list items found - destroy existing items in scene
        _gridPropertyDictionary = sceneSave.GridPropertyDetailsDictionary;
    }
}
