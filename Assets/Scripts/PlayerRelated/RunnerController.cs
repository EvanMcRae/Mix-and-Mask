using UnityEngine;

public class RunnerController : ControllableEnemy
{
    RunnerEnemy runnerEnemy = null;
    private float dashSpeed = 0;
    private float dashDuration = 0;
    private float dashTimer = 0;
    private bool isDashing = false;
    private float dashCooldown = 0;
    private UnityEngine.AI.NavMeshAgent navAgent = null;

    [Header("Cat Model Specific")]
    [SerializeField] private float maxDashCooldown = 5f;
    [SerializeField] private GameObject catModel = null;

    public override void Start()
    {
        base.Start();
        runnerEnemy = GetComponent<RunnerEnemy>();
        navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        dashSpeed = runnerEnemy.dashSpeed;
        dashDuration = runnerEnemy.dashDuration;
        type = ControllableEnemy.EnemyType.Runner;
    }

    void FixedUpdate()
    {
        if (!isUnderControl) return;

        Debug.Log("Enemy is under Control");

        if (isDashing)
        {
            transform.position += this.transform.forward * dashSpeed * Time.deltaTime;
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0) isDashing = false;
            return;
        }

        if (dashCooldown > 0) dashCooldown -= Time.deltaTime;

        if (rigidbody.linearVelocity.magnitude < maxSpeed) rigidbody.AddForce(new Vector3(moveDir.x, 0, moveDir.y) * moveAcceleration, ForceMode.Acceleration);
        else rigidbody.linearVelocity = rigidbody.linearVelocity.normalized * maxSpeed;
    }

    public override void Rotate(float yRotation)
    {
        if (isDashing) return;
        base.Rotate(yRotation);
        catModel.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    public override void PrimaryAction()
    {
        if (isDashing || dashCooldown > 0) return;
        Debug.Log("Attempting Dash!");

        isDashing = true;
        dashTimer = dashDuration;
        dashCooldown = maxDashCooldown;
    }

    public override void SecondaryAction()
    {
        Debug.Log("Attempting Secondary Runner Enemy Action!");
    }

    public override void SetControlled(bool underControl)
    {
        runnerEnemy.enabled = !underControl;
        navAgent.enabled = !underControl;
        rigidbody.isKinematic = !underControl;
        base.SetControlled(underControl);
    }
}
