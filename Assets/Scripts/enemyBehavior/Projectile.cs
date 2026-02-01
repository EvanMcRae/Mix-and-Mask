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
        bool dontDestroy = false;
        //if(belongsToPlayer) Debug.Log("Projectile Hit: " + other.gameObject.name);
        //if (other.gameObject.name.StartsWith("runnerEnemy")) Debug.Log("Compare tag enemy: " + other.CompareTag("Enemy"));
        if (other.CompareTag("Enemy") || other.isTrigger)
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            ControllableEnemy ce = other.GetComponent<ControllableEnemy>();
            //if (other.gameObject.name.StartsWith("runnerEnemy")) Debug.Log("Is enemy null: " + enemy == null);
            //if (other.gameObject.name.StartsWith("runnerEnemy")) Debug.Log("Is ce null: " + ce == null);
            //if (other.gameObject.name.StartsWith("runnerEnemy")) Debug.Log("Is ce under control: " + ce.isUnderControl);
            if (enemy != null)
            {
                if (belongsToPlayer)
                {
                    if (ce != null && !ce.isUnderControl) enemy.TakeDamage(5f);
                    else if (ce == null) enemy.TakeDamage(5f);
                    if (!enemy.isSolid) dontDestroy = true;
                }
                else
                {
                    if (ce != null && ce.isUnderControl) ce.TakeDamage(damage);
                    if (ce != null && !ce.isSolid) dontDestroy = true;
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
                if (!player.isSolid) dontDestroy = true;
            }
            else
            {
                playerHealth playerAsMask = other.GetComponent<playerHealth>();
                playerAsMask.playerTakeDamage(5);
            }

        }

        if (Time.time - spawnTime > iframes && !dontDestroy)
        {
            Debug.Log("Destroying Projectile!");
            Destroy(gameObject);
        }
    }
}
