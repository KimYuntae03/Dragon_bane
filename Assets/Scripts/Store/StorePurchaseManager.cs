using UnityEngine;
using TMPro;

public class StorePurchaseManager : MonoBehaviour
{
    [Header("Purchase Popup")]
    [SerializeField] private GameObject purchasePopup;

    [Header("Not Enough Gold Popup")]
    [SerializeField] private GameObject notEnoughGoldPopup;

    [Header("Store UI")]
    [SerializeField] private StoreUIManager storeUIManager;

    private StoreItemData selectedItem;

    private void Start()
    {
        if (purchasePopup != null)
            purchasePopup.SetActive(false);

        if (notEnoughGoldPopup != null)
            notEnoughGoldPopup.SetActive(false);
    }

    // 아이템 버튼을 눌렀을 때 호출
    public void OpenPurchasePopup(StoreItemData itemData)
    {
        if (itemData == null)
            return;

        // 이미 보유한 장비는 아무 반응 없음
        if (itemData.ItemType == StoreItemType.Gauntlet ||
            itemData.ItemType == StoreItemType.Robe)
        {
            if (PlayerInventoryManager.Instance != null &&
                PlayerInventoryManager.Instance.HasEquipment(itemData.ItemId))
            {
                return;
            }
        }

        selectedItem = itemData;

        purchasePopup.SetActive(true);
    }

    // 구매 버튼
    public void BuySelectedItem()
    {
        if (selectedItem == null ||
            PlayerInventoryManager.Instance == null)
            return;

        if (!PlayerInventoryManager.Instance.HasEnoughGold(
                selectedItem.Price))
        {
            purchasePopup.SetActive(false);
            notEnoughGoldPopup.SetActive(true);
            return;
        }

        // 골드 차감
        PlayerInventoryManager.Instance.SpendGold(
            selectedItem.Price);

        // 장비라면 보유 목록에 추가
        if (selectedItem.ItemType == StoreItemType.Gauntlet ||
            selectedItem.ItemType == StoreItemType.Robe)
        {
            PlayerInventoryManager.Instance.AddEquipment(
                selectedItem.ItemId);
        }

        //구매 완료 후 상점 UI 갱신
        StoreItemButton[] itemButtons =
            FindObjectsByType<StoreItemButton>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (StoreItemButton itemButton in itemButtons)
        {
            itemButton.UpdateItemUI();
        }

        purchasePopup.SetActive(false);

        // 상단 골드 UI 갱신
        if (storeUIManager != null)
            storeUIManager.UpdateCurrencyUI();

        selectedItem = null;
    }

    // 구매 취소
    public void ClosePurchasePopup()
    {
        purchasePopup.SetActive(false);
        selectedItem = null;
    }

    // 골드 부족창 닫기
    public void CloseNotEnoughGoldPopup()
    {
        notEnoughGoldPopup.SetActive(false);
    }
}