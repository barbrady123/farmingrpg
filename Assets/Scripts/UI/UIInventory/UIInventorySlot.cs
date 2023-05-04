using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventorySlot : MonoBehaviour
{
    public Image InventorySlotHighlight;
    
    public Image InventorySlotImage;

    public TextMeshProUGUI TextMeshProUGUI;

    [HideInInspector]
    public ItemDetails ItemDetails;

    [HideInInspector]
    public int ItemQuantity;
}
