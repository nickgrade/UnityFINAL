using UnityEngine;

[System.Serializable]
public struct DropEntry
{
    public PowerupType type;
    [Range(0,1)] public float   weight;
    public GameObject prefab; // Powerup prefab
    public float amount;
}

public class EnemyDropper : MonoBehaviour
{
    [Tooltip("What this enemy can drop")]
    public DropEntry[] drops = new DropEntry[]
    {
        new DropEntry { type = PowerupType.HealPercent, weight = 0.3f, prefab = null, amount = 0.5f },
        new DropEntry { type = PowerupType.MaxHealthBoost, weight = 0.2f, prefab = null, amount = 1f },
        new DropEntry { type = PowerupType.AttackSpeed, weight = 0.2f, prefab = null, amount = 0.1f },
        new DropEntry { type = PowerupType.AttackDamage, weight = 0.2f, prefab = null, amount = 1f },
        new DropEntry { type = PowerupType.BossKey, weight = 0.1f, prefab = null, amount = 1f }
    };
    [Range(0,1)] public float dropChance = 0.5f;

    public void TryDrop()
    {
        if (Random.value > dropChance) return;

        // pick by weight
        float total = 0f;
        foreach (var e in drops) total += e.weight;
        float roll = Random.value * total;
        foreach (var e in drops)
        {
            if (e.prefab == null) continue; // Skip if prefab is null

            if (roll <= e.weight)
            {
                var go = Instantiate(e.prefab, transform.position, Quaternion.identity);
                var pu = go.GetComponent<Powerup>();
                pu.type   = e.type;
                pu.amount = e.amount;
                break;
            }
            roll -= e.weight;
        }
    }
}
