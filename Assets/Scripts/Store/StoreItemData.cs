using UnityEngine;

public enum StoreItemType
{
    Gauntlet,
    Robe,
    Potion
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
    [SerializeField] private StoreItemType itemType;

    [Header("Price")]
    [SerializeField] private int price;

    public string ItemId => itemId;
    public string ItemName => itemName;
    public StoreItemType ItemType => itemType;
    public int Price => price;
}