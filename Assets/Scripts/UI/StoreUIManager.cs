using UnityEngine;
using TMPro;

public class StoreUIManager : MonoBehaviour
{
    [Header("Currency UI")]
    [SerializeField] private TMP_Text goldText; // 상점 화면의 골드 표시 텍스트
    [SerializeField] private TMP_Text upgradeStoneText; // 상점 화면의 강화석 표시 텍스트

    private void Start()
    {
        UpdateCurrencyUI();
    }

    public void UpdateCurrencyUI()
    {
        //PlayerInventoryManager에 저장된 재화,강화석 개수를 읽어와서 상점 UI Text에 연결
        if (PlayerInventoryManager.Instance == null)
            return;

        if (goldText != null)
        {
            goldText.text =
                PlayerInventoryManager.Instance.Gold.ToString();
            
        }

        if (upgradeStoneText != null)
        {
            upgradeStoneText.text =
                PlayerInventoryManager.Instance.UpgradeStone.ToString();
        }
    }
}