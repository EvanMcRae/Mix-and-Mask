using UnityEngine;

public class MaskEffect : MonoBehaviour {
    public float speed = 1;
    public bool transitionDirection = false;
    public bool isTransitioning = false;
    private float t;
    private Color color;

    public void StartFade(bool direction) {
        transitionDirection = direction;
        isTransitioning = true;
    }

    public void Update() {
        if (!isTransitioning) return;
        t += (transitionDirection ? 1 : -1) * speed * Time.deltaTime;
        t = Mathf.Clamp(t, 0, 1);
        if (isTransitioning) MaskRendererFeature.instance.ApplyMask(t, color);
        if (Mathf.Approximately(t, 1f) || t == 0f) isTransitioning = false;
    }

    public void SetMaskColor(Color newColor) {
        color = newColor;
    }
}