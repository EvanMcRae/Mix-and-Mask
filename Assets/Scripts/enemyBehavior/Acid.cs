using UnityEngine;

public class Acid : MonoBehaviour
{
    [SerializeField] float maxHeight = 1.5f;
    [SerializeField] float dps = 2f;
    [SerializeField] float lifeTime = 20f;
    [SerializeField] float riseSpeed = 0.2f;
    public bool belongsToPlayer = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.transform.localScale.y < maxHeight) this.transform.localScale += new Vector3(0, 0.3f * Time.deltaTime, 0);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") || other.isTrigger)
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            ControllableEnemy ce = other.GetComponent<ControllableEnemy>();
            if (enemy != null)
            {
                if (belongsToPlayer)
                {
                    //Debug.Log("Possessed stepped in acid");
                    if (ce != null && !ce.isUnderControl) enemy.TakeDamageOverTime(dps * Time.deltaTime);
                }
                else
                {
                    if (ce != null && ce.isUnderControl) ce.TakeDamageOverTime(dps * Time.deltaTime);
                }
            }
        }
        
        if (other.CompareTag("Player") && !belongsToPlayer)
        {
            //Debug.Log("Player stepped in acid");

            ControllableEnemy player = other.GetComponent<ControllableEnemy>();
            if (player != null)
            {
                player.TakeDamageOverTime(dps * Time.deltaTime);
            }
            else
            {
                playerHealth playerAsMask = other.GetComponent<playerHealth>();
                playerAsMask.playerTakeDamageOverTime(dps * Time.deltaTime);
            }

        }
    }
}
