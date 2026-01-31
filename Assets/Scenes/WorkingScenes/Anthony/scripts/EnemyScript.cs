using UnityEngine;

public class EnemyScript : MonoBehaviour 
{
    public Transform playerTransform; // Keep this public, but don't drag anything into it in the prefab
    public float moveSpeed = 3f;

    void Start()
    {
        // This finds the GameObject in the scene tagged "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Enemy spawned but couldn't find an object with the 'Player' tag!");
        }
    }

    void Update() 
    {
        if (playerTransform != null) 
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.LookAt(playerTransform);
        }
    }
}