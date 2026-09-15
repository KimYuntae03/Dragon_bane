using UnityEngine;

public class GauntletManager : MonoBehaviour
{
    public enum GauntletType
    {
        Fire,
        Lightning
    }

    [Header("Equipped Gauntlet")]
    [SerializeField] private GauntletType equippedGauntlet;

    [Header("Projectile VFX")]
    [SerializeField] private GameObject fireProjectileVFX;
    [SerializeField] private GameObject lightningProjectileVFX;

    public GauntletType EquippedGauntlet => equippedGauntlet;

    public GameObject GetProjectileVFX()
    {
        switch (equippedGauntlet)
        {
            case GauntletType.Fire:
                return fireProjectileVFX;

            case GauntletType.Lightning:
                return lightningProjectileVFX;

            default:
                return fireProjectileVFX;
        }
    }

    public void EquipGauntlet(GauntletType gauntletType)
    {
        equippedGauntlet = gauntletType;
    }
}