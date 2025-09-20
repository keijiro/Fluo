using UnityEngine;
using Klak.Motion;
using System;

namespace Fluo {

public sealed class CameraController : MonoBehaviour
{
    [Serializable]
    struct AngleSetting
    {
        public float baseAngle;
        public Vector3 rotation;
    }

    [SerializeField] Transform _targetBase = null;
    [SerializeField] BrownianMotion _targetMotion = null;
    [SerializeField] Transform _rotationBase = null;
    [SerializeField] BrownianMotion _rotationMotion = null;
    [SerializeField] Transform _distanceBase = null;
    [SerializeField] BrownianMotion _distanceMotion = null;
    [Space]
    [SerializeField] Vector2[] _distanceRanges = null;
    [SerializeField] AngleSetting[] _angleSettings = null;

    public void SetDistanceMode(int mode)
    {
        var range = _distanceRanges[mode];
        var z = (range.x + range.y) * -0.5f;
        var dz = (range.y - range.x) * 0.5f;
        _distanceBase.localPosition = new Vector3(0, 0, z);
        _distanceMotion.positionAmount = new Vector3(0, 0, dz);
    }

    public void SetAngleMode(int mode)
    {
        ref var setting = ref _angleSettings[mode];

        _rotationBase.localRotation =
          Quaternion.AngleAxis(90 - setting.baseAngle, Vector3.right);

        _rotationMotion.rotationAmount = setting.rotation;
    }

    void Start()
    {
        SetDistanceMode(1);
        SetAngleMode(1);

        // FIXME: Just for suppressing the warning.
        _targetBase.position = Vector3.zero;
        _targetMotion.positionAmount = Vector3.zero;
    }
}

} // namespace Fluo
