using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Crop : MonoBehaviour
{
    private int _harvestActionCount = 0;

    [SerializeField]
    [Tooltip("This should be populated from the child transform gameobject showing harvest effect spawn point")]
    private Transform _harvestActionEffectTransform = null;

    [SerializeField]
    [Tooltip("This should be populated from the child gameobject")]
    private SpriteRenderer _cropHarvestedSpriteRenderer = null;

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

    public void ProcessToolAction(ItemDetails itemDetails, bool isToolRight, bool isToolLeft, bool isToolUp, bool isToolDown)
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

        // Get animator for crop, if present
        var animator = GetComponentInChildren<Animator>();

        if (animator != null)
        {
            if (isToolRight || isToolUp)
            {
                animator.SetTrigger(Global.Animations.Triggers.UseToolRight);
            }
            else if (isToolLeft || isToolDown)
            {
                animator.SetTrigger(Global.Animations.Triggers.UseToolLeft);
            }
        }

        // Trigger tool particle effect on crop
        if (cropDetails.IsHarvestActionEffect)
        {
            EventHandler.CallHarvestActionEffectEvent(_harvestActionEffectTransform.position, cropDetails.HarvestActionEffect);
        }

        _harvestActionCount++;

        if (_harvestActionCount >= requiredHarvestActions)
        {
            HarvestCrop(gridPropertyDetails, cropDetails, animator, isToolRight || isToolUp);
        }
    }

    private void HarvestCrop(GridPropertyDetails gridPropertyDetails, CropDetails cropDetails, Animator animator, bool usingToolRight)
    {
        // Handle animation if exists
        bool runningAnimation = false;

        if (cropDetails.IsHarvestedAnimation && (animator != null))
        {
            if (cropDetails.HarvestedSprite != null)
            {
                _cropHarvestedSpriteRenderer.sprite = cropDetails.HarvestedSprite;
            }

            animator.SetTrigger(usingToolRight ? Global.Animations.Triggers.HarvestRight : Global.Animations.Triggers.HarvestLeft);
            runningAnimation = true;
        }

        AudioManager.Instance.PlaySound(cropDetails.HarvestSound);

        // Delete crop from grid properties
        gridPropertyDetails.ClearCropData();

        // Should the crop be hidden before the harvest animation
        if (cropDetails.HideCropBeforeHarvestedAnimation)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        // Should box collisders be disabled before harvest
        // (this is necessary for a "crop" like the rock, where it's box collider will "push" the player when it's up animation runs)
        if (cropDetails.DisableCropCollidersBeforeHarvestedAnimation)
        {
            foreach (var collider in GetComponentsInChildren<Collider2D>())
            {
                collider.enabled = false;
            }
        }

        StartCoroutine(ProcessHarvestActionsAfterAnimation(gridPropertyDetails, cropDetails, runningAnimation, animator));
    }

    private IEnumerator ProcessHarvestActionsAfterAnimation(GridPropertyDetails gridPropertyDetails, CropDetails cropDetails, bool runningAnimation, Animator animator)
    {
        if (runningAnimation)
        {
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))
            {
                yield return null;
            }
        }

        SpawnHarvestedItems(cropDetails);

        // Does this crop transform into another crop
        if (cropDetails.HarvestedTransformItemCode > 0)
        {
            CreateHarvestedTransformCrop(gridPropertyDetails, cropDetails);
        }

        Destroy(gameObject);
    }

    private void CreateHarvestedTransformCrop(GridPropertyDetails gridPropertyDetails, CropDetails cropDetails)
    {
        gridPropertyDetails.SeedItemCode = cropDetails.HarvestedTransformItemCode;
        gridPropertyDetails.GrowthDays = 0;
        gridPropertyDetails.DaysSinceLastHarvest = -1;
        gridPropertyDetails.DaysSinceWatered = -1;

        GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
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
