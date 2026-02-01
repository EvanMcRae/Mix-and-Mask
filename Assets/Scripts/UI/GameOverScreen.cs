using TMPro;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    public Color highlightColor;

    public void UpdateText(int possessions, int kills)
    {
        string colorHex = ColorUtility.ToHtmlStringRGB(highlightColor);

        text.text =
            $"You possessed monsters <color=#{colorHex}>{possessions}</color> times\n" +
            $"You killed <color=#{colorHex}>{kills}</color> monsters";
    }
}
