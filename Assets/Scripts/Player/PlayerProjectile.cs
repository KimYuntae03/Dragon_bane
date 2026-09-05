using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float curveStrength = 4f;
    [SerializeField] private float homingStrength = 8f;

    [Header("Damage")]
    [SerializeField] private float damage = 10f;

    [Header("Life Time")]
    [SerializeField] private float lifeTime = 5f;

    private Transform target;

    private Vector3 curveDirection;
    private float elapsedTime = 0f;

    public void Initialize(Transform targetTransform, bool curveRight)
    {
        target = targetTransform;

        // 좌/우 공격에 따라 바깥으로 휘는 방향 결정
        curveDirection =
            curveRight ? transform.right : -transform.right;

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (target == null)
            return;

        elapsedTime += Time.deltaTime;

        Vector3 targetDirection =
            (target.position - transform.position).normalized;

        // 시간이 지나면서 곡선 영향 감소
        float curveAmount =
            Mathf.Clamp01(1f - elapsedTime * homingStrength);

        Vector3 moveDirection =
            targetDirection +
            curveDirection * curveStrength * curveAmount;

        moveDirection.Normalize();

        transform.position +=
            moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        DragonHealth dragonHealth =
            other.GetComponentInParent<DragonHealth>();

        if (dragonHealth == null)
            return;

        dragonHealth.TakeDamage(damage);

        Destroy(gameObject);
    }
}