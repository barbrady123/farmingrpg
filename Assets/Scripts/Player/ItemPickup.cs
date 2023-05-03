using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var item = collision.GetComponent<Item>();
        if (item == null)
            return;

        var itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

        print(itemDetails.ItemDescription);
    }
}
