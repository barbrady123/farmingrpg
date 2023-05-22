using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : SingletonMonobehavior<SaveLoadManager>
{
    public GameSave GameSave;

    public List<ISaveable> SaveableObjectList;

    private string SaveFileName => $"{Application.persistentDataPath}/WildHopeCreek.dat";

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

    public void LoadDataFromFile()
    {
        var bf = new BinaryFormatter();

        if (!File.Exists(this.SaveFileName))
            return;

        var file = File.Open(this.SaveFileName, FileMode.Open);

        var gameSave = (GameSave)bf.Deserialize(file);

        // loop through all ISaveable objects and apply save data
        foreach (var item in this.SaveableObjectList)
        {
            if (gameSave.GameObjectData.ContainsKey(item.ISaveableUniqueID))
            {
                item.ISaveableLoad(gameSave);
            }
            // else if ISaveableObject unique ID is not in the game object data then destroy object
            else
            {
                Destroy(((Component)item).gameObject);
            }
        }

        UIManager.Instance.DisablePauseMenu();
    }

    public void SaveDataToFile()
    {
        foreach (var iSaveableObject in this.SaveableObjectList)
        {
            GameSave.GameObjectData[iSaveableObject.ISaveableUniqueID] = iSaveableObject.ISaveableSave();
        }

        var bf = new BinaryFormatter();

        var file = File.Open(SaveFileName, FileMode.Create);

        bf.Serialize(file, GameSave);

        file.Close();

        UIManager.Instance.DisablePauseMenu();
    }
}
