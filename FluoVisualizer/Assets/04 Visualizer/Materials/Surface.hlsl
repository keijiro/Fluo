// Light grid (LED) pattern generator
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

// Pseudo bump node
void AlbedoToNormal_float(float3 albedo, float bumpiness, out float3 outNormal)
{
    float level = length(albedo);
    float2 grad = float2(ddx(level), ddy(level));
    outNormal = normalize(float3(-grad * bumpiness, 1));
}

// Compositor core implementation
void CompositorCore_float
(
    UnityTexture2D sourceTex,
    UnityTexture2D canvasTex,
    UnityTexture2D bgTex,
    UnityTexture2D velocityTex,
    float2 uv,
    float innerScale,
    float soften,
    float2 opacityParams,
    float2 transParams,
    float3 bgTint,
    float3 lightTint,
    out float3 outAlbedo,
    out float3 outEmission
)
{
    // UV for inner rectangle
    float2 uv_inner = (uv - 0.5) / innerScale + 0.5;

    // Color samples
    float4 c_bg = tex2D(bgTex, uv);
    float4 c_light = LightGrid(sourceTex, uv_inner);
    float4 c_canvas = tex2D(canvasTex, uv_inner);

    // Fading parameter (soften edges)
    float fade_dist = length(max(0, abs(uv_inner * 2 - 1) - 1 + soften));
    float fade = saturate(1 - fade_dist / soften);

    // Canvas layer opacity
    float opacity = saturate(c_canvas.a / opacityParams.y) * opacityParams.x;

    // Albedo from canvas layer
    outAlbedo = c_canvas.rgb * opacity * fade;

    // Emission: BG layer / Light grid layer / Canvas layer
    float3 e_bg = c_bg.rgb * bgTint;
    float3 e_light = c_light.rgb * lightTint;
    float3 e_canvas = (c_light.rgb * transParams.x + transParams.y) * c_canvas.rgb;
    outEmission = lerp(e_bg, lerp(e_light, e_canvas, opacity), fade);
}
