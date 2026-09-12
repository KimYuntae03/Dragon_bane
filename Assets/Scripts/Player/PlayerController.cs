using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    
    [SerializeField] private PlayerShield playerShield;

    [SerializeField] private PlayerDodge playerDodge;

    private bool isAttacking = false;
    private bool hasEnteredAttackState = false;
    private bool useRightAttack = true;
    private bool isDead = false;

    private int attackCount = 0;
    private const int PowerAttackRequirement = 7;
    private int currentNormalAttackStateHash = 0;
    private bool powerAttackStarted = false;

    

    private static readonly int AttackRightHash = Animator.StringToHash("AttackRight");
    private static readonly int AttackLeftHash = Animator.StringToHash("AttackLeft");
    private static readonly int PunchRightHash = Animator.StringToHash("Attackright");
    private static readonly int PunchLeftHash = Animator.StringToHash("Attackleft");
        
    private static readonly int DieHash = Animator.StringToHash("Die");

    private static readonly int PowerAttackHash = Animator.StringToHash("PowerAttack");

    private static readonly int PowerAttackStateHash = Animator.StringToHash("PowerAttack");
    
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

        if (Keyboard.current.aKey.wasPressedThisFrame)
            TryAttack();
        
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            if (!playerDodge.IsDodging)
            {
                CancelAttack();
                playerShield.StartGuard();
            }
        }

        if (Keyboard.current.sKey.wasReleasedThisFrame)
        {
            playerShield.StopGuard();
        }

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            CancelAttack();
            playerShield.StopGuard();
            playerDodge.DodgeLeft();
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            CancelAttack();
            playerShield.StopGuard();
            playerDodge.DodgeRight();
        }
    }

    private void TryAttack()
    {
        if (playerDodge.IsDodging || isAttacking)
            return;

        isAttacking = true;
        hasEnteredAttackState = false;

        // 일반 공격 7회 누적 후
        // 8번째 공격은 강화공격
        if (attackCount >= PowerAttackRequirement)
        {
            animator.SetTrigger(PowerAttackHash);

            ResetAttackChain();

            return;
        }

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

        bool isPowerAttack =
            stateInfo.shortNameHash == PowerAttackStateHash;


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
                // 이전 공격 애니메이션이 끝났으므로 카운트 증가
                RegisterNormalAttack();

                currentNormalAttackStateHash =
                    stateInfo.shortNameHash;
            }

            return;
        }


        // 일반 공격에서 다른 State로 완전히 빠져나온 경우
        if (currentNormalAttackStateHash != 0 &&
            !animator.IsInTransition(0))
        {
            // 마지막 일반 공격 완료
            RegisterNormalAttack();

            currentNormalAttackStateHash = 0;
        }


        // 강화공격도 공격 중 상태로 취급
        if (isPowerAttack)
        {
            hasEnteredAttackState = true;

            if (!powerAttackStarted)
            {
                powerAttackStarted = true;
                ResetAttackChain();
            }

            return;
        }


        // 모든 공격이 끝나 Idle 등으로 복귀
        if (isAttacking &&
            hasEnteredAttackState &&
            !animator.IsInTransition(0))
        {
            Debug.Log("공격 상태 종료");
            isAttacking = false;
            hasEnteredAttackState = false;
            powerAttackStarted = false;
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
        animator.ResetTrigger(PowerAttackHash);

        animator.SetTrigger(DieHash);
    }

    private void ResetAttackChain()
    {
        attackCount = 0;
        currentNormalAttackStateHash = 0;
        powerAttackStarted = false;

        animator.ResetTrigger(AttackRightHash);
        animator.ResetTrigger(AttackLeftHash);
        animator.ResetTrigger(PowerAttackHash);
    }
    private void RegisterNormalAttack()
    {
        attackCount++;
    }

    private void CancelAttack()
    {
        isAttacking = false;
        hasEnteredAttackState = false;
        currentNormalAttackStateHash = 0;
        powerAttackStarted = false;

        animator.ResetTrigger(AttackRightHash);
        animator.ResetTrigger(AttackLeftHash);
        animator.ResetTrigger(PowerAttackHash);
    }
    
}