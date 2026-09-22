using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private DragonController dragonController;
    [SerializeField] private GameObject bottomButtonUI;

    private bool battleStarted = false;

    public bool BattleStarted => battleStarted;

    private void Start()
    {
        SetBattleState(false);
    }

    public void StartBattle()
    {
        if (battleStarted)
            return;

        battleStarted = true;

         // 게임 시작 시 하단 버튼 UI 숨기기
        if (bottomButtonUI != null)
            bottomButtonUI.SetActive(false);

        SetBattleState(true);
    }

    private void SetBattleState(bool active)
    {
        if (playerController != null)
            playerController.SetBattleActive(active);

        if (dragonController != null)
            dragonController.SetBattleActive(active);
    }
}