using System;

[Serializable]
public class InventoryItem
{
    public int ItemCode;

    public int ItemQuantity;

    public InventoryItem(int itemCode, int itemQuantity = 0)
    {
        this.ItemCode = itemCode;
        this.ItemQuantity = itemQuantity;
    }
}
