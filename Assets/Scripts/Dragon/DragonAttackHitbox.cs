using UnityEngine;

public class DragonAttackHitbox : MonoBehaviour
{

    public enum AttackType
    {
        Charge,
        Flame
    }

    [SerializeField] private AttackType attackType;
    [SerializeField] private float damage = 20f;
    

    private bool hasHitPlayer = false;
    private Transform attackCenter;
    private Vector3 attackDirection;
    private float minimumHitAngle;
    private float maximumHitAngle;
    private bool dodgeAvoidsAttack;

    public void SetAttackArc(
        Transform center,
        Vector3 direction,
        float minimumAngle,
        float maximumAngle,
        bool allowDodgeEvasion = false)
    {
        attackCenter = center;
        attackDirection = Vector3.ProjectOnPlane(direction, Vector3.up).normalized;
        minimumHitAngle = minimumAngle;
        maximumHitAngle = maximumAngle;
        dodgeAvoidsAttack = allowDodgeEvasion;
    }

    public void Configure(AttackType type, float attackDamage)
    {
        attackType = type;
        damage = attackDamage;
    }

    public void SetDamage(float attackDamage)
    {
        damage = attackDamage;
    }

    private void OnEnable()
    {
        hasHitPlayer = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Shield"))
            return; //Claw공격은 쉴드로 못막게 설계

        if (attackType == AttackType.Charge)
        {
            TryDamagePlayer(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // ShieldCollider 자체는 무시
        if (other.gameObject.layer == LayerMask.NameToLayer("Shield"))
            return;

        // 타격 구간에 이미 플레이어와 겹친 채 켜진 근접 판정도 감지한다.
        if (attackType == AttackType.Charge)
        {
            TryDamagePlayer(other);
            return;
        }

        if (attackType != AttackType.Flame)
            return;

        if (hasHitPlayer)
            return;

        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        PlayerShield playerShield =
            other.GetComponentInParent<PlayerShield>();

        if (playerHealth == null)
            return;

        // 쉴드가 켜져 있는 동안은 계속 막힘
        if (playerShield != null &&
            playerShield.IsGuarding)
        {
            return;
        }

        // 쉴드를 풀었는데 Flame 안에 아직 있으면 데미지
        hasHitPlayer = true;
        playerHealth.TakeDamage(damage,PlayerHealth.HitType.Flame);
    }

    private void TryDamagePlayer(Collider other)
    {
        if (hasHitPlayer)
            return;

        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
            return;

        if (dodgeAvoidsAttack)
        {
            PlayerDodge dodge = playerHealth.GetComponent<PlayerDodge>();
            if (dodge != null && dodge.IsEvadingHorn)
                return;
        }

        if (attackCenter != null && attackDirection.sqrMagnitude > 0f)
        {
            Vector3 playerDirection = playerHealth.transform.position - attackCenter.position;
            playerDirection.y = 0f;
            float angle = Vector3.SignedAngle(
                attackDirection,
                playerDirection,
                Vector3.up
            );

            if (angle < minimumHitAngle || angle > maximumHitAngle)
                return;
        }

        hasHitPlayer = true;
#if UNITY_EDITOR
        Debug.Log($"Dragon hitbox hit player: {gameObject.name}", this);
#endif
        playerHealth.TakeDamage(damage,PlayerHealth.HitType.Claw);
    }
}
