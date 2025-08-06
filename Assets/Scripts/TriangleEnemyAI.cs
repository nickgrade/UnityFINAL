// TriangleEnemyAI.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TriangleEnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Rotation")]
    public float rotationSpeed = 360f;

    [Header("Spin Attack Timing")]
    public float spinAttackInterval = 5f;
    public float spinUpDuration      = 1f;
    public float spinRotationSpeed   = 720f;
    public float chargeSpeed         = 8f;
    public float chargeDuration      = 1f;

    [Header("Sight")]
    [Tooltip("Which layers block sight (e.g. Walls)")]
    public LayerMask obstacleMask;

    Transform   player;
    Rigidbody2D rb;
    Health      hp;

    float  nextSpinTime;
    bool   isSpinningUp;
    bool   isCharging;
    float  spinStartTime;
    float  chargeStartTime;
    Vector2 chargeDirection;

    void Awake()
    {
        player       = GameObject.FindWithTag("Player")?.transform;
        rb           = GetComponent<Rigidbody2D>();
        hp           = GetComponent<Health>();
        nextSpinTime = Time.time + spinAttackInterval;
    }

    void FixedUpdate()
    {
        if (player == null || hp == null)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (hp.IsDead)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // compute vector toward player
        Vector2 myPos     = rb.position;
        Vector2 toPlayerV = (player.position - transform.position);
        float   distToP   = toPlayerV.magnitude;

        // —— Line-of-Sight Check ——
        if (Physics2D.Raycast(myPos, toPlayerV.normalized, distToP, obstacleMask))
        {
            // player is behind a wall → idle spin only
            rb.velocity = Vector2.zero;
            float idleSpin = rb.rotation + rotationSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(idleSpin);
            return;
        }

        float now = Time.time;
        Vector2 toPlayerDir = toPlayerV.normalized;

        // ---- State transitions ----
        if (!isSpinningUp && !isCharging && now >= nextSpinTime)
        {
            isSpinningUp  = true;
            spinStartTime = now;
        }
        if (isSpinningUp && now >= spinStartTime + spinUpDuration)
        {
            isSpinningUp    = false;
            isCharging      = true;
            chargeStartTime = now;
            chargeDirection = toPlayerDir;
            float faceA = Mathf.Atan2(chargeDirection.y, chargeDirection.x) * Mathf.Rad2Deg - 90f;
            rb.MoveRotation(faceA);
        }
        if (isCharging && now >= chargeStartTime + chargeDuration)
        {
            isCharging   = false;
            nextSpinTime = now + spinAttackInterval;
        }

        // ---- Behaviors ----
        if (isSpinningUp)
        {
            // prep spin in place
            rb.velocity = Vector2.zero;
            float spinAngle = rb.rotation + spinRotationSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(spinAngle);
        }
        else if (isCharging)
        {
            // dash toward locked direction
            rb.velocity = chargeDirection * chargeSpeed;
        }
        else
        {
            // normal idle spin + chase
            float spinAngle = rb.rotation + rotationSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(spinAngle);
            rb.velocity = toPlayerDir * moveSpeed;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
        {
            // interrupt any attack and reset
            isSpinningUp = false;
            isCharging   = false;
            rb.velocity  = Vector2.zero;
            nextSpinTime = Time.time + spinAttackInterval;
        }
    }
}
