using UnityEngine;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    private Canvas _canvas;

    private Camera _mainCamera;

    [SerializeField]
    private Image _cursorImage = null;

    [SerializeField]
    private RectTransform _cursorRectTransform = null;

    [SerializeField]
    private Sprite _greenCursorSprite = null;

    [SerializeField]
    private Sprite _transparentCursorSprite = null;

    [SerializeField]
    private GridCursor _gridCursor = null;

    private bool _cursorIsEnabled = false;

    public bool CursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; }

    private bool _cursorPositionIsValid = false;

    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

    private ItemType _selectedItemType;

    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType = value; }

    private float _itemUseRadius = 0f;

    public float ItemUseRadius { get => _itemUseRadius; set => _itemUseRadius = value; }

    private void Start()
    {
        _mainCamera = Camera.main;
        _canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if (_cursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    private void DisplayCursor()
    {
        var cursorWorldPosition = GetWorldPositionForCursor();

        SetCursorValidity(cursorWorldPosition, Player.Instance.GetPlayerCenterPosition());

        _cursorRectTransform.position = GetRecrTransformPositionForCusor();
    }

    private void SetCursorValidity(Vector3 cursorPosition, Vector3 playerPosition)
    {
        SetCursorToValid();

        // Check use radius corners
        if (
            cursorPosition.x > (playerPosition.x + _itemUseRadius / 2f) && cursorPosition.y > (playerPosition.y + _itemUseRadius / 2f)
            ||
            cursorPosition.x < (playerPosition.x - _itemUseRadius / 2f) && cursorPosition.y > (playerPosition.y + _itemUseRadius / 2f)
            ||
            cursorPosition.x < (playerPosition.x - _itemUseRadius / 2f) && cursorPosition.y < (playerPosition.y - _itemUseRadius / 2f)
            ||
            cursorPosition.x > (playerPosition.x + _itemUseRadius / 2f) && cursorPosition.y < (playerPosition.y - _itemUseRadius / 2f))
        {
            SetCursorToInvalid();
            return;
        }

        // Check item use radius is valid
        if (Mathf.Abs(cursorPosition.x - playerPosition.x) > _itemUseRadius
            || Mathf.Abs(cursorPosition.y - playerPosition.y) > _itemUseRadius)
        {
            SetCursorToInvalid();
            return;
        }

        // Get selected item details
        var itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.Player);
        if (itemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        switch (itemDetails.ItemType)
        {
            case ItemType.WateringTool:
            case ItemType.BreakingTool:
            case ItemType.ChoppingTool:
            case ItemType.HoeingTool:
            case ItemType.ReapingTool:
            case ItemType.CollectingTool:
                if (!SetCursorValidityForTool(cursorPosition, playerPosition, itemDetails))
                {
                    SetCursorToInvalid();
                    return;
                }
                break;
        }
    }

    private void SetCursorToValid()
    {
        _cursorImage.sprite = _greenCursorSprite;
        _cursorPositionIsValid = true;
        _gridCursor.DisableCursor();
    }

    private void SetCursorToInvalid()
    {
        _cursorImage.sprite = _transparentCursorSprite;
        _cursorPositionIsValid = false;
        _gridCursor.EnableCursor();
    }

    private bool SetCursorValidityForTool(Vector3 cursorPosition, Vector3 playerPosition, ItemDetails itemDetails)
    {
        switch (itemDetails.ItemType)
        {
            case ItemType.ReapingTool:
                return SetCursorValidityReapingTool(cursorPosition, playerPosition, itemDetails);
        }

        return false;
    }

    private bool SetCursorValidityReapingTool(Vector3 cursorPosition, Vector3 playerPosition, ItemDetails equippedItemDetails)
    {
        foreach (var item in Util.GetComponentsAtCursorLocation<Item>(cursorPosition))
        {
            if (InventoryManager.Instance.GetItemDetails(item.ItemCode).ItemType == ItemType.ReapableScenery)
            {
                return true;
            }
        }

        return false;
    }

    public void EnableCursor()
    {
        _cursorImage.SetImageOpaque();
        _cursorIsEnabled = true;
    }

    public void DisableCursor()
    {
        _cursorImage.SetImageTransparent();
        _cursorIsEnabled = false;
    }

    public Vector3 GetWorldPositionForCursor()
    {
        var screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        return _mainCamera.ScreenToWorldPoint(screenPosition);

    }

    public Vector2 GetRecrTransformPositionForCusor()
    {
        var screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        return RectTransformUtility.PixelAdjustPoint(screenPosition, _cursorRectTransform, _canvas);
    }
}
