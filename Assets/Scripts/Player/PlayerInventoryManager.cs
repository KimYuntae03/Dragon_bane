using System.Collections.Generic;
using UnityEngine;


// 건틀릿 / 로브처럼 강화 레벨이 필요한 장비 데이터
[System.Serializable]
public class EquipmentInventoryData
{
    public string itemId;
    public int upgradeLevel;

    public EquipmentInventoryData(string id)
    {
        itemId = id;
        upgradeLevel = 0;
    }
}


// 포션 등 수량이 필요한 아이템 데이터
[System.Serializable]
public class ItemInventoryData
{
    public string itemId;
    public int quantity;

    public ItemInventoryData(string id, int amount)
    {
        itemId = id;
        quantity = amount;
    }
}


public class PlayerInventoryManager : MonoBehaviour
{
    public static PlayerInventoryManager Instance { get; private set; }


    [Header("Currency")]
    [SerializeField] private int gold = 0;
    [SerializeField] private int upgradeStone = 0;


    [Header("Owned Gauntlets")]
    [SerializeField]
    private List<EquipmentInventoryData> ownedGauntlets
        = new List<EquipmentInventoryData>();


    [Header("Owned Robes")]
    [SerializeField]
    private List<EquipmentInventoryData> ownedRobes
        = new List<EquipmentInventoryData>();


    [Header("Owned Items")]
    [SerializeField]
    private List<ItemInventoryData> ownedItems
        = new List<ItemInventoryData>();


    [Header("Owned Pets")]
    [SerializeField]
    private List<string> ownedPetIds
        = new List<string>();


    [Header("Equipped")]
    [SerializeField]
    private ElementType equippedGauntlet = ElementType.Fire;

    [SerializeField]
    private string equippedRobeId = "";

    [SerializeField]
    private string equippedPetId = "";


    [Header("Consumable Items")]
    [SerializeField] private int healthPotion = 0;


    // =========================
    // 외부 데이터 확인용
    // =========================

    public int Gold => gold;
    public int UpgradeStone => upgradeStone;
    public int HealthPotion => healthPotion;

    public ElementType EquippedGauntlet => equippedGauntlet;
    public string EquippedRobeId => equippedRobeId;
    public string EquippedPetId => equippedPetId;


    // 기존 EquipmentInventoryUI와 호환하기 위한 ID 목록
    public List<string> OwnedGauntletIds
    {
        get
        {
            return ownedGauntlets.ConvertAll(
                item => item.itemId
            );
        }
    }

    public List<string> OwnedRobeIds
    {
        get
        {
            return ownedRobes.ConvertAll(
                item => item.itemId
            );
        }
    }

    public List<string> OwnedItemIds
    {
        get
        {
            return ownedItems.ConvertAll(
                item => item.itemId
            );
        }
    }

    public List<string> OwnedPetIds => ownedPetIds;


    private void Awake()
    {
        // PlayerInventoryManager 중복 생성 방지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 씬이 변경되어도 플레이어 데이터 유지
        DontDestroyOnLoad(gameObject);
    }


    // =========================
    // Gauntlet
    // =========================

    // 건틀릿 보유 여부 확인
    public bool HasGauntlet(string itemId)
    {
        return ownedGauntlets.Exists(
            item => item.itemId == itemId
        );
    }


    // 건틀릿 추가
    public void AddGauntlet(string itemId)
    {
        if (HasGauntlet(itemId))
            return;

        ownedGauntlets.Add(
            new EquipmentInventoryData(itemId)
        );
    }


    // 건틀릿 강화 레벨 확인
    public int GetGauntletUpgradeLevel(string itemId)
    {
        EquipmentInventoryData data =
            ownedGauntlets.Find(
                item => item.itemId == itemId
            );

        return data != null
            ? data.upgradeLevel
            : 0;
    }


    // =========================
    // Robe
    // =========================

    // 로브 보유 여부 확인
    public bool HasRobe(string itemId)
    {
        return ownedRobes.Exists(
            item => item.itemId == itemId
        );
    }


    // 로브 추가
    public void AddRobe(string itemId)
    {
        if (HasRobe(itemId))
            return;

        ownedRobes.Add(
            new EquipmentInventoryData(itemId)
        );
    }


    // 로브 강화 레벨 확인
    public int GetRobeUpgradeLevel(string itemId)
    {
        EquipmentInventoryData data =
            ownedRobes.Find(
                item => item.itemId == itemId
            );

        return data != null
            ? data.upgradeLevel
            : 0;
    }


    // =========================
    // Item
    // =========================

    // 일반 아이템 보유 여부 확인
    public bool HasItem(string itemId)
    {
        return ownedItems.Exists(
            item => item.itemId == itemId
        );
    }


    // 일반 아이템 추가
    public void AddItem(string itemId, int amount = 1)
    {
        ItemInventoryData item =
            ownedItems.Find(
                data => data.itemId == itemId
            );

        // 이미 가지고 있으면 수량 증가
        if (item != null)
        {
            item.quantity += amount;
        }
        else
        {
            // 처음 얻은 아이템이면 새로 추가
            ownedItems.Add(
                new ItemInventoryData(itemId, amount)
            );
        }
    }


    // 아이템 수량 확인
    public int GetItemQuantity(string itemId)
    {
        ItemInventoryData item =
            ownedItems.Find(
                data => data.itemId == itemId
            );

        return item != null
            ? item.quantity
            : 0;
    }


    // =========================
    // Pet
    // =========================

    // 펫 보유 여부 확인
    public bool HasPet(string itemId)
    {
        return ownedPetIds.Contains(itemId);
    }


    // 펫 추가
    public void AddPet(string itemId)
    {
        if (!ownedPetIds.Contains(itemId))
            ownedPetIds.Add(itemId);
    }


    // =========================
    // Currency
    // =========================

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

    // 건틀릿 장착
    public void EquipGauntlet(
        string itemId,
        ElementType elementType)
    {
        // 보유하지 않은 건틀릿은 장착 불가
        if (!HasGauntlet(itemId))
            return;

        equippedGauntlet = elementType;
    }


    // 로브 장착
    public void EquipRobe(string itemId)
    {
        // 보유하지 않은 로브는 장착 불가
        if (!HasRobe(itemId))
            return;

        equippedRobeId = itemId;
    }

    public void EquipPet(string itemId)
    {
        if (!HasPet(itemId))
            return;

        equippedPetId = itemId;
    }

    // 강화에 필요한 강화석이 충분한지 확인
    public bool HasEnoughUpgradeStone(int amount)
    {
        return upgradeStone >= amount;
    }


    // 강화석 사용
    public bool SpendUpgradeStone(int amount)
    {
        if (upgradeStone < amount)
            return false;

        upgradeStone -= amount;
        return true;
    }
    public bool UpgradeGauntlet(string itemId)
    {
        EquipmentInventoryData data =
            ownedGauntlets.Find(
                item => item.itemId == itemId
            );

        if (data == null)
            return false;

        // 강화 레벨 최대 +17
        if (data.upgradeLevel >= 17)
            return false;

        data.upgradeLevel++;

        return true;
    }

    public int GetGauntletDamage(StoreItemData itemData)
    {
        if (itemData == null)
            return 0;

        int level =
            GetGauntletUpgradeLevel(
                itemData.ItemId
            );

        float finalDamage =
            itemData.Damage *
            (1f + level * 0.2f);

        return Mathf.RoundToInt(finalDamage);
    }
}