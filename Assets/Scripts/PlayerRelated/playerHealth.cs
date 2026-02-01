using DG.Tweening.Core.Easing;
using UnityEngine;

public class playerHealth : MonoBehaviour
{
    public float maxPlayerHealth = 10f;
    public float currPlayerHealth = 10f;
    [SerializeField] private float iframes = 0.1f;
    private float canDamageTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currPlayerHealth = maxPlayerHealth;
        UpdateHealthUI();
    }

    public void playerTakeDamage(float dmg)
    {
        if (Time.time < canDamageTime) { Debug.Log("iframes stopped player damage"); return; }

        currPlayerHealth -= dmg;

        AkUnitySoundEngine.PostEvent("PlayerDamage", Utils.WwiseGlobal);

        UpdateHealthUI();

        if (currPlayerHealth <= 0)
        {
            
            Die();
        }

        GiveIFrames();
        Debug.Log("Took player damage");
    }

    public void playerTakeDamageOverTime(float dmg)
    {
        currPlayerHealth -= dmg;

        UpdateHealthUI();

        if (currPlayerHealth <= 0)
            Die();
    }

    public void UpdateHealthUI()
    {
        //update UI element
        HealthUI healthUI = FindAnyObjectByType<HealthUI>();
        if (healthUI != null)
        {
            healthUI.UpdateHealth((int)currPlayerHealth, (int)maxPlayerHealth);
        }
        if (currPlayerHealth <= 0)
        {
            UpdateAbilitiesIcons abilityUI = FindFirstObjectByType<UpdateAbilitiesIcons>();
            abilityUI.OnPlayerDie();
        }
    }
    public void Die()
    {
        print("Game over!");
        AkUnitySoundEngine.PostEvent("StopMusic", Utils.WwiseGlobal);
        
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

    public void GiveIFrames()
    {
        canDamageTime = Time.time + iframes;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
