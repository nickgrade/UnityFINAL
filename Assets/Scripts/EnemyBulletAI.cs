using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 8f;
    public int   damage = 1;

    [Header("Despawning")]
    [Tooltip("Select the layer(s) that represent walls or obstacles.")]
    public LayerMask wallMask;

    void OnEnable()
    {
        // Failsafe: despawn after 3s
        Invoke(nameof(Deactivate), 3f);
    }

    void Update()
    {
        // Move up in local space
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 1) If we hit a wall layer, just deactivate
        if (((1 << other.gameObject.layer) & wallMask) != 0)
        {
            Deactivate();
            return;
        }

        // 2) Otherwise, if we hit the player, damage and deactivate
        if (other.CompareTag("Player") &&
            other.TryGetComponent<Health>(out var hp) &&
            !hp.IsDead)
        {
            hp.TakeDamage(damage);
            Deactivate();
        }
    }

    void Deactivate()
    {
        CancelInvoke();
        gameObject.SetActive(false);
    }
}
