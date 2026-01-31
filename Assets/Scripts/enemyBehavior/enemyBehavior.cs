using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public Transform player;
    public float health = 10f;
    public float attackRange = 5f;
    public float attackCooldown = 1f;

    public float nextAttackTime;

    public virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public virtual void TakeDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0)
            Destroy(gameObject);
    }
}