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
                }
            }
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

    public void ISaveableStoreScene(string sceneName)
    {
        // Remove old scene save for gameObject if exists
        GameObjectSave.SceneData.Remove(sceneName);

        var sceneItemList = new List<SceneItem>();

        // Loop through all scene items
        foreach (var item in FindObjectsOfType<Item>())
        {
            sceneItemList.Add(new SceneItem {
                ItemCode = item.ItemCode,
                Position = new Vector3Serializable(item.transform.position),
                ItemName = item.name
            });
        }

        // Create list scene items in scene save, then add it to gameobject
        GameObjectSave.SceneData.Add(sceneName, new SceneSave(sceneItemList));
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
