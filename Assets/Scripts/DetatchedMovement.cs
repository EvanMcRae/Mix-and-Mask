using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DetatchedMovement : MonoBehaviour
{
    private Vector2 initialClickPoint = new Vector2(0, 0);
    private Vector2 finalClickPoint = new Vector2(0, 0);
    private Rigidbody rigidBody = null;

    [SerializeField] float maxRealSlingLength = 20f;
    [SerializeField] float maxSlingVelocity = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Grabs the position of the mouse when the slingshot button is initially pressed and when it is released
    // Then uses the difference between the positions to create a launch vector for the mask and flings it in that direction
    public void Slingshot(InputAction.CallbackContext context)
    {
        // When first pressed, get mouse position
        if (context.started)
        {
            Debug.Log("Sling Started");
            initialClickPoint = Mouse.current.position.ReadValue();
            Debug.Log("Position: " + initialClickPoint.x + ", " + initialClickPoint.y);
        }

        // When released, get mouse position again and launch mask
        if (context.canceled)
        {
            Debug.Log("Sling released");
            finalClickPoint = Mouse.current.position.ReadValue();
            Vector2 slingDirection = initialClickPoint - finalClickPoint;
            // Debug.Log("Sling Direction: " + slingDirection.x + ", " + slingDirection.y);


            float slingLength = slingDirection.magnitude;
            Debug.Log("Sling Length: " + slingLength);
            float slingVelocity = (slingLength / maxRealSlingLength) * maxSlingVelocity; // Uses percentage of real screenspace vector to calculate force vector length
            if (slingVelocity > maxSlingVelocity) slingVelocity = maxSlingVelocity; // Caps force magnitude
            slingDirection = slingDirection.normalized;
            rigidBody.AddForce(new Vector3(slingDirection.x, 0.5f, slingDirection.y) * slingVelocity, ForceMode.Impulse);
        }
    }
}
