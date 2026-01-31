using UnityEngine;
using UnityEngine.InputSystem;

public class AttachedMask : MonoBehaviour
{
    private Vector2 moveDir = new Vector2(0, 0);
    private ControllableEnemy controlledEnemy = null;
    private Rigidbody rigidbody = null;
    private PlayerInput actions = null;
    private DetatchedMask detatchedMask = null;
    private bool isControlling = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        actions = GetComponent<PlayerInput>();
        detatchedMask = GetComponent<DetatchedMask>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isControlling) return;

        if (controlledEnemy != null)
        {
            // Set the move direction of this enemy
            controlledEnemy.Move(moveDir);

            // Find the angle of the current mouse position relative to the center of the screen
            // And then set the controlled enemy's rotation to that
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector2 screenCenter = new Vector2(Screen.width/2, Screen.height/2);
            Vector2 centerToMouse = mousePos - screenCenter;
            float zRotation = Mathf.Atan2(centerToMouse.x, centerToMouse.y);
            zRotation *= Mathf.Rad2Deg;
            controlledEnemy.Rotate(zRotation);
        }

        this.transform.position = controlledEnemy.maskTransform.position;
        this.transform.rotation = controlledEnemy.maskTransform.rotation;
        rigidbody.linearVelocity = new Vector3(0, 0, 0);
    }

    public void SwtichToAttachedControls()
    {
        actions.SwitchCurrentActionMap("possession");
        isControlling = true;
    }

    public void SetControlledEnemy(ControllableEnemy toControl) => controlledEnemy = toControl;
    
     public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled) moveDir = context.ReadValue<Vector2>();
    }

    public void PrimaryAction(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (controlledEnemy != null) controlledEnemy.PrimaryAction();
    }

    public void SecondaryAction(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (controlledEnemy != null) controlledEnemy.SecondaryAction();
    }

    public void Detatch(InputAction.CallbackContext context)
    {
        isControlling = false;
        detatchedMask.SwitchToDetachedMovement();
        controlledEnemy.SetControlled(false);
        this.transform.position = new Vector3(this.transform.position.x + 1, this.transform.position.y, this.transform.position.z); // Plus 1 is to avoid immediate collisions
    }
}
