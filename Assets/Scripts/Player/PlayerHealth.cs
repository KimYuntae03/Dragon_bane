using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("UI")]
    [SerializeField] private Image hpFill;
    [SerializeField] private TMP_Text playerHPText;

    [Header("Controller")]
    [SerializeField] private PlayerController playerController;

    [Header("Element Resistance")]
    [SerializeField] private DragonHealth dragonHealth;
    [SerializeField, Range(0f, 1f)]
    private float sameElementDamageReduction = 0.2f;

    //플레이어 피격 시 애니메이션 호출 변수
    [SerializeField] private PlayerHitReaction hitReaction;

    //회복포션 사용시 터트릴 이펙트 
    [SerializeField] private GameObject healVFX;
    //힐 이펙트 적용 위치
    [SerializeField] private Transform healEffectPoint;

    //플레이어 사망시 패배효과 참조할 변수
    [SerializeField] private DefeatResultDirector defeatResultDirector;

    private float currentHealth;
    private float baseMaxHealth;
    private bool isDead = false;
    public enum HitType
    {
        Claw,
        Flame
    }

    private void Awake()
    {
        baseMaxHealth = maxHealth;

        if (dragonHealth == null)
            dragonHealth = FindFirstObjectByType<DragonHealth>();

        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void Start()
    {
        ApplyEquippedRobeHealthBonus();
    }

    private void ApplyEquippedRobeHealthBonus()
    {
        int robeHealthBonus = 0;

        if (PlayerInventoryManager.Instance != null)
        {
            robeHealthBonus =
                PlayerInventoryManager.Instance
                    .GetEquippedRobeHealthBonus();
        }

        maxHealth = baseMaxHealth + robeHealthBonus;
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float damage, HitType hitType)
    {
        if (isDead)
            return;

        float finalDamage = CalculateIncomingDamage(damage);

        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }
         if (hitReaction != null)
        {
            if (hitType == HitType.Claw)
                hitReaction.PlayClawHitReaction();
            else if (hitType == HitType.Flame)
                hitReaction.PlayFlameHitReaction();
        }
    }

    private float CalculateIncomingDamage(float damage)
    {
        PlayerInventoryManager inventory =
            PlayerInventoryManager.Instance;

        if (inventory == null || dragonHealth == null)
            return damage;

        if (!inventory.TryGetEquippedRobeElement(
                out ElementType robeElement))
            return damage;

        if (robeElement != dragonHealth.DragonElement)
            return damage;

        float reduction =
            Mathf.Clamp01(sameElementDamageReduction);

        return damage * (1f - reduction);
    }

    private void UpdateHealthUI()
    {
        if (hpFill != null)
        {
            hpFill.fillAmount = currentHealth / maxHealth;
        }

        if (playerHPText != null)
        {
            playerHPText.text =
                Mathf.CeilToInt(currentHealth).ToString();
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        // 플레이어 사망 애니메이션
        if (playerController != null)
        {
            playerController.Die();
        }
        
        if (defeatResultDirector != null)//패배 연출 시작
        {
            defeatResultDirector.PlayDefeatCinematic();
        }
    }

    public void Heal(float amount)
    {
        if (isDead)
            return;
        
        if (currentHealth >= maxHealth)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        UpdateHealthUI();

        if (healVFX != null && healEffectPoint != null) //힐 이펙트 발동
        {
            GameObject effect =
                Instantiate(
                    healVFX,
                    healEffectPoint.position,
                    Quaternion.identity
                );

            Destroy(effect, 2f);
        }
    }
}
