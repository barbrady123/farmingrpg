using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Camera _camera;

    private Canvas _parentCanvas;

    private Transform _parentItem;

    private GameObject _draggedItem;

    [SerializeField]
    private UIInventoryBar _inventoryBar = null;

    [SerializeField]
    private GameObject _itemPrefab = null;

    [SerializeField]
    private int _slotNumber = 0;

    [SerializeField]
    private GameObject _inventoryTextBoxPrefab = null;

    public Image InventorySlotHighlight;

    public Image InventorySlotImage;

    public TextMeshProUGUI TextMeshProUGUI;

    [HideInInspector]
    public ItemDetails ItemDetails;

    [HideInInspector]
    public int ItemQuantity;

    [HideInInspector]
    public bool IsSelected = false;

    private void Awake()
    {
        _parentCanvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        _camera = Camera.main;
        _parentItem = GameObject.FindGameObjectWithTag(Global.Tags.ItemsParentTransform).transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (this.ItemDetails == null)
            return;

        Player.Instance.SetPlayerInputDisabled();

        // Instantiate gameobject as dragged item
        _draggedItem = Instantiate(_inventoryBar.InventoryBarDraggedItem, _inventoryBar.transform);

        // Get image for dragged item
        var draggedItemImage = _draggedItem.GetComponentInChildren<Image>();
        draggedItemImage.sprite = this.InventorySlotImage.sprite;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_draggedItem == null)
            return;

        _draggedItem.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_draggedItem == null)
            return;

        Destroy(_draggedItem);

        // If drag ends over inventory bar , get item drag is over and swap them
        var slot = eventData.pointerCurrentRaycast.gameObject?.GetComponent<UIInventorySlot>();
        if (slot != null)
        {
            InventoryManager.Instance.SwapInventoryItem(InventoryLocation.Player, _slotNumber, slot._slotNumber);
            Destroy(_inventoryBar.InventoryTextBoxGameObject);
        }
        // else attempt to drop the item if it can be dropped
        else if (this.ItemDetails.CanBeDropped) {
            DropSelectedItemAtMousePosition();
        }

        Player.Instance.SetPlayerInputDisabled(false);
    }

    private void DropSelectedItemAtMousePosition()
    {
        if (this.ItemDetails == null)
            return;

        var worldPosition =
            _camera.ScreenToWorldPoint(
                new Vector3(
                    Input.mousePosition.x,
                    Input.mousePosition.y,
                    -_camera.transform.position.z));

        // Create item from prefab at mouse position
        var itemGameObject = Instantiate(_itemPrefab, worldPosition, Quaternion.identity, _parentItem);
        var item = itemGameObject.GetComponent<Item>();
        item.ItemCode = ItemDetails.ItemCode;

        // Remove Item from players inventory
        InventoryManager.Instance.RemoveItem(InventoryLocation.Player, item.ItemCode);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.ItemQuantity == 0)
            return;

        _inventoryBar.InventoryTextBoxGameObject = Instantiate(_inventoryTextBoxPrefab, transform.position, Quaternion.identity);
        _inventoryBar.InventoryTextBoxGameObject.transform.SetParent(_parentCanvas.transform, false);

        var inventoryTextBox = _inventoryBar.InventoryTextBoxGameObject.GetComponent<UIInventoryTextBox>();

        string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(this.ItemDetails.ItemType);

        inventoryTextBox.SetTextboxText(
            this.ItemDetails.ItemDescription,
            itemTypeDescription,
            "",
            this.ItemDetails.ItemLongDescription,
            "",
            "");

        if (_inventoryBar.IsAtBottom)
        {
            _inventoryBar.InventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
            _inventoryBar.InventoryTextBoxGameObject.transform.position = new Vector3(
                transform.position.x,
                transform.position.y + 50f,
                transform.position.z);
        }
        else
        {
            _inventoryBar.InventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
            _inventoryBar.InventoryTextBoxGameObject.transform.position = new Vector3(
                transform.position.x,
                transform.position.y - 50f,
                transform.position.z);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(_inventoryBar.InventoryTextBoxGameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (this.IsSelected)
        {
            ClearSelectedItem();
        }
        else if (this.ItemQuantity > 0)
        {
            SetSelectedItem();
        }
    }

    public void ClearSelectedItem()
    {
        if (!this.IsSelected)
            return;

        this.IsSelected = false;
        this.InventorySlotHighlight.SetImageTransparent();
        InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.Player);

        Player.Instance.ClearCarriedItem();
    }

    public void SetSelectedItem()
    {
        if (this.IsSelected)
            return;

        _inventoryBar.ClearHighlightOnInventorySlots();

        this.IsSelected = true;
        this.InventorySlotHighlight.SetImageOpaque();
        InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.Player, this.ItemDetails.ItemCode);

        if (this.ItemDetails.CanBeCarried)
        {
            Player.Instance.ShowCarriedItem(this.ItemDetails.ItemCode);
        }
        else
        {
            Player.Instance.ClearCarriedItem();
        }
    }
}
