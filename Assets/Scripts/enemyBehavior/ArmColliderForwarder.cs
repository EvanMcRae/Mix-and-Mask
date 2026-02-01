using UnityEngine;

/// <summary>
/// Forwards trigger events from arm colliders to the parent LongArmEnemy.
/// Attach this to the arm collider GameObjects.
/// </summary>
public class ArmColliderForwarder : MonoBehaviour
{
    private LongArmEnemy parentEnemy;

    void Start()
    {
        // Find the LongArmEnemy component in parent hierarchy
        parentEnemy = GetComponentInParent<LongArmEnemy>();
        
        if (parentEnemy == null)
        {
            Debug.LogError("ArmColliderForwarder could not find LongArmEnemy parent!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (parentEnemy != null)
        {
            parentEnemy.OnArmColliderHit(other);
        }
    }
}
