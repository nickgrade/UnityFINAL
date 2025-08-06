using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Speed in world units/sec")]
    public float speed = 12f;

    [Header("Damage")]
    [Tooltip("HP taken on hit")]
    public int damage = 1;

    [Header("Lifetime")]
    [Tooltip("Seconds before auto‐despawn")]
    public float lifetime = 2f;

    void OnEnable()
    {
        // Schedule self‐deactivation in exactly 'lifetime' seconds
        Invoke(nameof(Deactivate), lifetime);
    }

    void OnDisable()
    {
        // Cancel the pending invocation if this bullet is turned off early
        CancelInvoke();
    }

    void Update()
    {
        // Always move forward in local "up" direction
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 1) Damage any Health
        if (other.TryGetComponent<Health>(out var hp) && !hp.IsDead)
            hp.TakeDamage(damage);

        // 2) Knockback via your shared receiver
        if (other.TryGetComponent<KnockbackReceiver>(out var kb))
            kb.TriggerKnockback();

        // 3) Deactivate immediately on any hit
        Deactivate();
    }

    void Deactivate()
    {
        CancelInvoke();          // clear the Invoke
        gameObject.SetActive(false);
    }
}
