using UnityEngine;

public enum StoreItemType
{
    Gauntlet,
    Robe,
    Potion,
    Pet
}

[CreateAssetMenu(
    fileName = "NewStoreItem",
    menuName = "Dragon Bane/Store Item"
)]
public class StoreItemData : ScriptableObject
{
    [Header("Basic Info")]
    [SerializeField] private string itemId;
    [SerializeField] private string itemName;
    [TextArea(2, 4)]
    [SerializeField] private string itemDescription;
    [SerializeField] private StoreItemType itemType;

    [Header("Visual")]
    [SerializeField] private Sprite itemIcon;

    [Header("Price")]
    [SerializeField] private int price;

    [Header("Equipment")]
    [SerializeField] private int damage;
    [SerializeField] private int upgradeLevel = 0;

    [Header("Item")]
    [SerializeField] private int quantity = 0;

    [Header("Item Stats")]
    [SerializeField] private int defense;
    [SerializeField] private int healAmount;

    [Header("Gauntlet Element")]//건틀릿 속성
    [SerializeField] private ElementType elementType;

    public ElementType ElementType => elementType;

    public string ItemId => itemId;
    public string ItemName => itemName;
    public string ItemDescription => itemDescription;
    public StoreItemType ItemType => itemType;
    public Sprite ItemIcon => itemIcon;
    public int Price => price;
    public int Damage => damage;
    public int UpgradeLevel => upgradeLevel;
    public int Quantity => quantity;

    public int Defense => defense;
    public int HealAmount => healAmount;
}