using UnityEngine;
using UnityEngine.UI;

public class ElementIconUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GauntletManager gauntletManager;
    [SerializeField] private DragonHealth dragonHealth;

    [Header("UI")]
    [SerializeField] private Image playerElementIcon;
    [SerializeField] private Image dragonElementIcon;

    [Header("Element Icons")]
    [SerializeField] private Sprite fireIcon;
    [SerializeField] private Sprite lightningIcon;
    [SerializeField] private Sprite windIcon;

    private void Start()
    {
        UpdateElementIcons();
    }

    public void UpdateElementIcons()
    {
        if (gauntletManager != null && playerElementIcon != null)
        {
            playerElementIcon.sprite =
                GetElementIcon(gauntletManager.EquippedGauntlet);
        }

        if (dragonHealth != null && dragonElementIcon != null)
        {
            dragonElementIcon.sprite =
                GetElementIcon(dragonHealth.DragonElement);
        }
    }

    private Sprite GetElementIcon(ElementType element)
    {
        switch (element)
        {
            case ElementType.Fire:
                return fireIcon;

            case ElementType.Lightning:
                return lightningIcon;

            case ElementType.Wind:
                return windIcon;

            default:
                return null;
        }
    }
}