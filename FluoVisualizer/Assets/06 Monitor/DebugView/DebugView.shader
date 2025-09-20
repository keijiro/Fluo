Shader "Fluo/DebugView"
{
    Properties
    {
        _SourceTex("Source", 2D) = "Black"{}
        _MotionTex("Motion Vector", 2D) = "Black"{}
        _VelocityTex("Velocity Field", 2D) = "Black"{}
        _InjectionTex("Color Injection", 2D) = "Black"{}
        _CanvasTex("Canvas", 2D) = "Black"{}
    }

    HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/jp.keijiro.fluo/Shaders/CustomRenderTexture.hlsl"

TEXTURE2D(_SourceTex);
TEXTURE2D(_MotionTex);
TEXTURE2D(_VelocityTex);
TEXTURE2D(_InjectionTex);
TEXTURE2D(_CanvasTex);

float4 _HudColor;

float4 fragUpdate(CustomRenderTextureVaryings i) : SV_Target
{
    uint2 cell = i.globalTexcoord.xy * 3;
    float2 uv = frac(i.globalTexcoord.xy * 3);
    float3 output;

    if (cell.y == 0)
    {
        if (cell.x < 2)
        {
            float4 c = SAMPLE_TEXTURE2D(_InjectionTex, sampler_LinearClamp, uv);
            output = cell.x == 0 ? c.rgb : c.a;
        }
        else
        {
            output = SAMPLE_TEXTURE2D(_CanvasTex, sampler_LinearClamp, uv).rgb;
        }
    }
    else if (cell.y == 1)
    {
        if (cell.x == 0)
        {
            float2 v = SAMPLE_TEXTURE2D(_VelocityTex, sampler_LinearClamp, uv).rg;
            output = SRGBToLinear(float3(v * 4 + 0.5, 0));
        }
        else if (cell.x == 1)
        {
            float2 v = SAMPLE_TEXTURE2D(_MotionTex, sampler_LinearClamp, uv).rgb;
            output = SRGBToLinear(float3(v * 10 + 0.5, 0));
        }
        else
        {
            output = SAMPLE_TEXTURE2D(_CanvasTex, sampler_LinearClamp, uv).a;
        }
    }
    else
    {
        if (cell.x < 2)
        {
            float4 c = SAMPLE_TEXTURE2D(_SourceTex, sampler_LinearClamp, uv);
            output = cell.x == 0 ? c.rgb : c.a;
        }
        else
        {
            output = 0;
        }
    }

    return float4(output, 1);
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
