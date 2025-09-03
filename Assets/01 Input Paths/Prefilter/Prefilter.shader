Shader "Hidden/Fluo/Prefilter"
{
    Properties
    {
        _MainTex("", 2D) = ""
        _BodyPixTex("", 2D) = ""
        _LutTex("", 3D) = ""
    }

HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"
#include "Packages/jp.keijiro.bodypix/Shaders/Common.hlsl"

TEXTURE2D(_MainTex);
TEXTURE3D(_LutTex);
TEXTURE2D(_BodyPixTex);
float4 _BodyPixTex_TexelSize;

void Vertex(uint vertexID : SV_VertexID,
            out float4 positionCS : SV_POSITION,
            out float2 uv : TEXCOORD0)
{
    positionCS = GetFullScreenTriangleVertexPosition(vertexID);
    uv = GetFullScreenTriangleTexCoord(vertexID);
}

float4 Fragment(float4 positionCS : SV_POSITION,
                float2 uv : TEXCOORD0) : SV_Target
{
    // Video input + LUT
    float3 srgb = LinearToSRGB(SAMPLE_TEXTURE2D(_MainTex, sampler_TrilinearClamp, uv).rgb);
    float3 graded = SRGBToLinear(SAMPLE_TEXTURE3D(_LutTex, sampler_TrilinearClamp, srgb).rgb);

    // Human stencil
    BodyPix_Mask mask = BodyPix_SampleMask(uv, _BodyPixTex, _BodyPixTex_TexelSize.zw);
    float alpha = smoothstep(0.4, 0.6, BodyPix_EvalSegmentation(mask));

    return float4(graded, alpha);
}

ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDHLSL
        }
    }
}
