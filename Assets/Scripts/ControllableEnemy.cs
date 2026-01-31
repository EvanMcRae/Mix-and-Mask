using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllableEnemy : MonoBehaviour
{
    [Header("All Controllable Enemies")]
    [SerializeField] public float moveAcceleration = 5f;
    [SerializeField] public float maxSpeed = 7f;

    protected Vector2 moveDir = new Vector2(0, 0);
    protected Rigidbody rigidbody = null;
    protected bool isUnderControl = false;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!isUnderControl) return;

        Debug.Log("Enemy is under Control");

        if (rigidbody.linearVelocity.magnitude < maxSpeed) rigidbody.AddForce(new Vector3(moveDir.x, 0, moveDir.y) * moveAcceleration, ForceMode.Acceleration);
        else rigidbody.linearVelocity = rigidbody.linearVelocity.normalized * maxSpeed;
    }

    public virtual void Move(Vector2 moveDir)
    {
        this.moveDir = moveDir;
        Debug.Log("Enemy moveDir: " + moveDir.x + ", " + moveDir.y);
    }

    public virtual void Rotate(float zRotation)
    {
        if (!isUnderControl) return;
        Debug.Log("Enemy zRotation: " + zRotation);
        this.gameObject.transform.eulerAngles = new Vector3(0, zRotation, 0);
    }

    public virtual void PrimaryAction()
    {
        Debug.Log("Attempting Primary Enemy Action!");
    }

    public virtual void SecondaryAction()
    {
        Debug.Log("Attempting Secondary Enemy Action!");
    }

    public void SetControlled(bool underControl)
    {
        isUnderControl = underControl;
    }
}
