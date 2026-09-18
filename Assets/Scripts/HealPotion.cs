using UnityEngine;

public class HealPotion : MonoBehaviour
{
    [SerializeField] private float healAmount = 20f;

    public void UsePotion()
    {
        PlayerHealth playerHealth =
            FindFirstObjectByType<PlayerHealth>();

        if (playerHealth == null)
            return;

        playerHealth.Heal(healAmount);

        Destroy(gameObject);
    }

    
}