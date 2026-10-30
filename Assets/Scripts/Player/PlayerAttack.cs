using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Transform dragon;
    [SerializeField] private GauntletManager gauntletManager;

    [Header("Damage Variance")]
    [SerializeField, Range(0f, 1f)]
    private float damageReductionRate = 0.1f;

    [SerializeField, Range(0f, 1f)]
    private float damageIncreaseRate = 0.2f;

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

        int baseDamage =
            gauntletManager.GetEquippedGauntletDamage();

        float attackDamage =
            GetRandomizedDamage(baseDamage);

        bool isHighDamageRoll = attackDamage > baseDamage;

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
                    dragonHealth.TakeDamage(
                        attackDamage,
                        ElementType.Lightning,
                        isHighDamageRoll
                    );

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
            projectile.Initialize(
                dragon,
                curveRight,
                gauntletManager.EquippedGauntlet,
                attackDamage,
                isHighDamageRoll
            );

            GameObject projectileVFX = gauntletManager.GetProjectileVFX();

            projectile.SetVFX(projectileVFX);
        
        }
    }

    private float GetRandomizedDamage(float baseDamage)
    {
        float reductionRate = Mathf.Clamp01(damageReductionRate);
        float increaseRate = Mathf.Clamp01(damageIncreaseRate);

        float multiplier = Random.Range(
            1f - reductionRate,
            1f + increaseRate
        );

        return Mathf.RoundToInt(baseDamage * multiplier);
    }
}
