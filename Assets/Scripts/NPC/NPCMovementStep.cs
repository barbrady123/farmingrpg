using UnityEngine;

public class NPCMovementStep
{
    public SceneName SceneName { get; set; }

    public int Hour { get; set; }

    public int Minute { get; set; }

    public int Second { get; set; }

    public Vector2Int GridCoordinate { get; set; }

    public NPCMovementStep(SceneName sceneName, Vector2Int gridCoordinate)
    {
        this.SceneName = sceneName;
        this.GridCoordinate = gridCoordinate;
    }
}
