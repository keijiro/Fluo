using UnityEngine;

namespace Fluo {

public sealed class DuotoneShuffler : MonoBehaviour
{
    [SerializeField] Vector2 _lowSL = new Vector2(100, 40);
    [SerializeField] Vector2 _highSL = new Vector2(100, 50);

    public void RandomizeColors()
    {
        var hue1 = Random.value * Mathf.PI * 2;
        var hue2 = Random.value * Mathf.PI * 2;
        _duotone.LowColor  = Pugrad.Hsluv.ToRgb(hue1, _lowSL.x, _lowSL.y);
        _duotone.HighColor = Pugrad.Hsluv.ToRgb(hue2, _highSL.x, _highSL.y);
    }

    Duotone.DuotoneController _duotone;

    void Start()
      => _duotone = GetComponent<Duotone.DuotoneController>();
}

} // namespace Fluo
