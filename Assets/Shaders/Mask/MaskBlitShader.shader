Shader "Custom/MaskShader"
{
    Properties
    {
        _AnimationProgress("Animation Progress", Range(0, 1)) = 0
        _TransitionColor("Transition Color", Color) = (0, 0, 0, 1)
        _EffectScreenDist("Effect Screen Distance", Range(0, 1)) = 0.5
        _EffectScreenStrength("Effect Screen Strength", Range(0, 100)) = 2
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
                float _EffectScreenDist;
                float _EffectScreenStrength;
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
                float uvLen = length(centeredUV);
                
                float root2 =  1.4142135623;
                float anim = (1 - _AnimationProgress);
                anim *= anim;
                
                float noise = sample_noise(float3(centeredUV * 10, _Time.y));
                float clampedNoise = Remap(-1, 1, uvLen * root2, 1, noise);
                float smoothNoise = Smoothstep01(clampedNoise);
                float lenDiff = uvLen * 2 * smoothNoise - anim * root2;
                smoothNoise = saturate(min(smoothNoise, lenDiff));
                smoothNoise = saturate((smoothNoise - _EffectScreenDist) * _EffectScreenStrength);
                
                return color * (1 - smoothNoise) + _TransitionColor * smoothNoise;
            }
            
            ENDHLSL
        }
    }
}
