using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    [ItemCodeDescription]
    private int _itemCode;

    private SpriteRenderer _spriteRenderer;

    public int ItemCode { get => _itemCode; set => _itemCode = value; }

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (_itemCode != 0)
        {
            Init(_itemCode);
        }
    }

    public void Init(int itemCode)
    {
        if (itemCode <= 0)
            return;

        _itemCode = itemCode;

        var details = InventoryManager.Instance.GetItemDetails(_itemCode);
        if (details == null)
            return;

        _spriteRenderer.sprite = details.ItemSprite;

        if (details.ItemType == ItemType.ReapableScenery)
        {
            gameObject.AddComponent<ItemNudge>();
        }
    }
}
