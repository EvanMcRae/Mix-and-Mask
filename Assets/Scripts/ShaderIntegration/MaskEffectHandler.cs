using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "MaskEffectHandler", menuName = "Shader Controllers/MaskEffectHandler")]
public class MaskEffectHandler : ScriptableObject {

    public static MaskEffectHandler instance;

    public FullScreenPassRendererFeature maskFeature;

    public void OnEnable() {
        if (instance) Debug.LogError("Instance already exists! Only one MaskEffectHandler can exist at a time!");
        else instance = this;
    }

    public void SetProgress(float progress) {
        if (progress < 0 || progress > 1) Debug.LogError("Progress must be between 0 and 1.");
        maskFeature.passMaterial.SetFloat(Shader.PropertyToID("_AnimationProgress"), progress);
    }

    public void SetColor(Color color) {
        maskFeature.passMaterial.SetColor(Shader.PropertyToID("_TransitionColor"), color);
    }
    
    

}
