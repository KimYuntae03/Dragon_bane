using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Transform dragon;

    public void FireRightProjectile()
    {
        FireProjectile(true);
    }

    public void FireLeftProjectile()
    {
        FireProjectile(false);
    }

    private void FireProjectile(bool curveRight)
    {
        if (projectilePrefab == null ||
            projectileSpawnPoint == null ||
            dragon == null)
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
                curveRight
            );
        }
    }
}