using System;
using System.Collections.Generic;

[Serializable]
public class SceneSave
{
    public List<SceneItem> ListSceneItem;

    public Dictionary<string, GridPropertyDetails> GridPropertyDetailsDictionary;

    public SceneSave() { }

    public SceneSave(List<SceneItem> sceneItems)
    {
        this.ListSceneItem = sceneItems;
    }

    public SceneSave(Dictionary<string,GridPropertyDetails> gridPropertyDetails)
    {
        this.GridPropertyDetailsDictionary = gridPropertyDetails;
    }
}
