using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var item = collision.GetComponent<Item>();
        if (item == null)
            return;

        if (!InventoryManager.Instance.GetItemDetails(item.ItemCode).CanBePickedUp)
            return;

        if (InventoryManager.Instance.AddItem(InventoryLocation.Player, item, 1))
        {
            Destroy(collision.gameObject);
        }
    }
}
