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

    // 구매창을 열 수 있도록 StorePurchaseManager 참조
    [SerializeField] private StorePurchaseManager purchaseManager;


    private void Start()
    {
        UpdateItemUI();
    }


    public void UpdateItemUI()
    {
        if (itemData == null)
            return;

        // 기본 가격 표시
        if (priceText != null)
            priceText.text = itemData.Price.ToString();

        CheckOwnedItem();
    }


    private void CheckOwnedItem()
    {
        if (PlayerInventoryManager.Instance == null)
            return;

        bool isOwned = false;


        // 건틀릿 보유 여부 확인
        if (itemData.ItemType == StoreItemType.Gauntlet)
        {
            isOwned =
                PlayerInventoryManager.Instance.HasGauntlet(
                    itemData.ItemId
                );
        }

        // 로브 보유 여부 확인
        else if (itemData.ItemType == StoreItemType.Robe)
        {
            isOwned =
                PlayerInventoryManager.Instance.HasRobe(
                    itemData.ItemId
                );
        }

        // 건틀릿 / 로브
        if (itemData.ItemType == StoreItemType.Gauntlet ||
            itemData.ItemType == StoreItemType.Robe)
        {
            if (isOwned)
            {
                // 이미 소유한 장비는 가격 대신 OWNED 표시
                if (priceText != null)
                    priceText.text = "OWNED";

                if (goldIcon != null)
                    goldIcon.SetActive(false);

                if (ownedCheckIcon != null)
                    ownedCheckIcon.SetActive(true);

                // 재구매 방지
                if (button != null)
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

        // 포션은 여러 번 구매 가능
        else if (itemData.ItemType == StoreItemType.Potion)
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


    public void OnClickItem()
    {
        if (itemData == null || purchaseManager == null)
            return;

        purchaseManager.OpenPurchasePopup(itemData);
    }
}