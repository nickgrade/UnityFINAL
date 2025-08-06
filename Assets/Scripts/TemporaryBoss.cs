using UnityEngine;

public class SimpleBossShooter : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag your Player here (or it will try to FindWithTag in Awake)")]
    public Transform player;
    [Tooltip("Empty child transform marking the muzzle")]
    public Transform firePoint;
    [Tooltip("Bullet prefab, must have Rigidbody2D")]
    public GameObject bulletPrefab;

    [Header("Firing")]
    [Tooltip("Shots per second")]
    public float fireRate = 1f;
    [Tooltip("Speed of the bullets")]
    public float bulletSpeed = 10f;

    [Header("Baseline Volley")]
    [Tooltip("Directions per volley")]
    public int directions = 8;
    [Tooltip("Seconds between baseline volleys")]
    public float baselineInterval = 2f;

    float nextFireTime;
    float nextVolleyTime;

    void Awake()
    {
        // If you forgot to assign in the Inspector, try to find it:
        if (player == null)
        {
            var pgo = GameObject.FindWithTag("Player");
            if (pgo != null)
                player = pgo.transform;
            else
                Debug.LogError("SimpleBossShooter: No GameObject with tag 'Player' found in scene!");
        }

        // Make sure you set a firePoint, too
        if (firePoint == null)
            Debug.LogError("SimpleBossShooter: firePoint is not assigned!");
    }

    void Update()
    {
        if (player == null || firePoint == null || bulletPrefab == null)
            return;  // bail if setup is incomplete

        // Fire at player
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            ShootAtPlayer();
        }

        // Baseline volley
        if (Time.time >= nextVolleyTime)
        {
            nextVolleyTime = Time.time + baselineInterval;
            FireRadial();
        }
    }

    void ShootAtPlayer()
    {
        // 1) Aim direction
        Vector2 dir = ((Vector2)player.position - (Vector2)firePoint.position).normalized;
        // 2) Compute angle so "up" points at the player
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        // 3) Spawn
        var b = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
        // 4) Give it velocity
        if (b.TryGetComponent<Rigidbody2D>(out var rb))
            rb.velocity = dir * bulletSpeed;
    }

    void FireRadial()
    {
        float step = 360f / directions;
        for (int i = 0; i < directions; i++)
        {
            float angle = i * step;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector3 position = firePoint.position + rotation * Vector3.up;

            var bullet = Instantiate(bulletPrefab, position, rotation);
            if (bullet.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.velocity = rotation * Vector3.up * bulletSpeed;
            }
        }
    }
}
