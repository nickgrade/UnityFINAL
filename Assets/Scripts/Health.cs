using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("HP Settings")]
    [Tooltip("Maximum hit points")]
    public int maxHP = 5;

    [Header("Events")]
    [Tooltip("Invoked whenever damage is taken (but before death)")]
    public UnityEvent onDamage;
    [Tooltip("Invoked once when HP reaches zero")]
    public UnityEvent onDeath;

    int current;

    /// <summary>
    /// True if current HP is 0 or less.
    /// </summary>
    public bool IsDead => current <= 0;

    /// <summary>
    /// Returns a 0→1 value for UI bars.
    /// </summary>
    public float Normalized => (float)current / maxHP;

    void Awake()
    {
        current = maxHP;
    }

    /// <summary>
    /// Subtracts HP, flashes if you have a FlashOnHit component,
    /// invokes onDamage, then onDeath if HP drops to zero.
    /// </summary>
    public void TakeDamage(int dmg)
    {
        if (IsDead) 
            return;

        // 1) Reduce HP
        current -= dmg;

        // 2) Trigger a flash if present
        if (TryGetComponent<FlashOnHit>(out var flasher))
            flasher.Flash();

        // 3) Fire the onDamage event
        onDamage?.Invoke();

        // 4) Handle death
        if (current <= 0)
        {
    GetComponent<EnemyDropper>()?.TryDrop();
    onDeath?.Invoke();
    Destroy(gameObject);
        }
    }
}
