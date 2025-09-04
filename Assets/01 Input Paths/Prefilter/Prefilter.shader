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
float4 _MainTex_TexelSize;

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

// Multiplexing: Color grading and human stencil
float4 FragmentMultiplex(float4 positionCS : SV_POSITION,
                         float2 uv : TEXCOORD0) : SV_Target
{
    // Input + LUT
    float3 srgb = LinearToSRGB(SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv).rgb);
    float3 graded = SRGBToLinear(SAMPLE_TEXTURE3D(_LutTex, sampler_LinearClamp, srgb).rgb);

    // Human stencil
    BodyPix_Mask mask = BodyPix_SampleMask(uv, _BodyPixTex, _BodyPixTex_TexelSize.zw);
    float alpha = smoothstep(0.4, 0.6, BodyPix_EvalSegmentation(mask));

    return float4(graded, alpha);
}

// Separable Gaussian blur: Horizontal pass
float4 FragmentBlurH(float4 positionCS : SV_POSITION,
                     float2 uv : TEXCOORD0) : SV_Target
{
    float2 d = float2(_MainTex_TexelSize.x, 0) * 4.5;

    // 9-tap Gaussian weights
    const float w0 = 0.2270270270; // center
    const float w1 = 0.1945945946;
    const float w2 = 0.1216216216;
    const float w3 = 0.0540540541;
    const float w4 = 0.0162162162;

    float4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_TrilinearClamp, uv) * w0;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv + d * 1) * w1;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv - d * 1) * w1;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv + d * 2) * w2;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv - d * 2) * w2;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv + d * 3) * w3;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv - d * 3) * w3;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv + d * 4) * w4;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv - d * 4) * w4;
    return c;
}

// Separable Gaussian blur: Vertical pass
float4 FragmentBlurV(float4 positionCS : SV_POSITION,
                     float2 uv : TEXCOORD0) : SV_Target
{
    float2 d = float2(0, _MainTex_TexelSize.y) * 4.5;

    // 9-tap Gaussian weights
    const float w0 = 0.2270270270; // center
    const float w1 = 0.1945945946;
    const float w2 = 0.1216216216;
    const float w3 = 0.0540540541;
    const float w4 = 0.0162162162;

    float4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_TrilinearClamp, uv) * w0;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv + d * 1) * w1;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv - d * 1) * w1;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv + d * 2) * w2;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv - d * 2) * w2;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv + d * 3) * w3;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv - d * 3) * w3;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv + d * 4) * w4;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv - d * 4) * w4;
    return c;
}

ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentMultiplex
            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentBlurH
            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentBlurV
            ENDHLSL
        }
    }
}
