using UnityEngine;

public class PlayerHitReaction : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private PlayerShield playerShield;

    private bool isHitReacting = false;

    private static readonly int HitClawHash =
        Animator.StringToHash("HitClaw");

    private static readonly int HitFlameHash =
        Animator.StringToHash("HitFlame");

    public bool IsHitReacting => isHitReacting;

    public void PlayClawHitReaction()
    {
        if (isHitReacting)
            return;

        isHitReacting = true;

        if (playerShield != null)
            playerShield.StopGuard();

        animator.SetTrigger(HitClawHash);
    }

    public void PlayFlameHitReaction()
    {
        if (isHitReacting)
            return;

        isHitReacting = true;

        if (playerShield != null)
            playerShield.StopGuard();
            
        animator.SetTrigger(HitFlameHash);
    }

    public void EndHitReaction()
    {
        isHitReacting = false;
    }
}