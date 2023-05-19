using System.Collections.Generic;
using UnityEngine;

public class PauseMenuInventoryManagement : MonoBehaviour
{
    [SerializeField]
    private PauseMenuInventoryManagementSlot[] _inventoryManagementSlot = null;

    [SerializeField]
    private Sprite transparent16x16 = null;

    [HideInInspector]
    public GameObject InventoryTextBoxGameObject;

    public GameObject InventoryManagementDraggedItemPrefab;

    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += PopulatePlayerInventory;

        if (InventoryManager.Instance == null)
            return;

        PopulatePlayerInventory(InventoryLocation.Player, InventoryManager.Instance.InventoryLists[(int)InventoryLocation.Player]);
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= PopulatePlayerInventory;

        DestroyInventoryTextBoxGameObject();
    }

    public void DestroyInventoryTextBoxGameObject()
    {
        Destroy(this.InventoryTextBoxGameObject);
    }

    public void DestroyCurrentlyDraggedItems()
    {
        foreach (var slot in _inventoryManagementSlot)
        {
            Destroy(slot.DraggedItem);
        }
    }

    private void PopulatePlayerInventory(InventoryLocation inventoryLocation, List<InventoryItem> playerInventoryList)
    {
        if (inventoryLocation == InventoryLocation.Player)
        {
            InitializeInventoryManagementSlot();

            foreach (var inventoryItem in playerInventoryList.WithIndex())
            {
                // Get inventory item details
                _inventoryManagementSlot[inventoryItem.index].ItemDetails = InventoryManager.Instance.GetItemDetails(inventoryItem.item.ItemCode);
                _inventoryManagementSlot[inventoryItem.index].ItemQuantity = inventoryItem.item.ItemQuantity;

                if (_inventoryManagementSlot[inventoryItem.index].ItemDetails != null)
                {
                    // update inventory management slot with image and quality
                    _inventoryManagementSlot[inventoryItem.index].InventoryManagementSlotImage.sprite = _inventoryManagementSlot[inventoryItem.index].ItemDetails.ItemSprite;
                    _inventoryManagementSlot[inventoryItem.index].TextMeshProUGUI.text = _inventoryManagementSlot[inventoryItem.index].ItemQuantity.ToString();
                }
            }
        }
    }

    private void InitializeInventoryManagementSlot()
    {
        int currentMaxCapacity = InventoryManager.Instance.InventoryListCapacityIntArray[(int)InventoryLocation.Player];

        for (int x = 0; x < Settings.PlayerMaximumInventoryCapacity; x++)
        {
            if (x < currentMaxCapacity)
            {
                _inventoryManagementSlot[x].ItemDetails = null;
                _inventoryManagementSlot[x].ItemQuantity = 0;
                _inventoryManagementSlot[x].InventoryManagementSlotImage.sprite = transparent16x16;
                _inventoryManagementSlot[x].TextMeshProUGUI.text = "";
            }

            _inventoryManagementSlot[x].GreyedOutImageGO.SetActive(x >= currentMaxCapacity);
        }
    }
}
