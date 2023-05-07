using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveLoadManager : SingletonMonobehavior<SaveLoadManager>
{
    public List<ISaveable> SaveableObjectList;

    protected override void Awake()
    {
        base.Awake();

        this.SaveableObjectList = new List<ISaveable>();
    }

    public void StoreCurrentSceneData()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        // Loop through all ISaveable objects and trigger store scene data for each
        foreach (var saveableObject in this.SaveableObjectList)
        {
            saveableObject.ISaveableStoreScene(sceneName);
        }
    }

    public void RestoreCurrentSceneData()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        // Loop through all ISaveable objects and trigger restore scene data for each
        foreach (var saveableObject in this.SaveableObjectList)
        {
            saveableObject.ISaveableRestoreScene(sceneName);
        }
    }
}
