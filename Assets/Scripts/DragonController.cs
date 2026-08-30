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

    private bool isDead = false;

    private static readonly int ClawAttackHash =
        Animator.StringToHash("ClawAttack");

    private static readonly int FlameAttackHash =
        Animator.StringToHash("FlameAttack");

    private static readonly int DieHash =
        Animator.StringToHash("Die");

    
    private void Start()
    {
        StartCoroutine(AttackRoutine());
    }

    private void Update()
    {
        if (isDead)
            return;

        FacePlayer();
    }

    private IEnumerator AttackRoutine()
{
    while (!isDead)
    {
        yield return new WaitForSeconds(attackDelay);

        if (isDead)
            yield break;

        RandomAttack();
    }
}

    private void RandomAttack()
    {
        int randomAttack = Random.Range(0, 2);

        if (randomAttack == 0)
        {
            animator.SetTrigger(ClawAttackHash);
        }
        else
        {
            animator.SetTrigger(FlameAttackHash);
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

        animator.ResetTrigger(ClawAttackHash);
        animator.ResetTrigger(FlameAttackHash);

        animator.SetTrigger(DieHash);
    }
}