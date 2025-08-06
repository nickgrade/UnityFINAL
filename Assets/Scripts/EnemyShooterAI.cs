// EnemyShooterAI.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyShooterAI : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform  firePoint;

    [Header("Orbit Settings")]
    [Tooltip("Degrees per second around the player")]
    public float orbitSpeed = 30f;
    [Tooltip("Desired radius from the player")]
    public float orbitRadius = 8f;
    [Range(0f, 0.5f)]
    public float orbitRadiusJitter = 0.2f;
    [Tooltip("How stiff the spring holding you at orbitRadius is")]
    public float radialSpringStrength = 5f;

    [Header("Separation")]
    public float    separationRadius   = 3f;
    public float    separationStrength = 1f;
    public LayerMask enemyLayer;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Firing")]
    public float fireRate = 1.5f;

    [Header("Sight")]
    [Tooltip("Layers that block sight (e.g. Walls)")]
    public LayerMask obstacleMask;

    Rigidbody2D rb;
    Transform    player;
    float        nextFireTime;

    void Awake()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        rb     = GetComponent<Rigidbody2D>();
        rb.interpolation          = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Randomize starting radius
        float rJ = Random.Range(1f - orbitRadiusJitter, 1f + orbitRadiusJitter);
        orbitRadius *= rJ;

        nextFireTime = Time.time + Random.Range(0f, fireRate);
    }

    void Update()
    {
        if (player == null) return;

        // Only fire if we can actually see the player
        if (Time.time >= nextFireTime && CanSeePlayer())
        {
            nextFireTime = Time.time + fireRate;
            ShootAtPlayer();
        }
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        // If sight is blocked, stop immediately
        if (!CanSeePlayer())
        {
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 myPos     = rb.position;
        Vector2 playerPos = player.position;

        // 1) Separation force
        Vector2 sep = Vector2.zero;
        foreach (var h in Physics2D.OverlapCircleAll(myPos, separationRadius, enemyLayer))
        {
            if (h.transform == transform) continue;
            Vector2 diff = myPos - (Vector2)h.transform.position;
            sep += diff.normalized / (diff.magnitude + 0.1f);
        }
        sep = sep.normalized * separationStrength;

        // 2) Tangential orbit velocity
        Vector2 offset = myPos - playerPos;
        float   currR  = offset.magnitude;
        if (currR < 0.01f) offset = Vector2.up * orbitRadius;
        float   ω       = orbitSpeed * Mathf.Deg2Rad;
        Vector2 tangent = new Vector2(-offset.y, offset.x).normalized;
        Vector2 orbitVel = tangent * (ω * orbitRadius);

        // 3) Radial spring to hold orbitRadius
        float   radialError = currR - orbitRadius;
        Vector2 radialVel   = -offset.normalized * (radialError * radialSpringStrength);

        // 4) Combine & clamp
        Vector2 finalVel = orbitVel + radialVel + sep;
        finalVel = Vector2.ClampMagnitude(finalVel, moveSpeed * 2f);

        // 5) Move
        rb.MovePosition(myPos + finalVel * Time.fixedDeltaTime);

        // 6) Face the player
        Vector2 toPlayer = (playerPos - myPos).normalized;
        float   faceAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg - 90f;
        rb.MoveRotation(faceAngle);
    }

    bool CanSeePlayer()
    {
        Vector2 origin = firePoint != null 
            ? (Vector2)firePoint.position 
            : rb.position;
        Vector2 toPlayer = (player.position - (Vector3)origin);
        float   dist     = toPlayer.magnitude;

        // Raycast only against obstacleMask—nothing else
        return Physics2D.Raycast(origin, toPlayer.normalized, dist, obstacleMask) == false;
    }

    void ShootAtPlayer()
    {
        Vector2 dir   = ((Vector2)player.position - (Vector2)firePoint.position).normalized;
        float   ang   = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        Vector3 spawn = firePoint.position + (Vector3)dir * 0.5f;
        Instantiate(bulletPrefab, spawn, Quaternion.Euler(0, 0, ang));
    }
}
