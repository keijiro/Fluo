#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

void SetInitialState(inout VFXAttributes attributes, VFXSampler2D source)
{
    uint seed = attributes.seed + 23841;
    float2 uv = float2(FixedRand(seed++), FixedRand(seed++));
    float rz = FixedRand(seed++) * 360;

    float4 c_src = SampleTexture(source, uv, 0);

    attributes.position = float3((uv - 0.5) * float2(16.0 / 9, 1), 0) * 2;
    attributes.angleZ = rz;
    attributes.color = c_src.rgb;
    attributes.alive = c_src.a > 0.3;
}

void ApplyPalette(inout VFXAttributes attributes, VFXGradient palette, float amplitude)
{
    float luma = Luminance(LinearToSRGB(attributes.color));
    attributes.color = SampleGradient(palette, luma * amplitude).rgb;
}
