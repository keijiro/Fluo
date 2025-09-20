#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

void SetRandomVelocity(inout VFXAttributes attributes, float amplitude, float2 rand)
{
    attributes.velocity = float3((rand - 0.5) * 2 * amplitude * float2(16, 9) / 9, 0);
}

void UpdateColor(inout VFXAttributes attributes, float cycle, float hueWidth, float2 rand)
{
    float hue = frac(_Time.y / cycle + rand.x * hueWidth);
    attributes.color = SRGBToLinear(HsvToRgb(float3(hue, 1, rand.y)));
}

#if defined(VFX_USE_SIZE_CURRENT)

void ApplyThrottleToSize(inout VFXAttributes attributes, float throttle, uint maxCount)
{
    uint count = round(throttle * maxCount);
    attributes.size *= attributes.particleId < count;
}

#endif
