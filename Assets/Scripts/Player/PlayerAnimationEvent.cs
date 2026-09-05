using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private PlayerShield playerShield;
    [SerializeField] private PlayerAttack playerAttack;

    public void StopGuard()
    {
        if (playerShield != null)
            playerShield.StopGuard();
    }

    public void FireRightProjectile()
    {
        if (playerAttack != null)
            playerAttack.FireRightProjectile();
    }

    public void FireLeftProjectile()
    {
        if (playerAttack != null)
            playerAttack.FireLeftProjectile();
    }
}