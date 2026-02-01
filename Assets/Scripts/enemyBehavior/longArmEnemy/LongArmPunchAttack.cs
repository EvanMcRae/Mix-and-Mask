using UnityEngine;
using System.Collections.Generic;

public class LongArmPunchAttack
{
    private LongArmEnemy enemy;
    private Transform player;
    
    private Vector3 leftArmOriginalPosition;
    private Vector3 leftArmTargetPosition;
    private Vector3 rightArmOriginalPosition;
    private Vector3 rightArmTargetPosition;
    private Vector3 punchDirection;
    
    public LongArmPunchAttack(LongArmEnemy enemy, Transform player)
    {
        this.enemy = enemy;
        this.player = player;
    }

    public void UpdatePlayer(Transform player)
    {
        this.player = player;
    }

    public void Start(GameObject leftArmCollider, GameObject rightArmCollider, float punchExtendDistance, HashSet<GameObject> hitThisSpin)
    {
        // Calculate punch direction toward player
        punchDirection = (player.position - enemy.transform.position).normalized;
        punchDirection.y = 0f;
    
        // Convert punch direction to model's local space once - both arms move in same direction
        Vector3 punchDirectionLocal = enemy.model.InverseTransformDirection(punchDirection * punchExtendDistance);
    
        // Use both arms for punch - they move parallel to each other
        if (leftArmCollider != null)
        {
            leftArmOriginalPosition = leftArmCollider.transform.localPosition;
            leftArmTargetPosition = leftArmOriginalPosition + punchDirectionLocal;
            leftArmCollider.SetActive(true);
            Debug.Log("Left arm puncdh started!");
        }
        
        if (rightArmCollider != null)
        {
            rightArmOriginalPosition = rightArmCollider.transform.localPosition;
            rightArmTargetPosition = rightArmOriginalPosition + punchDirectionLocal;
            rightArmCollider.SetActive(true);
            Debug.Log("Right arm punch started!");
        }
    
        hitThisSpin.Clear();
    }

    public void PerformExtend(GameObject leftArmCollider, GameObject rightArmCollider, float stateTimer, float punchExtendDuration)
    {
        float progress = 1f - (stateTimer / punchExtendDuration);
        
        if (leftArmCollider != null)
        {
            leftArmCollider.transform.localPosition = Vector3.Lerp(leftArmOriginalPosition, leftArmTargetPosition, progress);
        }
        
        if (rightArmCollider != null)
        {
            rightArmCollider.transform.localPosition = Vector3.Lerp(rightArmOriginalPosition, rightArmTargetPosition, progress);
        }
    }

    public void PerformRetract(GameObject leftArmCollider, GameObject rightArmCollider, float stateTimer, float punchRetractDuration)
    {
        float progress = 1f - (stateTimer / punchRetractDuration);
        
        if (leftArmCollider != null)
        {
            leftArmCollider.transform.localPosition = Vector3.Lerp(leftArmTargetPosition, leftArmOriginalPosition, progress);
        }
        
        if (rightArmCollider != null)
        {
            rightArmCollider.transform.localPosition = Vector3.Lerp(rightArmTargetPosition, rightArmOriginalPosition, progress);
        }
    }

    public void End(GameObject leftArmCollider, GameObject rightArmCollider, HashSet<GameObject> hitThisSpin)
    {
        // Disable arm colliders
        if (leftArmCollider != null)
        {
            leftArmCollider.SetActive(false);
            leftArmCollider.transform.localPosition = leftArmOriginalPosition;
        }
        
        if (rightArmCollider != null)
        {
            rightArmCollider.SetActive(false);
            rightArmCollider.transform.localPosition = rightArmOriginalPosition;
        }

        // Clear hit tracking
        hitThisSpin.Clear();
    }
}
