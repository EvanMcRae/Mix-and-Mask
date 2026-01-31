using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyPrefab;
    public Transform playerTransform;

    [Header("Wave Settings")]
    public float timeBetweenWaves = 10f;
    public int baseEnemiesPerPoint = 5; // The "Universal Toggle"
    public int waveIncreasePerPoint = 2; // How many more each point gets next wave

    private List<WaveSpawnPoint> spawnPoints = new List<WaveSpawnPoint>();
    private int currentWave = 0;
    private float waveTimer;

    void Start()
    {
        // Automatically find all spawn points in the scene
        spawnPoints.AddRange(FindObjectsByType<WaveSpawnPoint>(FindObjectsSortMode.None));
        
        if (spawnPoints.Count == 0)
            Debug.LogError("No WaveSpawnPoints found in the scene!");

        waveTimer = timeBetweenWaves;
        StartCoroutine(SpawnWave());
    }

    void Update()
    {
        waveTimer -= Time.deltaTime;
        if (waveTimer <= 0)
        {
            currentWave++;
            waveTimer = timeBetweenWaves;
            StartCoroutine(SpawnWave());
        }
    }

    IEnumerator SpawnWave()
    {
        Debug.Log($"--- Starting Wave {currentWave + 1} ---");

        foreach (WaveSpawnPoint point in spawnPoints)
        {
            int countForThisPoint = baseEnemiesPerPoint + (waveIncreasePerPoint * currentWave) + point.enemyCountDelta;
            countForThisPoint = Mathf.Max(0, countForThisPoint);

            for (int i = 0; i < countForThisPoint; i++)
            {
                // NEW: Pass the radius to the spawn function
                SpawnEnemyAtPoint(point.transform.position, point.scatterRadius);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    void SpawnEnemyAtPoint(Vector3 center, float radius)
    {
        // Generate a random 2D point inside a circle
        Vector2 randomPoint = Random.insideUnitCircle * radius;
        
        // Convert 2D (x, y) to 3D (x, 0, z) relative to the spawn point center
        Vector3 spawnPos = new Vector3(
            center.x + randomPoint.x,
            center.y, // Keeps them at the same height as the spawn point
            center.z + randomPoint.y
        );

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}