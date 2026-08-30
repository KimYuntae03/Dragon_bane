using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform dragon;

    [Header("Camera")]
    [SerializeField] private float backDistance = 3f;
    [SerializeField] private float height = 2f;

    [SerializeField] private float followSpeed = 12f;
    [SerializeField] private float rotationSpeed = 10f;

    private void LateUpdate()
    {
        if (player == null || dragon == null)
            return;

        // 드래곤에서 플레이어를 향하는 방향
        Vector3 radialDirection =
            player.position - dragon.position;

        radialDirection.y = 0f;

        if (radialDirection.sqrMagnitude < 0.001f)
            return;

        radialDirection.Normalize();

        // 항상 플레이어의 뒤쪽(드래곤 반대 방향)에 카메라 배치
        Vector3 targetPosition =
            player.position
            + radialDirection * backDistance
            + Vector3.up * height;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );

        // 카메라는 드래곤을 바라봄
        Vector3 lookDirection =
            dragon.position - transform.position;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(lookDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}