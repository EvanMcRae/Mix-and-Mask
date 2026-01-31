using DG.Tweening.Core.Easing;
using UnityEngine;

public class playerHealth : MonoBehaviour
{
    public float maxPlayerHealth = 10f;
    public float currPlayerHealth = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currPlayerHealth = maxPlayerHealth;
    }

    public void playerTakeDamage(float dmg)
    {
        currPlayerHealth -= dmg;
        if (currPlayerHealth <= 0)
            Die();
    }

    public void Die()
    {
        print("Game over!");

        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
