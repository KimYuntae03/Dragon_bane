using UnityEngine;

public class GauntletManager : MonoBehaviour
{
    public enum GauntletType
    {
        Fire,
        Lightning,
        Wind
    }

    [Header("Equipped Gauntlet")]
    [SerializeField] private GauntletType equippedGauntlet;

    [Header("Projectile VFX")]
    [SerializeField] private GameObject fireProjectileVFX;//불 투사체 필드
    [SerializeField] private GameObject windProjectileVFX;//바람 투사체 필드
    [SerializeField] private GameObject lightningProjectileVFX;//번개공격필드

    public GauntletType EquippedGauntlet => equippedGauntlet;

    public GameObject GetProjectileVFX()
    {
        switch (equippedGauntlet)
        {
            case GauntletType.Fire:
                return fireProjectileVFX;

            case GauntletType.Wind:
                return windProjectileVFX;

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