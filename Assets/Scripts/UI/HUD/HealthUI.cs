using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Heart Images (Left to Right)")]
    [SerializeField] private Image[] images;

    [Header("Heart Sprites")]
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private Sprite halfHeart;
    [SerializeField] private Sprite fullHeart;

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        int maxHearts = maxHealth / 2;

        for (int i = 0; i < images.Length; i++)
        {
            // Hide unused hearts
            if (i >= maxHearts)
            {
                images[i].gameObject.SetActive(false);
                continue;
            }

            images[i].gameObject.SetActive(true);

            int heartHealth = currentHealth - (i * 2);

            if (heartHealth >= 2)
                images[i].sprite = fullHeart;
            else if (heartHealth == 1)
                images[i].sprite = halfHeart;
            else
                images[i].sprite = emptyHeart;
        }
    }
}