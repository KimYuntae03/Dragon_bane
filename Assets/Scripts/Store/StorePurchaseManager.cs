using UnityEngine;

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

        if (PlayerInventoryManager.Instance == null)
            return;


        // 이미 보유한 건틀릿은 구매 불가
        if (itemData.ItemType == StoreItemType.Gauntlet)
        {
            if (PlayerInventoryManager.Instance.HasGauntlet(
                    itemData.ItemId))
            {
                return;
            }
        }

        // 이미 보유한 로브는 구매 불가
        else if (itemData.ItemType == StoreItemType.Robe)
        {
            if (PlayerInventoryManager.Instance.HasRobe(
                    itemData.ItemId))
            {
                return;
            }
        }


        selectedItem = itemData;

        if (purchasePopup != null)
            purchasePopup.SetActive(true);
    }


    // 구매 버튼
    public void BuySelectedItem()
    {
        if (selectedItem == null ||
            PlayerInventoryManager.Instance == null)
            return;


        // 골드 부족 확인
        if (!PlayerInventoryManager.Instance.HasEnoughGold(
                selectedItem.Price))
        {
            if (purchasePopup != null)
                purchasePopup.SetActive(false);

            if (notEnoughGoldPopup != null)
                notEnoughGoldPopup.SetActive(true);

            return;
        }


        // 골드 차감
        PlayerInventoryManager.Instance.SpendGold(
            selectedItem.Price);


        // 건틀릿 구매
        if (selectedItem.ItemType == StoreItemType.Gauntlet)
        {
            PlayerInventoryManager.Instance.AddGauntlet(
                selectedItem.ItemId
            );
        }

        // 로브 구매
        else if (selectedItem.ItemType == StoreItemType.Robe)
        {
            PlayerInventoryManager.Instance.AddRobe(
                selectedItem.ItemId
            );
        }

        // 포션은 이후 수량 시스템 구현할 때 처리
        else if (selectedItem.ItemType == StoreItemType.Potion)
        {
            PlayerInventoryManager.Instance.AddItem(
                selectedItem.ItemId
            );
        }


        // 구매 완료 후 모든 상점 아이템 UI 갱신
        StoreItemButton[] itemButtons =
            FindObjectsByType<StoreItemButton>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (StoreItemButton itemButton in itemButtons)
        {
            itemButton.UpdateItemUI();
        }


        if (purchasePopup != null)
            purchasePopup.SetActive(false);


        // 상단 골드 UI 갱신
        if (storeUIManager != null)
            storeUIManager.UpdateCurrencyUI();


        selectedItem = null;
    }


    // 구매 취소
    public void ClosePurchasePopup()
    {
        if (purchasePopup != null)
            purchasePopup.SetActive(false);

        selectedItem = null;
    }


    // 골드 부족창 닫기
    public void CloseNotEnoughGoldPopup()
    {
        if (notEnoughGoldPopup != null)
            notEnoughGoldPopup.SetActive(false);
    }
}