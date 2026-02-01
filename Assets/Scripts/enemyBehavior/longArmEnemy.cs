using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class LongArmEnemy : EnemyBase
{
    [Header("Movement")]
    public float orbitRadius = 6f;

    [Header("Attack")]
    public float windupTime = 0.8f;
    public float lungeDistance = 3f;
    public float spinDuration = 1.0f;
    public float spinRotationDegrees = 540f;
    public float cooldownTime = 0.5f;
    public float damage = 2f;

    [Header("Punch Attack")]
    public float punchWindupTime = 0.3f;
    public float punchExtendDistance = 5f;
    public float punchExtendDuration = 0.2f;
    public float punchRetractDuration = 0.3f;
    public float punchCooldown = 2f;
    public float punchDamage = 1.5f;
    private float nextPunchTime = 0f;

    [Header("Arm Colliders")]
    public GameObject leftArmCollider;
    public GameObject rightArmCollider;

    [Header("Visual")]
    public Transform model;
    public float turnSpeed = 10f;

    private NavMeshAgent agent;
    private float stateTimer;
    private Vector3 lungeDirection;
    private Vector3 lungeStartPosition;
    private float spinRotationProgress;
    private float spinStartRotation;

    // Track which objects we've hit this spin to prevent multiple hits
    private System.Collections.Generic.HashSet<GameObject> hitThisSpin = new System.Collections.Generic.HashSet<GameObject>();
    
    // Punch attack state
    private Vector3 armOriginalPosition;
    private Vector3 armTargetPosition;
    private Vector3 punchDirection;

    public enum State
    {
        Idle,
        Move,
        Windup,
        Spin,
        Cooldown,
        PunchWindup,
        PunchExtend,
        PunchRetract
    }

    public State state;

    public override void Start()
    {
        base.Start();

        agent = GetComponent<NavMeshAgent>();
        agent.speed = 5f;
        agent.acceleration = 15f;
        agent.autoBraking = false;
        agent.updateRotation = false;

        state = State.Move;

        // Disable arm colliders initially
        if (leftArmCollider != null)
            leftArmCollider.SetActive(false);
        if (rightArmCollider != null)
            rightArmCollider.SetActive(false);
    }

    public override void Update()
    {
        base.Update();
        UpdateState();
        Act();
    }

    void LateUpdate()
    {
        // Only rotate model to face player when in Move or Windup state
        if ((state == State.Move || state == State.Windup) && player != null && model != null)
        {
            Vector3 dir = player.position - model.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                model.rotation = Quaternion.Slerp(
                    model.rotation,
                    targetRot,
                    turnSpeed * Time.deltaTime
                );
            }
        }
    }

    public void UpdateState()
    {
        if (player == null || !agent.isOnNavMesh)
            return;

        float dist = Vector3.Distance(transform.position, player.position);

        // Transition from Move to attack when in range
        if (state == State.Move && dist < attackRange && Time.time > nextAttackTime)
        {
            // Randomly choose between spin and punch attack
            if (Time.time > nextPunchTime && Random.value > 0.5f)
            {
                // Punch attack
                state = State.PunchWindup;
                stateTimer = punchWindupTime;
            }
            else
            {
                // Spin attack
                state = State.Windup;
                stateTimer = windupTime;
            }
            
            // Only set destination if agent is enabled
            if (agent.enabled && agent.isOnNavMesh)
                agent.SetDestination(transform.position); // Stop moving
        }
    }

    public void Act()
    {
        if (player == null)
            return;

        switch (state)
        {
            case State.Move:
                OrbitPlayer();
                break;

            case State.Windup:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    StartSpin();
                }
                break;

            case State.Spin:
                PerformSpin();
                break;

            case State.Cooldown:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    state = State.Move;
                    
                    // Re-enable agent and warp to current position to ensure it's on NavMesh
                    agent.enabled = true;
                    if (agent.isOnNavMesh)
                    {
                        agent.Warp(transform.position);
                    }
                    else
                    {
                        // If not on NavMesh, try to find nearest valid position
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
                        {
                            agent.Warp(hit.position);
                        }
                        else
                        {
                            Debug.LogWarning("LongArmEnemy could not find valid NavMesh position after spin!");
                        }
                    }
                }
                break;

            case State.PunchWindup:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    StartPunch();
                }
                break;

            case State.PunchExtend:
                PerformPunchExtend();
                break;

            case State.PunchRetract:
                PerformPunchRetract();
                break;
        }
    }

    void OrbitPlayer()
    {
        // Ensure agent is active and on NavMesh
        if (!agent.enabled || !agent.isOnNavMesh)
            return;

        Vector3 dir = transform.position - player.position;
        if (dir.sqrMagnitude < 0.1f)
            dir = Random.insideUnitSphere;

        dir = dir.normalized;
        Vector3 tangent = Vector3.Cross(dir, Vector3.up);

        Vector3 target = player.position + tangent * orbitRadius;
        agent.SetDestination(target);
    }

    void StartSpin()
    {
        agent.enabled = false;

        // Calculate lunge direction toward player
        lungeDirection = (player.position - transform.position).normalized;
        lungeDirection.y = 0f;

        lungeStartPosition = transform.position;
        spinRotationProgress = 0f;
        spinStartRotation = model.eulerAngles.y;
        stateTimer = spinDuration;
        state = State.Spin;

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

    void PerformSpin()
    {
        // Move forward
        float lungeProgress = 1f - (stateTimer / spinDuration);
        Vector3 targetPosition = lungeStartPosition + lungeDirection * lungeDistance;
        transform.position = Vector3.Lerp(lungeStartPosition, targetPosition, lungeProgress);

        // Rotate the entire enemy (and thus the arm colliders)
        spinRotationProgress = 1f - (stateTimer / spinDuration);
        float currentRotation = spinStartRotation + (spinRotationDegrees * spinRotationProgress);
        model.rotation = Quaternion.Euler(0f, currentRotation, 0f);

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            EndSpin();
        }
    }

    void EndSpin()
    {
        // Disable arm colliders
        if (leftArmCollider != null)
            leftArmCollider.SetActive(false);
        if (rightArmCollider != null)
            rightArmCollider.SetActive(false);

        // Clear hit tracking
        hitThisSpin.Clear();

        // Transition to cooldown
        state = State.Cooldown;
        stateTimer = cooldownTime;
        nextAttackTime = Time.time + attackCooldown;
    }

    // This should be attached to the arm colliders as a trigger handler
    public void OnArmColliderHit(Collider other)
    {
        Debug.Log($"OnArmColliderHit called! State: {state}, Collider: {other.name}, Tag: {other.tag}");
        
        if (state != State.Spin)
        {
            Debug.Log("Not in Spin state, ignoring collision");
            return;
        }

        // Prevent hitting the same target multiple times in one spin
        if (hitThisSpin.Contains(other.gameObject))
        {
            Debug.Log($"Already hit {other.name} this spin, ignoring");
            return;
        }

        if (other.CompareTag("Player"))
        {
            // Mark as hit
            hitThisSpin.Add(other.gameObject);

            // Try to damage controlled enemy first
            ControllableEnemy controlledEnemy = other.GetComponent<ControllableEnemy>();
            if (controlledEnemy != null && controlledEnemy.isUnderControl)
            {
                Debug.Log($"Player get hit by long arm enemy (possessed enemy, health before: {controlledEnemy.health}, damage: {damage})");
                controlledEnemy.TakeDamage(damage);
                Debug.Log($"Player health after hit: {controlledEnemy.health}");
            }
            else
            {
                // Fallback to mask player
                playerHealth playerAsMask = other.GetComponent<playerHealth>();
                if (playerAsMask != null)
                {
                    Debug.Log($"Player get hit by long arm enemy (mask form, health before: {playerAsMask.currPlayerHealth}, damage: {damage})");
                    playerAsMask.playerTakeDamage(damage);
                    Debug.Log($"Player health after hit: {playerAsMask.currPlayerHealth}");
                }
                else
                {
                    Debug.LogWarning("LongArm hit Player tag but found no health component!");
                }
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            // Can hit other possessed enemies
            ControllableEnemy controlledEnemy = other.GetComponent<ControllableEnemy>();
            if (controlledEnemy != null && controlledEnemy.isUnderControl)
            {
                hitThisSpin.Add(other.gameObject);
                Debug.Log($"Player-controlled enemy hit by long arm enemy (health before: {controlledEnemy.health}, damage: {damage})");
                controlledEnemy.TakeDamage(damage);
                Debug.Log($"Enemy health after hit: {controlledEnemy.health}");
            }
        }
    }

    void StartPunch()
    {
        agent.enabled = false;

        // Calculate punch direction toward player
        punchDirection = (player.position - transform.position).normalized;
        punchDirection.y = 0f;

        // Use right arm for punch
        if (rightArmCollider != null)
        {
            armOriginalPosition = rightArmCollider.transform.localPosition;
            armTargetPosition = armOriginalPosition + rightArmCollider.transform.InverseTransformDirection(punchDirection * punchExtendDistance);
            rightArmCollider.SetActive(true);
            Debug.Log("Right arm punch started!");
        }

        hitThisSpin.Clear();
        stateTimer = punchExtendDuration;
        state = State.PunchExtend;
    }

    void PerformPunchExtend()
    {
        if (rightArmCollider != null)
        {
            float progress = 1f - (stateTimer / punchExtendDuration);
            rightArmCollider.transform.localPosition = Vector3.Lerp(armOriginalPosition, armTargetPosition, progress);
        }

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            stateTimer = punchRetractDuration;
            state = State.PunchRetract;
        }
    }

    void PerformPunchRetract()
    {
        if (rightArmCollider != null)
        {
            float progress = 1f - (stateTimer / punchRetractDuration);
            rightArmCollider.transform.localPosition = Vector3.Lerp(armTargetPosition, armOriginalPosition, progress);
        }

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            EndPunch();
        }
    }

    void EndPunch()
    {
        // Disable arm collider
        if (rightArmCollider != null)
        {
            rightArmCollider.SetActive(false);
            rightArmCollider.transform.localPosition = armOriginalPosition;
        }

        // Clear hit tracking
        hitThisSpin.Clear();

        // Transition to cooldown
        state = State.Cooldown;
        stateTimer = cooldownTime;
        nextAttackTime = Time.time + attackCooldown;
        nextPunchTime = Time.time + punchCooldown;
    }
}
