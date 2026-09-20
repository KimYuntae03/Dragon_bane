using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Transform dragon;
    [SerializeField] private GauntletManager gauntletManager;
    [SerializeField] private float lightningDamage = 10f; //번개 데미지

    public void FireRightProjectile()
    {
        PerformAttack(true);
    }

    public void FireLeftProjectile()
    {
        PerformAttack(false);
    }

    private void PerformAttack(bool curveRight)
    {
        if (gauntletManager == null || dragon == null)
            return;

        //번개 속성 공격(투사체 아니라 먼저 필터)
         if (gauntletManager.EquippedGauntlet ==
                ElementType.Lightning)
            {
                GameObject lightningVFX =
                    gauntletManager.GetProjectileVFX();

                if (lightningVFX == null)
                    return;

                Collider dragonCollider =
                    dragon.GetComponentInChildren<Collider>();

                Vector3 spawnPosition =
                    dragonCollider != null
                    ? dragonCollider.bounds.center
                    : dragon.position;

                GameObject lightningObject =
                    Instantiate(
                        lightningVFX,
                        spawnPosition,
                        Quaternion.identity
                    );

                // 이펙트 재생 후 자동 삭제
                Destroy(lightningObject, 2f);

                 DragonHealth dragonHealth =
                    dragon.GetComponentInParent<DragonHealth>();

                if (dragonHealth == null)
                    dragonHealth = dragon.GetComponent<DragonHealth>();

                if (dragonHealth != null)
                    dragonHealth.TakeDamage(lightningDamage,ElementType.Lightning);

                return;
            }
        
        //번개 공격이 아니면 원래대로 불 투사체 발사
        if (projectilePrefab == null ||
            projectileSpawnPoint == null)
            return;

        GameObject projectileObject =
            Instantiate(
                projectilePrefab,
                projectileSpawnPoint.position,
                projectileSpawnPoint.rotation
            );

        PlayerProjectile projectile =
            projectileObject.GetComponent<PlayerProjectile>();

        if (projectile != null)
        {
            projectile.Initialize(dragon, curveRight,gauntletManager.EquippedGauntlet);

            GameObject projectileVFX = gauntletManager.GetProjectileVFX();

            projectile.SetVFX(projectileVFX);
        
        }
    }
}