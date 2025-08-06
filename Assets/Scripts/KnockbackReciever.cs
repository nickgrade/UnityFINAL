using UnityEngine;

public class KnockbackReceiver : MonoBehaviour
{
    [Header("Hit Reaction")]
    [Tooltip("Total distance (in world units) to shove this object")]
    public float knockbackForce    = 2f;

    [Tooltip("How many seconds the shove lasts")]
    public float knockbackDuration = 0.2f;

    Vector2 velocity;
    float   timer;
    Transform player;

    void Awake()
    {
        // Cache the player's transform once
        var p = GameObject.FindWithTag("Player");
        player = p != null ? p.transform : null;
    }

    void Update()
    {
        if (timer > 0f)
        {
            // Move by our knockback velocity each frame
            float dt = Time.deltaTime;
            transform.position += (Vector3)(velocity * dt);
            timer -= dt;
        }
    }

    /// <summary>
    /// Call this when you want to shove the object away from the player.
    /// </summary>
    public void TriggerKnockback()
    {
        if (player == null) return;

        // 1) Compute direction from player → this object
        Vector2 dir = ((Vector2)transform.position - (Vector2)player.position).normalized;

        // 2) Spread the total force over the duration to get velocity
        velocity = dir * (knockbackForce / knockbackDuration);

        // 3) Start the timer
        timer = knockbackDuration;
    }
}
