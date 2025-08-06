using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Health))]
public class EnemyAI : MonoBehaviour
{
    public enum State { Chase, Circle, Flee, Dash, Ranged }

    [Header("Movement")]
    public float moveSpeed   = 3f;
    public float circleSpeed = 2f;
    public float dashSpeed   = 12f;            // faster for committed dash

    [Header("Rotation")]
    public float rotationSpeed = 360f;

    [Header("Sight")]
    public LayerMask obstacleMask;
    public float    chaseRange   = 10f;
    public float    rangedRange  = 6f;
    public float    dashCooldown = 5f;

    [Header("Health Thresholds")]
    [Range(0f,1f)] public float fleeThreshold   = 0.3f;
    [Range(0f,1f)] public float circleThreshold = 0.6f;

    [Header("Circle Dive")]
    [Tooltip("How long to circle before diving in")]
    public float circleDuration = 2f;

    [Header("Ranged Attack")]
    public GameObject projectilePrefab;
    public float      fireInterval    = 2f;
    public float      projectileSpeed = 6f;

    Rigidbody2D rb;
    Health      hp;
    Transform   player;
    State       state, prevState;
    float       nextDashTime, nextFireTime, circleStartTime;
    bool        isDashing;

    void Awake()
    {
        rb     = GetComponent<Rigidbody2D>();
        hp     = GetComponent<Health>();
        player = GameObject.FindWithTag("Player")?.transform;
    }

    void FixedUpdate()
    {
        if (isDashing)
            return;             // full commit: don’t override dash

        if (player == null || hp.IsDead)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 toPlayer = (player.position - transform.position);
        float   dist     = toPlayer.magnitude;

        // Line-of-sight check
        if (Physics2D.Raycast(rb.position, toPlayer.normalized, dist, obstacleMask))
        {
            rb.velocity = Vector2.zero;
            return;
        }

        float healthPct = hp.Normalized;

        // Decide desired state
        State desired;
        if (dist < chaseRange && Time.time >= nextDashTime)
            desired = State.Dash;
        else if (healthPct < fleeThreshold)
            desired = State.Flee;
        else if (healthPct < circleThreshold && dist > 1f)
            desired = State.Circle;
        else if (dist < rangedRange && Time.time >= nextFireTime)
            desired = State.Ranged;
        else
            desired = State.Chase;

        // On entering Circle, record start
        if (desired == State.Circle && prevState != State.Circle)
            circleStartTime = Time.time;

        // After circling long enough, force dash
        if (desired == State.Circle && Time.time >= circleStartTime + circleDuration)
            desired = State.Dash;

        state     = desired;
        prevState = state;

        // Execute behavior
        switch (state)
        {
            case State.Chase:  DoChase(toPlayer); break;
            case State.Circle: DoCircle(toPlayer);break;
            case State.Flee:   DoFlee(toPlayer);  break;
            case State.Ranged: DoRanged(toPlayer.normalized); break;
            case State.Dash:   StartCoroutine(DoDash(toPlayer.normalized)); break;
        }
    }

    void DoChase(Vector2 toPlayer) =>
        MoveAndRotate(toPlayer.normalized, moveSpeed);

    void DoCircle(Vector2 toPlayer)
    {
        Vector2 away    = (rb.position - (Vector2)player.position).normalized;
        Vector2 tangent = new Vector2(-away.y, away.x);
        MoveAndRotate(tangent, circleSpeed);
    }

    void DoFlee(Vector2 toPlayer) =>
        MoveAndRotate(-toPlayer.normalized, moveSpeed);

    IEnumerator DoDash(Vector2 dir)
    {
        isDashing    = true;
        nextDashTime = Time.time + dashCooldown;

        // Set velocity once and keep it
        rb.velocity = dir * dashSpeed;
        rb.angularVelocity = 0f;

        // Wait until a collision stops us
        while (isDashing)
            yield return null;
    }

    void DoRanged(Vector2 dir)
    {
        nextFireTime = Time.time + fireInterval;
        rb.velocity  = Vector2.zero;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        var rot = Quaternion.Euler(0,0,angle);
        var proj = Instantiate(projectilePrefab, transform.position, rot);
        if (proj.TryGetComponent<Rigidbody2D>(out var prb))
            prb.velocity = dir * projectileSpeed;
    }

    void MoveAndRotate(Vector2 dir, float speed)
    {
        rb.velocity = dir * speed;
        float targetA = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        float a = Mathf.MoveTowardsAngle(rb.rotation, targetA, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(a);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!isDashing) return;

        // Stop dash on any collision (player or wall)
        isDashing = false;
        rb.velocity = Vector2.zero;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDashing && other.CompareTag("EnemyBullet") && other.attachedRigidbody)
        {
            Vector2 bv    = other.attachedRigidbody.velocity.normalized;
            Vector2 dodge = new Vector2(-bv.y, bv.x);
            rb.AddForce(dodge * moveSpeed * 2f, ForceMode2D.Impulse);
        }
    }
}
