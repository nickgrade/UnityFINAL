using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [Tooltip("How much HP to remove per hit")]
    public int contactDamage = 1;

    [Tooltip("Seconds between each damage tick while colliding")]
    public float damageInterval = 0.5f;

    float lastDamageTime;

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player") &&
            Time.time >= lastDamageTime + damageInterval &&
            col.collider.TryGetComponent<Health>(out var hp))
        {
            hp.TakeDamage(contactDamage);
            lastDamageTime = Time.time;
        }
    }
}
