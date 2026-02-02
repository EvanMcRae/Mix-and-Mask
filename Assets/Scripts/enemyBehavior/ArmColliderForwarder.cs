using UnityEngine;

/// <summary>
/// Forwards trigger events from arm colliders to the parent LongArmEnemy.
/// Attach this to the arm collider GameObjects.
/// </summary>
public class ArmColliderForwarder : MonoBehaviour
{
    private LongArmEnemy parentEnemy;
    private LongArmController parentController;

    void Start()
    {
        // Find the LongArmEnemy component in parent hierarchy
        parentEnemy = GetComponentInParent<LongArmEnemy>();
        parentController = GetComponentInParent<LongArmController>();
        
        if (parentEnemy == null)
        {
            Debug.LogError("ArmColliderForwarder could not find LongArmEnemy parent!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (parentEnemy != null && parentController != null)
        {
            if (parentController.isUnderControl) parentController.OnArmColliderHit(other);
            else parentEnemy.OnArmColliderHit(other);
        }
    }
}
