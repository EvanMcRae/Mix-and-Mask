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
    
    [Header("Shoulder Bones")]
    public Transform shoulderL;
    public Transform shoulderR;

    private NavMeshAgent agent;
    private float stateTimer;

    // Track which objects we've hit this spin to prevent multiple hits
    private System.Collections.Generic.HashSet<GameObject> hitThisSpin = new System.Collections.Generic.HashSet<GameObject>();
    
    // Attack handlers
    public LongArmSpinAttack spinAttack;
    public LongArmPunchAttack punchAttack;

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

        // Initialize attack handlers
        spinAttack = new LongArmSpinAttack(this, model, player);
        spinAttack.SetShoulders(shoulderL, shoulderR);
        punchAttack = new LongArmPunchAttack(this, player);
        punchAttack.SetShoulders(shoulderL, shoulderR);

        // Disable arm colliders initially
        if (leftArmCollider != null)
            leftArmCollider.SetActive(false);
        if (rightArmCollider != null)
            rightArmCollider.SetActive(false);
    }

    public override void Update()
    {
        base.Update();
        
        // Update player references in attack handlers
        if (spinAttack != null)
            spinAttack.UpdatePlayer(player);
        if (punchAttack != null)
            punchAttack.UpdatePlayer(player);
            
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
                // Continue updating shoulder reset animation
                if (spinAttack != null)
                    spinAttack.UpdateShoulderReset();
                    
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
        stateTimer = spinDuration;
        state = State.Spin;
        spinAttack.Start(leftArmCollider, rightArmCollider, hitThisSpin);
    }

    void PerformSpin()
    {
        spinAttack.Perform(stateTimer, spinDuration, lungeDistance, spinRotationDegrees);
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            EndSpin();
        }
    }

    void EndSpin()
    {
        spinAttack.End(leftArmCollider, rightArmCollider, hitThisSpin);
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
            ControllableEnemy hitEnemy = other.GetComponent<ControllableEnemy>();
            
            // Don't hit yourself
            if (hitEnemy != null && hitEnemy != this.GetComponent<ControllableEnemy>())
            {
                hitThisSpin.Add(other.gameObject);
                hitEnemy.TakeDamage(damage);
                Debug.Log($"Enemy hit by long arm enemy, damage: {damage}");
            }
        }
    }

    void StartPunch()
    {
        agent.enabled = false;
        punchAttack.Start(leftArmCollider, rightArmCollider, punchExtendDistance, hitThisSpin);
        stateTimer = punchExtendDuration;
        state = State.PunchExtend;
    }

    void PerformPunchExtend()
    {
        punchAttack.PerformExtend(leftArmCollider, rightArmCollider, stateTimer, punchExtendDuration);
        punchAttack.UpdateShoulderRotationManually();
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            stateTimer = punchRetractDuration;
            state = State.PunchRetract;
        }
    }

    void PerformPunchRetract()
    {
        punchAttack.PerformRetract(leftArmCollider, rightArmCollider, stateTimer, punchRetractDuration);
        punchAttack.UpdateShoulderRotationManually();
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            EndPunch();
        }
    }

    void EndPunch()
    {
        punchAttack.End(leftArmCollider, rightArmCollider, hitThisSpin);
        state = State.Cooldown;
        stateTimer = cooldownTime;
        nextAttackTime = Time.time + attackCooldown;
        nextPunchTime = Time.time + punchCooldown;
    }
}
