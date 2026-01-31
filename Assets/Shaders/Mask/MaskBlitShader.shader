Shader "Custom/MaskShader"
{
    Properties
    {
        _AnimationProgress("Animation Progress", Range(0, 1)) = 0
        _TransitionColor("Transition Color", Color) = (0.8, 0.2, 0.2, 1)
       
    }
    
    
    SubShader
    {
        HLSLINCLUDE
        #include "Assets/Noisier-Nodes/Noise/HLSL/SimplexNoise3D.hlsl"
        #include "Assets/Noisier-Nodes/Noise/HLSL/SimplexNoise2D.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
        #include "Assets/Shaders/Util/blend.hlsl"
        ENDHLSL

        Tags { "RenderType"="Opaque" }
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "MaskShader"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment Frag

            CBUFFER_START(UNITY_PER_MATERIAL)
                float _AnimationProgress;
                half4 _TransitionColor;
            CBUFFER_END
            
            float sample_noise(float3 pos)
            {
                // cheap fractional brownian motion lol
                float s1 = snoise(pos) * 2;
                float s2 = snoise(pos * 2);
                return (s1 + s2) / 3;
            }
            
            float4 Frag (Varyings input) : SV_Target
            {
                float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord).rgba;
                
                float2 centeredUV = input.texcoord - 0.5;
                float2 uvDir = normalize(centeredUV);
                float2 uvLen = length(centeredUV);
                
                float root2 =  1.4142135623;
                
                float noise = sample_noise(float3(centeredUV * 10, _Time.y));
                float clampedNoise = Remap(-1, 1, uvLen * root2, 1, noise);
                float smoothNoise = Smoothstep01(clampedNoise);
                float lenDiff = uvLen * 2 * smoothNoise - _AnimationProgress * root2;
                smoothNoise = saturate(min(smoothNoise, lenDiff));
                smoothNoise = saturate((smoothNoise - 0.5) * 2);
                
                return color * (1 - smoothNoise) + _TransitionColor * smoothNoise;
            }
            
            ENDHLSL
        }
    }
}
