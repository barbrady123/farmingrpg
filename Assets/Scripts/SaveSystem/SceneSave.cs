using System;
using System.Collections.Generic;

[Serializable]
public class SceneSave
{
    public List<SceneItem> ListSceneItem;

    public Dictionary<string, GridPropertyDetails> GridPropertyDetailsDictionary;

    public Dictionary<string, bool> BoolDictionary;

    public Dictionary<string, string> StringDictionary;

    public Dictionary<string, Vector3Serializable> Vector3Dictionary;

    public Dictionary<string, int[]> IntArrayDictionary;

    public Dictionary<string, int> IntDictionary;

    public List<InventoryItem>[] ListInvItemArray;

    public SceneSave()
    {
        this.BoolDictionary = new Dictionary<string, bool>();
        this.StringDictionary = new Dictionary<string, string>();
        this.Vector3Dictionary = new Dictionary<string, Vector3Serializable>();
        this.IntArrayDictionary = new Dictionary<string, int[]>();
        this.IntDictionary = new Dictionary<string, int>();
    }

    public SceneSave(List<SceneItem> sceneItems) : this()
    {
        this.ListSceneItem = sceneItems;
    }

    public SceneSave(Dictionary<string,GridPropertyDetails> gridPropertyDetails) : this()
    {
        this.GridPropertyDetailsDictionary = gridPropertyDetails;
    }
}
