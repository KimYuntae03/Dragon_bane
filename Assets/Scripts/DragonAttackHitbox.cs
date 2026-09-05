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

        if (hasHitPlayer)
            return;

        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();
        PlayerShield playerShield =
            other.GetComponentInParent<PlayerShield>();

        if (playerHealth == null)
            return;

        if (attackType == AttackType.Flame &&
            playerShield != null &&
            playerShield.IsGuarding)
        {
            hasHitPlayer = true;

            return;
        }

        hasHitPlayer = true;

        playerHealth.TakeDamage(damage);
    }
}