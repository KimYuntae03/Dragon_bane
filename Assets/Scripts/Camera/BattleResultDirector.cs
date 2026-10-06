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

    [Header("Cinematic Camera")]
    [SerializeField] private Vector3 cameraOffset = new Vector3(-7.2f, 4.8f, 7.2f);
    [SerializeField] private float cameraLookHeight = 1.5f;
    [SerializeField] private float cinematicFieldOfView = 55f;
    [SerializeField] private float cameraMoveSpeed = 4f;
    [SerializeField] private float cameraRotateSpeed = 6f;
    [SerializeField] private float cinematicDuration = 2.5f;

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
}
