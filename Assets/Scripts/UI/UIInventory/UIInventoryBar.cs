using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIInventoryBar : MonoBehaviour
{
    [SerializeField]
    private Sprite _blankSprite = null;

    [SerializeField]
    private UIInventorySlot[] _inventorySlot = null;

    private RectTransform _rectTransform;

    private bool _isAtBottom = true;

    public bool IsAtBottom { get => _isAtBottom; set => _isAtBottom = value; }

    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += InventoryUpdated;
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= InventoryUpdated;
    }

    private void InventoryUpdated(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if (inventoryLocation != InventoryLocation.Player)
            return;

        ClearInventorySlots();

        for (int x = 0; x < _inventorySlot.Length; x++)
        {
            if (x >= inventoryList.Count)
                break;

            var itemDetails = InventoryManager.Instance.GetItemDetails(inventoryList[x].ItemCode);

            if (itemDetails != null)
            {
                _inventorySlot[x].InventorySlotImage.sprite = itemDetails.ItemSprite;
                _inventorySlot[x].TextMeshProUGUI.text = inventoryList[x].ItemQuantity.ToString();
                _inventorySlot[x].ItemDetails = itemDetails;
                _inventorySlot[x].ItemQuantity = inventoryList[x].ItemQuantity;
            }
        }
    }

    private void ClearInventorySlots()
    {
        foreach (var slot in _inventorySlot)
        {
            slot.InventorySlotImage.sprite = _blankSprite;
            slot.TextMeshProUGUI.text = null;
            slot.ItemDetails = null;
            slot.ItemQuantity = 0;
        }
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        SwitchInventoryBarPosition();
    }

    private void SwitchInventoryBarPosition()
    {
        var playerViewportPosition = Player.Instance.GetViewportPosition();

        if (playerViewportPosition.y > 0.3f && !_isAtBottom)
        {
            _rectTransform.pivot = new Vector2(0.5f, 0f);
            _rectTransform.anchorMin = new Vector2(0.5f, 0f);
            _rectTransform.anchorMax = new Vector2(0.5f, 0f);
            _rectTransform.anchoredPosition = new Vector2(0.0f, 2.5f);
            _isAtBottom = true;
        }
        else if (playerViewportPosition.y <= 0.3f && _isAtBottom)
        {
            _rectTransform.pivot = new Vector2(0.5f, 1f);
            _rectTransform.anchorMin = new Vector2(0.5f, 1f);
            _rectTransform.anchorMax = new Vector2(0.5f, 1f);
            _rectTransform.anchoredPosition = new Vector2(0.0f, -2.5f);
            _isAtBottom = false;
        }
    }


}
