using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Powerup : MonoBehaviour
{
    public PowerupType type;
    [Tooltip("How many bars/keys/stacks this gives")]
    public float amount = 1f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            Debug.LogWarning("Powerup collided with a non-player object: " + other.name);
            return;
        }

        var stats = other.GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("PlayerStats component is missing on the Player object!");
            return;
        }

        switch (type)
        {
            case PowerupType.HealPercent:
                Debug.Log("Applying HealPercent powerup.");
                stats.health.RestoreFullHealth(); // Heal to full health
                break;
            case PowerupType.MaxHealthBoost:
                Debug.Log("Applying MaxHealthBoost powerup.");
                stats.health.AddMaxHealth(Mathf.CeilToInt(amount)); // Use AddMaxHealth to update max HP and trigger UI update
                stats.health.RestoreFullHealth(); // Optionally heal to full
                break;
            case PowerupType.AttackSpeed:
                Debug.Log("Applying AttackSpeed powerup.");
                stats.shooter.fireRate = Mathf.Max(0.05f, stats.shooter.fireRate - 0.1f * amount); // Decrease fire rate
                break;
            case PowerupType.AttackDamage:
                Debug.Log("Applying AttackDamage powerup.");
                stats.shooter.bulletDamage += Mathf.CeilToInt(amount); // Increase bullet damage
                break;
            case PowerupType.BossKey:
                Debug.Log("Applying BossKey powerup.");
                stats.AddKeys(Mathf.CeilToInt(amount)); // Add keys
                break;
            default:
                Debug.LogError("Unsupported PowerupType: " + type);
                break;
        }

        Destroy(gameObject);
    }
}
