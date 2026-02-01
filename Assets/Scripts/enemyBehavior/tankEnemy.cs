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
    public GameObject acidPrefab;
    public Transform firePoint;
    public Transform acidPoint;
    public float projectileSpeed = 10f;
    [SerializeField] public float maxAcidCooldown = 10f;
    private float acidCooldown = 0f;

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

        if (!canMove) {
            agent.enabled = false;
            return;
        }
        else {
            agent.enabled = true;
        }
        
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

        if (acidCooldown > 0) acidCooldown -= Time.deltaTime;
    }

    void Shoot()
    {
        if (player == null || projectilePrefab == null || firePoint == null)
            return;

        Vector3 dir = (player.position - firePoint.position).normalized;

        AkUnitySoundEngine.PostEvent("BlobCannon", gameObject);

        GameObject proj = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(dir)
        );

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = dir * projectileSpeed;
    }

    public override void TakeDamage(float dmg)
    {
        if (acidCooldown <= 0)
        {
            LeakAcid();
        }

        base.TakeDamage(dmg);
    }

    public void LeakAcid()
    {
        if (acidPrefab == null) return;

        Debug.Log("Leaking Acid!");

        GameObject acidSpawn = Instantiate(
            acidPrefab,
            acidPoint.position,
            acidPrefab.transform.rotation
        );

        acidCooldown = maxAcidCooldown;
    }

}
