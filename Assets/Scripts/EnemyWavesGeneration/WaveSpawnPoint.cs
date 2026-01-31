using UnityEngine;

public class WaveSpawnPoint : MonoBehaviour
{
    [Tooltip("Adds or subtracts from the Wave Manager's base count.")]
    public int enemyCountDelta = 0;

    [Tooltip("How far from the center point enemies can spawn.")]
    public float scatterRadius = 10f;

    // Updated Gizmos to show the scatter area
    private void OnDrawGizmos()
    {
        // Semi-transparent red
        Gizmos.color = new Color(1, 0, 0, 0.5f); 
        
        // Draw the center point
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Draw the scatter boundary as a sphere (usually fine for 3D)
        // If you want it to look like a flat ring, we'd use Handles (see below)
        Gizmos.DrawWireSphere(transform.position, scatterRadius);
    }
}