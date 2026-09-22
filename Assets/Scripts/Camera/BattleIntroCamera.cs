using UnityEngine;

public class BattleIntroCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform dragon;

    [Header("Orbit")]
    [SerializeField] private float orbitSpeed = 5f;
    [SerializeField] private float lookHeight = 4f;

    private bool isOrbiting = true;

    private Vector3 battlePosition;
    private Quaternion battleRotation;

    private void Start()
    {
        // Fight 이후 돌아갈 기존 카메라 위치 저장
        battlePosition = transform.position;
        battleRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        if (!isOrbiting || dragon == null)
            return;

        // 드래곤을 중심으로 천천히 회전
        transform.RotateAround(
            dragon.position,
            Vector3.up,
            orbitSpeed * Time.deltaTime
        );

        // 드래곤 몸 중심을 바라보도록 설정
        Vector3 lookTarget =
            dragon.position + Vector3.up * lookHeight;

        transform.LookAt(lookTarget);
    }

    public void StartBattle()
    {
        isOrbiting = false;

        // 원래 전투 카메라 위치로 복귀
        transform.position = battlePosition;
        transform.rotation = battleRotation;
    }
}