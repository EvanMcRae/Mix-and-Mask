using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "Shaders", menuName = "Shaders/ShaderPassHolder")]
public class ShaderPassHolder : ScriptableObject
{
    [SerializeField] private FullScreenPassRendererFeature _pass;
    [SerializeField] private Material _passMaterial;
    public static FullScreenPassRendererFeature Pass { get; private set; }
    private static Material PassMaterial;

    private void OnValidate()
    {
        Pass = _pass;
        PassMaterial = _passMaterial;
    }

    public static void EnablePass()
    {
        Pass.passMaterial = PassMaterial;
    }

    public static void DisablePass()
    {
        Pass.passMaterial = null;
    }
}