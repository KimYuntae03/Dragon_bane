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
    [SerializeField] private GameObject goldIcon;
    [SerializeField] private GameObject ownedCheckIcon;

    //구매창을 열 수 있도록 StorePurchaseManager.cs참조
    [SerializeField] private StorePurchaseManager purchaseManager;

    private void Start()
    {
        UpdateItemUI();
    }

    public void UpdateItemUI()
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

            if (isOwned) //소유 중이라면 가격 대신 OWNED표시
            {
                if (priceText != null)
                    priceText.text = "OWNED";
                
                if (goldIcon != null)
                    goldIcon.SetActive(false);

                if (ownedCheckIcon != null)
                    ownedCheckIcon.SetActive(true);

                if (button != null) //이미 소유중이라면 클릭 안되게 막기
                    button.interactable = false;
            }
            else
            {
                if (priceText != null)
                    priceText.text = itemData.Price.ToString();
                
                if (goldIcon != null)
                    goldIcon.SetActive(true);

                if (ownedCheckIcon != null)
                    ownedCheckIcon.SetActive(false);

                if (button != null)
                    button.interactable = true;
            }
        }
    }

    public void OnClickItem()
    {
        if (itemData == null || purchaseManager == null)
            return;

        purchaseManager.OpenPurchasePopup(itemData);
    }
}