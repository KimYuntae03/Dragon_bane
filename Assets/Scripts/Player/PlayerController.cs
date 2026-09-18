using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    
    [SerializeField] private PlayerShield playerShield;

    [SerializeField] private PlayerDodge playerDodge;

    [SerializeField] private PlayerHealth playerHealth;

    [SerializeField] private PlayerHitReaction playerHitReaction;

    private bool isAttacking = false;
    private bool hasEnteredAttackState = false;
    private bool useRightAttack = true;
    private bool isDead = false;

    private int currentNormalAttackStateHash = 0;

    private bool isBusy = false;

    private static readonly int AttackRightHash = Animator.StringToHash("AttackRight");
    private static readonly int AttackLeftHash = Animator.StringToHash("AttackLeft");
    private static readonly int PunchRightHash = Animator.StringToHash("Attackright");
    private static readonly int PunchLeftHash = Animator.StringToHash("Attackleft");
        
    private static readonly int DieHash = Animator.StringToHash("Die");
    
    private void Update()
    {
        if (!isDead)
        {
            CheckAttackState();
        }

        if (Keyboard.current == null)
            return;

        // 죽은 뒤 모든 조작 차단
        if (isDead)
            return;
        if (playerShield.IsGuarding &&
            Keyboard.current.sKey.wasReleasedThisFrame)
        {
            playerShield.StopGuard();
            EndAction();
            return;
        }

        if (isBusy)
            return;

        if (Keyboard.current.aKey.wasPressedThisFrame)
            TryAttack();
        
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            if (!playerDodge.IsDodging)
            {
                CancelAttack();
                isBusy = true;
                playerShield.StartGuard();
            }
        }

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            if (!playerDodge.IsDodging){
                CancelAttack();
                playerShield.StopGuard();
                isBusy = true;
                playerDodge.DodgeLeft();
            }
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!playerDodge.IsDodging)
            {
                CancelAttack();
                playerShield.StopGuard();
                isBusy = true;
                playerDodge.DodgeRight();
            }
        }

        if (Keyboard.current.hKey.wasPressedThisFrame)//H키 누르면 힐(임시)
        {
            //구르기, 쉴드, 피격중에는 힐 불가
            if (!playerDodge.IsDodging &&
                !playerShield.IsGuarding &&
                !playerHitReaction.IsHitReacting)
            {
                playerHealth.Heal(20f);
            }
        }
    }

    private void TryAttack()
    {
        if (playerDodge.IsDodging || isAttacking)
            return;

        isBusy = true;
        isAttacking = true;
        hasEnteredAttackState = false;

        // 일반 공격
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

        bool isRightAttack =
            stateInfo.shortNameHash == PunchRightHash;

        bool isLeftAttack =
            stateInfo.shortNameHash == PunchLeftHash;

        bool isNormalAttack =
            isRightAttack || isLeftAttack;


        // 일반 공격 State에 들어온 경우
        if (isNormalAttack)
        {
            hasEnteredAttackState = true;

            // 처음 공격 State에 진입
            if (currentNormalAttackStateHash == 0)
            {
                currentNormalAttackStateHash =
                    stateInfo.shortNameHash;
            }
            // Right → Left 또는 Left → Right로 넘어온 경우
            else if (currentNormalAttackStateHash != stateInfo.shortNameHash)
            {
                currentNormalAttackStateHash =
                    stateInfo.shortNameHash;
            }

            return;
        }


        // 일반 공격에서 다른 State로 완전히 빠져나온 경우
        if (currentNormalAttackStateHash != 0 &&
            !animator.IsInTransition(0))
        {
            currentNormalAttackStateHash = 0;
        }

        // 모든 공격이 끝나 Idle 등으로 복귀
        if (isAttacking &&
            hasEnteredAttackState &&
            !animator.IsInTransition(0))
        {
            Debug.Log("공격 상태 종료");
            isAttacking = false;
            hasEnteredAttackState = false;
        }
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        ResetAttackChain(); //연속공격 끊김
        playerShield.StopGuard(); //쉴드 중지
        playerDodge.CancelDodge();   // 추가

        // 현재 행동 상태 정리
        isAttacking = false;
        hasEnteredAttackState = false;

        // 남아 있는 공격/회피 Trigger 제거
        animator.ResetTrigger(AttackRightHash);
        animator.ResetTrigger(AttackLeftHash);

        animator.SetTrigger(DieHash);
    }

    private void ResetAttackChain()
    {
        currentNormalAttackStateHash = 0;

        animator.ResetTrigger(AttackRightHash);
        animator.ResetTrigger(AttackLeftHash);
    }

    private void CancelAttack()
    {
        isAttacking = false;
        hasEnteredAttackState = false;
        currentNormalAttackStateHash = 0;

        animator.ResetTrigger(AttackRightHash);
        animator.ResetTrigger(AttackLeftHash);
    }

    public void EndAction()
    {
        isBusy = false;
    }
    
}