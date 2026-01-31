using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "Shaders", menuName = "Shaders/ShaderPassHolder")]
public class ShaderPassHolder : ScriptableObject
{
    [SerializeField] private FullScreenPassRendererFeature _pass;
    public static FullScreenPassRendererFeature Pass { get; private set; }

    private void OnValidate()
    {
        Pass = _pass;
    }
}