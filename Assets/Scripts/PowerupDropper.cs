// Assets/Scripts/PowerupDropper.cs
using UnityEngine;

[RequireComponent(typeof(Health))]
public class PowerupDropper : MonoBehaviour
{
    [System.Serializable]
    public struct DropEntry
    {
        [Tooltip("The Powerup prefab to drop (must have your Powerup script on it)")]
        public GameObject powerupPrefab;
        [Range(0f, 1f), Tooltip("Chance (0–1) that this powerup will drop when the enemy dies")]
        public float dropChance;
    }

    [Tooltip("Configure one entry per power‐up you want this enemy to potentially drop")]
    public DropEntry[] drops;

    Health _health;

    void Awake()
    {
        _health = GetComponent<Health>();
        _health.onDeath.AddListener(OnDeath);
    }

    void OnDeath()
    {
        // Loop through each configured drop
        foreach (var entry in drops)
        {
            if (entry.powerupPrefab == null)
                continue;

            // Random roll
            if (Random.value <= entry.dropChance)
            {
                Instantiate(
                    entry.powerupPrefab,
                    transform.position,
                    Quaternion.identity
                );
            }
        }
    }

    void OnDestroy()
    {
        // Clean up listener in case this was destroyed some other way
        if (_health != null)
            _health.onDeath.RemoveListener(OnDeath);
    }
}
