using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventoryUI : MonoBehaviour
{
    [Header("Gauntlet")]
    [SerializeField] private EquipmentSlotUI[] gauntletSlots;
    [SerializeField] private List<StoreItemData> gauntletItemData;

    [Header("Robe")]
    [SerializeField] private EquipmentSlotUI[] robeSlots;
    [SerializeField] private List<StoreItemData> robeItemData;

    [Header("Item")]
    [SerializeField] private EquipmentSlotUI[] itemSlots;
    [SerializeField] private List<StoreItemData> itemData;

    [Header("Pet")]
    [SerializeField] private EquipmentSlotUI[] petSlots;
    [SerializeField] private List<StoreItemData> petItemData;


    private void Start()
    {
        UpdateAllInventory();
    }


    public void UpdateAllInventory()
    {
        if (PlayerInventoryManager.Instance == null)
            return;

        UpdateCategory(
            gauntletSlots,
            gauntletItemData,
            PlayerInventoryManager.Instance.OwnedGauntletIds
        );

        UpdateCategory(
            robeSlots,
            robeItemData,
            PlayerInventoryManager.Instance.OwnedRobeIds
        );

        UpdateCategory(
            itemSlots,
            itemData,
            PlayerInventoryManager.Instance.OwnedItemIds
        );

        UpdateCategory(
            petSlots,
            petItemData,
            PlayerInventoryManager.Instance.OwnedPetIds
        );
    }


    private void UpdateCategory(
        EquipmentSlotUI[] slots,
        List<StoreItemData> dataList,
        List<string> ownedIds)
    {
        // 슬롯 초기화
        foreach (EquipmentSlotUI slot in slots)
        {
            slot.ClearSlot();
        }

        int slotIndex = 0;

        foreach (string ownedId in ownedIds)
        {
            if (slotIndex >= slots.Length)
                break;

            StoreItemData data =
                dataList.Find(
                    item => item.ItemId == ownedId
                );

            if (data == null)
                continue;

            slots[slotIndex].SetItem(data);

            slotIndex++;
        }
    }
}