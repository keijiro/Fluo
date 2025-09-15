#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

// Helper functions

float GetColorDistance(float3 c1, float3 c2)
{
    c1 = RgbToHsv(LinearToSRGB(c1)) * float3(1, 0.6, 0.6);
    c2 = RgbToHsv(LinearToSRGB(c2)) * float3(1, 0.6, 0.6);
    return distance(c1, c2);
}

// VFX custom function blocks

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

void ApplyPaletteByLuma(inout VFXAttributes attributes, VFXGradient palette)
{
    float luma = Luminance(LinearToSRGB(attributes.color));
    attributes.color = SampleGradient(palette, luma * 2).rgb;
}

void ApplyPaletteBySearch(inout VFXAttributes attributes, VFXGradient palette)
{
    float3 c_org = attributes.color;
    float3 c_min = 0;
    float d_min = 1000;

    [unroll]
    for (int i = 0; i < 8; i++)
    {
        float3 c = SampleGradient(palette, (i + 0.5) / 8).rgb;
        float d = GetColorDistance(c_org, c);
        if (d < d_min)
        {
            c_min = c;
            d_min = d;
        }
    }

    attributes.color = c_min;
}
