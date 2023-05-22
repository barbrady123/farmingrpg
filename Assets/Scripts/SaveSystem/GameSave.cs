using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSave
{
    // Key: GUID GameObject ID
    public Dictionary<string, GameObjectSave> GameObjectData;

    public GameSave()
    {
        this.GameObjectData = new Dictionary<string, GameObjectSave>();
    }
}
