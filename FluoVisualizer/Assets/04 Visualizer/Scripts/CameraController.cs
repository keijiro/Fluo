using UnityEngine;
using Unity.Mathematics;
using Klak.Motion;
using System;

namespace Fluo {

public sealed class CameraController : MonoBehaviour
{
    #region Public properties

    [field:SerializeField, Range(0, 1)]
    public float DistanceParameter { get; set; } = 0.5f;

    [field:SerializeField, Range(0, 1)]
    public float AngleParameter { get; set; } = 0.5f;

    #endregion

    #region Scene object references

    [Space]
    [SerializeField] Transform _targetBase = null;
    [SerializeField] BrownianMotion _targetMotion = null;
    [SerializeField] Transform _rotationBase = null;
    [SerializeField] BrownianMotion _rotationMotion = null;
    [SerializeField] Transform _distanceBase = null;
    [SerializeField] BrownianMotion _distanceMotion = null;

    #endregion

    #region Settings

    [Serializable]
    struct AngleSetting
    {
        public float baseAngle;
        public Vector3 rotation;
    }

    [Space]
    [SerializeField] Vector2[] _distanceRanges = null;
    [SerializeField] AngleSetting[] _angleSettings = null;

    #endregion

    #region Parameter application

    void ApplyDistanceSettings(float parameter)
    {
        var count = _distanceRanges.Length;
        parameter *= count - 1;
        var index0 = (int)math.floor(parameter);
        var index1 = math.min(index0 + 1, count - 1);
        ApplyLerpedDistanceSettings(index0, index1, parameter - index0);
    }

    void ApplyAngleSettings(float parameter)
    {
        var count = _angleSettings.Length;
        parameter *= count - 1;
        var index0 = (int)math.floor(parameter);
        var index1 = math.min(index0 + 1, count - 1);
        ApplyLerpedAngleSettings(index0, index1, parameter - index0);
    }

    void ApplyLerpedDistanceSettings(int index0, int index1, float t)
    {
        var range = math.lerp(_distanceRanges[index0], _distanceRanges[index1], t);
        var z = (range.x + range.y) * -0.5f;
        var dz = (range.y - range.x) * 0.5f;
        _distanceBase.localPosition = new Vector3(0, 0, z);
        _distanceMotion.positionAmount = new Vector3(0, 0, dz);
    }

    void ApplyLerpedAngleSettings(int index0, int index1, float t)
    {
        ref var setting0 = ref _angleSettings[index0];
        ref var setting1 = ref _angleSettings[index1];

        var baseAngle = math.lerp(setting0.baseAngle, setting1.baseAngle, t);
        var rotation = math.lerp(setting0.rotation, setting1.rotation, t);

        _rotationBase.localRotation = Quaternion.AngleAxis(90 - baseAngle, Vector3.right);
        _rotationMotion.rotationAmount = rotation;
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        // FIXME: Just for suppressing the warning.
        _targetBase.position = Vector3.zero;
        _targetMotion.positionAmount = Vector3.zero;
    }

    void Update()
    {
        ApplyDistanceSettings(DistanceParameter);
        ApplyAngleSettings(AngleParameter);
    }

    #endregion
}

} // namespace Fluo
