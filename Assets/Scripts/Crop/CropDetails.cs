using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class CropDetails
{
    [ItemCodeDescription]
    public int SeedItemCode;    // this is the item code for the corresponding seed

    public int[] GrowthDays;    // total growth days for each stage

    public GameObject[] GrowthPrefab;   // Prefab to use when instantiating growth stages

    public Sprite[] GrowthSprite;       // Growth sprite

    public Season[] Seasons;        // growth seasons

    public Sprite HarvestedSprite;      // sprite used once harvested

    [ItemCodeDescription]
    public int HarvestedTransformItemCode;      // if the item transforms into another item when harvested this item code will be populated

    public bool HideCropBeforeHarvestedAnimation;       // if the crop should be disabled before the harvested animation

    public bool DisableCropCollidersBeforeHarvestedAnimation;       // if colliders on the crop should be disabled to avoid the harvested animation effecting any other game objects

    public bool IsHarvestedAnimation;       // true is harvested animation to be played on final growth stage prefab

    public bool IsHarvestActionEffect = false;      // flag to determine whether there is a harvest action effect

    public bool SpawnCropProducedInPlayerInventory;

    public HarvestActionEffect HarvestActionEffect;     // the harvest action effect for the crop

    [ItemCodeDescription]
    public int[] HarvestToolItemCode;       // array of item codes for the tools that can harvest or 0 array elements if no tool is required

    public int[] RequiresHarvestActions;    // Number of harvest actions required for the corresponding tool in harvest tool item code array

    [ItemCodeDescription]
    public int[] CropProducedItemCode;      // Array of item codes produced for the harvested crop

    public int[] CropProducedMinQuantity;   // minimum quantities produced for the harvested crop

    public int[] CropProducedMaxQuantity;   // if max quantity > min quantity then a random number of crops between min and max are produced

    public int DaysToRegrow;    // days to regrow next crop or -1 if a single crop

    public SoundName HarvestSound;      // The harvest sound for the crop

    public bool CanUseToolToHarvestCrop(int toolItemCode) => RequiresHarvestActionsForTool(toolItemCode) != -1;

    public int RequiresHarvestActionsForTool(int toolItemCode)
    {
        var harvestTool = HarvestToolItemCode.Cast<int?>().WithIndex().FirstOrDefault(x => x.item == toolItemCode);
        return harvestTool.item != null ? this.RequiresHarvestActions[harvestTool.index] : -1;
    }

    public int GetGrowthStageForDays(int days)
    {
        for (int x = this.GrowthDays.Length - 1; x >= 0; x--)
        {
            if (days >= this.GrowthDays[x])
                return x;
        }

        throw new Exception($"Unexpected error calculating growth stage for days '{days}'");
    }
}
