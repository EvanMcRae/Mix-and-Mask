using UnityEngine;

public class TankController : ControllableEnemy
{
    TankEnemy tankEnemy = null;
    private UnityEngine.AI.NavMeshAgent navAgent = null;

    [Header("Tank Specific")]
    [SerializeField] private GameObject tankModel = null;

    public override void Start()
    {
        base.Start();
        tankEnemy = GetComponent<TankEnemy>();
        navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        type = ControllableEnemy.EnemyType.Tank;
    }

    public override void FixedUpdate()
    {
        if (!isUnderControl) return;

        if (secondaryCooldown > 0) secondaryCooldown -= Time.deltaTime;

        if (_rigidbody.linearVelocity.magnitude < maxSpeed) _rigidbody.AddForce(new Vector3(moveDir.x, 0, moveDir.y) * moveAcceleration, ForceMode.Acceleration);
        else _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * maxSpeed;
    }

    public override void Rotate(float yRotation)
    {
        base.Rotate(yRotation);
        tankModel.transform.localEulerAngles = new Vector3(0, -90, 0);
    }

    public override void PrimaryAction()
    {
        Debug.Log("Player shooting!");
        Shoot();
    }

    public override void SecondaryAction()
    {
        if (secondaryCooldown > 0) return; 
        Debug.Log("Player acid!");
        LeakAcid();
    }

    public override void SetControlled(bool underControl)
    {
        tankEnemy.enabled = !underControl;
        navAgent.enabled = !underControl;
        _rigidbody.isKinematic = !underControl;
        base.SetControlled(underControl);
    }

    void Shoot()
    {
        if (tankEnemy.projectilePrefab == null || tankEnemy.firePoint == null)
            return;

        Vector3 dir = this.transform.forward;

        GameObject proj = Instantiate(
            tankEnemy.projectilePrefab,
            tankEnemy.firePoint.position,
            Quaternion.LookRotation(dir)
        );

        Projectile projectile = proj.GetComponent<Projectile>();
        projectile.belongsToPlayer = true;

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = dir * tankEnemy.projectileSpeed;
    }

    void LeakAcid()
    {
        if (tankEnemy.acidPrefab == null) return;

        GameObject acidSpawn = Instantiate(
            tankEnemy.acidPrefab,
            tankEnemy.acidPoint.position,
            tankEnemy.acidPrefab.transform.rotation
        );

        Acid acid = acidSpawn.GetComponent<Acid>();
        acid.belongsToPlayer = true;

        secondaryCooldown = maxSecondaryCooldown;
    }
}
