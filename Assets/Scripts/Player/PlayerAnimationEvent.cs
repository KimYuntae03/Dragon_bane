using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private PlayerShield playerShield;
    [SerializeField] private PlayerAttack playerAttack;

    [SerializeField] private PlayerHitReaction playerHitReaction;
    [SerializeField] private PlayerController playerController;

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

    public void EndHitReaction()
    {
        if (playerHitReaction != null)
            playerHitReaction.EndHitReaction();
    }

    public void EndAction()
    {
        if (playerController != null)
            playerController.EndAction();
    }

    public void EndGuard()
    {
        if (playerShield != null)
            playerShield.StopGuard();

        if (playerController != null)
            playerController.EndAction();
    }
}