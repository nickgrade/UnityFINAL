using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FloatEvent : UnityEvent<float> { }

public class Health : MonoBehaviour
{
    [Header("HP Settings")]
    [Tooltip("Maximum hit points")]
    public float maxHP = 5f;

    [Header("Events")]
    [Tooltip("Invoked whenever HP changes (damage or heal)")]
    public UnityEvent onHealthChanged;
    [Tooltip("Invoked once when HP reaches zero")]
    public UnityEvent onDeath;

    float current;

    /// <summary>
    /// True if current HP is 0 or less.
    /// </summary>
    public bool IsDead => current <= 0f;

    /// <summary>
    /// Returns a 0→1 value for UI bars.
    /// </summary>
    public float Normalized => Mathf.Clamp01(current / maxHP);

    void Awake()
    {
        current = maxHP;
        onHealthChanged?.Invoke();
    }

    /// <summary>
    /// Change HP by amt (positive = damage, negative = heal).
    /// </summary>
    public void TakeDamage(float amt)
    {
        bool easy = PlayerPrefs.GetInt("EasyMode", 0) == 1;
        bool isPlayer = TryGetComponent<PlayerStats>(out _);

        // 1) If this is the player in normal mode, ignore further damage when dead.
        if (isPlayer && IsDead && !easy && amt > 0f)
            return;

        // 2) Apply change
        current -= amt;
        current = Mathf.Clamp(current, 0f, maxHP);

        // 3) Notify of HP change
        onHealthChanged?.Invoke();

        // 4) Flash only if this was damage
        if (amt > 0f && TryGetComponent<FlashOnHit>(out var flasher))
            flasher.Flash();

        // 5) Handle death
        if (current <= 0f)
        {
            onDeath?.Invoke();

            // Destroy if:
            //  - Not the player, or
            //  - The player in Normal mode
            if (!isPlayer || (isPlayer && !easy))
                Destroy(gameObject);
            else
                // If player in Easy mode, reset health so they can continue
                RestoreFullHealth();
        }
    }

    /// <summary>
    /// Instantly heals to full HP.
    /// </summary>
    public void RestoreFullHealth()
    {
        current = maxHP;
        onHealthChanged?.Invoke();
    }

    /// <summary>
    /// Increases the maximum health by the given amount.
    /// </summary>
    public void AddMaxHealth(int amount)
    {
        maxHP += amount;
        current = Mathf.Clamp(current, 0, maxHP);
        onHealthChanged?.Invoke();
    }
}
