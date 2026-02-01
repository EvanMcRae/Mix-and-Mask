using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public Transform player;
    public float health = 10f;
    public float attackRange = 5f;
    public float attackCooldown = 1f;
    public float nextAttackTime;

    public float timeToStartMoving = 1f;
    public bool canMove = false;
    private float t;
    
    public bool isSolid { get; protected set; }

    [HideInInspector] 
    public WaveManager waveManager; // Added reference

    public virtual void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        isSolid = true;

        // Find the manager in the scene
        waveManager = Object.FindFirstObjectByType<WaveManager>();
    }

    public virtual void Update()
    {
        if (!canMove) t += Time.deltaTime;
        if (!canMove && t > timeToStartMoving) {
            canMove = true;
        }
        
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    public virtual void TakeDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0)
            Die();
    }

    protected virtual void Die()
    {
        if (waveManager != null)
            waveManager.EnemyDied(); // Report death to progress the wave

        PlayerStats.EnemiesKilled++;

        Destroy(gameObject);
    }
}