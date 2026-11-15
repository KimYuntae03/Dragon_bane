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

    private int primaryAttackHash = Animator.StringToHash("ClawAttack");
    private int secondaryAttackHash = Animator.StringToHash("FlameAttack");

    private static readonly int DieHash =
        Animator.StringToHash("Die");

    
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
            animator.SetTrigger(primaryAttackHash);
        }
        else
        {
            animator.SetTrigger(secondaryAttackHash);
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
        if (chargeHitbox != null)
            chargeHitbox.SetActive(true);
    }

    public void StopChargeHitbox()
    {
        if (chargeHitbox != null)
            chargeHitbox.SetActive(false);
    }

    public void StartSecondaryMeleeHitbox()
    {
        if (flameHitbox != null)
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
