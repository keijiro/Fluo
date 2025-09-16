Shader "Hidden/URP-CameraTextureUtils/CameraTextureRouter"
{
HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

TEXTURE2D_X(_MotionVectorTexture);

int _DepthEncoding;   // 0: RawBuffer, 1: Linear01, 2: LinearEye
int _MotionEncoding;  // 0: Signed, 1: Centered01

// Helpers
float  EncodeDepth(float rawDepth)
{
    float d = rawDepth;
    if (_DepthEncoding == 1) d = Linear01Depth(rawDepth, _ZBufferParams);
    else if (_DepthEncoding == 2) d = LinearEyeDepth(rawDepth, _ZBufferParams);
    return d;
}

float2 SampleMotion(float2 uv)
{
    return SAMPLE_TEXTURE2D_X_LOD(_MotionVectorTexture, sampler_LinearClamp, uv, 0).xy;
}

float2 EncodeMotion(float2 m)
{
    return (_MotionEncoding == 1) ? saturate(m * 0.5 + 0.5) : m;
}

// Fullscreen triangle vertex without structs
void Vert(uint vertexID : SV_VertexID,
          out float4 positionCS : SV_Position,
          out float2 uv : TEXCOORD0)
{
    positionCS = GetFullScreenTriangleVertexPosition(vertexID);
    uv = GetFullScreenTriangleTexCoord(vertexID);
}

// Fragment shaders
float4 FragDepthOnly(float2 uv : TEXCOORD0) : SV_Target
{
    return float4(EncodeDepth(SampleSceneDepth(uv)), 0, 0, 1);
}

float4 FragMotionOnly(float2 uv : TEXCOORD0) : SV_Target
{
    return float4(EncodeMotion(SampleMotion(uv)), 0, 1);
}

void FragDepthMotion(float2 uv : TEXCOORD0,
                     out float4 outDepth  : SV_Target0,
                     out float4 outMotion : SV_Target1)
{
    outDepth  = float4(EncodeDepth(SampleSceneDepth(uv)), 0, 0, 1);
    outMotion = float4(EncodeMotion(SampleMotion(uv)), 0, 1);
}

ENDHLSL

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        ZWrite Off ZTest Always Cull Off

        // Pass 0: Single-target, Depth only
        Pass
        {
            Name "CameraTextureRouter_DepthOnly"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment FragDepthOnly
            #pragma target 4.5
            ENDHLSL
        }

        // Pass 1: Single-target, Motion only
        Pass
        {
            Name "CameraTextureRouter_MotionOnly"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment FragMotionOnly
            #pragma target 4.5
            ENDHLSL
        }

        // Pass 2: Dual-target
        Pass
        {
            Name "CameraTextureRouter"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment FragDepthMotion
            #pragma target 4.5
            ENDHLSL
        }
    }
}
