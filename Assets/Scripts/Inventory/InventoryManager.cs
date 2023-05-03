using System.Collections.Generic;
using System.Linq;
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

        CreateItemDetailsDictionary();
    }

    private void CreateItemDetailsDictionary()
    {
        this.ItemDetailsDictionary = _itemList.ItemDetails.ToDictionary(x => x.ItemCode);
    }

}
