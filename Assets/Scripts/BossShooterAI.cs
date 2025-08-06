// BossTurretAI.cs
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class BossTurretAI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Your player Transform")]
    public Transform player;

    [Header("Prefabs")]
    [Tooltip("Straight‐moving bullet")]
    public GameObject bulletPrefab;

    [Header("Movement")]
    [Tooltip("How fast the turret shuffles toward player")]
    public float moveSpeed = 1f;

    [Header("Spin")]
    [Tooltip("Base spin speed (deg/sec)")]
    public float baseSpinSpeed = 30f;

    [Header("Sight")]
    [Tooltip("Layers that block sight")]
    public LayerMask obstacleMask;

    [Header("Bullet Settings")]
    public float bulletOffset    = 1f;
    public float bulletSpeed     = 8f;

    [Header("Baseline Volley")]
    [Tooltip("Directions per volley")]
    public int   directions           = 16;
    [Tooltip("Seconds between baseline volleys")]
    public float baselineInterval     = 1f;
    [Tooltip("How many baseline volleys before a special")]
    public int   volleysBeforeSpecial = 3;

    [Header("Special #1: Rapid Aimed Stream")]
    public int   rapidAimCount    = 16;
    public float rapidAimInterval = 0.05f;
    [Tooltip("± degrees around player aim")]
    public float rapidAimSpread   = 15f;

    [Header("Special #3: Rapid Spin Volley")]
    [Tooltip("Spin speed during this special")]
    public float rapidSpinSpeed    = 360f;
    [Tooltip("Interval between each spin‐volley shot")]
    public float rapidSpinInterval = 0.1f;
    [Tooltip("Total duration of this special")]
    public float rapidSpinDuration = 3f;

    [Header("Special #4: Charge Attack")]
    [Tooltip("Spin‐up before dash (sec)")]
    public float spinUpDuration = 4f;
    [Tooltip("Dash speed")]
    public float chargeSpeed    = 10f;
    [Tooltip("Dash duration (sec)")]
    public float chargeDuration = 1f;

    int   volleyCounter;
    int   specialIndex;
    float nextActionTime;

    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

        nextActionTime = Time.time + baselineInterval;
    }

    void Update()
    {
        if (player == null) return;

        // 1) Always spin
        transform.Rotate(0, 0, baseSpinSpeed * Time.deltaTime);

        // 2) Line‐of‐sight
        Vector2 myPos = transform.position;
        Vector2 toP  = (Vector2)player.position - myPos;
        float   dist = toP.magnitude;
        bool    canSee = Physics2D.Raycast(myPos, toP.normalized, dist, obstacleMask) == false;
        if (!canSee) return;

        // 3) Slow chase
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        // 4) Timing
        if (Time.time < nextActionTime) return;

        if (volleyCounter < volleysBeforeSpecial)
        {
            FireRadial(directions, bulletPrefab, bulletOffset, bulletSpeed);
            volleyCounter++;
            nextActionTime += baselineInterval;
        }
        else
        {
            StartCoroutine(RunSpecial(specialIndex));
            specialIndex = (specialIndex + 1) % 3;
            volleyCounter = 0;
            nextActionTime = Time.time + baselineInterval;
        }
    }

    IEnumerator RunSpecial(int idx)
    {
        switch (idx)
        {
            case 0: yield return RapidAimed(); break;
            case 1: yield return RapidSpinVolley(); break;
            case 2: yield return ChargeAttack(); break;
        }
    }

    // ===== Helpers & Specials =====

    void FireRadial(int count, GameObject prefab, float offset, float speed)
    {
        float step = 360f / count;
        for (int i = 0; i < count; i++)
        {
            float ang = i * step;
            Quaternion rot = Quaternion.Euler(0,0,ang);
            Vector3    pos = transform.position + rot * Vector3.up * offset;
            var b = Instantiate(prefab, pos, rot);
            if (b.TryGetComponent<Rigidbody2D>(out var rb))
                rb.velocity = rot * Vector3.up * speed;
        }
    }

    IEnumerator RapidAimed()
    {
        // Fire a stream of bullets aimed at player, with spread
        for (int i = 0; i < rapidAimCount; i++)
        {
            Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
            float   baseA = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float   spread = Random.Range(-rapidAimSpread, rapidAimSpread);
            Quaternion rot = Quaternion.Euler(0, 0, baseA + spread);
            Vector3    pos = transform.position + rot * Vector3.up * bulletOffset;
            var b = Instantiate(bulletPrefab, pos, rot);
            if (b.TryGetComponent<Rigidbody2D>(out var rb2))
                rb2.velocity = rot * Vector3.up * bulletSpeed;
            yield return new WaitForSeconds(rapidAimInterval);
        }
    }

    IEnumerator RapidSpinVolley()
    {
        float oldSpin = baseSpinSpeed;
        baseSpinSpeed = rapidSpinSpeed;
        float endTime = Time.time + rapidSpinDuration;

        while (Time.time < endTime)
        {
            FireRadial(directions, bulletPrefab, bulletOffset, bulletSpeed);
            yield return new WaitForSeconds(rapidSpinInterval);
        }

        baseSpinSpeed = oldSpin;
    }

    IEnumerator ChargeAttack()
    {
        // spin-up
        float spunTime = Time.time + spinUpDuration;
        while (Time.time < spunTime)
        {
            transform.Rotate(0,0,baseSpinSpeed * Time.deltaTime);
            yield return null;
        }

        // dash
        float endDash = Time.time + chargeDuration;
        Vector2 dashDir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        while (Time.time < endDash)
        {
            transform.position += (Vector3)dashDir * chargeSpeed * Time.deltaTime;
            yield return null;
        }
    }
}
