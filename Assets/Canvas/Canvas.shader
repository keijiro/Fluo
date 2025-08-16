Shader "Fluo/Canvas"
{
    Properties
    {
        [Header(Brush Input)]
        [Space]
        _MainTex("Source", 2D) = "Black"{}

        [Header(Fluid)]
        [Space]
        _VelocityTex("Velocity Field", 2D) = "Black"{}
    }

    HLSLINCLUDE

#include "CustomRenderTexture.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/jp.keijiro.spectral-js-unity/Shaders/SpectralUnity.hlsl"

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);
float4 _MainTex_TexelSize;

TEXTURE2D(_VelocityTex);
SAMPLER(sampler_VelocityTex);

// Pass 0: Init
half4 fragInit(InitCustomRenderTextureVaryings i) : SV_Target
{
    return 0;
}

// Pass 1: Update
half4 fragUpdate(CustomRenderTextureVaryings i) : SV_Target
{
    float2 uv = i.globalTexcoord.xy;

    // Velocity field sample
    float2 v = SAMPLE_TEXTURE2D(_VelocityTex, sampler_VelocityTex, uv).xy;

    // Aspect ratio compensation (width-based normalization)
    v.y *= _MainTex_TexelSize.y * _MainTex_TexelSize.z;

    // Sample from advected position
    float2 uv_prev = uv - v * unity_DeltaTime.x;
    float4 c0 = SAMPLE_TEXTURE2D(_SelfTexture2D, sampler_SelfTexture2D, uv_prev);

    float4 c1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
    float3 c = SpectralMix(c0.rgb, c0.a, c1.rgb / LinearToSRGB(c1.a), c1.a);
    return float4(c, max(c0.a, c1.a));
}

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        
        // Pass 0: Init
        Pass
        {
            Name "Init"
            HLSLPROGRAM
            #pragma vertex InitCustomRenderTextureVertexShader
            #pragma fragment fragInit
            ENDHLSL
        }
        
        // Pass 1: Update
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