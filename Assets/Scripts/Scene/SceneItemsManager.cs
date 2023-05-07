using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GenerateGUID))]
public class SceneItemsManager : SingletonMonobehavior<SceneItemsManager>, ISaveable
{
    public const string SceneItemListKey = "sceneItemList";

    private Transform _parentItem;

    [SerializeField]
    private GameObject _itemPrefab = null;

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

    private void AfterSceneLoad()
    {
        _parentItem = GameObject.FindGameObjectWithTag(Global.Tags.ItemsParentTransform).transform;
    }

    private void OnEnable()
    {
        ISaveableRegister();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.SaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.SaveableObjectList.Remove(this);
    }

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

        // Create list scene item dictionary in scene save and add to it
        var sceneSave = new SceneSave(SceneItemsManager.SceneItemListKey, sceneItemList);

        // Add scene save to gameobject
        GameObjectSave.SceneData.Add(sceneName, sceneSave);
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        if (!GameObjectSave.SceneData.TryGetValue(sceneName, out var sceneSave))
            return;

        if (!sceneSave.ListSceneItemDictionary.TryGetValue(SceneItemsManager.SceneItemListKey, out var sceneItemList))
            return;

        // Scene list items found - destroy existing items in scene
        DestroySceneItems();

        // Now instantiate the list of scene items
        InstantiateSceneItems(sceneItemList);
    }

    public void InstantiateSceneItem(int itemCode, Vector3 itemPosition)
    {
        var itemGameObject = Instantiate(_itemPrefab, itemPosition, Quaternion.identity, _parentItem);
        var item = itemGameObject.GetComponent<Item>();
        item.Init(itemCode);
    }

    private void DestroySceneItems()
    {
        var itemsInScene = GameObject.FindObjectsOfType<Item>();

        for (int x = itemsInScene.Length - 1; x >= 0; x--)
        {
            Destroy(itemsInScene[x].gameObject);
        }
    }

    private void InstantiateSceneItems(List<SceneItem> sceneItemList)
    {
        foreach (var sceneItem in sceneItemList)
        {
            var itemGameObject = Instantiate(_itemPrefab, sceneItem.Position.ToVector3(), Quaternion.identity, _parentItem);

            var item = itemGameObject.GetComponent<Item>();
            item.ItemCode = sceneItem.ItemCode;
            item.name = sceneItem.ItemName;
        }
    }
}
