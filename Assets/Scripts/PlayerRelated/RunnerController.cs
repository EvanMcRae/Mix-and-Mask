using UnityEngine;

public class RunnerController : ControllableEnemy
{
    RunnerEnemy runnerEnemy = null;
    private float dashSpeed = 0;
    private float dashDuration = 0;
    private float dashTimer = 0;
    private bool isDashing = false;
    private float maxInvulTime = 2f;
    private float invulTime = 0;
    private bool isInvulnerable = false;
    private UnityEngine.AI.NavMeshAgent navAgent = null;

    [Header("Cat Model Specific")]
    [SerializeField] private float maxDashCooldown = 5f;
    [SerializeField] private GameObject catModel = null;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Collider _collider;

    public override void Start()
    {
        base.Start();
        runnerEnemy = GetComponent<RunnerEnemy>();
        navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        dashSpeed = runnerEnemy.dashSpeed;
        dashDuration = runnerEnemy.dashDuration;
        maxInvulTime = runnerEnemy.maxInvulTime;
        maxSecondaryCooldown = runnerEnemy.maxInvulCooldown;
        type = ControllableEnemy.EnemyType.Runner;
    }

    public override void FixedUpdate()
    {
        if (!isUnderControl) return;

        //Debug.Log("Enemy is under Control");

        if (isDashing)
        {
            transform.position += this.transform.forward * dashSpeed * Time.deltaTime;
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0) isDashing = false;
            return;
        }

        if (isInvulnerable)
        {
            invulTime -= Time.deltaTime;
            if (invulTime <= 0)
            {
                isInvulnerable = false;
                _collider.isTrigger = false;

                _renderer.material.SetFloat("_MaxAlpha", 1f);
                isSolid = true;
            }
        }


        if (secondaryCooldown > 0) secondaryCooldown -= Time.deltaTime;
        if (primaryCooldown > 0) primaryCooldown -= Time.deltaTime;

        if (_rigidbody.linearVelocity.magnitude < maxSpeed) _rigidbody.AddForce(new Vector3(moveDir.x, 0, moveDir.y) * moveAcceleration, ForceMode.Acceleration);
        else _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * maxSpeed;
    }

    public override void Rotate(float yRotation)
    {
        if (isDashing) return;
        base.Rotate(yRotation);
        catModel.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    public override void PrimaryAction()
    {
        if (isDashing || primaryCooldown > 0) return;
        Debug.Log("Attempting Dash!");

        isDashing = true;
        dashTimer = dashDuration;
        primaryCooldown = maxPrimaryCooldown;
    }

    public override void SecondaryAction()
    {
        if (isInvulnerable || invulTime > 0 || secondaryCooldown > 0) return;
        Debug.Log("Temporary Invulnerability!");
        BecomeInvulnerable();
    }

    private void BecomeInvulnerable()
    {
        isInvulnerable = true;
        secondaryCooldown = maxSecondaryCooldown;
        invulTime = maxInvulTime;

        _collider.isTrigger = true;

        _renderer.material.SetFloat("_MaxAlpha", 0.3f);
        isSolid = false;
    }

    public override void SetControlled(bool underControl)
    {
        runnerEnemy.enabled = !underControl;
        navAgent.enabled = !underControl;
        _rigidbody.isKinematic = !underControl;
        _rigidbody.useGravity = !underControl;
        if(underControl) _rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
        else _rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
        base.SetControlled(underControl);
    }

    public override void TakeDamage(float dmg)
    {
        if (isInvulnerable) return;
        Debug.Log("Runner Took Damage!");
        base.TakeDamage(dmg);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDashing && collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Runner slams the player like a fucking missile");

            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(5f);
            }
        }
    }

}
