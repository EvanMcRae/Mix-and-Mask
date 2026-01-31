using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class MaskRendererFeature : FullScreenPassRendererFeature {
    public static MaskRendererFeature instance;
    
    public Shader shader;
    private float progress;
    private Color color;

    public void OnEnable() {
        if (instance&& instance != this) Debug.LogWarning("Instance already exists! Only one MaskRendererFeature can exist at a time!");
        instance = this;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        passMaterial = CoreUtils.CreateEngineMaterial(shader);
        passMaterial.SetFloat(Shader.PropertyToID("_AnimationProgress"), progress);
        passMaterial.SetColor(Shader.PropertyToID("_TransitionColor"), color);
        passMaterial.SetFloat(Shader.PropertyToID("_EffectScreenDist"), 0.5f);
        passMaterial.SetFloat(Shader.PropertyToID("_EffectScreenStrength"), 2);
        base.AddRenderPasses(renderer, ref renderingData);
    }

    public void ApplyMask(float newProgress, Color newColor) {
        if (progress < 0 || progress > 1) Debug.LogError("Progress must be between 0 and 1.");
        progress = newProgress;
        color = newColor;
    }
}