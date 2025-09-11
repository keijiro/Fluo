void EffectMain_float(float4 input, float2 spos, bool alternate, out float output)
{
    uint2 ipos = (uint2)spos >> 1;

    const float bayer[] =
    {
        0.000000, 0.531250, 0.132812, 0.664062,
        0.796875, 0.265625, 0.929688, 0.398438,
        0.199219, 0.730469, 0.066406, 0.597656,
        0.996094, 0.464844, 0.863281, 0.332031,
    };

    float dither = bayer[(ipos.x % 4) * 4 + ipos.y % 4] - 0.5;

    float lv = dot(LinearToSRGB(input.rgb), 1.0 / 3);
    lv = lerp(lv, input.a, alternate);

    output = ((lv + dither * 0.5) > 0.5) * all(ipos & 1);
}


