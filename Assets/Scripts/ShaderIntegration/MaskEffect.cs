using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using UnityEngine.VFX;

public class MaskEffect : MonoBehaviour {
    
    public DetatchedMask mask;
    public VisualEffect possessVFX;
    public VisualEffect flingVFX;
    public float speed = 1;
    public bool transitionDirection = false;
    public bool isTransitioning = false;
    private float t;
    [SerializeField] private Color color;

    public void OnEnable() {
        mask.onAttach.AddListener(OnAttach);
        mask.onDetach.AddListener(OnDetach);
        mask.onFling.AddListener(OnFling);
    }

    public void OnDisable() {
        mask.onAttach.RemoveListener(OnAttach);
        mask.onDetach.RemoveListener(OnDetach);
        mask.onFling.RemoveListener(OnFling);
    }

    public void OnAttach() {
        StartFade(true);
        possessVFX.Play();
    }

    public void OnDetach() {
        StartFade(false);
    }

    public void OnFling(Vector3 direction) {
        flingVFX.SetVector3("Direction", -direction);
        flingVFX.Play();
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

    public void OnDestroy()
    {
        MaskRendererFeature.instance.ApplyMask(0, color);
    }

    public void OnApplicationQuit()
    {
        MaskRendererFeature.instance.ApplyMask(0, color);
    }
}