using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float heightAbovePlayer = 5f;
    DetatchedMask playerMask = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMask = FindObjectOfType<DetatchedMask>();
        transform.eulerAngles = new Vector3(90, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMask != null) this.transform.position = new Vector3(playerMask.transform.position.x, heightAbovePlayer, playerMask.transform.position.z);
    }
}
