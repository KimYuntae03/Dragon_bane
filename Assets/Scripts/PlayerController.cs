using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Dodge")]
    [SerializeField] private Transform dragon;
    [SerializeField] private Transform playerModel;

    [SerializeField] private float dodgeAngle = 20f;

    private bool isAttacking = false;
    private bool hasEnteredAttackState = false;
    private bool useRightAttack = true;
    private bool attackQueued = false;
    private bool isDodging = false;
    private bool isDead = false;

    private static readonly int AttackRightHash = Animator.StringToHash("AttackRight");
    private static readonly int AttackLeftHash = Animator.StringToHash("AttackLeft");
    private static readonly int PunchRightHash = Animator.StringToHash("Attackright");
    private static readonly int PunchLeftHash = Animator.StringToHash("Attackleft");

    private static readonly int IsGuardingHash = Animator.StringToHash("IsGuarding");

    private static readonly int DodgeLeftHash = Animator.StringToHash("DodgeLeft");

    private static readonly int DodgeLeftStateHash = Animator.StringToHash("Dodge_Left");

    private static readonly int DodgeRightHash = Animator.StringToHash("DodgeRight");

    private static readonly int DodgeRightStateHash = Animator.StringToHash("Dodge_Right");
        
    private static readonly int DieHash = Animator.StringToHash("Die");

    private void Update()
    {
        if (!isDead)
        {
            CheckAttackState();
            CheckDodgeState();
        }

        if (Keyboard.current == null)
            return;

        // 임시 사망 테스트
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            Die();
            return;
        }

        // 죽은 뒤 모든 조작 차단
        if (isDead)
            return;

        if (Keyboard.current.aKey.wasPressedThisFrame)
            TryAttack();

        if (Keyboard.current.sKey.wasPressedThisFrame)
            StartGuard();

        if (Keyboard.current.sKey.wasReleasedThisFrame)
            StopGuard();

        if (Keyboard.current.qKey.wasPressedThisFrame)
            TryDodgeLeft();

        if (Keyboard.current.eKey.wasPressedThisFrame)
            TryDodgeRight();
    }

    private void TryAttack()
    {
        if (isDodging)
            return;

        // 공격 중이면 다음 공격 하나만 예약
        if (isAttacking)
        {
            if (attackQueued)
                return;

            attackQueued = true;

            if (useRightAttack)
                animator.SetTrigger(AttackRightHash);
            else
                animator.SetTrigger(AttackLeftHash);

            useRightAttack = !useRightAttack;

            return;
        }

        // 첫 공격
        isAttacking = true;
        hasEnteredAttackState = false;
        attackQueued = false;

        if (useRightAttack)
            animator.SetTrigger(AttackRightHash);
        else
            animator.SetTrigger(AttackLeftHash);

        useRightAttack = !useRightAttack;
    }


    private void CheckAttackState()
    {
        AnimatorStateInfo stateInfo =
            animator.GetCurrentAnimatorStateInfo(0);

        bool isPunching =
            stateInfo.shortNameHash == PunchRightHash ||
            stateInfo.shortNameHash == PunchLeftHash;

        if (isPunching)
        {
            hasEnteredAttackState = true;
            return;
        }

        // 공격 State를 완전히 빠져나와 Idle로 돌아온 경우
        if (isAttacking &&
            hasEnteredAttackState &&
            !animator.IsInTransition(0))
        {
            isAttacking = false;
            hasEnteredAttackState = false;
            attackQueued = false;
        }
    }

    private void StartGuard()
    {
        if (isAttacking || isDodging)
            return;

        animator.SetBool(IsGuardingHash, true);
    }

    private void StopGuard()
    {
        animator.SetBool(IsGuardingHash, false);
    }
    
    private void TryDodgeLeft()
    {
        if (isAttacking || isDodging)
            return;

        animator.SetBool(IsGuardingHash, false);

        isDodging = true;
        animator.SetTrigger(DodgeLeftHash);

        StartCoroutine(DodgeAroundDragon(dodgeAngle,DodgeLeftStateHash));
    }

    private void TryDodgeRight()
    {
        if (isAttacking || isDodging)
            return;

        animator.SetBool(IsGuardingHash, false);

        isDodging = true;
        animator.SetTrigger(DodgeRightHash);

        StartCoroutine(DodgeAroundDragon(-dodgeAngle,DodgeRightStateHash));
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

        if (!isDodgeState && !animator.IsInTransition(0))
        {
            isDodging = false;
        }
    }

    private IEnumerator DodgeAroundDragon(float angle, int dodgeStateHash)
    {
        Vector3 center = dragon.position;

        Vector3 startPosition = transform.position;
        Vector3 startOffset = startPosition - center;

        // 최종 도착 위치 계산
        Quaternion finalOrbitRotation =
            Quaternion.Euler(0f, angle, 0f);

        Vector3 targetPosition =
            center + finalOrbitRotation * startOffset;

        // 구르기 시작 방향 결정

        // 현재 위치 → 도착 위치 방향
        Vector3 dodgeDirection =
            targetPosition - startPosition;

        dodgeDirection.y = 0f;

        if (dodgeDirection.sqrMagnitude > 0.001f)
        {
            float directionOffset = 0f;

            if (angle > 0f) // 왼쪽 구르기
            {
                directionOffset = 30f;
            }
            else if (angle < 0f) // 오른쪽 구르기
            {
                directionOffset = 40f;
            }

            playerModel.rotation =
                Quaternion.LookRotation(dodgeDirection.normalized)
                * Quaternion.Euler(0f, directionOffset, 0f);
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
                Quaternion.Euler(0f, angle * t, 0f);

            transform.position =
                center + orbitRotation * startOffset;

            // 구르는 동안에는 방향 변경 안 함

            yield return null;
        }

        // 최종 위치 정확히 맞춤
        transform.position = targetPosition;

        // 구르기가 끝난 뒤에만 Dragon을 바라봄
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
            Quaternion.LookRotation(direction.normalized);
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        // 현재 행동 상태 정리
        isAttacking = false;
        isDodging = false;
        attackQueued = false;
        hasEnteredAttackState = false;

        // 방어 중이었다면 해제
        animator.SetBool(IsGuardingHash, false);

        // 남아 있는 공격/회피 Trigger 제거
        animator.ResetTrigger(AttackRightHash);
        animator.ResetTrigger(AttackLeftHash);
        animator.ResetTrigger(DodgeLeftHash);
        animator.ResetTrigger(DodgeRightHash);

        animator.SetTrigger(DieHash);
    }
}