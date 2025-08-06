// CircleTurretAI.cs
using UnityEngine;

public class CircleTurretAI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Bullet prefab (with its own movement script)")]
    public GameObject bulletPrefab;

    [Header("Movement")]
    public float moveSpeed = 1f;

    [Header("Spin")]
    public float spinSpeed = 30f;

    [Header("Firing")]
    public float fireRate     = 2f;
    public float bulletOffset = 1f;
    public float bulletSpeed  = 8f;

    [Header("Sight")]
    [Tooltip("Layers that block sight (e.g. Walls)")]
    public LayerMask obstacleMask;

    float    nextFireTime;
    Transform player;

    void Awake()
    {
        player = GameObject.FindWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null)
            return;

        // Always spin for visual feedback
        if (spinSpeed != 0f)
            transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);

        Vector2 myPos    = transform.position;
        Vector2 toPlayer = (player.position - transform.position);
        float   dist     = toPlayer.magnitude;

        // —— Line of Sight ——
        bool canSee = Physics2D.Raycast(myPos, toPlayer.normalized, dist, obstacleMask) == false;

        // 1) Move only if player is visible
        if (canSee && moveSpeed > 0f)
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime
            );

        // 2) Fire only if visible
        if (canSee && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            FireEightDirections();
        }
    }

    void FireEightDirections()
    {
        for (int i = 0; i < 8; i++)
        {
            Quaternion rot = transform.rotation * Quaternion.Euler(0f, 0f, i * 45f);
            Vector3 spawn  = transform.position + (rot * Vector3.up) * bulletOffset;
            GameObject b   = Instantiate(bulletPrefab, spawn, rot);
            var eb         = b.GetComponent<EnemyBullet>();
            if (eb != null) eb.speed = bulletSpeed;
        }
    }
}
