using UnityEngine;
using UnityEngine.UI;

public class DragonHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 300f;

    [Header("UI")]
    [SerializeField] private Image hpFill;

    [Header("Controller")]
    [SerializeField] private DragonController dragonController;

    private float currentHealth;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0f)
            Die();
    }

    private void UpdateHealthUI()
    {
        if (hpFill != null)
        {
            hpFill.fillAmount =
                currentHealth / maxHealth;
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (dragonController != null)
            dragonController.Die();
    }
}