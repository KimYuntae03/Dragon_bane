using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    private bool isAttacking = false;
    private bool hasEnteredAttackState = false;
    private bool useRightAttack = true;

    private static readonly int AttackRightHash = Animator.StringToHash("AttackRight");
    private static readonly int AttackLeftHash = Animator.StringToHash("AttackLeft");
    private static readonly int PunchRightHash = Animator.StringToHash("Punch_Right");
    private static readonly int PunchLeftHash = Animator.StringToHash("Punch_Left");

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
        if (isAttacking)
            return;

        isAttacking = true;
        hasEnteredAttackState = false;

        if (useRightAttack)
            animator.SetTrigger(AttackRightHash);
        else
            animator.SetTrigger(AttackLeftHash);

        useRightAttack = !useRightAttack;
    }

    private void CheckAttackState()
    {
        if (!isAttacking)
            return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        bool isPunching =
            stateInfo.shortNameHash == PunchRightHash ||
            stateInfo.shortNameHash == PunchLeftHash;

        if (isPunching)
        {
            hasEnteredAttackState = true;

            // 공격 애니메이션이 90% 이상 재생되면 다음 공격 입력 허용
            if (stateInfo.normalizedTime >= 0.9f)
            {
                isAttacking = false;
                hasEnteredAttackState = false;
            }

            return;
        }

        if (hasEnteredAttackState && !animator.IsInTransition(0))
        {
            isAttacking = false;
            hasEnteredAttackState = false;
        }
    }
}