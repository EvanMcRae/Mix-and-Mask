using UnityEngine;
using UnityEngine.AI;

public class RunnerEnemy : EnemyBase
{
    public float orbitRadius = 4f;
    [SerializeField] Renderer _renderer = null;

    [Header("Attack")]
    public float windupTime = 0.4f;
    public float backupDistance = 1.5f;
    public float dashSpeed = 12f;
    public float dashDuration = 0.4f;
    public float damage = 1f;
    public float maxInvulTime = 5f;
    public float maxInvulCooldown = 10f;
    private float invulCooldown = 5f;
    private float invulTime = 0f;
    [SerializeField] Collider _collider = null;


    private NavMeshAgent agent;
    private float stateTimer;
    private Vector3 dashDirection;
    private bool facePlayer = false;

    public Transform model;//enemy model
    public float turnSpeed = 10f;

    public enum State
    {
        Idle,
        Move,
        Attack,
        Invulnerable,
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
        agent.updateRotation = true;

        invulCooldown = UnityEngine.Random.Range(maxInvulCooldown/2, maxInvulCooldown + 6); // Adds randomness so all cats on the map don't become invulnerable at once

        state = State.Move;

        agent.speed = UnityEngine.Random.Range(4, 8);
    }

    public override void Update()
    {
        base.Update();
        UpdateState();
        Act();
    }

    void LateUpdate() 
    {
        if (facePlayer == true && player != null)
        {
            // makes the model rotate to face the player\

            agent.updateRotation = false;

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
        else
        {
            agent.updateRotation = true;
        }
    }

    public void UpdateState()
    {
        if (player == null || !agent.isOnNavMesh)
            return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (invulCooldown > 0) invulCooldown -= Time.deltaTime;

        if (state == State.Move && invulCooldown <= 0) BecomeInvulnerable();

        if (state == State.Invulnerable)
        {
            if (invulTime <= 0)
            {
                BecomeVulnerable();
            }
        }

        if (state == State.Move && dist < attackRange && Time.time > nextAttackTime)
        {
            state = State.Attack;
            stateTimer = windupTime;

            //back up slightly (the tell)
            facePlayer = true;
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

            case State.Invulnerable:
                InvulnerabilityChecks();
                OrbitPlayer();
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
            facePlayer = false;
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
        else if (state == State.Dash && collision.gameObject.CompareTag("Enemy"))
        {
            ControllableEnemy player = collision.gameObject.GetComponent<ControllableEnemy>();
            if (player != null && player.isUnderControl)
            {
                Debug.Log("possessed was hit by runner enemy");
                player.TakeDamage(damage);
            }
        }
    }

    public override void TakeDamage(float dmg)
    {
        if (state == State.Invulnerable) return;
        base.TakeDamage(dmg);
    }

    private void BecomeInvulnerable()
    {
        invulCooldown = maxInvulCooldown;
        invulTime = maxInvulTime;

        Color color = _renderer.material.color;
        color.a = 0.3f;
        _renderer.material.SetColor("_BaseColor", new Color(color.r, color.g, color.b, color.a));
        isSolid = false;
        state = State.Invulnerable;
        _collider.isTrigger = true;
    }

    private void BecomeVulnerable()
    {
        invulCooldown = maxInvulCooldown + UnityEngine.Random.Range(0, 6); // Adds randomness so all cats on the map don't become invulnerable at once
        Color color = _renderer.material.color;
        color.a = 1f;
        _renderer.material.SetColor("_BaseColor", new Color(color.r, color.g, color.b, color.a));
        isSolid = true;
        state = State.Move;
        _collider.isTrigger = false;
    }

    private void InvulnerabilityChecks()
    {
        if (invulTime > 0) invulTime -= Time.deltaTime;
    }
}
