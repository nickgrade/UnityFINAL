// Assets/Scripts/WaveSpawner.cs
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct EnemySpawnInfo
    {
        [Tooltip("Enemy prefab to spawn")]
        public GameObject prefab;
        [Tooltip("Relative weight for spawning this enemy")]
        public float weight;
    }

    [Header("Spawn Settings")]
    [Tooltip("Where enemies will appear")]
    public Transform[] spawnPoints;

    [Tooltip("Radius around each spawn point to randomize position")]
    public float spawnRadius = 2f;

    [Tooltip("Configure each regular enemy type and its spawn weight")]
    public EnemySpawnInfo[] regularEnemies;

    [Tooltip("Final buffed tank that drops a key")]
    public GameObject keyedTankPrefab;

    [Tooltip("How many enemies in each wave (e.g. [3,5] = wave1:3, wave2:5)")]
    public int[] waveCounts;

    [Tooltip("Delay between finishing one wave and spawning the next")]
    public float timeBetweenWaves = 2f;

    [Header("Wave-Controlled Doors")]
    [Tooltip("Drag WaveDoorController instances for doors to close/open")]
    public WaveDoorController[] waveDoors;

    [Header("Key-Controlled Doors (optional)")]
    [Tooltip("Drag DoorController instances for doors that open on keys")]
    public DoorController[] keyDoors;

    [Header("Events")]
    public UnityEvent onWaveSequenceStart;
    public UnityEvent onAllWavesComplete;

    bool triggered;
    int _nextSpawnIndex = 0; 

    // Precomputed cumulative weights for fast selection
    float[] _cumulativeWeights;
    float _totalWeight;

    void Awake()
    {
        // ensure trigger
        var col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;

        // validate spawnPoints
        if (spawnPoints.Length == 0)
            Debug.LogError("WaveSpawner: No spawnPoints assigned!", this);

        // build weight table
        BuildWeightTable();
    }

    void BuildWeightTable()
    {
        int n = regularEnemies.Length;
        _cumulativeWeights = new float[n];
        _totalWeight = 0f;
        for (int i = 0; i < n; i++)
        {
            _totalWeight += Mathf.Max(0f, regularEnemies[i].weight);
            _cumulativeWeights[i] = _totalWeight;
        }

        if (_totalWeight <= 0f)
            Debug.LogWarning("WaveSpawner: Total spawn weight is zero; no regular enemies will spawn.");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        // close doors
        onWaveSequenceStart?.Invoke();
        foreach (var d in waveDoors) d.CloseDoor();

        // regular waves
        for (int w = 0; w < waveCounts.Length; w++)
        {
            int count = waveCounts[w];
            for (int i = 0; i < count; i++)
            {
                var prefab = PickWeightedEnemy();
                Spawn(prefab, "waveEnemies");
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("waveEnemies").Length == 0);
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        // final boss
        Spawn(keyedTankPrefab, "KeyedTank");
        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("KeyedTank").Length == 0);

        // open doors
        onAllWavesComplete?.Invoke();
        foreach (var d in waveDoors) d.OpenDoor();
    }

    GameObject PickWeightedEnemy()
    {
        if (_totalWeight <= 0f || regularEnemies.Length == 0)
            return null;

        float r = Random.value * _totalWeight;
        for (int i = 0; i < _cumulativeWeights.Length; i++)
        {
            if (r <= _cumulativeWeights[i])
                return regularEnemies[i].prefab;
        }
        // fallback
        return regularEnemies[regularEnemies.Length - 1].prefab;
    }

    void Spawn(GameObject prefab, string tag)
    {
        if (prefab == null) return;

        // round-robin spawnPoint
        var sp = spawnPoints[_nextSpawnIndex];
        _nextSpawnIndex = (_nextSpawnIndex + 1) % spawnPoints.Length;

        // random offset
        Vector2 offs = Random.insideUnitCircle * spawnRadius;
        Vector3 pos = sp.position + (Vector3)offs;

        var go = Instantiate(prefab, pos, Quaternion.identity);
        go.tag = tag;
    }

    public void UnlockKeyDoors()
    {
        foreach (var d in keyDoors)
            d.TriggerOpenDoor();
    }
}
