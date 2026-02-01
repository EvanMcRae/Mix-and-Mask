using UnityEngine;

public class ShoulderRotation
{
    public Transform shoulderL;
    public Transform shoulderR;
    
    private Quaternion shoulderLInitialRotation;
    private Quaternion shoulderRInitialRotation;
    private float shoulderRotationDuration = 0.5f;
    private float shoulderRotationTimer = 0f;
    private bool isRotatingShoulders = false;
    private bool isResettingShoulders = false;
    
    // Configuration for rotation
    public enum RotationAxis { X, Y, Z }
    private RotationAxis rotationAxis;
    private float leftShoulderRotation;
    private float rightShoulderRotation;
    
    public ShoulderRotation(RotationAxis axis, float leftRotation, float rightRotation)
    {
        this.rotationAxis = axis;
        this.leftShoulderRotation = leftRotation;
        this.rightShoulderRotation = rightRotation;
    }
    
    public void StartRotation()
    {
        // Store initial shoulder rotations and start rotation
        if (shoulderL != null)
            shoulderLInitialRotation = shoulderL.localRotation;
        if (shoulderR != null)
            shoulderRInitialRotation = shoulderR.localRotation;
            
        shoulderRotationTimer = 0f;
        isRotatingShoulders = true;
        isResettingShoulders = false;
    }
    
    public void Update()
    {
        if (isRotatingShoulders)
        {
            shoulderRotationTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(shoulderRotationTimer / shoulderRotationDuration);
            
            if (shoulderL != null)
            {
                Vector3 currentEuler = shoulderLInitialRotation.eulerAngles;
                Vector3 targetEuler = GetTargetEuler(currentEuler, leftShoulderRotation);
                shoulderL.localRotation = Quaternion.Lerp(shoulderLInitialRotation, Quaternion.Euler(targetEuler), progress);
            }
            
            if (shoulderR != null)
            {
                Vector3 currentEuler = shoulderRInitialRotation.eulerAngles;
                Vector3 targetEuler = GetTargetEuler(currentEuler, rightShoulderRotation);
                shoulderR.localRotation = Quaternion.Lerp(shoulderRInitialRotation, Quaternion.Euler(targetEuler), progress);
            }
            
            if (progress >= 1f)
            {
                isRotatingShoulders = false;
            }
        }
        else if (isResettingShoulders)
        {
            shoulderRotationTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(shoulderRotationTimer / shoulderRotationDuration);
            
            if (shoulderL != null)
            {
                Quaternion currentRotation = shoulderL.localRotation;
                shoulderL.localRotation = Quaternion.Lerp(currentRotation, shoulderLInitialRotation, progress);
            }
            
            if (shoulderR != null)
            {
                Quaternion currentRotation = shoulderR.localRotation;
                shoulderR.localRotation = Quaternion.Lerp(currentRotation, shoulderRInitialRotation, progress);
            }
            
            if (progress >= 1f)
            {
                isResettingShoulders = false;
            }
        }
    }
    
    private Vector3 GetTargetEuler(Vector3 currentEuler, float rotationAmount)
    {
        switch (rotationAxis)
        {
            case RotationAxis.X:
                return new Vector3(currentEuler.x + rotationAmount, currentEuler.y, currentEuler.z);
            case RotationAxis.Y:
                return new Vector3(currentEuler.x, currentEuler.y + rotationAmount, currentEuler.z);
            case RotationAxis.Z:
                return new Vector3(currentEuler.x, currentEuler.y, currentEuler.z + rotationAmount);
            default:
                return currentEuler;
        }
    }
    
    public void StartReset()
    {
        shoulderRotationTimer = 0f;
        isRotatingShoulders = false;
        isResettingShoulders = true;
    }
    
    public void UpdateReset()
    {
        // Continue updating shoulder reset during cooldown
        if (isResettingShoulders)
        {
            Update();
        }
    }
    
    public void ResetImmediately()
    {
        // Immediately reset shoulders to initial rotation
        isRotatingShoulders = false;
        isResettingShoulders = false;
        shoulderRotationTimer = 0f;
        
        if (shoulderL != null)
        {
            shoulderL.localRotation = shoulderLInitialRotation;
        }
        if (shoulderR != null)
        {
            shoulderR.localRotation = shoulderRInitialRotation;
        }
    }
}
