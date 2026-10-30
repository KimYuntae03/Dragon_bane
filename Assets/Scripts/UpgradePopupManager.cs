using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UpgradePopupManager : MonoBehaviour
{
    public static bool IsUpgradeInProgress { get; private set; }

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

    [Header("Upgrade Progress")] //강화 게이지
    [SerializeField] private GameObject upgradeProgressUI;
    [SerializeField] private Image upgradeGaugeFill;
    [SerializeField] private float upgradeDuration = 1f;

    private StoreItemData selectedItem;
    private bool isUpgrading;
    private GameObject upgradeInputBlocker;


    private void Start()
    {
        CreateUpgradeInputBlocker();

        if (upgradePopup != null)
            upgradePopup.SetActive(false);

        if (notEnoughStoneUI != null)
            notEnoughStoneUI.SetActive(false);

        if (upgradeSuccessUI != null)
            upgradeSuccessUI.SetActive(false);

        if (upgradeFailUI != null)
            upgradeFailUI.SetActive(false);

        // 강화 진행 UI 초기화
        if (upgradeProgressUI != null)
            upgradeProgressUI.SetActive(false);

        if (upgradeInputBlocker != null)
            upgradeInputBlocker.SetActive(false);

        if (upgradeGaugeFill != null)
            upgradeGaugeFill.fillAmount = 0f;
    }


    // 선택한 장비의 강화 팝업 열기
    public void OpenUpgradePopup(StoreItemData itemData)
    {
        if (isUpgrading ||
            itemData == null ||
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
        if (isUpgrading ||
            selectedItem == null ||
            PlayerInventoryManager.Instance == null)
            return;

        // 연출 도중 다른 아이템을 선택해도
        // 처음 강화한 장비가 유지되도록 참조를 고정한다.
        StoreItemData itemToUpgrade = selectedItem;

        int currentLevel = 0;

        if (itemToUpgrade.ItemType == StoreItemType.Gauntlet)
        {
            currentLevel =
                PlayerInventoryManager.Instance
                    .GetGauntletUpgradeLevel(
                        itemToUpgrade.ItemId
                    );
        }
        else if (itemToUpgrade.ItemType == StoreItemType.Robe)
        {
            currentLevel =
                PlayerInventoryManager.Instance
                    .GetRobeUpgradeLevel(
                        itemToUpgrade.ItemId
                    );
        }
        else
        {
            return;
        }

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

        bool isSuccess =
            Random.value <= successRate;

        // 기존 강화 확인 팝업 닫기
        if (upgradePopup != null)
            upgradePopup.SetActive(false);

        isUpgrading = true;
        IsUpgradeInProgress = true;

        // 강화 게이지 연출 시작
        StartCoroutine(
            PlayUpgradeProgress(
                itemToUpgrade,
                isSuccess
            )
        );
    }
    private IEnumerator PlayUpgradeProgress(
        StoreItemData itemToUpgrade,
        bool isSuccess)
    {
        // 강화 진행 UI 표시
        if (upgradeInputBlocker != null)
        {
            upgradeInputBlocker.SetActive(true);
            upgradeInputBlocker.transform.SetAsLastSibling();
        }

        if (upgradeProgressUI != null)
        {
            upgradeProgressUI.SetActive(true);
            upgradeProgressUI.transform.SetAsLastSibling();
        }

        // 게이지 초기화
        if (upgradeGaugeFill != null)
            upgradeGaugeFill.fillAmount = 0f;

        float elapsedTime = 0f;

        // 0 → 100%까지 게이지 채우기
        while (elapsedTime < upgradeDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress =
                elapsedTime / upgradeDuration;

            if (upgradeGaugeFill != null)
            {
                upgradeGaugeFill.fillAmount =
                    Mathf.Clamp01(progress);
            }

            yield return null;
        }

        // 확실하게 100%로 마무리
        if (upgradeGaugeFill != null)
            upgradeGaugeFill.fillAmount = 1f;

        // 진행 UI 종료
        if (upgradeProgressUI != null)
            upgradeProgressUI.SetActive(false);

        if (upgradeInputBlocker != null)
            upgradeInputBlocker.SetActive(false);

        // 강화 결과 적용
        if (isSuccess &&
            itemToUpgrade != null &&
            PlayerInventoryManager.Instance != null)
        {
            if (itemToUpgrade.ItemType == StoreItemType.Gauntlet)
            {
                PlayerInventoryManager.Instance
                    .UpgradeGauntlet(
                        itemToUpgrade.ItemId
                    );
            }
            else if (itemToUpgrade.ItemType == StoreItemType.Robe)
            {
                PlayerInventoryManager.Instance
                    .UpgradeRobe(
                        itemToUpgrade.ItemId
                    );
            }

            if (equipmentDetailUI != null)
                equipmentDetailUI.RefreshSelectedItem();

            if (upgradeSuccessUI != null)
                upgradeSuccessUI.SetActive(true);
        }
        else
        {
            if (upgradeFailUI != null)
                upgradeFailUI.SetActive(true);
        }

        isUpgrading = false;
        IsUpgradeInProgress = false;
    }

    private void CreateUpgradeInputBlocker()
    {
        if (upgradeProgressUI == null ||
            upgradeProgressUI.transform.parent == null)
        {
            return;
        }

        upgradeInputBlocker = new GameObject(
            "UpgradeInputBlocker",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(UnityEngine.UI.Image)
        );

        RectTransform blockerRect =
            upgradeInputBlocker.GetComponent<RectTransform>();

        blockerRect.SetParent(
            upgradeProgressUI.transform.parent,
            false
        );
        blockerRect.anchorMin = Vector2.zero;
        blockerRect.anchorMax = Vector2.one;
        blockerRect.offsetMin = Vector2.zero;
        blockerRect.offsetMax = Vector2.zero;

        UnityEngine.UI.Image blockerImage =
            upgradeInputBlocker.GetComponent<UnityEngine.UI.Image>();

        blockerImage.color = Color.clear;
        blockerImage.raycastTarget = true;
        upgradeInputBlocker.SetActive(false);
    }

    private void OnDestroy()
    {
        IsUpgradeInProgress = false;
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
