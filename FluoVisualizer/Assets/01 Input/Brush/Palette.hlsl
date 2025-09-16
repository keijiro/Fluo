#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

float3 SelectColorByLuma(float3 input, VFXGradient palette, uint seed)
{
    float luma = Luminance(LinearToSRGB(input));
    float disp = (GenerateHashedRandomFloat(uint2(seed, 100)) - 0.5) * 0.2;
    return SampleGradient(palette, luma + disp).rgb;
}

float3 PickNearestColor(float3 input, VFXGradient palette, uint seed)
{
    float3 c0 = LinearToSRGB(input);
    float3 c_min = 0;
    float d_min = 1000;

    for (int i = 0; i < 6; i++)
    {
        float x = GenerateHashedRandomFloat(uint2(seed, i));
        float3 c = LinearToSRGB(SampleGradient(palette, x).rgb);
        float d = distance(c0, c);
        if (d < d_min)
        {
            c_min = c;
            d_min = d;
        }
    }

    return SRGBToLinear(c_min);
}
