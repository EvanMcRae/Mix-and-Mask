using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements.Experimental;

public class TransitionRendererFeature : FullScreenPassRendererFeature {
    public static TransitionRendererFeature instance;
    
    public Shader shader;
    public float progress;

    public void OnEnable() {
        if (instance && instance != this) Debug.LogWarning("Instance already exists! Only one TransitionRendererFeature can exist at a time!");
        else instance = this;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        passMaterial.SetFloat(Shader.PropertyToID("_AnimationProgress"), 1);
        passMaterial.SetColor(Shader.PropertyToID("_TransitionColor"), Color.black);
        float easedProgress = 1 - progress; //No easing, we could add some.
        passMaterial.SetFloat(Shader.PropertyToID("_EffectScreenDist"), easedProgress * 2 - 1);
        passMaterial.SetFloat(Shader.PropertyToID("_EffectScreenStrength"), 5);
        base.AddRenderPasses(renderer, ref renderingData);
    }

    public void ApplyTransition(float newProgress) {
        if (progress < 0 || progress > 1) Debug.LogError("Progress must be between 0 and 1.");
        Debug.Log("NewProgress: " + newProgress);
        progress = newProgress;
    }
}