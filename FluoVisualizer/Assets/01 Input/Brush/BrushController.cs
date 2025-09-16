using UnityEngine;
using UnityEngine.VFX;
using Unity.Mathematics;
using Random = UnityEngine.Random;

namespace Fluo {

public sealed class BrushController : MonoBehaviour
{
    void Start()
    {
        var colorKeys = new GradientColorKey[8];

        for (var i = 0; i < 8; i++)
        {
            var t = (i + 1) / 8.0f;

            var h = Random.value * math.PI * 2;;
            var s = 120.0f;
            var v = t * t * t * 59 + 1;

            HsluvHelper.HsluvToRgb(math.float3(h, s, v), out float3 rgb);
            var c = new Color(rgb.x, rgb.y, rgb.z);

            colorKeys[i] = new GradientColorKey(c, t);
        }

        var g = new Gradient();
        g.mode = GradientMode.Fixed;
        g.colorKeys = colorKeys;

        GetComponent<VisualEffect>().SetGradient("Palette", g);
    }
}

} // namespace Fluo
