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

    //사망 & 클리어 시 UI를 실행할 오브젝트 연결
    [SerializeField] private BattleResultDirector battleResultDirector;

    private float currentHealth;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(
        float damage,
        ElementType attackElement,
        bool isHighDamageRoll = false)
    {
        if (isDead)
            return;

        float multiplier =
            ElementManager.GetDamageMultiplier(
                dragonElement,
                attackElement
            );

        int finalDamage = Mathf.Max(
            0,
            Mathf.RoundToInt(damage * multiplier)
        );

        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        DamagePopupSpawner.ShowDamage(
            finalDamage,
            GetDamagePopupPosition(),
            isHighDamageRoll
        );

        UpdateHealthUI();

        if (currentHealth <= 0f)
            Die();
    }

    private Vector3 GetDamagePopupPosition()
    {
        Collider dragonCollider = GetComponentInChildren<Collider>();

        if (dragonCollider != null)
        {
            Bounds bounds = dragonCollider.bounds;
            return bounds.center + Vector3.up * bounds.extents.y * 0.35f;
        }

        return transform.position + Vector3.up * 2f;
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
        if (battleResultDirector != null)
            battleResultDirector.PlayDragonDeathCinematic();
    }
}
