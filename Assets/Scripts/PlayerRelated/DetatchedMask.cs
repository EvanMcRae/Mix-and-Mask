using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DetatchedMask : MonoBehaviour
{
    private Vector2 initialClickPoint = new Vector2(0, 0);
    private Vector2 finalClickPoint = new Vector2(0, 0);
    private Rigidbody _rigidbody = null;
    private PlayerInput actions = null;
    private AttachedMask attachedMask = null; // The attached compliment to this script on the player entity
    private BoxCollider _collider = null;
    public Camera mainCamera;

    [SerializeField] float maxRealSlingLength = 20f;
    [SerializeField] float maxSlingVelocity = 12f;
    [SerializeField] float slingStrengthScalar = 3f;
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
    public UnityEvent<Vector3> onFling; //Passes in direction.
    private playerHealth healthScript = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        actions = GetComponent<PlayerInput>();
        attachedMask = GetComponent<AttachedMask>();
        _collider = GetComponent<BoxCollider>();
        healthScript = GetComponent<playerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movementCooldown > 0) movementCooldown -= Time.deltaTime;
        if (attachedCooldown > 0 && !isControlling) attachedCooldown -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (isDrawing && Time.timeScale != 0)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            Vector2 localMousePos;
            Vector2 maskPosScreenSpace;
            Vector2 maskPosCanvasSpace;

            maskPosScreenSpace = mainCamera.WorldToScreenPoint(transform.position);

            RectTransform parentRect = arrow.parent as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                currentMousePos,
                null,
                out localMousePos
            );

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                maskPosScreenSpace,
                null,
                out maskPosCanvasSpace
            );

            Vector2 arrowDir = localMousePos - maskPosCanvasSpace;

            float angle = Mathf.Atan2(arrowDir.y, arrowDir.x) * Mathf.Rad2Deg;

            float arrowLength = (arrowDir.magnitude / maxRealSlingLength) * maxRealSlingLength;
            if (arrowLength > maxRealSlingLength) arrowLength = maxRealSlingLength;

            arrow.anchoredPosition = maskPosCanvasSpace;
            arrow.localRotation = Quaternion.Euler(0, 0, angle);
            arrow.sizeDelta = new Vector2(arrowLength, 3f);
        }
    }

    public Vector3 GetPositionAtHeight(Vector3 startPos, Vector3 worldDir, float targetHeight)
    {
        if (Mathf.Approximately(worldDir.y, 0f))
        {
            return Vector3.zero;
        }
        
        float t = (targetHeight - startPos.y) / worldDir.y;
        
        return startPos + (worldDir * t);
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


            Vector3 wsFinalClickPoint = transform.position;
            Vector3 rayDir = Vector3.Normalize(mainCamera.ScreenToWorldPoint(new Vector3(finalClickPoint.x, finalClickPoint.y, 1)) -
                             mainCamera.transform.position);
            Vector3 wsInitialClickPoint = GetPositionAtHeight(mainCamera.transform.position, rayDir, transform.position.y);
            
            Vector3 wsSlingDelta = wsFinalClickPoint - wsInitialClickPoint;
            Vector3 wsSlingDirection = Vector3.Normalize(wsSlingDelta);
            float wsSlingLength = wsSlingDelta.magnitude * slingStrengthScalar;
            
            if (wsSlingLength > maxSlingVelocity) wsSlingLength = maxSlingVelocity; // Caps force magnitude
            _rigidbody.AddForce(new Vector3(wsSlingDirection.x, 0.5f, wsSlingDirection.z) * wsSlingLength, ForceMode.Impulse);

            movementCooldown = maxMovementCooldown;
            arrow.gameObject.SetActive(false);
            isDrawing = false;
            
            onFling.Invoke(wsSlingDirection);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_rigidbody.linearVelocity.magnitude < minVelocityToTakeOver && !isControlling) return;

        if (collision.gameObject.TryGetComponent<ControllableEnemy>(out ControllableEnemy enemyScript))
        {
            if (attachedCooldown > 0) return;
            if (enemyScript == null) return;
            if (!enemyScript.isSolid) return;
            Debug.Log("Hit a controllable enemy!");
            BeginEnemyControl(enemyScript);
        }
    }

    // Called when a player starts possessing
    private void BeginEnemyControl(ControllableEnemy enemyScript)
    {
        PlayerStats.EnemiesPossessed++;
        attachedMask.SetControlledEnemy(enemyScript);
        attachedMask.SwtichToAttachedControls();
        _collider.enabled = false;
        isControlling = true;
        enemyScript.SetControlled(true);

        _rigidbody.detectCollisions = false;
        //rigidbody.freezeRotation = true;

        attachedCooldown = maxAttachedCooldown;
        controlledEnemyType = enemyScript.type;
        
        onAttach.Invoke();
    }

    // Called when a player stops possessing
    public void SwitchToDetachedMovement()
    {
        _collider.enabled = true;
        actions.SwitchCurrentActionMap("detatched");
        _rigidbody.detectCollisions = true;
        //rigidbody.freezeRotation = false;
        isControlling = false;
        controlledEnemyType = ControllableEnemy.EnemyType.None;

        onDetach.Invoke();
        healthScript.GiveIFrames();
        healthScript.UpdateHealthUI();
    }
    
    public ControllableEnemy.EnemyType GetCurrentlyControlledEnemyType() { return controlledEnemyType; }
}