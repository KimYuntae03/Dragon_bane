using UnityEngine;
using TMPro;

public class BattleItemUIManager : MonoBehaviour
{
    private const float SmallHealAmount = 20f;
    private const float LargeHealAmount = 40f;
    private const float DamagePotionBonusRate = 0.5f;
    private const float DamagePotionDuration = 10f;

    [Header("Potion IDs")]
    [SerializeField] private string damagePotionId;
    [SerializeField] private string healPotionId;
    [SerializeField] private string largeHealPotionId;

    [Header("Potion Count UI")]
    [SerializeField] private TMP_Text damagePotionCountText;
    [SerializeField] private TMP_Text healPotionCountText;
    [SerializeField] private TMP_Text largeHealPotionCountText;
    
    [Header("Player")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerAttack playerAttack;
    
    private void Start()
    {
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerAttack == null)
            playerAttack = FindFirstObjectByType<PlayerAttack>();

        UpdatePotionUI();
    }

    public void UpdatePotionUI()
    {
        if (PlayerInventoryManager.Instance == null)
            return;

        int damagePotionCount =
            PlayerInventoryManager.Instance
                .GetItemQuantity(damagePotionId);

        int healPotionCount =
            PlayerInventoryManager.Instance
                .GetItemQuantity(healPotionId);

        int largeHealPotionCount =
            PlayerInventoryManager.Instance
                .GetItemQuantity(largeHealPotionId);

        if (damagePotionCountText != null)
            damagePotionCountText.text =
                damagePotionCount.ToString();

        if (healPotionCountText != null)
            healPotionCountText.text =
                healPotionCount.ToString();

        if (largeHealPotionCountText != null)
            largeHealPotionCountText.text =
                largeHealPotionCount.ToString();
    }
    public void UseHealPotion()
    {
        if (PlayerInventoryManager.Instance == null ||
            playerHealth == null)
            return;

        if (!PlayerInventoryManager.Instance.UseItem(healPotionId))
            return;

        playerHealth.Heal(SmallHealAmount);

        UpdatePotionUI();
    }

    public void UseLargeHealPotion()
    {
        if (PlayerInventoryManager.Instance == null ||
            playerHealth == null)
            return;

        if (!PlayerInventoryManager.Instance.UseItem(largeHealPotionId))
            return;

        playerHealth.Heal(LargeHealAmount);

        UpdatePotionUI();
    }

    public void UseDamagePotion()
    {
        if (PlayerInventoryManager.Instance == null ||
            playerAttack == null)
            return;

        // 공격력 강화 포션이 없으면 사용 불가
        if (!PlayerInventoryManager.Instance.UseItem(damagePotionId))
            return;

        playerAttack.ApplyDamagePotion(
            DamagePotionBonusRate,
            DamagePotionDuration
        );

        UpdatePotionUI();
    }

    
}
