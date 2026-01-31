using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public Transform player;
    public float health = 10f;
    public float attackRange = 5f;
    public float attackCooldown = 1f;
    public float nextAttackTime;

    [HideInInspector] 
    public WaveManager waveManager; // Added reference

    public virtual void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
            
        // Find the manager in the scene
        waveManager = Object.FindFirstObjectByType<WaveManager>();
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

        Destroy(gameObject);
    }
}