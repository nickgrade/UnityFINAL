// GameManager.cs
using UnityEngine;
using TMPro;
using System.Collections;
public class GameManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject enemyPrefab, bossPrefab;
    public TextMeshProUGUI counterText;
    int alive;
    void Start() => StartCoroutine(SpawnWaves());

    IEnumerator SpawnWaves()
    {
        int wave = 1;
        while (true)
        {
            int toSpawn = wave * 3;
            for (int i = 0; i < toSpawn; i++)
            {
                Spawn(enemyPrefab);
                yield return new WaitForSeconds(0.2f);
            }
            while (alive > 0) { counterText.text = $"{alive} Enemies"; yield return null; }
            wave++;
            if (wave == 4) { Spawn(bossPrefab); break; }          // boss wave
            yield return new WaitForSeconds(2f);
        }
    }
    void Spawn(GameObject prefab)
    {
        Transform p = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(prefab, p.position, Quaternion.identity);
        alive++;
    }
    public void OnEnemyDeath() => alive--;
}
