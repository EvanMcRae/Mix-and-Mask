using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class LongArmController : ControllableEnemy
{
    [Header("LongArm Specific")]
    public float rotationSpeed = 180f;
    public float spinCooldown = 2f;
    public float punchCooldown = 1.5f;
    
    private LongArmEnemy enemyScript;
    private NavMeshAgent navAgent;
    private bool isSpinning = false;
    private bool isPunching = false;
    private float lastSpinTime = -999f;
    private float lastPunchTime = -999f;

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

    // NOTE: Removed duplicate FixedUpdate - base class ControllableEnemy.FixedUpdate() handles physics

    public override void Move(Vector2 moveDir)
    {
        Debug.Log($"LongArmController Move called: moveDir=({moveDir.x}, {moveDir.y}), isSpinning={isSpinning}, isPunching={isPunching}");
        
        // Don't allow movement while attacking
        if (isSpinning || isPunching) return;
        
        base.Move(moveDir);
    }

    public override void Rotate(float yRotation)
    {
        // Don't allow rotation while attacking
        if (!isUnderControl || isSpinning || isPunching) return;

        // Rotate the model transform
        if (enemyScript != null && enemyScript.model != null)
        {
            enemyScript.model.eulerAngles = new Vector3(0, yRotation, 0);
        }
    }

    public override void PrimaryAction()
    {
        if (!isUnderControl || isSpinning || isPunching) return;
        
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

    private IEnumerator PlayerControlledPunch()
    {
        isPunching = true;
        lastPunchTime = Time.time;
        
        // Use the enemy script's parameters
        float extendDuration = enemyScript.punchExtendDuration;
        float retractDuration = enemyScript.punchRetractDuration;
        float punchDistance = enemyScript.punchExtendDistance;
        
        // Get direction from model's forward
        Vector3 punchDirection = enemyScript.model.forward;
        punchDirection.y = 0f;
        punchDirection.Normalize();
        
        GameObject armCollider = enemyScript.rightArmCollider;
        if (armCollider == null)
        {
            Debug.LogWarning("Right arm collider is null, cannot punch!");
            isPunching = false;
            yield break;
        }
        
        // Store original position
        Vector3 armOriginalPosition = armCollider.transform.localPosition;
        Vector3 armTargetPosition = armOriginalPosition + armCollider.transform.InverseTransformDirection(punchDirection * punchDistance);
        
        // Enable arm collider
        armCollider.SetActive(true);
        Debug.Log("Player punch: Right arm extended!");
        
        // Extend phase
        float elapsed = 0f;
        while (elapsed < extendDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / extendDuration;
            armCollider.transform.localPosition = Vector3.Lerp(armOriginalPosition, armTargetPosition, progress);
            yield return null;
        }
        
        // Retract phase
        elapsed = 0f;
        while (elapsed < retractDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / retractDuration;
            armCollider.transform.localPosition = Vector3.Lerp(armTargetPosition, armOriginalPosition, progress);
            yield return null;
        }
        
        // Ensure arm is back at original position
        armCollider.transform.localPosition = armOriginalPosition;
        armCollider.SetActive(false);
        
        Debug.Log("Player punch: Complete!");
        isPunching = false;
    }

    public override void SecondaryAction()
    {
        if (!isUnderControl || isPunching || isSpinning) return;
        
        // Check cooldown
        if (Time.time < lastPunchTime + punchCooldown)
        {
            Debug.Log($"Punch on cooldown! {punchCooldown - (Time.time - lastPunchTime):F1}s remaining");
            return;
        }
        
        Debug.Log("LongArm Secondary Action: Starting punch attack!");
        StartCoroutine(PlayerControlledPunch());
    }

    public override void SetControlled(bool underControl)
    {
        Debug.Log($"LongArmController SetControlled: underControl={underControl}");
        
        isUnderControl = underControl;
        
        // CRITICAL: Reset moveDir to prevent unwanted movement from previous state
        if (underControl)
        {
            moveDir = Vector2.zero;
        }

        if (enemyScript != null)
        {
            enemyScript.enabled = !underControl;
        }

        if (navAgent != null)
        {
            if (underControl)
            {
                // CRITICAL: Only call NavMeshAgent methods if it's enabled and on NavMesh
                if (navAgent.enabled && navAgent.isOnNavMesh)
                {
                    navAgent.ResetPath();
                    navAgent.velocity = Vector3.zero;
                    navAgent.isStopped = true;
                }
                
                // Now disable it to prevent interference with Rigidbody
                navAgent.enabled = false;
                
                Debug.Log($"NavMeshAgent disabled for player control");
            }
            else
            {
                // Re-enable NavMeshAgent when releasing control
                navAgent.enabled = true;
                
                // Warp to ensure on NavMesh (only if already on NavMesh)
                if (navAgent.isOnNavMesh)
                {
                    navAgent.Warp(transform.position);
                }
                else
                {
                    // Try to find nearest NavMesh position
                    UnityEngine.AI.NavMeshHit hit;
                    if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out hit, 5f, UnityEngine.AI.NavMesh.AllAreas))
                    {
                        navAgent.Warp(hit.position);
                    }
                }
                
                navAgent.isStopped = false;
                
                Debug.Log($"NavMeshAgent re-enabled after releasing control");
            }
        }

        if (_rigidbody != null)
        {
            _rigidbody.isKinematic = !underControl;
            
            Debug.Log($"Rigidbody isKinematic set to: {_rigidbody.isKinematic}");
            
            // Ensure proper rigidbody constraints for movement
            if (underControl)
            {
                // Clear any existing velocity when taking control
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
                
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | 
                                        RigidbodyConstraints.FreezeRotationZ | 
                                        RigidbodyConstraints.FreezePositionY;
                Debug.Log($"Rigidbody constraints set to: {_rigidbody.constraints}");
                
                // Ensure these critical physics settings are correct
                _rigidbody.useGravity = true;
                _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
            else
            {
                // When releasing control, make kinematic for NavMesh to work
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
        }
        else
        {
            Debug.LogError("LongArmController: _rigidbody is NULL! Cannot control movement!");
        }

        // Ensure arm colliders are disabled when taking control
        if (underControl && enemyScript != null)
        {
            if (enemyScript.leftArmCollider != null)
                enemyScript.leftArmCollider.SetActive(false);
            if (enemyScript.rightArmCollider != null)
            {
                enemyScript.rightArmCollider.SetActive(false);
                // Reset arm position to original
                enemyScript.rightArmCollider.transform.localPosition = Vector3.zero;
            }
                
            // Reset attack states
            isSpinning = false;
            isPunching = false;
        }
    }


}
