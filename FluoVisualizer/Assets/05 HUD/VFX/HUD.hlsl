void SourceMonitor_float
  (UnityTexture2D sourceTex,
   float2 uv,
   float2 spos,
   bool alternate,
   out float output)
{
    const float bayer[] =
    {
        0.000000, 0.531250, 0.132812, 0.664062,
        0.796875, 0.265625, 0.929688, 0.398438,
        0.199219, 0.730469, 0.066406, 0.597656,
        0.996094, 0.464844, 0.863281, 0.332031,
    };

    // Dithering pattern
    uint2 ipos = (uint2)spos >> 1;
    float dither = bayer[(ipos.x % 4) * 4 + ipos.y % 4] - 0.5;

    // Source texture
    float4 src = tex2D(sourceTex, uv);
    float lv = Luminance(LinearToSRGB(src.rgb));
    lv = lerp(lv, src.a, alternate);

    // Binary value
    bool bin = ((lv + dither * 0.75) > 0.5) && all(ipos & 1);

    // Border lines
    float2 uv2 = min(uv, 1 - uv);
    float2 pt = uv2 / fwidth(uv2);
    float bline = 1 - min(pt.x, pt.y);

    output = max(bin, bline);
}

float LogoSampler(UnityTexture2D tex1, UnityTexture2D tex2, float2 uv, float t)
{
    if (uv.y < t)
    {
        return tex2D(tex2, float2(uv.x, uv.y / t)).r;
    }
    else
    {
        return tex2D(tex1, float2(uv.x, (uv.y - t) / (1 - t))).r;
    }
}

void LogoScroller_float
  (UnityTexture2D logo1Tex,
   UnityTexture2D logo2Tex,
   UnityTexture2D logo3Tex,
   UnityTexture2D logo4Tex,
   float2 uv,
   out float output)
{
    float t = _Time.y / 8;

    uint idx = ((uint)t) & 3;

    float disp = smoothstep(0.8, 1, frac(t));
    disp = smoothstep(0, 1, smoothstep(0, 1, disp));

    float alpha;

    if (idx == 0)
    {
        alpha = LogoSampler(logo1Tex, logo2Tex, uv, disp);
    }
    else if (idx == 1)
    {
        alpha = LogoSampler(logo2Tex, logo3Tex, uv, disp);
    }
    else if (idx == 2)
    {
        alpha = LogoSampler(logo3Tex, logo4Tex, uv, disp);
    }
    else
    {
        alpha = LogoSampler(logo4Tex, logo1Tex, uv, disp);
    }

    output = alpha;
}
