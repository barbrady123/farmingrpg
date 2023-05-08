using System;

[Serializable]
public class GridPropertyDetails
{
    public int GridX;

    public int GridY;

    public bool IsDiggable = false;

    public bool CanDropItem = false;

    public bool CanPlaceFurniture = false;

    public bool IsPath = false;

    public bool IsNPCObstacle = false;

    public int DaysSinceDug = -1;

    public int DaysSinceWatered = -1;

    public int SeedItemCode = -1;

    public int GrowthDays = -1;

    public int DaysSinceLastHarvest = -1;
}
