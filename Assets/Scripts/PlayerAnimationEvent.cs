using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private PlayerShield playerShield;

    public void StopGuard()
    {
        if (playerShield != null)
            playerShield.StopGuard();
    }
}