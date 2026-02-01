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

        //update UI element
        HealthUI healthUI = FindAnyObjectByType<HealthUI>();
        if (healthUI != null)
        {
            healthUI.UpdateHealth((int)currPlayerHealth, (int)maxPlayerHealth);
        }

        if (currPlayerHealth <= 0)
            Die();
    }

    public void Die()
    {
        print("Game over!");
        //Find the game manager object
        GameOverScreen gameOverUI = FindAnyObjectByType<GameOverScreen>();
        WaveManager.GameOver = true;
        
        if (gameOverUI == null)
        {
            Debug.LogError("No Game Over UI!");
        }

        //Update stats text
        gameOverUI.UpdateText();
        PopupPanel panel = gameOverUI.gameObject.GetComponent<PopupPanel>();
        panel.Up();

        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
