using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem; // <--- THIS LINE IS REQUIRED FOR 'Keyboard'

public class WaveManager : MonoBehaviour
{
    [Header("Enemy Pool")]
    public GameObject[] enemyPrefabs; // Drag Runner and Tank prefabs here
    
    [Header("Wave Settings")]
    public int baseEnemiesPerPoint = 3;
    public int waveIncreasePerPoint = 2;
    
    private List<WaveSpawnPoint> spawnPoints = new List<WaveSpawnPoint>();
    private int currentWave = 0;
    private int activeEnemies = 0;
    private bool spawningWave = false;
    private bool isClearing = false;
    public static bool GameOver = false;

    void Start()
    {
        spawnPoints.AddRange(FindObjectsByType<WaveSpawnPoint>(FindObjectsSortMode.None));
        StartNextWave(); 
    }

    void Update()
    {
        // CHEAT CODE: Press R to kill all enemies
        // (Ensure Player Settings > Active Input Handling is set to "Both" or "Legacy")
        // TODO: REMOVE
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            DebugKillAll();
        }
    }

    void StartNextWave()
    {
        spawningWave = true;
        currentWave++;
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        Debug.Log($"--- Wave {currentWave} Starting ---");

        foreach (WaveSpawnPoint point in spawnPoints)
        {
            // Calculation using your +10 logic or the per-point logic
            int count = baseEnemiesPerPoint + (waveIncreasePerPoint * (currentWave - 1)) + point.enemyCountDelta;
            
            for (int i = 0; i < Mathf.Max(0, count); i++)
            {
                SpawnRandomEnemy(point);
                activeEnemies++;
                yield return new WaitForSeconds(0.1f);
            }
        }
        spawningWave = false;
    }

    void SpawnRandomEnemy(WaveSpawnPoint point)
    {
        // Pick a random enemy from your teammate's scripts
        GameObject randomPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        
        Vector2 randomPoint = Random.insideUnitCircle * point.scatterRadius;
        Vector3 spawnPos = new Vector3(point.transform.position.x + randomPoint.x, point.transform.position.y, point.transform.position.z + randomPoint.y);
        
        Instantiate(randomPrefab, spawnPos, Quaternion.identity);
    }

    public void EnemyDied()
    {
        if (isClearing) 
        {
            activeEnemies--; 
            return;
        }

        activeEnemies--;
        if (activeEnemies <= 0 && !spawningWave)
        {
            StartNextWave();
        }
    }

    void DebugKillAll()
    {

        StopAllCoroutines(); // Stop any currently spawning enemies
        spawningWave = false;
        isClearing = true;

        EnemyBase[] allEnemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        foreach (EnemyBase enemy in allEnemies)
        {
            // Use TakeDamage so effects play, OR use Destroy but manually decrement activeEnemies. 
            // Using TakeDamage is safer for logic consistency.
            if(enemy != null) enemy.TakeDamage(9999f); 
        }

        Projectile[] allProjectiles = FindObjectsByType<Projectile>(FindObjectsSortMode.None);
        foreach (Projectile proj in allProjectiles)
        {
           if(proj != null) Destroy(proj.gameObject);
        }
        
        // Force reset the counter in case of any drift
        activeEnemies = 0; 
        isClearing = false; // <--- Turn off the safety flag

        StartNextWave();
    }

    void OnDestroy()
    {
        GameOver = false;
    }

    void OnApplicationQuit()
    {
        GameOver = false;
    }
}