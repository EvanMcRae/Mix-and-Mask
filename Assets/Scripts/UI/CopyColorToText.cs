using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CopyButtonColorToText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Button parentButton;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        parentButton = GetComponentInParent<Button>();

        if (parentButton == null)
        {
            Debug.LogWarning("No parent Button found for " + name);
        }
        else
        {
            // Initialize text color immediately
            text.color = parentButton.targetGraphic.color;
        }
    }

    // Call this whenever the Button changes color dynamically
    public void UpdateTextColor()
    {
        if (parentButton != null)
        {
            text.color = parentButton.image.color;
        }
    }
}
