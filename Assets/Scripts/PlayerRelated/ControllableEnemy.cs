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

    protected Vector2 moveDir = new Vector2(0, 0);
    protected Rigidbody _rigidbody = null;
    public bool isUnderControl = false;

    public float health = 10f;

    public enum EnemyType
    {
        None,
        Runner,
        Tank
    }

    public EnemyType type = EnemyType.None;

    public virtual void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        //rigidbody.freezeRotation = true;
    }

    void FixedUpdate()
    {
        if (!isUnderControl) return;

        //Debug.Log("Enemy is under Control");

        if (_rigidbody.linearVelocity.magnitude < maxSpeed) _rigidbody.AddForce(new Vector3(moveDir.x, 0, moveDir.y) * moveAcceleration, ForceMode.Acceleration);
        else _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * maxSpeed;
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
        Debug.Log("Attempting Primary Enemy Action!");
    }

    public virtual void SecondaryAction()
    {
        Debug.Log("Attempting Secondary Enemy Action!");
    }

    public virtual void SetControlled(bool underControl)
    {
        isUnderControl = underControl;
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0)
            Die();
    }

    public void Die()
    {
        Destroy(gameObject);
    }

}
