using UnityEngine;
using TMPro;

public class EquipmentUIManager : MonoBehaviour
{
    [Header("Currency UI")]
    [SerializeField] private TMP_Text goldText;// 장비창 상단에 표시할 골드 텍스트
    [SerializeField] private TMP_Text upgradeStoneText;// 장비창 상단에 표시할 강화석 텍스트

    private void Start()
    {
        UpdateCurrencyUI();// 장비 씬에 들어오면 현재 보유 중인 재화 값을 UI에 표시
    }

    //PlayerInventoryManager에서 사용자 보유 재화,골드값을 읽어와서 저장
    public void UpdateCurrencyUI()
    {
        if (PlayerInventoryManager.Instance == null)//PlayerInventoryManager존재하는지 확인
            return;

        if (goldText != null)
            goldText.text =
                PlayerInventoryManager.Instance.Gold.ToString();

        if (upgradeStoneText != null)
            upgradeStoneText.text =
                PlayerInventoryManager.Instance.UpgradeStone.ToString();
    }
}