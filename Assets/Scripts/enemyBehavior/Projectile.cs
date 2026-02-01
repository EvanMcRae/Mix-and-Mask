using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 1f;
    public float lifetime = 5f;
    public bool belongsToPlayer = false;
    private float spawnTime = 0;
    private const float iframes = 0.05f;

    void Start()
    {
        Destroy(gameObject, lifetime);
        spawnTime = Time.time;
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Enemy") || other.isTrigger) && belongsToPlayer)
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            ControllableEnemy ce = other.GetComponent<ControllableEnemy>();
            if (enemy != null)
            {
                if(ce != null && !ce.isUnderControl) enemy.TakeDamage(5f);
            }
        }
        
        if (other.CompareTag("Player") && !belongsToPlayer)
        {
            Debug.Log("Player got hit by tank shot");

            //later:
            ControllableEnemy player = other.GetComponent<ControllableEnemy>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            else
            {
                PlayerHealth playerAsMask = other.GetComponent<PlayerHealth>();
                playerAsMask.playerTakeDamage(5);
            }

        }
        

        if (Time.time - spawnTime > iframes)
        {
            Destroy(gameObject);
        }
    }
}
