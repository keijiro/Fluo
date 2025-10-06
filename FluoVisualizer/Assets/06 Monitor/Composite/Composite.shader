Shader "Fluo/Monitor/Composite"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "Black"{}
        _HudTex("HUD Texture", 2D) = "Black"{}
        _HudColor("HUD Color", Color) = (1, 1, 1, 1)
    }

    HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/jp.keijiro.fluo/Shaders/CustomRenderTexture.hlsl"

TEXTURE2D(_MainTex);
TEXTURE2D(_HudTex);

float4 _HudColor;
float4 _Fluo_AudioLevel;

float2 ApplyUVEffect(float2 uv)
{
    float t = _Time.y % 997;

    float dx = GenerateHashedRandomFloat(uint2(t * 60, uv.y * 135)) * 2 - 1;
    dx = dx * dx * dx;

    float amp = GenerateHashedRandomFloat(uint2(t * 10, uv.y * 32 + 2000));
    amp = 0.03 * (amp < 0.8 * _Fluo_AudioLevel.z);

    return float2(uv.x + dx * amp, uv.y);
}

half4 fragUpdate(CustomRenderTextureVaryings i) : SV_Target
{
    float2 uv1 = i.globalTexcoord.xy;
    float2 uv2 = ApplyUVEffect(uv1);
    float3 c_main = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv1).rgb;
    float3 c_hud = SAMPLE_TEXTURE2D(_HudTex, sampler_LinearClamp, uv2).rgb;
    return float4(c_main + c_hud * _HudColor.rgb, 1);
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
