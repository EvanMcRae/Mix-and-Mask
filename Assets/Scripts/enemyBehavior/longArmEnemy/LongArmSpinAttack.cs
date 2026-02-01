using UnityEngine;
using System.Collections.Generic;

public class LongArmSpinAttack
{
    private LongArmEnemy enemy;
    private Transform model;
    private Transform player;
    
    private Vector3 lungeDirection;
    private Vector3 lungeStartPosition;
    private float spinRotationProgress;
    private float spinStartRotation;
    
    public LongArmSpinAttack(LongArmEnemy enemy, Transform model, Transform player)
    {
        this.enemy = enemy;
        this.model = model;
        this.player = player;
    }

    public void UpdatePlayer(Transform player)
    {
        this.player = player;
    }

    public void Start(GameObject leftArmCollider, GameObject rightArmCollider, HashSet<GameObject> hitThisSpin)
    {
        // Calculate lunge direction toward player
        lungeDirection = (player.position - enemy.transform.position).normalized;
        lungeDirection.y = 0f;

        lungeStartPosition = enemy.transform.position;
        spinRotationProgress = 0f;
        spinStartRotation = model.eulerAngles.y;

        // Enable arm colliders
        if (leftArmCollider != null)
        {
            leftArmCollider.SetActive(true);
            Debug.Log("Left arm collider enabled");
        }
        else
        {
            Debug.LogWarning("Left arm collider is null!");
        }
        
        if (rightArmCollider != null)
        {
            rightArmCollider.SetActive(true);
            Debug.Log("Right arm collider enabled");
        }
        else
        {
            Debug.LogWarning("Right arm collider is null!");
        }

        // Clear hit tracking
        hitThisSpin.Clear();
        Debug.Log("Starting spin attack!");
    }

    public void Perform(float stateTimer, float spinDuration, float lungeDistance, float spinRotationDegrees)
    {
        // Move forward
        float lungeProgress = 1f - (stateTimer / spinDuration);
        Vector3 targetPosition = lungeStartPosition + lungeDirection * lungeDistance;
        enemy.transform.position = Vector3.Lerp(lungeStartPosition, targetPosition, lungeProgress);

        // Rotate the entire enemy (and thus the arm colliders)
        spinRotationProgress = 1f - (stateTimer / spinDuration);
        float currentRotation = spinStartRotation + (spinRotationDegrees * spinRotationProgress);
        model.rotation = Quaternion.Euler(0f, currentRotation, 0f);
    }

    public void End(GameObject leftArmCollider, GameObject rightArmCollider, HashSet<GameObject> hitThisSpin)
    {
        // Disable arm colliders
        if (leftArmCollider != null)
            leftArmCollider.SetActive(false);
        if (rightArmCollider != null)
            rightArmCollider.SetActive(false);

        // Clear hit tracking
        hitThisSpin.Clear();
    }
}
