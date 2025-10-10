using UnityEngine;
using UnityEngine.VFX;

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

            var h = Random.value * Mathf.PI * 2;
            var s = 100.0f;
            var v = t * t * t * 59 + 1;

            var c = Pugrad.Hsluv.ToRgb(h, s, v);
            _colorKeys[i] = new GradientColorKey(c, t);
        }

        _palette.colorKeys = _colorKeys;

        GetComponent<VisualEffect>().SetGradient("Palette", _palette);
    }

    void Start()
      => RandomizePalette();
}

} // namespace Fluo
