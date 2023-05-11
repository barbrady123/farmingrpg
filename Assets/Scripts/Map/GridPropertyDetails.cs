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

    public void ClearCropData()
    {
        this.SeedItemCode = -1;
        this.GrowthDays = -1;
        this.DaysSinceLastHarvest = -1;
        this.DaysSinceWatered = -1; // we should probably leave this, seems kind of independent from the crop status...
    }
}
