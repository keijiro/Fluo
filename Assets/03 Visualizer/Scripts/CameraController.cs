using UnityEngine;
using Klak.Motion;

namespace Fluo {

public sealed class CameraController : MonoBehaviour
{
    [SerializeField] Transform _targetBase = null;
    [SerializeField] BrownianMotion _targetMotion = null;
    [SerializeField] Transform _rotationBase = null;
    [SerializeField] BrownianMotion _rotationMotion = null;
    [SerializeField] Transform _distanceBase = null;
    [SerializeField] BrownianMotion _distanceMotion = null;
    [Space]
    [SerializeField] Vector2[] _distanceRanges = null;

    public void SetAngleMode(int mode)
    {
    }

    public void SetDistanceMode(int mode)
    {
        var range = _distanceRanges[mode];
        var z = (range.x + range.y) * -0.5f;
        var dz = (range.y - range.x) * 0.5f;
        _distanceBase.localPosition = new Vector3(0, 0, z);
        _distanceMotion.positionAmount = new Vector3(0, 0, dz);
    }
}

} // namespace Fluo
