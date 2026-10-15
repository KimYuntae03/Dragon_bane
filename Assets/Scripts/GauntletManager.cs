using UnityEngine;

public class GauntletManager : MonoBehaviour
{
    [Header("Equipped Gauntlet")]
    [SerializeField] private ElementType equippedGauntlet;

    [Header("Projectile VFX")]
    [SerializeField] private GameObject fireProjectileVFX;
    [SerializeField] private GameObject windProjectileVFX;
    [SerializeField] private GameObject lightningProjectileVFX;

    [Header("Gauntlet Data")]
    [SerializeField] private StoreItemData fireGauntletData;
    [SerializeField] private StoreItemData windGauntletData;
    [SerializeField] private StoreItemData lightningGauntletData;

    public ElementType EquippedGauntlet => equippedGauntlet;

    private void Start()
    {
        if (PlayerInventoryManager.Instance != null)
        {
            EquipGauntlet(
                PlayerInventoryManager.Instance.EquippedGauntlet
            );
        }
    }
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

    public StoreItemData GetEquippedGauntletData()
    {
        switch (equippedGauntlet)
        {
            case ElementType.Fire:
                return fireGauntletData;

            case ElementType.Wind:
                return windGauntletData;

            case ElementType.Lightning:
                return lightningGauntletData;

            default:
                return null;
        }
    }

    public int GetEquippedGauntletDamage()
    {
        StoreItemData data =
            GetEquippedGauntletData();

        if (data == null ||
            PlayerInventoryManager.Instance == null)
            return 0;

        return PlayerInventoryManager.Instance
            .GetGauntletDamage(data);
    }
}