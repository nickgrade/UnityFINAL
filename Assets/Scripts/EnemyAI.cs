// EnemyAI.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Rotation")]
    public float rotationSpeed = 360f;

    [Header("Sight")]
    [Tooltip("Layers that block sight (e.g. Walls)")]
    public LayerMask obstacleMask;

    Transform    player;
    Rigidbody2D  rb;
    Health       hp;

    void Awake()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        rb     = GetComponent<Rigidbody2D>();
        hp     = GetComponent<Health>();
    }

    void FixedUpdate()
    {
        if (player == null || hp == null || hp.IsDead)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 myPos    = rb.position;
        Vector2 toPlayer = (player.position - transform.position);
        float   dist     = toPlayer.magnitude;

        // —— Line of Sight ——
        if (Physics2D.Raycast(myPos, toPlayer.normalized, dist, obstacleMask))
        {
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 dir = toPlayer.normalized;

        // Only move when roughly facing
        float angleDiff = Vector2.Angle(transform.up, dir);
        rb.velocity = (angleDiff < 90f)
            ? transform.up * moveSpeed
            : Vector2.zero;

        // Smooth rotation toward player
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        float newAngle = Mathf.MoveTowardsAngle(
            rb.rotation,
            targetAngle,
            rotationSpeed * Time.fixedDeltaTime
        );
        rb.MoveRotation(newAngle);
    }
}
