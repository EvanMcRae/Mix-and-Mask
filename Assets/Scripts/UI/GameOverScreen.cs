using TMPro;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    public Color highlightColor;

    public void UpdateText()
    {
        //find the stats
        
        int possessions = PlayerStats.EnemiesPossessed;
        int kills = PlayerStats.EnemiesKilled;
        string colorHex = ColorUtility.ToHtmlStringRGB(highlightColor);

        text.text =
            $"You possessed monsters <color=#{colorHex}>{possessions}</color> times\n" +
            $"You killed <color=#{colorHex}>{kills}</color> monsters";
    }
}
