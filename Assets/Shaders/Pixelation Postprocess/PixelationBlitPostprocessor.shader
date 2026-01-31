Shader "Custom/PixelationBlitPostprocessor"
{
    Properties
    {
        _PixelDensity("Pixel Density", Float) = 5
        _BlurTestRadius("Blur Test Radius", Int) = 2
    }
    
    SubShader
    {
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
        ENDHLSL

        Tags { "RenderType"="Opaque" }
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "PixelationBlitPostprocessor"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment Frag
            
            CBUFFER_START(UNITY_PER_MATERIAL)
                float _PixelDensity;
                float _BlurTestRadius;
            CBUFFER_END

            float4 Frag (Varyings input) : SV_Target
            {
                float2 pixelCount = _ScreenParams.xy / _PixelDensity;
                float2 pixelUV = ceil(input.texcoord * pixelCount) / pixelCount;
                
                float4 colorSum = 0;
                int countedPixels = 0;
                for (int x = -_BlurTestRadius; x <= _BlurTestRadius; x++)
                {
                    for (int y = -_BlurTestRadius; y <= _BlurTestRadius; y++)
                    {
                        float2 uvOffset = float2(x, y) / _ScreenParams.xy;
                        colorSum += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, pixelUV + uvOffset).rgba;
                        countedPixels++;
                    }
                }
                colorSum /= countedPixels;
                
                return colorSum;
            }
            
            ENDHLSL
        }
    }
}
