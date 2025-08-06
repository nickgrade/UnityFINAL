using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Powerup : MonoBehaviour
{
    public PowerupType type;
    [Tooltip("How many bars/keys/stacks this gives")]
    public float amount = 1f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var stats = other.GetComponent<PlayerStats>();
        if (stats == null) return;

        switch (type)
        {
            case PowerupType.HealPercent:
                stats.HealPercent(0.5f); 
                break;
            case PowerupType.MaxHealthBoost:
                stats.AddMaxHealth(amount);
                break;
            case PowerupType.AttackSpeed:
                stats.ModifyFireRate(-0.1f * amount);
                break;
            case PowerupType.AttackDamage:
                stats.ModifyDamage(amount);
                break;
            case PowerupType.BossKey:
                stats.AddKeys(amount);
                break;
        }
        Destroy(gameObject);
    }
}
