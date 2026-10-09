using System.Collections;
using UnityEngine;

public class DefeatResultDirector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CameraFollow cameraFollow;

    [SerializeField] private Transform player;

    [SerializeField] private GameObject battleUIRoot;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private DragonController dragonController;


    [Header("Letterbox UI")]
    [SerializeField] private GameObject letterboxUI;
    [SerializeField] private RectTransform topBar;
    [SerializeField] private RectTransform bottomBar;

    [SerializeField] private float letterboxTargetHeight = 120f;
    [SerializeField] private float letterboxSpeed = 400f;

    [SerializeField] private float letterboxExitDistance = 180f;
    [SerializeField] private float letterboxExitSpeed = 150f;


    [Header("Defeat UI")]
    [SerializeField] private GameObject defeatUI;
    [SerializeField] private CanvasGroup defeatCanvasGroup;
    [SerializeField] private float defeatFadeDuration = 0.6f;


    [Header("Defeat Camera")]
    [SerializeField] private Vector3 cameraOffset =
        new Vector3(-4f, 2.5f, -4f);

    [SerializeField] private float cameraLookHeight = 1f;
    [SerializeField] private float cinematicFieldOfView = 55f;

    [SerializeField] private float cameraMoveSpeed = 4f;
    [SerializeField] private float cameraRotateSpeed = 6f;

    [SerializeField] private float cinematicDuration = 2.5f;


    private bool isPlayingCinematic = false;


    public void PlayDefeatCinematic()
    {
        if (isPlayingCinematic)
            return;

        StartCoroutine(DefeatCinematicRoutine());
    }


    private IEnumerator DefeatCinematicRoutine()
    {
        isPlayingCinematic = true;

        // 전투 UI 숨기기
        if (battleUIRoot != null)
            battleUIRoot.SetActive(false);

        // 레터박스 활성화
        if (letterboxUI != null)
            letterboxUI.SetActive(true);

        // 플레이어 입력 정지
        if (playerController != null)
            playerController.enabled = false;

        // 드래곤 행동 정지
        if (dragonController != null)
            dragonController.SetBattleActive(false);

        // 기존 카메라 추적 정지
        if (cameraFollow != null)
            cameraFollow.enabled = false;


        float elapsed = 0f;

        // 플레이어 사망 장면
        while (elapsed < cinematicDuration)
        {
            elapsed += Time.deltaTime;

            UpdateLetterbox();
            UpdateCameraToPlayer();

            yield return null;
        }


        // 레터박스가 빠지는 동안 Defeat UI 등장
        StartCoroutine(ShowDefeatUI());

        yield return StartCoroutine(HideLetterbox());
    }


    private void UpdateLetterbox()
    {
        if (topBar != null)
        {
            Vector2 size = topBar.sizeDelta;

            size.y = Mathf.MoveTowards(
                size.y,
                letterboxTargetHeight,
                letterboxSpeed * Time.deltaTime
            );

            topBar.sizeDelta = size;
        }


        if (bottomBar != null)
        {
            Vector2 size = bottomBar.sizeDelta;

            size.y = Mathf.MoveTowards(
                size.y,
                letterboxTargetHeight,
                letterboxSpeed * Time.deltaTime
            );

            bottomBar.sizeDelta = size;
        }
    }


    private void UpdateCameraToPlayer()
    {
        if (mainCamera == null || player == null)
            return;


        Vector3 targetPosition =
            player.position +
            player.TransformDirection(cameraOffset);


        mainCamera.transform.position =
            Vector3.Lerp(
                mainCamera.transform.position,
                targetPosition,
                cameraMoveSpeed * Time.deltaTime
            );


        mainCamera.fieldOfView =
            Mathf.Lerp(
                mainCamera.fieldOfView,
                cinematicFieldOfView,
                cameraMoveSpeed * Time.deltaTime
            );


        Vector3 lookTarget =
            player.position +
            Vector3.up * cameraLookHeight;


        Vector3 direction =
            lookTarget - mainCamera.transform.position;


        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);


            mainCamera.transform.rotation =
                Quaternion.Slerp(
                    mainCamera.transform.rotation,
                    targetRotation,
                    cameraRotateSpeed * Time.deltaTime
                );
        }
    }


    private IEnumerator HideLetterbox()
    {
        if (topBar == null || bottomBar == null)
            yield break;


        Vector2 topStartPosition =
            topBar.anchoredPosition;

        Vector2 bottomStartPosition =
            bottomBar.anchoredPosition;


        Vector2 topTargetPosition =
            topStartPosition +
            Vector2.up * letterboxExitDistance;

        Vector2 bottomTargetPosition =
            bottomStartPosition +
            Vector2.down * letterboxExitDistance;


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


        if (letterboxUI != null)
            letterboxUI.SetActive(false);
    }


    private IEnumerator ShowDefeatUI()
    {
        if (defeatUI == null || defeatCanvasGroup == null)
            yield break;


        defeatUI.SetActive(true);

        defeatCanvasGroup.alpha = 0f;
        defeatCanvasGroup.interactable = false;
        defeatCanvasGroup.blocksRaycasts = false;


        float elapsed = 0f;


        while (elapsed < defeatFadeDuration)
        {
            elapsed += Time.deltaTime;


            defeatCanvasGroup.alpha =
                Mathf.Lerp(
                    0f,
                    1f,
                    elapsed / defeatFadeDuration
                );


            yield return null;
        }


        defeatCanvasGroup.alpha = 1f;
        defeatCanvasGroup.interactable = true;
        defeatCanvasGroup.blocksRaycasts = true;
    }
}