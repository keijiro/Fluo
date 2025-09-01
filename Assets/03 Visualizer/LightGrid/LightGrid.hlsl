void LightGrid_float
    (UnityTexture2D source,
     float2 uv,
     float2 grid,
     float dotSize,
     out float3 outColor,
     out float alpha)
{
    // Grid coordinates / index
    float2 gc = uv * grid;
    float2 idx = floor(gc);

    // Color element selector
    float sel = frac(idx.x / 4);
    float3 mask = sel < 1.0 / 4 ? float3(1, 0, 0) :
                  sel < 2.0 / 4 ? float3(0, 1, 0) :
                  sel < 3.0 / 4 ? float3(0, 0, 1) : 0;

    // Color sample with quantized UV
    float2 q_uv = idx / grid;
    float4 src = SAMPLE_TEXTURE2D(source.tex, source.samplerstate, q_uv);

    // Distance from element edge
    float size = dotSize * 0.3;
    float dist = length(max(0, abs(frac(gc) - 0.5) - size));

    // Light level
    float level = 1 - smoothstep(0.1, 0.2, dist);

    // Vertical Shade
    float shade = saturate((frac(gc).y - 0.5) / (size + 0.5) + 0.5);

    outColor = src.rgb * mask * level * shade;
    alpha = level;
}
