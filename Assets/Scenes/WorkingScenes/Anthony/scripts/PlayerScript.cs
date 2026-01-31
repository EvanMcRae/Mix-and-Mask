using UnityEngine;
using UnityEngine.InputSystem; // You MUST add this line
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    private Rigidbody rb;
    private Vector2 input;

    [Header("Combat")]
    public float playerDamage = 5f;
    public float attackRange = 2f;
    public LayerMask enemyLayer;

    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        
        // Ensure the player is tagged so enemies can find it
        this.tag = "Player"; 
    }

    // NEW INPUT SYSTEM METHODS
    public void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
    }

    public void OnFire()
    {
        Attack();
    }

    void FixedUpdate()
    {
        // Physics-based movement
        Vector3 moveDirection = new Vector3(input.x, 0, input.y).normalized;
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    void Attack()
    {
        // Check for enemies in front of the player
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            // Call the TakeDamage function from the teammate's EnemyBase script
            EnemyBase enemyScript = enemy.GetComponent<EnemyBase>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(playerDamage);
                Debug.Log($"Hit enemy for {playerDamage} damage!");
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Player Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("Player is dead. Game Over.");
            // Restart level or show UI
        }
    }

    // Visual aid for attack range in the Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void Die()
    {
        Debug.Log("Player has died.");
        // For now, let's just reload the scene to reset
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}