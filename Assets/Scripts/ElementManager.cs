using UnityEngine;

public class ElementManager : MonoBehaviour
{
    public static float GetDamageMultiplier(
        ElementType dragonElement,
        ElementType attackElement)
    {
        if (dragonElement == ElementType.Wind &&
            attackElement == ElementType.Fire)
        {
            return 1.5f;
        }

        if (dragonElement == ElementType.Fire &&
            attackElement == ElementType.Lightning)
        {
            return 1.5f;
        }

        if (dragonElement == ElementType.Lightning &&
            attackElement == ElementType.Wind)
        {
            return 1.5f;
        }

        return 1f;
    }
}