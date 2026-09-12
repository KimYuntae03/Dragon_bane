using UnityEngine;
using System.Collections;

public class PlayerDodge : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Target")]
    [SerializeField] private Transform dragon;
    [SerializeField] private Transform playerModel;

    [Header("Dodge")]
    [SerializeField] private float dodgeAngle = 25f;

    private bool isDodging = false;

    public bool IsDodging => isDodging;

    private static readonly int DodgeLeftHash =
        Animator.StringToHash("DodgeLeft");

    private static readonly int DodgeRightHash =
        Animator.StringToHash("DodgeRight");

    private static readonly int DodgeLeftStateHash =
        Animator.StringToHash("Dodge_Left");

    private static readonly int DodgeRightStateHash =
        Animator.StringToHash("Dodge_Right");

    private void Update()
    {
        CheckDodgeState();
    }

    public void DodgeLeft()
    {
        if (isDodging)
            return;

        isDodging = true;

        animator.SetTrigger(DodgeLeftHash);

        StartCoroutine(
            DodgeAroundDragon(
                dodgeAngle,
                DodgeLeftStateHash
            )
        );
    }

    public void DodgeRight()
    {
        if (isDodging)
            return;

        isDodging = true;

        animator.SetTrigger(DodgeRightHash);

        StartCoroutine(
            DodgeAroundDragon(
                -dodgeAngle,
                DodgeRightStateHash
            )
        );
    }

    private void CheckDodgeState()
    {
        if (!isDodging)
            return;

        AnimatorStateInfo stateInfo =
            animator.GetCurrentAnimatorStateInfo(0);

        bool isDodgeState =
            stateInfo.shortNameHash == DodgeLeftStateHash ||
            stateInfo.shortNameHash == DodgeRightStateHash;

        if (!isDodgeState &&
            !animator.IsInTransition(0))
        {
            isDodging = false;
        }
    }

    private IEnumerator DodgeAroundDragon(
        float angle,
        int dodgeStateHash)
    {
        Vector3 center = dragon.position;

        Vector3 startPosition = transform.position;
        Vector3 startOffset = startPosition - center;

        Quaternion finalOrbitRotation =
            Quaternion.Euler(0f, angle, 0f);

        Vector3 targetPosition =
            center + finalOrbitRotation * startOffset;

        Vector3 dodgeDirection =
            targetPosition - startPosition;

        dodgeDirection.y = 0f;

        if (dodgeDirection.sqrMagnitude > 0.001f)
        {
            float directionOffset = 0f;

            if (angle > 0f)
                directionOffset = 30f;
            else if (angle < 0f)
                directionOffset = 40f;

            playerModel.rotation =
                Quaternion.LookRotation(dodgeDirection.normalized)
                * Quaternion.Euler(
                    0f,
                    directionOffset,
                    0f
                );
        }

        // 실제 Dodge State 진입 대기
        while (true)
        {
            AnimatorStateInfo stateInfo =
                animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.shortNameHash == dodgeStateHash)
                break;

            yield return null;
        }

        // 애니메이션 진행률에 맞춰 원호 이동
        while (true)
        {
            AnimatorStateInfo stateInfo =
                animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.shortNameHash != dodgeStateHash)
                break;

            float t =
                Mathf.Clamp01(stateInfo.normalizedTime);

            Quaternion orbitRotation =
                Quaternion.Euler(
                    0f,
                    angle * t,
                    0f
                );

            transform.position =
                center + orbitRotation * startOffset;

            yield return null;
        }

        transform.position = targetPosition;

        FaceDragon();
    }

    private void FaceDragon()
    {
        Vector3 direction =
            dragon.position - playerModel.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        playerModel.rotation =
            Quaternion.LookRotation(
                direction.normalized
            );
    }

    public void CancelDodge()
    {
        isDodging = false;

        animator.ResetTrigger(DodgeLeftHash);
        animator.ResetTrigger(DodgeRightHash);

        StopAllCoroutines();
    }
}