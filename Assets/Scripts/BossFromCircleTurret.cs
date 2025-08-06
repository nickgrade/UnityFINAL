using System.Collections;
using UnityEngine;

public class BossFromCircleTurret : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Bullet prefab (with its own movement script)")]
    public GameObject bulletPrefab;

    [Header("Movement")]
    public float moveSpeed = 1f;

    [Header("Spin")]
    [Tooltip("Default spin speed when idle")]
    public float spinSpeed = 30f;

    [Header("Firing")]
    public float bulletOffset = 1f;
    public float bulletSpeed  = 8f;

    [Header("Sight")]
    [Tooltip("Layers that block sight (e.g. Walls)")]
    public LayerMask obstacleMask;

    [Header("Special Attack Cycle")]
    [Tooltip("Seconds between baseline volleys")]
    public float baselineInterval     = 2f;
    [Tooltip("How many baseline volleys before a special")]
    public int   volleysBeforeSpecial = 3;

    [Header("Special #1: Rapid Aimed Stream")]
    public int   rapidAimCount        = 16;
    public float rapidAimInterval     = 0.05f;
    [Tooltip("± degrees around player aim")]
    public float rapidAimSpread       = 15f;

    [Header("Special #2: Rapid Spin Volley")]
    [Tooltip("Spin speed during this special")]
    public float rapidSpinSpeed       = 360f;
    [Tooltip("Interval between each spin-volley shot")]
    public float rapidSpinInterval    = 0.1f;
    [Tooltip("Total duration of this special")]
    public float rapidSpinDuration    = 3f;

    [Header("Special #3: Charge Attack")]
    [Tooltip("Time in seconds to spin up")]
    public float spinUpDuration       = 2f;
    [Tooltip("Final spin speed at end of spin-up and during dash")]
    public float dashSpinSpeed        = 360f;
    [Tooltip("Dash movement speed")]
    public float chargeSpeed          = 10f;
    [Tooltip("Duration of each dash in seconds")]
    public float chargeDuration       = 1f;

    [Header("Special #4: Rapid Near-Miss Attack")]
    public int   nearMissCount        = 6;
    public float nearMissInterval     = 0.3f;
    public float nearMissOffset       = 2f;
    [Tooltip("Seconds between each spray volley during dash")]
    public float nearMissSprayInterval = 0.1f;

    Transform player;
    float     nextActionTime;
    int       volleyCounter;
    int       specialIndex;
    bool      isPerformingSpecial;
    float     originalSpinSpeed;

    void Awake()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        originalSpinSpeed = spinSpeed;
    }

    void Update()
    {
        if (player == null) return;

        // Always rotate for feedback
        if (spinSpeed != 0f)
            transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);

        // Move toward player if visible
        Vector2 toPlayer = (Vector2)(player.position - transform.position);
        if (!Physics2D.Raycast(transform.position, toPlayer.normalized, toPlayer.magnitude, obstacleMask))
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }

        // Block until current special and cooldown finish
        if (isPerformingSpecial || Time.time < nextActionTime)
            return;

        if (volleyCounter < volleysBeforeSpecial)
        {
            FireEightDirections();
            volleyCounter++;
            nextActionTime = Time.time + baselineInterval;
        }
        else
        {
            isPerformingSpecial = true;
            StartCoroutine(RunSpecial(specialIndex));
            specialIndex = (specialIndex + 1) % 4;
            volleyCounter = 0;
            nextActionTime = Time.time + baselineInterval;
        }
    }

    void FireEightDirections()
    {
        for (int i = 0; i < 8; i++)
        {
            Quaternion rot = transform.rotation * Quaternion.Euler(0f, 0f, i * 45f);
            Vector3 spawn  = transform.position + rot * Vector3.up * bulletOffset;
            var b = Instantiate(bulletPrefab, spawn, rot);
            if (b.TryGetComponent<Rigidbody2D>(out var rb))
                rb.velocity = rot * Vector3.up * bulletSpeed;
        }
    }

    IEnumerator RunSpecial(int idx)
    {
        switch (idx)
        {
            case 0: yield return RapidAimed();          break;
            case 1: yield return RapidSpinVolley();     break;
            case 2: yield return ChargeAttack();        break;
            case 3: yield return RapidNearMissAttack(); break;
        }
        // Reset spin speed & state
        spinSpeed = originalSpinSpeed;
        isPerformingSpecial = false;
    }

    IEnumerator RapidAimed()
    {
        for (int i = 0; i < rapidAimCount; i++)
        {
            Vector2 dir   = (player.position - transform.position).normalized;
            float   baseA = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            float   spread = Random.Range(-rapidAimSpread, rapidAimSpread);
            Quaternion rot = Quaternion.Euler(0f, 0f, baseA + spread);
            Vector3  pos  = transform.position + rot * Vector3.up * bulletOffset;

            var b = Instantiate(bulletPrefab, pos, rot);
            if (b.TryGetComponent<Rigidbody2D>(out var rb))
                rb.velocity = rot * Vector3.up * bulletSpeed;

            yield return new WaitForSeconds(rapidAimInterval);
        }
    }

    IEnumerator RapidSpinVolley()
    {
        float oldSpin = spinSpeed;
        spinSpeed = rapidSpinSpeed;
        float endTime = Time.time + rapidSpinDuration;

        while (Time.time < endTime)
        {
            FireEightDirections();
            yield return new WaitForSeconds(rapidSpinInterval);
        }

        spinSpeed = oldSpin;
    }

    IEnumerator ChargeAttack()
    {
        // Spin-up: ramp from originalSpinSpeed to dashSpinSpeed
        float elapsed = 0f;
        while (elapsed < spinUpDuration)
        {
            float t = elapsed / spinUpDuration;
            spinSpeed = Mathf.Lerp(originalSpinSpeed, dashSpinSpeed, t);
            transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        spinSpeed = dashSpinSpeed;

        // First dash toward current player position
        yield return PerformDash(player.position);

        // Short pause
        yield return new WaitForSeconds(0.5f);

        // Second dash toward updated player position
        yield return PerformDash(player.position);
    }

    IEnumerator PerformDash(Vector3 target)
    {
        float endDash = Time.time + chargeDuration;
        Vector3 dir   = (target - transform.position).normalized;
        while (Time.time < endDash)
        {
            transform.position += dir * chargeSpeed * Time.deltaTime;
            transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator RapidNearMissAttack()
    {
        for (int i = 0; i < nearMissCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle.normalized * nearMissOffset;
            Vector3 pos    = player.position + (Vector3)offset;
            yield return PerformDashAndSpray(pos);
            yield return new WaitForSeconds(nearMissInterval);
        }
    }

    IEnumerator PerformDashAndSpray(Vector3 targetPosition)
    {
        float endDash   = Time.time + chargeDuration;
        Vector3 dir     = (targetPosition - transform.position).normalized;
        float  nextSpray = Time.time;

        while (Time.time < endDash)
        {
            transform.position += dir * chargeSpeed * Time.deltaTime;
            transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);

            if (Time.time >= nextSpray)
            {
                FireEightDirections();
                nextSpray = Time.time + nearMissSprayInterval;
            }

            yield return null;
        }
    }
}
