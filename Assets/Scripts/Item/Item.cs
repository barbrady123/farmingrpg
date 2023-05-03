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
        Init();
    }

    public void Init()
    {
        if (_itemCode <= 0)
            return;

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
