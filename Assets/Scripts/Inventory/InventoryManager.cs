using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

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
            this.InventoryLists[x] = new List<InventoryItem>();
        }

        this.InventoryListCapaciyIntArray = new int[this.InventoryLists.Length];
        this.InventoryListCapaciyIntArray[(int)InventoryLocation.Player] = Settings.PlayerInitialInventoryCapacity;
    }

    private void CreateItemDetailsDictionary()
    {
        this.ItemDetailsDictionary = _itemList.ItemDetails.ToDictionary(x => x.ItemCode);
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

        OutputToDebug(inventory);

        return true;
    }

    private void OutputToDebug(List<InventoryItem> inventory)
    {
        foreach (var item in inventory)
        {
            print($"Item: {InventoryManager.Instance.GetItemDetails(item.ItemCode).ItemDescription} [{item.ItemCode}].  Qty: {item.ItemQuantity}");
        }

        print("-------------------------------------------------------");
    }
}
