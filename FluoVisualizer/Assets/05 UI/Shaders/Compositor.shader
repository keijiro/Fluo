Shader "Fluo/Compositor"
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

half4 fragUpdate(CustomRenderTextureVaryings i) : SV_Target
{
    float2 uv = i.globalTexcoord.xy;
    float3 c_main = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv).rgb;
    float3 c_hud = SAMPLE_TEXTURE2D(_HudTex, sampler_LinearClamp, uv).rgb;
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
