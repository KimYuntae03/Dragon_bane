using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private DragonController dragonController;
    [SerializeField] private GameObject bottomButtonUI;
    [SerializeField] private BattleIntroCamera battleIntroCamera;//배틀 시작전 카메라 참조
    [SerializeField] private CameraFollow cameraFollow;//시작 후 카메라 참조

    private bool battleStarted = false;

    public bool BattleStarted => battleStarted;

    private void Start()
    {
        SetBattleState(false);
        if (cameraFollow != null)
            cameraFollow.SetBattleActive(false);
    }

    public void StartBattle()
    {
        if (battleStarted)
            return;

        battleStarted = true;

         // 게임 시작 시 하단 버튼 UI 숨기기
        if (bottomButtonUI != null)
            bottomButtonUI.SetActive(false);
        if (battleIntroCamera != null)
            battleIntroCamera.StartBattle();
        if (cameraFollow != null)
            cameraFollow.SetBattleActive(true);

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