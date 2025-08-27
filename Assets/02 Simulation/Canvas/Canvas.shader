Shader "Fluo/Canvas"
{
    Properties
    {
        _InjectTex("Color Injection", 2D) = "Black"{}
        _VelocityTex("Velocity Field", 2D) = "Black"{}
    }

    HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/jp.keijiro.spectral-js-unity/Shaders/SpectralUnity.hlsl"
#include "Packages/jp.keijiro.fluo/Shaders/CustomRenderTexture.hlsl"

TEXTURE2D(_InjectTex);
SAMPLER(sampler_InjectTex);
float4 _InjectTex_TexelSize;

TEXTURE2D(_VelocityTex);
SAMPLER(sampler_VelocityTex);
float4 _VelocityTex_TexelSize;

half4 fragUpdate(CustomRenderTextureVaryings i) : SV_Target
{
    float2 uv = i.globalTexcoord.xy;

    // Injection color sample
    float4 c1 = SAMPLE_TEXTURE2D(_InjectTex, sampler_InjectTex, uv);

    // Velocity field sample
    float2 v = SAMPLE_TEXTURE2D(_VelocityTex, sampler_VelocityTex, uv).xy;

    // Aspect ratio compensation (width-based normalization)
    v.y *= _VelocityTex_TexelSize.y * _VelocityTex_TexelSize.z;

    // Sample from advected position
    float2 uv_prev = uv - v * unity_DeltaTime.x;
    float4 c0 = SAMPLE_TEXTURE2D(_SelfTexture2D, sampler_SelfTexture2D, uv_prev);

    float3 c = SpectralMix(c0.rgb, c0.a, c1.rgb / LinearToSRGB(c1.a), c1.a);
    return float4(c, max(c0.a, c1.a));
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