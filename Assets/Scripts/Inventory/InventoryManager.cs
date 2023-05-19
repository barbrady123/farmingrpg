using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : SingletonMonobehavior<InventoryManager>
{
    private Dictionary<int, ItemDetails> ItemDetailsDictionary;

    [SerializeField]
    private so_ItemList _itemList = null;

    public List<InventoryItem>[] InventoryLists;

    private int[] _selectedInventoryItem;

    [HideInInspector]
    public int[] InventoryListCapacityIntArray;

    public ItemDetails GetItemDetails(int itemCode) => this.ItemDetailsDictionary.TryGetValue(itemCode, out var details) ? details : null;

    public ItemDetails GetSelectedInventoryItemDetails(InventoryLocation inventoryLocation)
    {
        int itemCode = GetSelectedInventoryItem(inventoryLocation);
        return itemCode > 0 ? GetItemDetails(itemCode) : null;
    }

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

        this.InventoryListCapacityIntArray = new int[this.InventoryLists.Length];
        this.InventoryListCapacityIntArray[(int)InventoryLocation.Player] = Settings.PlayerInitialInventoryCapacity;

        _selectedInventoryItem = new int[this.InventoryLists.Length];
        _selectedInventoryItem.SetAllToValue(-1);
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

    public bool AddItem(InventoryLocation inventoryLocation, int itemCode, int quantity = 1)
    {
        if (quantity <= 0)
            return false;

        var inventory = this.InventoryLists[(int)inventoryLocation];
        int capacity = this.InventoryListCapacityIntArray[(int)inventoryLocation];

        var currentItem = inventory.FirstOrDefault(x => x.ItemCode == itemCode);
        if (currentItem == null)
        {
            if (inventory.Count >= capacity)
                return false;

            currentItem = new InventoryItem(itemCode);
            inventory.Add(currentItem);
        }

        currentItem.ItemQuantity++;

        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventory);

        return true;
    }

    public bool AddItem(InventoryLocation inventoryLocation, Item item, int quantity = 1) => AddItem(inventoryLocation, item.ItemCode, quantity);

    /// <summary>
    /// Returns remaining quantity
    /// </summary>
    public int RemoveItem(InventoryLocation inventoryLocation, int itemCode, int quantity = 1)
    {
        var inventory = this.InventoryLists[(int)inventoryLocation];

        var currentItem = inventory.FirstOrDefault(x => x.ItemCode == itemCode);
        if (currentItem == null)
            return 0;

        if (quantity <= 0)
            return currentItem.ItemQuantity;

        currentItem.ItemQuantity -= quantity;

        if (currentItem.ItemQuantity <= 0)
        {
            inventory.Remove(currentItem);
        }

        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventory);
        return currentItem.ItemQuantity;
    }

    public void SwapInventoryItem(InventoryLocation inventoryLocation, int fromSlotNumber, int toSlotNumber)
    {
        var inventory = this.InventoryLists[(int)inventoryLocation];

        var fromItem = inventory[fromSlotNumber];

        if (toSlotNumber >= inventory.Count)
            toSlotNumber = inventory.Count - 1;

        var toItem = inventory[toSlotNumber];

        inventory[fromSlotNumber] = toItem;
        inventory[toSlotNumber ] = fromItem;

        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventory);
    }

    /// <summary>
    /// This is terrible as it only expects an item with a given itemCode to be in a single location...should set this by index
    /// </summary>
    public void SetSelectedInventoryItem(InventoryLocation inventoryLocation, int itemCode)
    {
        _selectedInventoryItem[(int)inventoryLocation] = itemCode;
    }

    public void ClearSelectedInventoryItem(InventoryLocation inventoryLocation) => SetSelectedInventoryItem(inventoryLocation, -1);

    public int GetSelectedInventoryItem(InventoryLocation inventoryLocation) => _selectedInventoryItem[(int)inventoryLocation];
}
