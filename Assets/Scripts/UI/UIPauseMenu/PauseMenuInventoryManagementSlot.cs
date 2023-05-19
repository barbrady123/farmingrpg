using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuInventoryManagementSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private PauseMenuInventoryManagement _inventoryManagement = null;

    [SerializeField]
    private GameObject _inventoryTextBoxPrefab = null;

    [SerializeField]
    private int _slotNumber;

    // private Vector3 _startingPosition;

    private Canvas _parentCanvas;

    public Image InventoryManagementSlotImage;

    public TextMeshProUGUI TextMeshProUGUI;

    public GameObject GreyedOutImageGO;

    [HideInInspector]
    public ItemDetails ItemDetails;

    [HideInInspector]
    public int ItemQuantity;

    public GameObject DraggedItem;

    private void Awake()
    {
        _parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (this.ItemQuantity <= 0)
            return;

        // Instantiate gameobject as dragged item
        this.DraggedItem = Instantiate(_inventoryManagement.InventoryManagementDraggedItemPrefab, _inventoryManagement.transform);

        // Get image for dragged item
        var draggedItemImage = this.DraggedItem.GetComponentInChildren<Image>();
        draggedItemImage.sprite = this.InventoryManagementSlotImage.sprite;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (this.DraggedItem == null)
            return;

        this.DraggedItem.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (this.DraggedItem == null)
            return;

        Destroy(this.DraggedItem);

        var destSlot = eventData.pointerCurrentRaycast.gameObject?.GetComponent<PauseMenuInventoryManagementSlot>();
        if (destSlot == null)
            return;

        InventoryManager.Instance.SwapInventoryItem(InventoryLocation.Player, _slotNumber, destSlot._slotNumber);
        _inventoryManagement.DestroyInventoryTextBoxGameObject();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.ItemQuantity == 0)
            return;

        _inventoryManagement.InventoryTextBoxGameObject = Instantiate(_inventoryTextBoxPrefab, transform.position, Quaternion.identity);
        _inventoryManagement.InventoryTextBoxGameObject.transform.SetParent(_parentCanvas.transform, false);

        var inventoryTextBox = _inventoryManagement.InventoryTextBoxGameObject.GetComponent<UIInventoryTextBox>();

        string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(this.ItemDetails.ItemType);

        inventoryTextBox.SetTextboxText(
            this.ItemDetails.ItemDescription,
            itemTypeDescription,
            "",
            this.ItemDetails.ItemLongDescription,
            "",
            "");

        if (_slotNumber > 23)
        {
            _inventoryManagement.InventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
            _inventoryManagement.InventoryTextBoxGameObject.transform.position = new Vector3(
                transform.position.x,
                transform.position.y + 50f,
                transform.position.z);
        }
        else
        {
            _inventoryManagement.InventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
            _inventoryManagement.InventoryTextBoxGameObject.transform.position = new Vector3(
                transform.position.x,
                transform.position.y - 50f,
                transform.position.z);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _inventoryManagement.DestroyInventoryTextBoxGameObject();
    }
}
