using UnityEngine;
using System.Collections;

public class DragonController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Attack")]
    [SerializeField] private float attackDelay  = 10f;

    [Header("Effect")]
    [SerializeField] private ParticleSystem flameEffect;

    [Header("Attack Hitbox")]
    [SerializeField] private GameObject chargeHitbox;
    [SerializeField] private GameObject flameHitbox;

    [SerializeField] private Transform flamePoint;

    private bool isDead = false;
    private bool isAttacking = false;
    private bool battleActive = false;//전투 활성화,비활성화 변수
    private Coroutine attackRoutine;
    private Vector3 primaryAttackAim;
    private Vector3 secondaryAttackAim;
    private bool usesSecondaryMelee;
    private NightmareAttack currentNightmareAttack;

    private enum NightmareAttack { None, Horn, Claw }

    private int primaryAttackHash = Animator.StringToHash("ClawAttack");
    private int secondaryAttackHash = Animator.StringToHash("FlameAttack");

    private static readonly int DieHash =
        Animator.StringToHash("Die");
    private static readonly int NightmareClawStateHash =
        Animator.StringToHash("Claw Attack");
    private static readonly int NightmareHornStateHash =
        Animator.StringToHash("Horn Attack");

    
    private void Start()
    {
        StartCoroutine(AttackRoutine());
    }
    
    private void Update()
    {
        if (!battleActive)
            return;

        if (isDead)
            return;

        if (!isAttacking)
        {
            FacePlayer();
        }

        if (usesSecondaryMelee &&
            currentNightmareAttack != NightmareAttack.Claw &&
            flameHitbox != null && flameHitbox.activeSelf)
        {
            flameHitbox.SetActive(false);
        }

        if (usesSecondaryMelee && currentNightmareAttack == NightmareAttack.Claw)
        {
            bool inDamageWindow = IsClawDamageWindow();
            if (inDamageWindow && flameHitbox != null && !flameHitbox.activeSelf)
                StartSecondaryMeleeHitbox();
            else if (!inDamageWindow)
                StopSecondaryMeleeHitbox();
        }

        if (usesSecondaryMelee && currentNightmareAttack == NightmareAttack.Horn)
        {
            StopSecondaryMeleeHitbox();
            bool inDamageWindow = IsHornDamageWindow();
            if (inDamageWindow && chargeHitbox != null && !chargeHitbox.activeSelf)
                StartChargeHitbox();
            else if (!inDamageWindow)
                StopChargeHitbox();
        }
    }

    private bool IsHornDamageWindow()
    {
        if (animator == null)
            return false;

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        return state.shortNameHash == NightmareHornStateHash &&
               state.normalizedTime >= 0.62f &&
               state.normalizedTime < 0.70f;
    }

    private bool IsClawDamageWindow()
    {
        if (animator == null)
            return false;

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        return state.shortNameHash == NightmareClawStateHash &&
               state.normalizedTime >= 0.52f &&
               state.normalizedTime < 0.64f;
    }

    private IEnumerator AttackRoutine()
    {
        while (!isDead && battleActive)
        {
            yield return new WaitForSeconds(attackDelay);

            if (isDead || !battleActive)
                break;

            RandomAttack();
        }
        attackRoutine = null;
    }

    private void RandomAttack()
    {
        if (isAttacking)
            return;
        FacePlayer();
        isAttacking = true;

        int randomAttack = Random.Range(0, 2);

        if (randomAttack == 0)
        {
            if (usesSecondaryMelee)
            {
                currentNightmareAttack = NightmareAttack.Horn;
                StopSecondaryMeleeHitbox();
                SetNightmareAttackArc(chargeHitbox, -8f, 8f, true);
                if (player != null)
                    primaryAttackAim = player.position;
            }
            animator.SetTrigger(primaryAttackHash);
        }
        else
        {
            if (usesSecondaryMelee)
            {
                currentNightmareAttack = NightmareAttack.Claw;
                StopChargeHitbox();
                SetNightmareAttackArc(flameHitbox, -33f, 8f);
            }
            if (player != null)
                secondaryAttackAim = player.position;
            animator.SetTrigger(secondaryAttackHash);
        }
    }

    private void SetNightmareAttackArc(
        GameObject hitbox,
        float minimumAngle,
        float maximumAngle,
        bool allowDodgeEvasion = false)
    {
        if (hitbox == null || player == null)
            return;

        DragonAttackHitbox damageHitbox =
            hitbox.GetComponent<DragonAttackHitbox>();

        if (damageHitbox != null)
        {
            damageHitbox.SetAttackArc(
                transform,
                player.position - transform.position,
                minimumAngle,
                maximumAngle,
                allowDodgeEvasion
            );
        }
    }

    private void FacePlayer()
    {
        if (player == null)
            return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction.normalized);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;
        isAttacking = false;
        currentNightmareAttack = NightmareAttack.None;

        StopFlame();
        if (chargeHitbox != null)
            chargeHitbox.SetActive(false);

        if (flameHitbox != null)
            flameHitbox.SetActive(false);
            
        animator.ResetTrigger(primaryAttackHash);
        animator.ResetTrigger(secondaryAttackHash);

        animator.SetTrigger(DieHash);
    }

    public void StartFlame()
    {
        if (player != null && flamePoint != null)
        {
            Vector3 direction =
                player.position - flamePoint.position;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                flamePoint.rotation =
                    Quaternion.LookRotation(direction.normalized);
            }
        }

        if (flameEffect != null)
            flameEffect.Play();

        if (flameHitbox != null)
            flameHitbox.SetActive(true);
    }

    public void StopFlame()
    {
        if (flameEffect != null)
            flameEffect.Stop();

        if (flameHitbox != null)
            flameHitbox.SetActive(false);
    }

    public void StartChargeHitbox()
    {
        if (usesSecondaryMelee &&
            (currentNightmareAttack != NightmareAttack.Horn ||
             !IsHornDamageWindow()))
            return;

        // Horn 판정 중에는 오른팔 Claw 판정이 남아 있지 않게 한다.
        if (usesSecondaryMelee)
        {
            StopSecondaryMeleeHitbox();
            PositionHornHitbox();
        }
        if (chargeHitbox != null)
            chargeHitbox.SetActive(true);
    }

    private void PositionHornHitbox()
    {
        if (chargeHitbox == null || player == null)
            return;

        Transform hitbox = chargeHitbox.transform;
        Vector3 headPosition = hitbox.parent.position;
        Vector3 targetPosition = primaryAttackAim + Vector3.up * 0.9f;
        Vector3 direction = targetPosition - headPosition;
        Vector3 flatDirection = Vector3.ProjectOnPlane(direction, Vector3.up);

        if (flatDirection.sqrMagnitude < 0.001f)
            return;

        hitbox.position = (headPosition + targetPosition) * 0.5f;
        hitbox.rotation = Quaternion.LookRotation(flatDirection.normalized);

        BoxCollider collider = hitbox.GetComponent<BoxCollider>();
        if (collider != null)
        {
            Vector3 scale = hitbox.lossyScale;
            Vector3 size = collider.size;
            size.y = Mathf.Max(2f, Mathf.Abs(direction.y) + 1f) /
                Mathf.Max(0.001f, Mathf.Abs(scale.y));
            size.z = (flatDirection.magnitude + 0.5f) /
                Mathf.Max(0.001f, Mathf.Abs(scale.z));
            collider.size = size;
        }
    }

    public void StopChargeHitbox()
    {
        if (chargeHitbox != null)
            chargeHitbox.SetActive(false);
    }

    public void StartSecondaryMeleeHitbox()
    {
        if (!usesSecondaryMelee ||
            currentNightmareAttack != NightmareAttack.Claw ||
            !IsClawDamageWindow())
            return;

        StopChargeHitbox();
        if (flameHitbox == null)
            return;

        if (player != null)
        {
            Vector3 handPosition = flameHitbox.transform.parent.position;
            Vector3 direction = secondaryAttackAim - handPosition;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                float reach = direction.magnitude;
                direction.Normalize();
                // 플레이어의 오른쪽 회피 방향으로 치우친 Claw 궤도.
                Vector3 leftDodgeDirection =
                    Vector3.Cross(Vector3.up, direction);
                flameHitbox.transform.position =
                    handPosition + direction * (reach * 0.5f)
                    - leftDodgeDirection * 1.2f;
                flameHitbox.transform.rotation =
                    Quaternion.LookRotation(direction);

                BoxCollider clawCollider = flameHitbox.GetComponent<BoxCollider>();
                if (clawCollider != null)
                {
                    Vector3 size = clawCollider.size;
                    size.z = Mathf.Max(4.5f, reach + 0.5f);
                    clawCollider.size = size;
                }
            }
        }

        flameHitbox.SetActive(true);
    }

    public void StopSecondaryMeleeHitbox()
    {
        if (flameHitbox != null)
            flameHitbox.SetActive(false);
    }

    public void ConfigureAttackProfile(
        Animator newAnimator,
        string primaryTrigger,
        string secondaryTrigger,
        GameObject primaryHitbox,
        GameObject secondaryHitbox)
    {
        if (chargeHitbox != null)
            chargeHitbox.SetActive(false);
        if (flameHitbox != null)
            flameHitbox.SetActive(false);

        if (newAnimator != null)
            animator = newAnimator;

        primaryAttackHash = Animator.StringToHash(primaryTrigger);
        secondaryAttackHash = Animator.StringToHash(secondaryTrigger);
        usesSecondaryMelee = true;
        currentNightmareAttack = NightmareAttack.None;
        chargeHitbox = primaryHitbox;
        flameHitbox = secondaryHitbox;
        flameEffect = null;
        flamePoint = null;

        if (chargeHitbox != null)
            chargeHitbox.SetActive(false);
        if (flameHitbox != null)
            flameHitbox.SetActive(false);
    }

    public void EndAttack()
    {
        if (usesSecondaryMelee)
        {
            StopChargeHitbox();
            StopSecondaryMeleeHitbox();
            currentNightmareAttack = NightmareAttack.None;
        }
        isAttacking = false;
    }

    public void SetAnimator(Animator newAnimator)
    {
        if (newAnimator != null)
            animator = newAnimator;
    }

    public void SetBattleActive(bool active)
    {
        battleActive = active;
        if (battleActive)
        {
            if (attackRoutine == null && !isDead)
            {
                attackRoutine = StartCoroutine(AttackRoutine());
            }
        }
        else
        {
            if (attackRoutine != null)
            {
                StopCoroutine(attackRoutine);
                attackRoutine = null;
            }
        }
    }
}
