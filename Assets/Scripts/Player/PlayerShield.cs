using UnityEngine;
using System.Collections;

public class PlayerShield : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject shieldEffect;

    [SerializeField] private float shieldDelay = 1f;

    [SerializeField] private Collider shieldCollider;

    private static readonly int HittedHash =
    Animator.StringToHash("Hitted");

    private static readonly int Hitted2Hash =
        Animator.StringToHash("Hitted2");

    private bool isGuarding = false;
    private Coroutine shieldCoroutine;

    private static readonly int IsGuardingHash =
        Animator.StringToHash("IsGuarding");

    public bool IsGuarding => isGuarding;

    public void StartGuard()
    {
        if (isGuarding)
            return;

        isGuarding = true;

        //방어 애니메이션 시작
        animator.SetBool(IsGuardingHash, true);

        //기존 코루틴이 있다면 중지
        if (shieldCoroutine != null)
            StopCoroutine(shieldCoroutine);

        shieldCoroutine = StartCoroutine(ShowShieldAfterDelay());
    }

    public void StopGuard()
    {
        isGuarding = false;

        //방어 애니메이션 종료
        animator.SetBool(IsGuardingHash, false);

        if (shieldCoroutine != null)
        {
            StopCoroutine(shieldCoroutine);
            shieldCoroutine = null;
        }

        // 쉴드 VFX 비활성화
        if (shieldEffect != null)
            shieldEffect.SetActive(false);

        // Flame Effect 충돌용 Collider 비활성화
        if (shieldCollider != null)
            shieldCollider.enabled = false;
    }

    // 방어 애니메이션에 맞춰 쉴드를 늦게 전개
    private IEnumerator ShowShieldAfterDelay()
    {
        yield return new WaitForSeconds(shieldDelay);

        AnimatorStateInfo stateInfo =
            animator.GetCurrentAnimatorStateInfo(0);

        bool isHitState =
            stateInfo.shortNameHash == HittedHash ||
            stateInfo.shortNameHash == Hitted2Hash;

        if (isGuarding && !isHitState)
        {
            if (shieldEffect != null)
                shieldEffect.SetActive(true);

            if (shieldCollider != null)
                shieldCollider.enabled = true;
        }

        shieldCoroutine = null;
    }
}