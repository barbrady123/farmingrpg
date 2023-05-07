using System;
using System.Collections.Generic;

[Serializable]
public class SceneSave
{
    public Dictionary<string, List<SceneItem>> ListSceneItemDictionary;

    public SceneSave()
    {
        this.ListSceneItemDictionary = new Dictionary<string, List<SceneItem>>();
    }

    public SceneSave(string key, List<SceneItem> sceneItems) : this()
    {
        this.ListSceneItemDictionary.Add(key, sceneItems);
    }
}
