using UnityEngine;
using UnityEngine.AI;

public class TankEnemy : EnemyBase
{
    public Transform[] patrolPoints;
    public float waitTime = 2f;

    private int currentPoint;
    private NavMeshAgent agent;
    private float waitTimer;

    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;

    public override void Start()
    {
        base.Start();

        agent = this.GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("TankEnemy missing NavMeshAgent, dumbass");
            enabled = false;
            return;
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError("TankEnemy has no patrol points, dumbass");
            enabled = false;
            return;
        }

        agent.speed = 2f;
        currentPoint = 0;
        agent.SetDestination(patrolPoints[currentPoint].position);
    }

    public override void Update()
    {
        base.Update();

        if (agent == null) agent = GetComponent<NavMeshAgent>();
        
        if (!agent.isOnNavMesh)
            return;

        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                waitTimer = 0f;
                currentPoint = UnityEngine.Random.Range(0, patrolPoints.Length);
                agent.SetDestination(patrolPoints[currentPoint].position);
            }
        }

        if (Time.time > nextAttackTime)
        {
            if (Time.time > nextAttackTime)
            {
                Shoot();
                nextAttackTime = Time.time + attackCooldown;
            }

            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Shoot()
    {
        if (player == null || projectilePrefab == null || firePoint == null)
            return;

        Vector3 dir = (player.position - firePoint.position).normalized;

        GameObject proj = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(dir)
        );

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = dir * projectileSpeed;
    }

}
