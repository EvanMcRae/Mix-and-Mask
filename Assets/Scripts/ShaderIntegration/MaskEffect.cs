using UnityEngine;

public class MaskEffect : MonoBehaviour {
    
    
    public bool maskEnabled;
    public float speed = 1;
    private float t;

    
    public void Update()
    {
        t += (maskEnabled ? 1 : -1) * speed * Time.deltaTime;
        t = Mathf.Clamp(t, 0, 1);
        MaskRendererFeature.instance.SetProgress(t);
    }

    public void SetMaskColor(Color color) {
        MaskRendererFeature.instance.SetColor(color);
    }
}
