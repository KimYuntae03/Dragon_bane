using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    private bool isAttacking = false;
    private bool hasEnteredAttackState = false;
    private bool useRightAttack = true;
    private bool attackQueued = false;

    private static readonly int AttackRightHash = Animator.StringToHash("AttackRight");
    private static readonly int AttackLeftHash = Animator.StringToHash("AttackLeft");
    private static readonly int PunchRightHash = Animator.StringToHash("Attackright");
    private static readonly int PunchLeftHash = Animator.StringToHash("Attackleft");

    private void Update()
    {
        CheckAttackState();

        if (Keyboard.current != null &&
            Keyboard.current.aKey.wasPressedThisFrame)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
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
}