using UnityEngine;
using UnityEngine.VFX;
using Unity.Mathematics;
using Random = UnityEngine.Random;

using System;

namespace Fluo {

public sealed class BrushController : MonoBehaviour
{
    Gradient _palette = new Gradient() { mode = GradientMode.Fixed };
    GradientColorKey[] _colorKeys = new GradientColorKey[8];

    public void RandomizePalette()
    {
        for (var i = 0; i < _colorKeys.Length; i++)
        {
            var t = (i + 1.0f) / _colorKeys.Length;

            var h = Random.value * math.PI * 2;;
            var s = 100.0f;
            var v = t * t * t * 59 + 1;

            HsluvHelper.HsluvToRgb(math.float3(h, s, v), out float3 rgb);
            var c = new Color(rgb.x, rgb.y, rgb.z);

            _colorKeys[i] = new GradientColorKey(c, t);
        }

        _palette.colorKeys = _colorKeys;

        GetComponent<VisualEffect>().SetGradient("Palette", _palette);
    }

    void Start()
      => RandomizePalette();
}

} // namespace Fluo
