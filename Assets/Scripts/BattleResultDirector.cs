using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultDirector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private Transform dragon;
    [SerializeField] private GameObject battleUIRoot;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private DragonController dragonController;

    [Header("Letterbox UI")]
    [SerializeField] private GameObject letterboxUI;
    [SerializeField] private RectTransform topBar;
    [SerializeField] private RectTransform bottomBar;
    [SerializeField] private float letterboxTargetHeight = 120f;
    [SerializeField] private float letterboxSpeed = 400f;

    [Header("Letterbox Exit")]
    [SerializeField] private float letterboxExitDistance = 160f;
    [SerializeField] private float letterboxExitSpeed = 300f;

    [Header("Cinematic Camera")]
    [SerializeField] private Vector3 cameraOffset = new Vector3(-7.2f, 4.8f, 7.2f);
    [SerializeField] private float cameraLookHeight = 1.5f;
    [SerializeField] private float cinematicFieldOfView = 55f;
    [SerializeField] private float cameraMoveSpeed = 4f;
    [SerializeField] private float cameraRotateSpeed = 6f;
    [SerializeField] private float cinematicDuration = 2.5f;

    [Header("Victory UI")]
    [SerializeField] private GameObject victoryUI;
    [SerializeField] private CanvasGroup victoryCanvasGroup;
    [SerializeField] private float victoryFadeDuration = 1.0f;

    [Header("Reward UI")]
    [SerializeField] private CanvasGroup expReward;
    [SerializeField] private CanvasGroup goldReward;
    [SerializeField] private CanvasGroup upgradeStoneReward;

    [SerializeField] private float rewardFadeDuration = 0.35f;
    [SerializeField] private float rewardInterval = 0.15f;

    private bool isPlayingCinematic = false;

    public void PlayDragonDeathCinematic()
    {
        if (isPlayingCinematic)
            return;

        StartCoroutine(DragonDeathCinematicRoutine());
    }

    private IEnumerator DragonDeathCinematicRoutine()
    {
        
        isPlayingCinematic = true;

        // 전투 UI 끄기
        if (battleUIRoot != null)
            battleUIRoot.SetActive(false);
        // 레터박스 UI 활성화
        if (letterboxUI != null)
            letterboxUI.SetActive(true);

        // 플레이어 입력 정지
        if (playerController != null)
            playerController.enabled = false;

        // 기존 카메라 추적 끄기
        if (cameraFollow != null)
            cameraFollow.enabled = false;

        float elapsed = 0f;

        while (elapsed < cinematicDuration)
        {
            elapsed += Time.deltaTime;

            UpdateLetterbox();
            UpdateCameraToDragon();

            yield return null;
        }

        //VictoryUI띄우기
        StartCoroutine(ShowVictoryUI());
        // 레터박스가 위/아래로 자라지도록 함
        yield return StartCoroutine(HideLetterbox());

        // 여기서 승리 UI 띄우기
        Debug.Log("드래곤 처치 연출 종료 -> Victory UI 표시");
    }

    private void UpdateLetterbox()
    {
        if (topBar != null)
        {
            Vector2 size = topBar.sizeDelta;
            size.y = Mathf.MoveTowards(size.y, letterboxTargetHeight, letterboxSpeed * Time.deltaTime);
            topBar.sizeDelta = size;
        }

        if (bottomBar != null)
        {
            Vector2 size = bottomBar.sizeDelta;
            size.y = Mathf.MoveTowards(size.y, letterboxTargetHeight, letterboxSpeed * Time.deltaTime);
            bottomBar.sizeDelta = size;
        }
    }

    private void UpdateCameraToDragon()
    {
        if (mainCamera == null || dragon == null)
            return;

        Vector3 targetPosition = dragon.position + dragon.TransformDirection(cameraOffset);

        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position,
            targetPosition,
            cameraMoveSpeed * Time.deltaTime
        );

        mainCamera.fieldOfView = Mathf.Lerp(
            mainCamera.fieldOfView,
            cinematicFieldOfView,
            cameraMoveSpeed * Time.deltaTime
        );

        Vector3 lookTarget = dragon.position + Vector3.up * cameraLookHeight;
        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - mainCamera.transform.position);

        mainCamera.transform.rotation = Quaternion.Slerp(
            mainCamera.transform.rotation,
            targetRotation,
            cameraRotateSpeed * Time.deltaTime
        );
    }

    private IEnumerator HideLetterbox()
    {
        if (topBar == null || bottomBar == null)
            yield break;

        Vector2 topStartPosition = topBar.anchoredPosition;
        Vector2 bottomStartPosition = bottomBar.anchoredPosition;

        Vector2 topTargetPosition =
            topStartPosition + Vector2.up * letterboxExitDistance;

        Vector2 bottomTargetPosition =
            bottomStartPosition + Vector2.down * letterboxExitDistance;

        while (true)
        {
            topBar.anchoredPosition =
                Vector2.MoveTowards(
                    topBar.anchoredPosition,
                    topTargetPosition,
                    letterboxExitSpeed * Time.deltaTime
                );

            bottomBar.anchoredPosition =
                Vector2.MoveTowards(
                    bottomBar.anchoredPosition,
                    bottomTargetPosition,
                    letterboxExitSpeed * Time.deltaTime
                );

            bool topFinished =
                Vector2.Distance(
                    topBar.anchoredPosition,
                    topTargetPosition
                ) < 0.1f;

            bool bottomFinished =
                Vector2.Distance(
                    bottomBar.anchoredPosition,
                    bottomTargetPosition
                ) < 0.1f;

            if (topFinished && bottomFinished)
                break;

            yield return null;
        }

        // 완전히 빠진 뒤 LetterboxUI 자체 비활성화
        if (letterboxUI != null)
            letterboxUI.SetActive(false);
    }
    private IEnumerator ShowVictoryUI()
    {
        if (victoryUI == null || victoryCanvasGroup == null)
            yield break;

        victoryUI.SetActive(true);

        victoryCanvasGroup.alpha = 0f;
        victoryCanvasGroup.interactable = false;
        victoryCanvasGroup.blocksRaycasts = false;

        float elapsed = 0f;

        while (elapsed < victoryFadeDuration)
        {
            elapsed += Time.deltaTime;

            victoryCanvasGroup.alpha = Mathf.Lerp(
                0f,
                1f,
                elapsed / victoryFadeDuration
            );

            yield return null;
        }

        victoryCanvasGroup.alpha = 1f;
        victoryCanvasGroup.interactable = true;
        victoryCanvasGroup.blocksRaycasts = true;
        yield return StartCoroutine(ShowRewardsSequentially());
    }

    private IEnumerator ShowRewardsSequentially() //보상 효과
    {
        if (expReward != null)
        {
            yield return StartCoroutine(FadeInReward(expReward));
            yield return new WaitForSeconds(rewardInterval);
        }

        if (goldReward != null)
        {
            yield return StartCoroutine(FadeInReward(goldReward));
            yield return new WaitForSeconds(rewardInterval);
        }

        if (upgradeStoneReward != null)
        {
            yield return StartCoroutine(FadeInReward(upgradeStoneReward));
        }
    }
    private IEnumerator FadeInReward(CanvasGroup reward)//공통 페이드 함수
    {
        reward.alpha = 0f;

        float elapsed = 0f;

        while (elapsed < rewardFadeDuration)
        {
            elapsed += Time.deltaTime;

            reward.alpha = Mathf.Lerp(
                0f,
                1f,
                elapsed / rewardFadeDuration
            );

            yield return null;
        }

        reward.alpha = 1f;
    }
}
