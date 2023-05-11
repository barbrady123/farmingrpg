using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class GridCursor : MonoBehaviour
{
    private Canvas _canvas;

    private Grid _grid;

    private Camera _mainCamera;

    [SerializeField]
    private Image _cursorImage = null;

    [SerializeField]
    private RectTransform _cursorRectTransform = null;

    [SerializeField]
    private Sprite _greenCursorSprite = null;

    [SerializeField]
    private Sprite _redCursorSprite = null;

    [SerializeField]
    private SO_CropDetailsList _so_CropDetailsList = null;

    private bool _cursorPositionIsValid = false;

    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

    private int _itemUseGridRadius = 0;

    public int ItemUseGridRadius { get => _itemUseGridRadius; set => _itemUseGridRadius = value; }

    private ItemType _selectedItemType;

    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType = value; }

    private bool _cursorIsEnabled = false;

    public bool CursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
    }

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

    private Vector3Int DisplayCursor()
    {
        if (_grid == null)
            return Vector3Int.zero;

        var gridPosition = GetGridPositionForCursor();
        var playerPosition = GetGridPositionForPlayer();

        SetCursorValidity(gridPosition, playerPosition);

        _cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

        return gridPosition;
    }

    private void SceneLoaded()
    {
        _grid = GameObject.FindObjectOfType<Grid>();
    }

    private void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        SetCursorToValid();

        // Check item use radius is valid
        // This is NOT the right way to check radius...
        if (Mathf.Abs(cursorGridPosition.x - playerGridPosition.x) > _itemUseGridRadius ||
            Mathf.Abs(cursorGridPosition.y - playerGridPosition.y) > _itemUseGridRadius)
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

        // Get grid property details at cursor position
        var gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        if (gridPropertyDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        switch (itemDetails.ItemType)
        {
            case ItemType.Seed:
                if (!IsCursorValidForSeed(gridPropertyDetails))
                {
                    SetCursorToInvalid();
                }
                break;
            case ItemType.Commodity:
                if (!IsCursorValidForCommodity(gridPropertyDetails))
                {
                    SetCursorToInvalid();
                }
                break;
            case ItemType.WateringTool:
            case ItemType.BreakingTool:
            case ItemType.ChoppingTool:
            case ItemType.HoeingTool:
            case ItemType.ReapingTool:
            case ItemType.CollectingTool:
                if (!IsCursorValidForTool(gridPropertyDetails, itemDetails))
                {
                    SetCursorToInvalid();
                }
                break;
        }
    }

    private void SetCursorToInvalid()
    {
        _cursorImage.sprite = _redCursorSprite;
        _cursorPositionIsValid = false;
    }

    private void SetCursorToValid()
    {
        _cursorImage.sprite = _greenCursorSprite;
        _cursorPositionIsValid = true;
    }

    private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        switch (itemDetails.ItemType)
        {
            case ItemType.HoeingTool:
                if ((!gridPropertyDetails.IsDiggable) || (gridPropertyDetails.DaysSinceDug >= 0))
                    return false;

                // Need to get any items at location so we can check if they are reapable
                var cursorBaseWorldPosition = GetWorldPositionForCursor();
                var cursorWorldPosition = new Vector3(cursorBaseWorldPosition.x + 0.5f, cursorBaseWorldPosition.y + 0.5f, 0f);

                // Loops through Items found to see if any are reapable type - we are not going to let the player dig where there are reapable scenery items
                var reapableItem =
                    Util.GetComponentsAtBoxLocation<Item>(
                        cursorWorldPosition,
                        Settings.CursorSize,
                        0f)
                        .FirstOrDefault(x => InventoryManager.Instance.GetItemDetails(x.ItemCode).ItemType == ItemType.ReapableScenery);

                return (reapableItem == null);
            case ItemType.WateringTool:
                return (gridPropertyDetails.DaysSinceDug >= 0) && (gridPropertyDetails.DaysSinceWatered < 0);
            case ItemType.CollectingTool:
                var cropDetails = _so_CropDetailsList.GetCropDetails(gridPropertyDetails.SeedItemCode);
                if (cropDetails == null)
                    return false;

                if (gridPropertyDetails.GrowthDays < cropDetails.TotalGrowthDays)
                    return false;

                return cropDetails.CanUseToolToHarvestCrop(itemDetails.ItemCode);
        }

        return false;
    }

    private bool IsCursorValidForCommodity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.CanDropItem;
    }

    private bool IsCursorValidForSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.CanDropItem;
    }

    public void DisableCursor()
    {
        _cursorImage.color = Color.clear;
        _cursorIsEnabled = false;
    }

    public void EnableCursor()
    {
        _cursorImage.color = Color.white;
        _cursorIsEnabled = true;
    }

    private Vector3 GetWorldPositionForCursor() => _grid.CellToWorld(GetGridPositionForCursor());

    public Vector3Int GetGridPositionForCursor()
    {
        var worldPosition =
            _mainCamera.ScreenToWorldPoint(
                new Vector3(
                    Input.mousePosition.x,
                    Input.mousePosition.y,
                    -_mainCamera.transform.position.z));

        return _grid.WorldToCell(worldPosition);
    }

    public Vector3Int GetGridPositionForPlayer()
    {
        return _grid.WorldToCell(Player.Instance.transform.position);
    }

    public Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        var gridWorldPosition = _grid.CellToWorld(gridPosition);
        var gridScreenPosition = _mainCamera.WorldToScreenPoint(gridWorldPosition);
        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, _cursorRectTransform, _canvas);
    }
}
