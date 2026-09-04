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
        if (hasHitPlayer)
            return;

        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();
        PlayerController playerController =
            other.GetComponentInParent<PlayerController>();

        if (playerHealth == null)
            return;

        if (attackType == AttackType.Flame &&
            playerController != null &&
            playerController.IsGuarding)
        {
            hasHitPlayer = true;

            Debug.Log("Flame Attack 방어 성공");
            return;
        }
        

        hasHitPlayer = true;

        playerHealth.TakeDamage(damage);
    }
}