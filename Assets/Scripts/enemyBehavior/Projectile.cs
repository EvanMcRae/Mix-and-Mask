using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 1f;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.isTrigger) 
            return;
        
        if (other.CompareTag("Player"))
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

            Destroy(gameObject);
        }

        Destroy(gameObject);
        
    }
}
