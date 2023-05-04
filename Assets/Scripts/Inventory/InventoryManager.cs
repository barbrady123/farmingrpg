using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class InventoryManager : SingletonMonobehavior<InventoryManager>
{
    private Dictionary<int, ItemDetails> ItemDetailsDictionary;

    [SerializeField]
    private so_ItemList _itemList = null;

    public List<InventoryItem>[] InventoryLists;

    [HideInInspector]
    public int[] InventoryListCapaciyIntArray;

    public ItemDetails GetItemDetails(int itemCode) => this.ItemDetailsDictionary.TryGetValue(itemCode, out var details) ? details : null;

    protected override void Awake()
    {
        base.Awake();

        CreateInventoryLists();
        CreateItemDetailsDictionary();
    }

    private void CreateInventoryLists()
    {
        int count = Enum.GetNames(typeof(InventoryLocation)).Length;

        this.InventoryLists = new List<InventoryItem>[count];

        for (int x = 0; x < this.InventoryLists.Length; x++)
        {
            this.InventoryLists[x] = new List<InventoryItem>(Settings.PlayerInitialInventoryCapacity);
        }

        this.InventoryListCapaciyIntArray = new int[this.InventoryLists.Length];
        this.InventoryListCapaciyIntArray[(int)InventoryLocation.Player] = Settings.PlayerInitialInventoryCapacity;
    }

    private void CreateItemDetailsDictionary()
    {
        this.ItemDetailsDictionary = _itemList.ItemDetails.ToDictionary(x => x.ItemCode);
    }

    public string GetItemTypeDescription(ItemType itemType)
    {
        return (itemType) switch
        {
            ItemType.BreakingTool => Global.Tools.Breaking,
            ItemType.ChoppingTool => Global.Tools.Chopping,
            ItemType.HoeingTool => Global.Tools.Hoeing,
            ItemType.ReapingTool => Global.Tools.ReapingTool,
            ItemType.WateringTool => Global.Tools.WateringTool,
            ItemType.CollectingTool => Global.Tools.CollectingTool,
            _ => itemType.ToString()
        };
    }

    public bool AddItem(InventoryLocation inventoryLocation, Item item, int quantity = 1)
    {
        if (quantity <= 0)
            return false;

        var inventory = this.InventoryLists[(int)inventoryLocation];
        int capacity = this.InventoryListCapaciyIntArray[(int)inventoryLocation];

        var currentItem = inventory.FirstOrDefault(x => x.ItemCode == item.ItemCode);
        if (currentItem == null)
        {
            if (inventory.Count >= capacity)
                return false;

            currentItem = new InventoryItem(item.ItemCode);
            inventory.Add(currentItem);
        }

        currentItem.ItemQuantity++;

        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventory);

        return true;
    }

    public void RemoveItem(InventoryLocation inventoryLocation, int itemCode, int quantity = 1)
    {
        if (quantity <= 0)
            return;

        var inventory = this.InventoryLists[(int)inventoryLocation];

        var currentItem = inventory.FirstOrDefault(x => x.ItemCode == itemCode);
        if (currentItem == null)
            return;

        currentItem.ItemQuantity -= quantity;

        if (currentItem.ItemQuantity <= 0)
        {
            inventory.Remove(currentItem);
        }

        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventory);
    }

    public void SwapInventoryItem(InventoryLocation inventoryLocation, int fromSlotNumber, int toSlotNumber)
    {
        var inventory = this.InventoryLists[(int)inventoryLocation];

        var fromItem = inventory[fromSlotNumber];
        var toItem = inventory[toSlotNumber];

        inventory[fromSlotNumber] = toItem;
        inventory[toSlotNumber ] = fromItem;

        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventory);
    }
}
