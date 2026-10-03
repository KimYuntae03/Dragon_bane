using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{
    public static PlayerInventoryManager Instance { get; private set; }

    [Header("Currency")]
    [SerializeField] private int gold = 0;              // 보유 골드
    [SerializeField] private int upgradeStone = 0;      // 보유 강화석

    [Header("Consumable Items")]
    [SerializeField] private int healthPotion = 0;      // 회복 포션

    [Header("Equipment")]
    [SerializeField] private List<ElementType> ownedGauntlets
        = new List<ElementType>();                      // 보유 건틀릿

    [SerializeField] private ElementType equippedGauntlet
        = ElementType.Fire;                             // 현재 장착 건틀릿

    [Header("Other Items")]
    [SerializeField] private List<string> ownedItems
        = new List<string>();                           // 추후 일반 아이템용
    [Header("Owned Equipment")]
    [SerializeField] private List<string> ownedEquipmentIds //장비 보유 데이터
        = new List<string>();

    // 외부에서 현재 데이터 확인용
    public int Gold => gold;
    public int UpgradeStone => upgradeStone;
    public int HealthPotion => healthPotion;

    public List<ElementType> OwnedGauntlets => ownedGauntlets;
    public ElementType EquippedGauntlet => equippedGauntlet;

    private void Awake()
    {
        // PlayerInventoryManager가 중복 생성되지 않도록 처리
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Scene이 변경되어도 유지
        DontDestroyOnLoad(gameObject);
    }
    public bool HasEquipment(string itemId)
    {
        return ownedEquipmentIds.Contains(itemId);
    }

    public void AddEquipment(string itemId)
    {
        if (ownedEquipmentIds.Contains(itemId))
            return;

        ownedEquipmentIds.Add(itemId);
    }

    // 골드가 충분한지 확인
    public bool HasEnoughGold(int amount)
    {
        return gold >= amount;
    }

    // 골드 사용
    public bool SpendGold(int amount)
    {
        if (gold < amount)
            return false;

        gold -= amount;
        return true;
    }
}