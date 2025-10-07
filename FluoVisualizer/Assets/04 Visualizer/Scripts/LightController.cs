using UnityEngine;
using Pugrad;

namespace Fluo {

public sealed class LightController : MonoBehaviour
{
    [field:SerializeField, Range(0, 1)]
    public float Amplitude { get; set; } = 1;

    [field:SerializeField]
    public float HueCycle { get; set; } = 0;

    Light _light;
    float _initialIntensity;

    void Start()
    {
        _light = GetComponent<Light>();
        _initialIntensity = _light.intensity;
    }

    void Update()
    {
        _light.enabled = Amplitude > 0.01f;
        _light.intensity = _initialIntensity * Amplitude;

        if (HueCycle > 0)
        {
            var h = (Time.time / HueCycle % 1) * Mathf.PI * 2;
            _light.color = Hsluv.ToRgb(h, 100, 80);
        }
    }
}

} // namespace Fluo
