Shader "Fluo/ForceField"
{
    Properties
    {
        _MainTex("Motion Vectors", 2D) = "Black"{}
        _Amplitude("Amplitude", Float) = 0.1
    }

    HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/jp.keijiro.fluo/Shaders/CustomRenderTexture.hlsl"

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

float _Amplitude;

half4 fragUpdate(CustomRenderTextureVaryings i) : SV_Target
{
    float2 uv = i.globalTexcoord.xy;

    float2 mv = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).xy;
    mv *= _Amplitude / unity_DeltaTime.x;

    return float4(mv, 0, 0);
}

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            Name "Update"
            HLSLPROGRAM
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment fragUpdate
            ENDHLSL
        }
    }
}
