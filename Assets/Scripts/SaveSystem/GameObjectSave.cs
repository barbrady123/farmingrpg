using System;
using System.Collections.Generic;

[Serializable]
public class GameObjectSave
{
    // Key: Scene Name
    public Dictionary<string, SceneSave> SceneData;

    public GameObjectSave()
    {
        this.SceneData = new Dictionary<string, SceneSave>();
    }

    public GameObjectSave(Dictionary<string, SceneSave> sceneData)
    {
        this.SceneData = sceneData;
    }
}
