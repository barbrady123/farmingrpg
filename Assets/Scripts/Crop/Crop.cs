using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Crop : MonoBehaviour
{
    private int _harvestActionCount = 0;

    [HideInInspector]
    public Vector2Int CropGridPosition;

    public static GameObject Create(
        GameObject prefab,
        Vector3 position,
        Sprite sprite,
        Transform parentTransform,
        int gridPosX,
        int gridPosY)
    {
        var cropInstance = Instantiate(prefab, position, Quaternion.identity);
        cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        cropInstance.transform.SetParent(parentTransform);
        cropInstance.GetComponent<Crop>().CropGridPosition = new Vector2Int(gridPosX, gridPosY);
        return cropInstance;
    }

    public void ProcessToolAction(ItemDetails itemDetails)
    {
        var gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(this.CropGridPosition);
        if (gridPropertyDetails == null)
            return;

        var seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.SeedItemCode);
        if (seedItemDetails == null)
            return;

        var cropDetails = GridPropertiesManager.Instance.GetCropDetails(seedItemDetails.ItemCode);
        if (cropDetails == null)
            return;

        int requiredHarvestActions = cropDetails.RequiresHarvestActionsForTool(itemDetails.ItemCode);
        if (requiredHarvestActions < 0)
            return;

        _harvestActionCount++;

        if (_harvestActionCount >= requiredHarvestActions)
        {
            HarvestCrop(gridPropertyDetails, cropDetails);
        }
    }

    private void HarvestCrop(GridPropertyDetails gridPropertyDetails, CropDetails cropDetails)
    {
        // Delete crop from grid properties
        gridPropertyDetails.ClearCropData();
        SpawnHarvestedItems(cropDetails);
        Destroy(gameObject);
    }

    private void SpawnHarvestedItems(CropDetails cropDetails)
    {
        foreach (var cropProduced in cropDetails.CropProducedItemCode.WithIndex())
        {
            int minAmount = cropDetails.CropProducedMinQuantity[cropProduced.index];
            int maxAmount = Math.Max(minAmount, cropDetails.CropProducedMaxQuantity[cropProduced.index]);
            int amount = Random.Range(minAmount, maxAmount + 1);

            for (int x = 0; x < amount; x++)
            {
                if (cropDetails.SpawnCropProducedInPlayerInventory)
                {
                    InventoryManager.Instance.AddItem(InventoryLocation.Player, cropProduced.item);
                }
                else
                {
                    var spawnPosition =
                        new Vector3(
                            transform.position.x + Random.Range(-1f, 1f),
                            transform.position.y + Random.Range(-1f, 1f),
                            0f);

                    SceneItemsManager.Instance.InstantiateSceneItem(cropProduced.item, spawnPosition);
                }
            }
        }
    }
}
