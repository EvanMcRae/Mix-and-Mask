using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DetatchedMask : MonoBehaviour
{
    private Vector2 initialClickPoint = new Vector2(0, 0);
    private Vector2 finalClickPoint = new Vector2(0, 0);
    private Rigidbody rigidbody = null;
    private PlayerInput actions = null;
    private AttachedMask attachedMask = null; // The attached compliment to this script on the player entity
    private BoxCollider collider = null;

    [SerializeField] float maxRealSlingLength = 20f;
    [SerializeField] float maxSlingVelocity = 5f;
    [SerializeField] float maxMovementCooldown = 1.5f;
    [SerializeField] float minVelocityToTakeOver = 2.5f;
    [SerializeField] float maxAttachedCooldown = 1f;
    [SerializeField] RectTransform arrow = null;
    private float movementCooldown = 0;
    private float attachedCooldown = 0;
    private bool isControlling = false;
    private ControllableEnemy.EnemyType controlledEnemyType = ControllableEnemy.EnemyType.None;
    private bool isDrawing = false; // Checks if the player is actively preparing to launch the mask

    public UnityEvent onAttach;
    public UnityEvent onDetach;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        actions = GetComponent<PlayerInput>();
        attachedMask = GetComponent<AttachedMask>();
        collider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movementCooldown > 0) movementCooldown -= Time.deltaTime;
        if (attachedCooldown > 0 && !isControlling) attachedCooldown -= Time.deltaTime;

        if (isDrawing)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            Vector2 localMousePos;
            Vector2 initialClickPointCanvasSpace;

            RectTransform parentRect = arrow.parent as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                currentMousePos,
                null,
                out localMousePos
            );

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                initialClickPoint,
                null,
                out initialClickPointCanvasSpace
            );

            Vector2 arrowDir = localMousePos - initialClickPointCanvasSpace;

            float angle = Mathf.Atan2(arrowDir.y, arrowDir.x) * Mathf.Rad2Deg;

            arrow.anchoredPosition = initialClickPointCanvasSpace;
            arrow.localRotation = Quaternion.Euler(0, 0, angle);
            arrow.sizeDelta = new Vector2(arrowDir.magnitude, 3f);
        }
    }

    // Grabs the position of the mouse when the slingshot button is initially pressed and when it is released
    // Then uses the difference between the positions to create a launch vector for the mask and flings it in that direction
    public void Slingshot(InputAction.CallbackContext context)
    {
        // When first pressed, get mouse position
        if (context.started)
        {
            //Debug.Log("Sling Started");
            initialClickPoint = Mouse.current.position.ReadValue();
            //Debug.Log("Position: " + initialClickPoint.x + ", " + initialClickPoint.y);
            arrow.gameObject.SetActive(true);
            isDrawing = true;
        }

        // When released, get mouse position again and launch mask
        if (context.canceled)
        {
            if (movementCooldown > 0)
            {
                arrow.gameObject.SetActive(false);
                isDrawing = false;
                return;
            }

            //Debug.Log("Sling released");
            finalClickPoint = Mouse.current.position.ReadValue();
            Vector2 slingDirection = initialClickPoint - finalClickPoint;
            // Debug.Log("Sling Direction: " + slingDirection.x + ", " + slingDirection.y);


            float slingLength = slingDirection.magnitude;
            //Debug.Log("Sling Length: " + slingLength);
            float slingVelocity = (slingLength / maxRealSlingLength) * maxSlingVelocity; // Uses percentage of real screenspace vector to calculate force vector length
            if (slingVelocity > maxSlingVelocity) slingVelocity = maxSlingVelocity; // Caps force magnitude
            slingDirection = slingDirection.normalized;
            rigidbody.AddForce(new Vector3(slingDirection.x, 0.5f, slingDirection.y) * slingVelocity, ForceMode.Impulse);

            movementCooldown = maxMovementCooldown;
            arrow.gameObject.SetActive(false);
            isDrawing = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (rigidbody.linearVelocity.magnitude < minVelocityToTakeOver && !isControlling) return;

        if (collision.gameObject.TryGetComponent<ControllableEnemy>(out ControllableEnemy enemyScript))
        {
            if (attachedCooldown > 0) return;
            Debug.Log("Hit a controllable enemy!");
            BeginEnemyControl(enemyScript);
        }
    }

    // Called when a player starts possessing
    private void BeginEnemyControl(ControllableEnemy enemyScript)
    {
        attachedMask.SetControlledEnemy(enemyScript);
        attachedMask.SwtichToAttachedControls();
        collider.enabled = false;
        isControlling = true;
        enemyScript.SetControlled(true);

        rigidbody.detectCollisions = false;
        //rigidbody.freezeRotation = true;

        attachedCooldown = maxAttachedCooldown;
        controlledEnemyType = enemyScript.type;
        
        onAttach.Invoke();
    }

     // Called when a player stops possessing
    public void SwitchToDetachedMovement()
    {
        collider.enabled = true;
        actions.SwitchCurrentActionMap("detatched");
        rigidbody.detectCollisions = true;
        //rigidbody.freezeRotation = false;
        isControlling = false;
        controlledEnemyType = ControllableEnemy.EnemyType.None;
        
        onDetach.Invoke();
    }
    
    public ControllableEnemy.EnemyType GetCurrentlyControlledEnemyType() { return controlledEnemyType; }
}
