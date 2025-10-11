Shader "Fluo/AudioBar"
{
    HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/jp.keijiro.fluo/Shaders/CustomRenderTexture.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise2D.hlsl"

float4 _Fluo_AudioLevel;
float4 _Fluo_ThemeColor;

float3 BarEffect(float x)
{
    float t = _Time.y % 997;

    float n1 = SimplexNoise(float2(x * 2, t * 2));
    float n2 = SimplexNoise(float2(x * 4, t * 2));
    float n3 = SimplexNoise(float2(x * 8, t * 2));

    float a1 = n1 - 1 + _Fluo_AudioLevel.x;
    float a2 = n2 - 1 + _Fluo_AudioLevel.y;
    float a3 = n3 - 1 + _Fluo_AudioLevel.z;

    float a = saturate(saturate(a1) + saturate(a2) + saturate(a3));

    return _Fluo_ThemeColor.rgb * a * a * a;
}

float4 FragmentUpdateH(CustomRenderTextureVaryings i) : SV_Target
{
    return float4(BarEffect(1 + i.globalTexcoord.x / 3), 1);
}

float4 FragmentUpdateV(CustomRenderTextureVaryings i) : SV_Target
{
    return float4(BarEffect(i.globalTexcoord.y), 1);
}

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            Name "Update H"
            HLSLPROGRAM
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment FragmentUpdateH
            ENDHLSL
        }
        Pass
        {
            Name "Update V"
            HLSLPROGRAM
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment FragmentUpdateV
            ENDHLSL
        }
    }
}
