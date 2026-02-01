using UnityEngine;
using UnityEngine.AI;

public class RunnerEnemy : EnemyBase
{
    public float orbitRadius = 4f;

    [Header("Attack")]
    public float windupTime = 0.4f;
    public float backupDistance = 1.5f;
    public float dashSpeed = 12f;
    public float dashDuration = 0.4f;
    public float damage = 1f;
    public float maxInvulTime = 2f;
    public float maxInvulCooldown = 10f;


    private NavMeshAgent agent;
    private float stateTimer;
    private Vector3 dashDirection;

    public Transform model;//enemy model
    public float turnSpeed = 10f;

    public enum State
    {
        Idle,
        Move,
        Attack,
        Dash
    }

    public State state;

    public override void Start()
    {
        base.Start();

        agent = GetComponent<NavMeshAgent>();
        agent.speed = 6f;
        agent.acceleration = 20f;
        agent.autoBraking = false;
        agent.updateRotation = false;

        state = State.Move;

        agent.speed = UnityEngine.Random.Range(4, 8);
    }

    public override void Update()
    {
        base.Update();
        UpdateState();
        Act();
    }

    void LateUpdate() // makes the placeholder model roatate to face the player
    {
        if (player == null) return;

        Vector3 dir = player.position - model.position;
        dir.y = 0f;//top-down, no tilting

        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        model.rotation = Quaternion.Slerp(
            model.rotation,
            targetRot,
            turnSpeed * Time.deltaTime
        );
    }

    public void UpdateState()
    {
        if (player == null || !agent.isOnNavMesh)
            return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (state == State.Move && dist < attackRange && Time.time > nextAttackTime)
        {
            state = State.Attack;
            stateTimer = windupTime;

            //back up slightly (the tell)
            Vector3 away = (transform.position - player.position).normalized;
            Vector3 backupPos = transform.position + away * backupDistance;
            agent.SetDestination(backupPos);
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

            case State.Attack:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    StartDash();
                }
                break;

            case State.Dash:
                DashForward();
                break;
        }
    }

    void OrbitPlayer()
    {
        Vector3 dir = transform.position - player.position;
        if (dir.sqrMagnitude < 0.1f)
            dir = Random.insideUnitSphere;

        dir = dir.normalized;
        Vector3 tangent = Vector3.Cross(dir, Vector3.up);

        Vector3 target = player.position + tangent * orbitRadius;
        agent.SetDestination(target);
    }

    void StartDash()
    {
        agent.enabled = false;

        dashDirection = (player.position - transform.position).normalized;
        dashDirection.y = 0f;

        stateTimer = dashDuration;
        state = State.Dash;
    }

    void DashForward()
    {
        transform.position += dashDirection * dashSpeed * Time.deltaTime;

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            agent.enabled = true;
            nextAttackTime = Time.time + attackCooldown;
            state = State.Move;
        }
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    if (state == State.Dash && other.CompareTag("Player"))
    //    {
    //        Debug.Log("Runner slams the player like a fucking missile");

    //        // later:
    //        ControllableEnemy player = other.GetComponent<ControllableEnemy>();
    //        if (player != null)
    //        {
    //            player.TakeDamage(damage);
    //        }
    //        else
    //        {
    //            playerHealth playerAsMask = other.GetComponent<playerHealth>();
    //            playerAsMask.playerTakeDamage(5);
    //        }

    //            nextAttackTime = Time.time + attackCooldown;
    //        agent.enabled = true;
    //        state = State.Move;
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if (state == State.Dash && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Runner slams the player like a fucking missile");

            // later:
            ControllableEnemy player = collision.gameObject.GetComponent<ControllableEnemy>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            else
            {
                playerHealth playerAsMask = collision.gameObject.GetComponent<playerHealth>();
                if (playerAsMask != null)
                {
                    playerAsMask.playerTakeDamage(5);
                }
                else
                {
                    Debug.LogWarning("Runner hit the Player (Mask) but no 'playerHealth' script was found on it!");
                }
            }

            nextAttackTime = Time.time + attackCooldown;
            agent.enabled = true;
            state = State.Move;
        }
        else if(state == State.Dash && collision.gameObject.CompareTag("Enemy"))
        {
            ControllableEnemy player = collision.gameObject.GetComponent<ControllableEnemy>();
            if (player != null && player.isUnderControl)
            {
                Debug.Log("possessed was hit by runner enemy");
                player.TakeDamage(damage);
            }
        }
    }
}
