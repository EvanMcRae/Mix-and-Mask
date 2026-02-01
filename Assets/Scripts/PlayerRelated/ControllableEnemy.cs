using DG.Tweening.Core.Easing;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllableEnemy : MonoBehaviour
{
    [Header("All Controllable Enemies")]
    [SerializeField] public float moveAcceleration = 5f;
    [SerializeField] public float maxSpeed = 7f;
    [SerializeField] public Transform maskTransform;
    [SerializeField] protected float maxPrimaryCooldown = 5f;
    [SerializeField] protected float maxSecondaryCooldown = 5f;
    protected float primaryCooldown = 0;
    protected float secondaryCooldown = 0;
    public bool isSolid { get; protected set; }


    protected Vector2 moveDir = new Vector2(0, 0);
    protected Rigidbody _rigidbody = null;
    public bool isUnderControl = false;

    //Set this one to show the max number of hearts
    public float maxHealth = 10f;
    //This one is the one that is actually representing player health
    public float health = 10f;

    UpdateAbilitiesIcons abilityUI;

    public enum EnemyType
    {
        None,
        Runner,
        Tank,
        LongArm
    }

    public EnemyType type = EnemyType.None;

    public virtual void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        isSolid = true;
        //rigidbody.freezeRotation = true;
        //Needed for cooldowns
        abilityUI = FindFirstObjectByType<UpdateAbilitiesIcons>();
    }

    public virtual void FixedUpdate()
    {
        if (!isUnderControl) return;

        //Debug.Log("Enemy is under Control");

        // Calculate target velocity based on input
        Vector3 targetVelocity = new Vector3(moveDir.x, 0, moveDir.y) * maxSpeed;
        
        // Smoothly interpolate current velocity towards target velocity
        Vector3 currentVelocity = _rigidbody.linearVelocity;
        Vector3 velocityChange = targetVelocity - new Vector3(currentVelocity.x, 0, currentVelocity.z);
        
        // Apply acceleration with deltaTime consideration
        Vector3 newVelocity = currentVelocity + velocityChange * (moveAcceleration * Time.fixedDeltaTime);
        
        // Preserve Y velocity (gravity) and clamp horizontal velocity to maxSpeed
        Vector3 horizontalVelocity = new Vector3(newVelocity.x, 0, newVelocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
        }
        
        _rigidbody.linearVelocity = new Vector3(horizontalVelocity.x, currentVelocity.y, horizontalVelocity.z);
    }

    public virtual void Move(Vector2 moveDir)
    {
        this.moveDir = moveDir;
        //Debug.Log("Enemy moveDir: " + moveDir.x + ", " + moveDir.y);
    }

    public virtual void Rotate(float yRotation)
    {
        if (!isUnderControl) return;
        //Debug.Log("Enemy zRotation: " + zRotation);
        this.gameObject.transform.eulerAngles = new Vector3(0, yRotation, 0);
    }

    public virtual void PrimaryAction()
    {
        if (primaryCooldown <= 0)
        {
            Debug.Log("Call for cooldown!");
            abilityUI.CoolDownPrimary(maxPrimaryCooldown);
        }
        Debug.Log("Attempting Primary Enemy Action!");
    }

    public virtual void SecondaryAction()
    {
        if (secondaryCooldown <= 0)
        {
            Debug.Log("Call for cooldown!");
            abilityUI.CoolDownSecondary(maxSecondaryCooldown);
        }
        Debug.Log("Attempting Secondary Enemy Action!");
    }

    public virtual void SetControlled(bool underControl)
    {
        isUnderControl = underControl;
        UpdateHealthUI();
    }

    public virtual void TakeDamage(float dmg)
    {
        health -= dmg;
        UpdateHealthUI();
        if (health <= 0)
            Die();
    }

    public virtual void Die(){
        Destroy(gameObject);
    }
    private void UpdateHealthUI()
    {
        if (isUnderControl)
        {
            HealthUI healthUI = FindAnyObjectByType<HealthUI>();
            if (healthUI != null)
            {
                healthUI.UpdateHealth((int)health, (int)maxHealth);
            }
        }
    }

}
