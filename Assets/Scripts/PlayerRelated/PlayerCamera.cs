using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float heightAbovePlayer = 5f;
    [SerializeField] float offsetFromPlayerX = 0f;
    [SerializeField] float offsetFromPlayerY = 0f;
    [SerializeField] float cameraAngleX = 45f;
    [SerializeField] float rotationY = 0f;
    DetatchedMask playerMask = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMask = FindFirstObjectByType<DetatchedMask>();
        transform.eulerAngles = new Vector3(cameraAngleX, rotationY, 0);
        transform.position = new Vector3(offsetFromPlayerX, heightAbovePlayer, offsetFromPlayerY);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMask != null)
        {
            this.transform.position = new Vector3(playerMask.transform.position.x + offsetFromPlayerX, heightAbovePlayer, playerMask.transform.position.z + offsetFromPlayerY);
        }
    }
}
