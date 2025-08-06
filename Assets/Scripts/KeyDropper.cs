// Assets/Scripts/KeyDropper.cs
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Health))]
public class KeyDropper : MonoBehaviour
{
    [Header("Key Power-Up Prefab")]
    [Tooltip("Assign your Powerup key prefab here (PowerupType.BossKey)")]
    public GameObject keyPowerupPrefab;

    [Header("Drop Settings")]
    [Tooltip("Local offset from the enemy's position")]
    public Vector3 dropOffset = Vector3.zero;

    private Health _health;

    void Awake()
    {
        // Cache Health and subscribe to its onDeath event
        _health = GetComponent<Health>();
        _health.onDeath.AddListener(DropKey);
    }

    /// <summary>
    /// Instantiates the key power-up when this enemy dies.
    /// </summary>
    void DropKey()
    {
        if (keyPowerupPrefab == null)
        {
            Debug.LogWarning($"[{name}] KeyDropper has no keyPowerupPrefab assigned!");
            return;
        }

        Vector3 spawnPos = transform.position + dropOffset;
        Instantiate(keyPowerupPrefab, spawnPos, Quaternion.identity);
    }

    void OnDestroy()
    {
        // Clean up the listener if the enemy is destroyed by other means
        if (_health != null)
            _health.onDeath.RemoveListener(DropKey);
    }
}
