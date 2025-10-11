Shader "Hidden/Fluo/Prefilter"
{
    Properties
    {
        _MainTex("", 2D) = "" {}
        _BodyPixTex("", 2D) = "" {}
        _LutTex("", 3D) = "" {}
    }

HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/jp.keijiro.bodypix/Shaders/Common.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise2D.hlsl"

TEXTURE2D(_MainTex);
float4 _MainTex_TexelSize;

TEXTURE3D(_LutTex);
float _LutBlend;

TEXTURE2D(_BodyPixTex);
float4 _BodyPixTex_TexelSize;

float4 _Fluo_AudioLevel;
float4 _Fluo_ThemeColor;

// LUT application
float3 ApplyLut(float3 input)
{
    float3 srgb = LinearToSRGB(input);
    float3 graded = SAMPLE_TEXTURE3D(_LutTex, sampler_LinearClamp, srgb).rgb;
    return SRGBToLinear(lerp(srgb, graded, _LutBlend));
}

// 17-tap Gaussian blur with bilinear pairing
float4 Blur17(float2 uv, float2 step)
{
    const float sigma = 3;
    const float s2 = sigma * sigma;

    float w0 = 1;
    float w1 = exp(-0.5 * 1 * 1 / s2);
    float w2 = exp(-0.5 * 2 * 2 / s2);
    float w3 = exp(-0.5 * 3 * 3 / s2);
    float w4 = exp(-0.5 * 4 * 4 / s2);
    float w5 = exp(-0.5 * 5 * 5 / s2);
    float w6 = exp(-0.5 * 6 * 6 / s2);
    float w7 = exp(-0.5 * 7 * 7 / s2);
    float w8 = exp(-0.5 * 8 * 8 / s2);

    float norm = w0 + 2 * (w1 + w2 + w3 + w4 + w5 + w6 + w7 + w8);
    w0 /= norm; w1 /= norm; w2 /= norm;
    w3 /= norm; w4 /= norm; w5 /= norm;
    w6 /= norm; w7 /= norm; w8 /= norm;

    float W12 = w1 + w2;
    float W34 = w3 + w4;
    float W56 = w5 + w6;
    float W78 = w7 + w8;
    float o12 = (1 * w1 + 2 * w2) / W12;
    float o34 = (3 * w3 + 4 * w4) / W34;
    float o56 = (5 * w5 + 6 * w6) / W56;
    float o78 = (7 * w7 + 8 * w8) / W78;

    float4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv) * w0;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv + step * o12) * W12;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv - step * o12) * W12;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv + step * o34) * W34;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv - step * o34) * W34;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv + step * o56) * W56;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv - step * o56) * W56;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv + step * o78) * W78;
    c += SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv - step * o78) * W78;
    return c;
}

// Background effect (animated light slit)
float4 BackgroundEffect(float2 uv)
{
    float t = _Time.y % 997;
    float n = SimplexNoise(float2(uv.x * 6, t * 2));
    float thresh = max(0, _Fluo_AudioLevel.x - 0.333);
    float alpha = 1 - smoothstep(thresh, 1.1 * thresh, abs(n));
    return float4(_Fluo_ThemeColor.rgb, alpha);
}

// Shared vertex shader
void Vertex(uint vertexID : SV_VertexID,
            out float4 positionCS : SV_POSITION,
            out float2 uv : TEXCOORD0)
{
    positionCS = GetFullScreenTriangleVertexPosition(vertexID);
    uv = GetFullScreenTriangleTexCoord(vertexID);
}

// Fragment shader: Multiplexing pass (color grading and human stencil)
float4 FragmentMultiplex(float4 positionCS : SV_POSITION,
                         float2 uv : TEXCOORD0) : SV_Target
{
    // Input + LUT
    float3 color = ApplyLut(SAMPLE_TEXTURE2D(_MainTex, sampler_LinearClamp, uv).rgb);

    // Human stencil
    BodyPix_Mask mask = BodyPix_SampleMask(uv, _BodyPixTex, _BodyPixTex_TexelSize.zw);
    float alpha = smoothstep(0.4, 0.6, BodyPix_EvalSegmentation(mask));

    // Background effect
    float4 effect = BackgroundEffect(uv);

    return float4(lerp(color, effect.rgb, effect.a * (1 - alpha)), alpha);
}

// Fragment shader: Horizontal blur pass
float4 FragmentBlurH(float4 positionCS : SV_POSITION,
                     float2 uv : TEXCOORD0) : SV_Target
{
    return Blur17(uv, float2(_MainTex_TexelSize.x, 0) * 2);
}

// Fragment shader: Vertical blur pass
float4 FragmentBlurV(float4 positionCS : SV_POSITION,
                     float2 uv : TEXCOORD0) : SV_Target
{
    return Blur17(uv, float2(0, _MainTex_TexelSize.y) * 2);
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
