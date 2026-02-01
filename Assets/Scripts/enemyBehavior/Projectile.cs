using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 1f;
    public float lifetime = 5f;
    private float spawnTime = 0;
    public bool belongsToPlayer = false;
    private const float iframes = 0.05f;

    void Start()
    {
        Destroy(gameObject, lifetime);
        spawnTime = Time.time;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.isTrigger)
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            ControllableEnemy ce = other.GetComponent<ControllableEnemy>();
            if (enemy != null)
            {
                if (belongsToPlayer)
                {
                    if (ce != null && !ce.isUnderControl) enemy.TakeDamage(5f);
                }
                else
                {
                    if (ce != null && ce.isUnderControl) ce.TakeDamage(damage);
                }
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
                playerHealth playerAsMask = other.GetComponent<playerHealth>();
                playerAsMask.playerTakeDamage(5);
            }

        }

        if (Time.time - spawnTime > iframes)
        {
            Destroy(gameObject);
        }
    }
}
