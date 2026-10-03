using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreItemButton : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private StoreItemData itemData;

    [Header("UI")]
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button button;

    private void Start()
    {
        UpdateItemUI();
    }

    private void UpdateItemUI()
    {
        if (itemData == null)
            return;

        // 아이템 데이터의 가격을 UI에 표시
        if (priceText != null)
        {
            priceText.text = itemData.Price.ToString();
        }

        CheckOwnedItem();
    }

    private void CheckOwnedItem()
    {
        if (PlayerInventoryManager.Instance == null)
            return;

        // 장비류만 보유 여부 확인
        if (itemData.ItemType == StoreItemType.Gauntlet ||
            itemData.ItemType == StoreItemType.Robe)
        {
            bool isOwned =
                PlayerInventoryManager.Instance.HasEquipment(
                    itemData.ItemId
                );

            // 이미 보유한 장비라면 클릭 불가능
            if (button != null)
                button.interactable = !isOwned;
        }
    }
}