using UnityEngine;

public class NightmareDragonCombatSetup : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject hornHitbox;
    [SerializeField] private GameObject clawHitbox;

    public Animator Animator => animator;
    public GameObject HornHitbox => hornHitbox;
    public GameObject ClawHitbox => clawHitbox;

    public void Configure(
        Animator targetAnimator,
        GameObject targetHornHitbox,
        GameObject targetClawHitbox)
    {
        animator = targetAnimator;
        hornHitbox = targetHornHitbox;
        clawHitbox = targetClawHitbox;
    }
}
