using UnityEngine;
using TMPro;

public class UpgradePopupManager : MonoBehaviour
{
    [Header("Popup")]
    [SerializeField] private GameObject upgradePopup;

    [Header("UI")]
    [SerializeField] private TMP_Text upgradeLevelText;
    [SerializeField] private TMP_Text upgradeStonePriceText;

    [SerializeField] private GameObject notEnoughStoneUI;
    [SerializeField] private GameObject upgradeSuccessUI;
    [SerializeField] private GameObject upgradeFailUI;

    [Header("Refresh UI")]
    [SerializeField] private EquipmentUIManager equipmentUIManager;
    [SerializeField] private EquipmentDetailUI equipmentDetailUI;

    private StoreItemData selectedItem;


    private void Start()
    {
        if (upgradePopup != null)
            upgradePopup.SetActive(false);
        if (notEnoughStoneUI != null)
            notEnoughStoneUI.SetActive(false);

        if (upgradeSuccessUI != null)
            upgradeSuccessUI.SetActive(false);

        if (upgradeFailUI != null)
            upgradeFailUI.SetActive(false);
    }


    // 선택한 장비의 강화 팝업 열기
    public void OpenUpgradePopup(StoreItemData itemData)
    {
        if (itemData == null ||
            PlayerInventoryManager.Instance == null)
            return;

        // 건틀릿과 로브만 강화 가능
        if (itemData.ItemType != StoreItemType.Gauntlet &&
            itemData.ItemType != StoreItemType.Robe)
            return;

        selectedItem = itemData;

        //떠있는 강화 성공,실패 팝업 닫기
        if (notEnoughStoneUI != null)
            notEnoughStoneUI.SetActive(false);

        if (upgradeSuccessUI != null)
            upgradeSuccessUI.SetActive(false);

        if (upgradeFailUI != null)
            upgradeFailUI.SetActive(false);

        int currentLevel = 0;

        if (itemData.ItemType == StoreItemType.Gauntlet)
        {
            currentLevel =
                PlayerInventoryManager.Instance
                    .GetGauntletUpgradeLevel(
                        itemData.ItemId
                    );
        }
        else if (itemData.ItemType == StoreItemType.Robe)
        {
            currentLevel =
                PlayerInventoryManager.Instance
                    .GetRobeUpgradeLevel(
                        itemData.ItemId
                    );
        }
        if (currentLevel >= 17)
            return;

        int nextLevel = currentLevel + 1;

        // 임시 강화 비용
        int upgradeCost = GetUpgradeCost(currentLevel);


        if (upgradeLevelText != null)
        {
            upgradeLevelText.text =
                "Upgrade Level: " +
                currentLevel + " Lv  →  " +
                nextLevel + " Lv";
        }


        if (upgradeStonePriceText != null)
        {
            upgradeStonePriceText.text =
                upgradeCost.ToString();
        }


        if (upgradePopup != null)
            upgradePopup.SetActive(true);
    }


    // 강화 팝업 닫기
    public void CloseUpgradePopup()
    {
        if (upgradePopup != null)
            upgradePopup.SetActive(false);

        selectedItem = null;
    }


    // 실제 강화 로직은 나중에 구현
    public void UpgradeSelectedItem()
    {
        if (selectedItem == null ||
            PlayerInventoryManager.Instance == null)
            return;

        // 현재는 건틀릿만 강화 테스트
        if (selectedItem.ItemType != StoreItemType.Gauntlet)
            return;

        int currentLevel =
            PlayerInventoryManager.Instance
                .GetGauntletUpgradeLevel(
                    selectedItem.ItemId
                );

        // 최대 강화
        if (currentLevel >= 17)
            return;

        // 현재 레벨 기준 강화 비용 계산
        int upgradeCost =
            GetUpgradeCost(currentLevel);

        // 강화석 부족
        if (!PlayerInventoryManager.Instance
            .HasEnoughUpgradeStone(upgradeCost))
        {
            if (upgradePopup != null)
                upgradePopup.SetActive(false);

            if (notEnoughStoneUI != null)
                notEnoughStoneUI.SetActive(true);

            return;
        }

        // 강화석 차감
        PlayerInventoryManager.Instance
            .SpendUpgradeStone(upgradeCost);
        if (equipmentUIManager != null)
            equipmentUIManager.UpdateCurrencyUI();

        // 현재 레벨 기준 성공 확률 계산
        float successRate =
            GetUpgradeSuccessRate(currentLevel);

        // 성공 여부 판정
        bool isSuccess =
            Random.value <= successRate;

        if (isSuccess)
        {
            PlayerInventoryManager.Instance
                .UpgradeGauntlet(
                    selectedItem.ItemId
                );
            if (equipmentDetailUI != null) //성공했을때 상세창 업데이트
                equipmentDetailUI.RefreshSelectedItem();

            if (upgradeSuccessUI != null)
                upgradeSuccessUI.SetActive(true);
        }
        else
        {
            if (upgradeFailUI != null)
                upgradeFailUI.SetActive(true);
        }

        // 기존 강화 확인 팝업 닫기
        if (upgradePopup != null)
            upgradePopup.SetActive(false);
    }

    
    private int GetUpgradeCost(int currentLevel)
    {
        int cost = 5;

        for (int level = 0; level < currentLevel; level++)
        {
            // +1 ~ +10 구간은 비용 증가량 +3
            if (level < 9)
            {
                cost += 3;
            }

            // +10 이후부터 증가량 자체가
            // +4, +5, +6 ... 으로 증가
            else
            {
                cost += level - 5;
            }
        }

        return cost;
    }

    private float GetUpgradeSuccessRate(int currentLevel)
    {
        // +9 → +10까지는 100%
        if (currentLevel <= 9)
            return 1f;

        // +10 → +11부터
        // +16 → +17까지 100% → 10% 선형 감소
        float t =
            (currentLevel - 9f) / 7f;

        return Mathf.Lerp(
            1f,
            0.1f,
            t
        );
    }

    public void CloseResultPopup() //강화 결과 확인버튼에 연결할 팝업창 닫기 기능 
    {
        if (notEnoughStoneUI != null)
            notEnoughStoneUI.SetActive(false);

        if (upgradeSuccessUI != null)
            upgradeSuccessUI.SetActive(false);

        if (upgradeFailUI != null)
            upgradeFailUI.SetActive(false);
    }
}