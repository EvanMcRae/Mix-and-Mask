using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class LongArmController : ControllableEnemy
{
    [Header("LongArm Specific")]
    public float rotationSpeed = 180f;
    public float spinCooldown = 2f;
    
    private LongArmEnemy enemyScript;
    private NavMeshAgent navAgent;
    private bool isSpinning = false;
    private float lastSpinTime = -999f;

    public override void Start()
    {
        base.Start();
        
        enemyScript = GetComponent<LongArmEnemy>();
        navAgent = GetComponent<NavMeshAgent>();
        
        if (enemyScript == null)
        {
            Debug.LogError("LongArmController requires LongArmEnemy component!");
        }
        
        type = ControllableEnemy.EnemyType.LongArm;
    }

    void FixedUpdate()
    {
        if (!isUnderControl) return;

        Debug.Log($"LongArmController FixedUpdate: moveDir=({moveDir.x}, {moveDir.y}), velocity={_rigidbody.linearVelocity.magnitude}, isKinematic={_rigidbody.isKinematic}");
        
        // Apply movement force
        if (_rigidbody.linearVelocity.magnitude < maxSpeed) 
            _rigidbody.AddForce(new Vector3(moveDir.x, 0, moveDir.y) * moveAcceleration, ForceMode.Acceleration);
        else 
            _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * maxSpeed;
    }

    public override void Move(Vector2 moveDir)
    {
        Debug.Log($"LongArmController Move called: moveDir=({moveDir.x}, {moveDir.y}), isSpinning={isSpinning}");
        
        // Don't allow movement while spinning
        if (isSpinning) return;
        
        base.Move(moveDir);
    }

    public override void Rotate(float yRotation)
    {
        // Don't allow rotation while spinning
        if (!isUnderControl || isSpinning) return;

        // Rotate the model transform
        if (enemyScript != null && enemyScript.model != null)
        {
            enemyScript.model.eulerAngles = new Vector3(0, yRotation, 0);
        }
    }

    public override void PrimaryAction()
    {
        if (!isUnderControl || isSpinning) return;
        
        // Check cooldown
        if (Time.time < lastSpinTime + spinCooldown)
        {
            Debug.Log($"Spin on cooldown! {spinCooldown - (Time.time - lastSpinTime):F1}s remaining");
            return;
        }
        
        Debug.Log("LongArm Primary Action: Starting spin attack!");
        StartCoroutine(PlayerControlledSpin());
    }

    private IEnumerator PlayerControlledSpin()
    {
        isSpinning = true;
        lastSpinTime = Time.time;
        
        // Use the enemy script's parameters
        float spinDuration = enemyScript.spinDuration;
        float lungeDistance = enemyScript.lungeDistance;
        float spinDegrees = enemyScript.spinRotationDegrees;
        
        // Get direction from model's forward
        Vector3 lungeDirection = enemyScript.model.forward;
        lungeDirection.y = 0f;
        lungeDirection.Normalize();
        
        Vector3 startPosition = transform.position;
        float startRotation = enemyScript.model.eulerAngles.y;
        
        // Enable arm colliders
        if (enemyScript.leftArmCollider != null)
        {
            enemyScript.leftArmCollider.SetActive(true);
            Debug.Log("Player spin: Left arm enabled");
        }
        if (enemyScript.rightArmCollider != null)
        {
            enemyScript.rightArmCollider.SetActive(true);
            Debug.Log("Player spin: Right arm enabled");
        }
        
        // Perform the spin
        float elapsed = 0f;
        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / spinDuration;
            
            // Lunge forward
            Vector3 targetPosition = startPosition + lungeDirection * lungeDistance;
            _rigidbody.MovePosition(Vector3.Lerp(startPosition, targetPosition, progress));
            
            // Spin
            float currentRotation = startRotation + (spinDegrees * progress);
            enemyScript.model.rotation = Quaternion.Euler(0f, currentRotation, 0f);
            
            yield return null;
        }
        
        // Disable arm colliders
        if (enemyScript.leftArmCollider != null)
            enemyScript.leftArmCollider.SetActive(false);
        if (enemyScript.rightArmCollider != null)
            enemyScript.rightArmCollider.SetActive(false);
        
        Debug.Log("Player spin: Complete!");
        isSpinning = false;
    }

    public override void SecondaryAction()
    {
        if (!isUnderControl) return;
        
        Debug.Log("LongArm Secondary Action: Not implemented");
    }

    public override void SetControlled(bool underControl)
    {
        Debug.Log($"LongArmController SetControlled: underControl={underControl}");
        
        isUnderControl = underControl;

        if (enemyScript != null)
        {
            enemyScript.enabled = !underControl;
        }

        if (navAgent != null)
        {
            navAgent.enabled = !underControl;
            
            // Prevent NavMeshAgent from overriding transform position when under player control
            if (underControl)
            {
                navAgent.updatePosition = false;
            }
            else
            {
                navAgent.updatePosition = true;
            }
        }

        if (_rigidbody != null)
        {
            _rigidbody.isKinematic = !underControl;
            
            Debug.Log($"Rigidbody isKinematic set to: {_rigidbody.isKinematic}");
            
            // Ensure proper rigidbody constraints for movement
            if (underControl)
            {
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | 
                                        RigidbodyConstraints.FreezeRotationZ | 
                                        RigidbodyConstraints.FreezePositionY;
                Debug.Log($"Rigidbody constraints set to: {_rigidbody.constraints}");
            }
        }

        // Ensure arm colliders are disabled when taking control
        if (underControl && enemyScript != null)
        {
            if (enemyScript.leftArmCollider != null)
                enemyScript.leftArmCollider.SetActive(false);
            if (enemyScript.rightArmCollider != null)
                enemyScript.rightArmCollider.SetActive(false);
                
            // Reset spinning state
            isSpinning = false;
        }
    }


}
