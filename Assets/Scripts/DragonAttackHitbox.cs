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
        playerHealth.TakeDamage(damage);
    }

    private void TryDamagePlayer(Collider other)
    {
        if (hasHitPlayer)
            return;

        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
            return;

        hasHitPlayer = true;
        playerHealth.TakeDamage(damage);
    }
}