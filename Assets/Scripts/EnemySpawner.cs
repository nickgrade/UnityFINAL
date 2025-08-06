using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("What to Spawn")]
    [Tooltip("Drag in your Enemy prefabs here")]
    public GameObject[] enemyPrefabs;

    [Header("Where to Spawn")]
    [Tooltip("Drag in empty GameObjects marking spawn positions")]
    public Transform[] spawnPoints;

    [Header("Spawn Timing")]
    [Tooltip("Seconds between each spawn")]
    public float spawnInterval = 2f;
    [Tooltip("Max enemies to spawn (0 = infinite)")]
    public int maxSpawnCount = 0;

    int spawnedCount = 0;

    void Start()
    {
        // Start calling Spawn() every spawnInterval seconds, beginning immediately
        InvokeRepeating(nameof(Spawn), 0f, spawnInterval);
    }

    void Spawn()
    {
        // If we've reached our max, stop spawning
        if (maxSpawnCount > 0 && spawnedCount >= maxSpawnCount)
        {
            CancelInvoke(nameof(Spawn));
            return;
        }

        // 1) Pick a random enemy prefab
        var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        // 2) Pick a random spawn point
        var point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // 3) Instantiate it at that point, with the same rotation
        Instantiate(prefab, point.position, point.rotation);

        spawnedCount++;
    }
}
