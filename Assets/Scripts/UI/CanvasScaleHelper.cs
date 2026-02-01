using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class CanvasScaleHelper : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        GetComponent<CanvasScaler>().matchWidthOrHeight = Screen.height > 9f/16f * Screen.width ? 0 : 1;
    }
}
