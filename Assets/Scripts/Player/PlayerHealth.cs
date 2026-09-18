using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("UI")]
    [SerializeField] private Image hpFill;
    [SerializeField] private TMP_Text hpText;

    [Header("Controller")]
    [SerializeField] private PlayerController playerController;

    //플레이어 피격 시 애니메이션 호출 변수
    [SerializeField] private PlayerHitReaction hitReaction;

    //회복포션 사용시 터트릴 이펙트 
    [SerializeField] private GameObject healVFX;
    //힐 이펙트 적용 위치
    [SerializeField] private Transform healEffectPoint;

    private float currentHealth;
    private bool isDead = false;
    public enum HitType
    {
        Claw,
        Flame
    }

    private void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float damage, HitType hitType)
    {
        if (isDead)
            return;

        currentHealth -= damage;
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

    private void UpdateHealthUI()
    {
        if (hpFill == null)
            return;

        hpFill.fillAmount = currentHealth / maxHealth;
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        if (playerController != null)
        {
            playerController.Die();
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