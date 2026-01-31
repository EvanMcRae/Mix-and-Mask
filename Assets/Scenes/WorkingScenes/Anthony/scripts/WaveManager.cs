using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyPrefab;
    public Transform playerTransform;

    [Header("Wave Settings")]
    public float timeBetweenWaves = 5f;
    public int initialEnemyCount = 10;
    public float spawnRadius = 20f;

    private int currentWave = 0;
    private int enemiesToSpawn;
    private float waveTimer;

    void Start()
    {
        enemiesToSpawn = initialEnemyCount;
        waveTimer = timeBetweenWaves;
        // Optional: Start the first wave immediately
        StartCoroutine(SpawnWave());
    }

    void Update()
    {
        // Timer logic
        waveTimer -= Time.deltaTime;

        if (waveTimer <= 0)
        {
            PrepareNextWave();
        }
    }

    void PrepareNextWave()
    {
        currentWave++;
        // The math logic: Previous count + 10
        if (currentWave > 1) {
            enemiesToSpawn += 10;
        }

        waveTimer = timeBetweenWaves; // Reset timer
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        Debug.Log($"Spawning Wave {currentWave}: {enemiesToSpawn} enemies.");

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.1f); // Slight delay so they don't pop in all at once
        }
    }

    void SpawnEnemy()
    {
        // Generate random position in a circle around the player
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = new Vector3(randomCircle.x, 0, randomCircle.y) + playerTransform.position;

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        
        // Setup the enemy placeholder script
        EnemyScript script = newEnemy.GetComponent<EnemyScript>();
        if (script != null)
        {
            script.playerTransform = playerTransform;
        }
    }
}