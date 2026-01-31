using System;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class MaskEffect : MonoBehaviour {
    
    public DetatchedMask mask;
    public float speed = 1;
    public bool transitionDirection = false;
    public bool isTransitioning = false;
    private float t;
    private Color color;

    public void OnEnable() {
        mask.onAttach.AddListener(OnAttach);
        mask.onDetach.AddListener(OnDetach);
    }

    public void OnDisable() {
        mask.onAttach.RemoveListener(OnAttach);
        mask.onDetach.RemoveListener(OnDetach);
    }

    public void OnAttach() {
        StartFade(true);
    }

    public void OnDetach() {
        StartFade(false);
    }

    public void StartFade(bool direction) {
        transitionDirection = direction;
        isTransitioning = true;
    }

    public void Update() {
        if (!isTransitioning) return;
        t += (transitionDirection ? 1 : -1) * speed * Time.deltaTime;
        t = Mathf.Clamp(t, 0, 1);
        float easedT = Mathf.Sqrt(t);
        
        if (isTransitioning) MaskRendererFeature.instance.ApplyMask(easedT, color);
        if (Mathf.Approximately(t, 1f) || t == 0f) isTransitioning = false;
    }

    public void SetMaskColor(Color newColor) {
        color = newColor;
    }
}