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
    
    // Shoulder rotation helper
    private ShoulderRotation shoulderRotation;
    
    public LongArmPunchAttack(LongArmEnemy enemy, Transform player)
    {
        this.enemy = enemy;
        this.player = player;
        
        // Initialize shoulder rotation: X-axis, both shoulders +100 degrees
        shoulderRotation = new ShoulderRotation(ShoulderRotation.RotationAxis.X, -100f, -100f);
    }

    public void UpdatePlayer(Transform player)
    {
        this.player = player;
    }
    
    public void SetShoulders(Transform shoulderL, Transform shoulderR)
    {
        shoulderRotation.shoulderL = shoulderL;
        shoulderRotation.shoulderR = shoulderR;
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
        
        // Start shoulder rotation
        shoulderRotation.StartRotation();
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
        
        // Update shoulder rotation
        shoulderRotation.Update();
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
        
        // Update shoulder rotation
        shoulderRotation.Update();
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
        
        // Start resetting shoulder rotation
        shoulderRotation.StartReset();
    }
    
    private void UpdateShoulderRotation()
    {
        shoulderRotation.Update();
    }
    
    public void UpdateShoulderReset()
    {
        shoulderRotation.UpdateReset();
    }
    
    // Public methods for player control
    public void StartShoulderRotation()
    {
        shoulderRotation.StartRotation();
    }
    
    public void UpdateShoulderRotationManually()
    {
        shoulderRotation.Update();
    }
    
    public void StartShoulderReset()
    {
        shoulderRotation.StartReset();
    }
    
    public void ResetShouldersImmediately()
    {
        shoulderRotation.ResetImmediately();
    }
}
