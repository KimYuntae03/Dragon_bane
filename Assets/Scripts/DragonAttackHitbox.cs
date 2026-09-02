using UnityEngine;

public class DragonAttackHitbox : MonoBehaviour
{
    [SerializeField] private float damage = 20f;
    

    private bool hasHitPlayer = false;

    private void OnEnable()
    {
        hasHitPlayer = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHitPlayer)
            return;

        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
        {
            return;
        }

        hasHitPlayer = true;

        playerHealth.TakeDamage(damage);
    }
}