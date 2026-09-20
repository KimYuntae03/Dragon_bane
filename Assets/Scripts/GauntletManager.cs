using UnityEngine;

public class GauntletManager : MonoBehaviour
{
    [Header("Equipped Gauntlet")]
    [SerializeField] private ElementType equippedGauntlet;

    [Header("Projectile VFX")]
    [SerializeField] private GameObject fireProjectileVFX;
    [SerializeField] private GameObject windProjectileVFX;
    [SerializeField] private GameObject lightningProjectileVFX;

    public ElementType EquippedGauntlet => equippedGauntlet;

    public GameObject GetProjectileVFX()
    {
        switch (equippedGauntlet)
        {
            case ElementType.Fire:
                return fireProjectileVFX;

            case ElementType.Wind:
                return windProjectileVFX;

            case ElementType.Lightning:
                return lightningProjectileVFX;

            default:
                return fireProjectileVFX;
        }
    }

    public void EquipGauntlet(ElementType elementType)
    {
        equippedGauntlet = elementType;
    }
}