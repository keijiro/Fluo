float4 LightGrid(UnityTexture2D source, float2 uv)
{
    const float2 grid = float2(640, 90);
    const float dotSize = 0.3;

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

    return float4(src.rgb * mask * level * shade, level);
}

void CompositorCore_float
(
    UnityTexture2D sourceTex,
    UnityTexture2D canvasTex,
    UnityTexture2D blurTex,
    UnityTexture2D velocityTex,
    float2 uv,
    float innerScale,
    float soften,
    float3 bgTint,
    float3 lightTint,
    out float3 outAlbedo,
    out float3 outEmission,
    out float3 outNormal
)
{
    float2 uv_i = (uv - 0.5) / innerScale + 0.5;

    float4 c_o = tex2D(blurTex, uv);
    float4 c_i = tex2D(canvasTex, uv_i);
    float4 c_l = LightGrid(sourceTex, uv_i) * float4(lightTint, 1);

    float fade = saturate(1 - length(max(0, abs(uv_i * 2 - 1) - 1 + soften)) / soften);

    bool inside = all(uv_i > 0 && uv_i < 1);
    float alpha = smoothstep(0, 0.04, dot(c_i.rgb, 1)) * inside;
    alpha *= 0.9;

    outAlbedo = c_i.rgb * fade;

    outEmission = c_o * bgTint;
    outEmission = lerp(outEmission, c_l.rgb, fade);
    outEmission *= 1 - alpha * fade;

    float2 duv = velocityTex.texelSize.xy * 1;
    float h1 = length(tex2D(velocityTex, uv_i - float2(duv.x, 0)).xy);
    float h2 = length(tex2D(velocityTex, uv_i + float2(duv.x, 0)).xy);
    float h3 = length(tex2D(velocityTex, uv_i - float2(0, duv.y)).xy);
    float h4 = length(tex2D(velocityTex, uv_i + float2(0, duv.y)).xy);
    float2 v = float2(h2 - h1, h4 - h3);

    outNormal = normalize(float3(v * 300 * alpha * fade, 1));
}
