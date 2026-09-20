using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DragonHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 300f;

    [Header("UI")]
    [SerializeField] private Image hpFill;

    [SerializeField] private TMP_Text dragonHPText;

    [Header("Controller")]
    [SerializeField] private DragonController dragonController;

    [SerializeField] private ElementType dragonElement;//드래곤 속성
    public ElementType DragonElement => dragonElement;

    private float currentHealth;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float damage, ElementType attackElement)
    {
        if (isDead)
            return;

        float multiplier =
            ElementManager.GetDamageMultiplier(
                dragonElement,
                attackElement
            );

        float finalDamage = damage * multiplier;

        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0f)
            Die();
    }

    private void UpdateHealthUI()
    {
        if (hpFill != null) //HPFill업데이트
        {
            hpFill.fillAmount =
                currentHealth / maxHealth;
        }

        if (dragonHPText != null) //HPText업데이트
        {
            dragonHPText.text =
                Mathf.CeilToInt(currentHealth).ToString();
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