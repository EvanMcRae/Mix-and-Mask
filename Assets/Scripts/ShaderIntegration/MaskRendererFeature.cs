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
        if (instance) Debug.LogError("Instance already exists! Only one MaskRendererFeature can exist at a time!");
        else instance = this;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        passMaterial = CoreUtils.CreateEngineMaterial(shader);
        passMaterial.SetFloat(Shader.PropertyToID("_AnimationProgress"), progress);
        passMaterial.SetColor(Shader.PropertyToID("_TransitionColor"), color);
        base.AddRenderPasses(renderer, ref renderingData);
    }

    public void SetProgress(float progress) {
        if (progress < 0 || progress > 1) Debug.LogError("Progress must be between 0 and 1.");
        this.progress = progress;
    }

    public void SetColor(Color color) {
        this.color = color;
    }
}