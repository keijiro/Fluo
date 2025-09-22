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

    #region Camera settings

    [Serializable]
    struct DistanceSetting
    {
        public float2 distanceMinMax;
        public float2 slideRange;
    }

    [Serializable]
    struct AngleSetting
    {
        public float baseAngle;
        public Vector3 swingRange;
    }

    [Space]
    [SerializeField] DistanceSetting[] _distanceSettings = null;
    [SerializeField] AngleSetting[] _angleSettings = null;

    #endregion

    #region Parameter application

    void ApplyDistanceSettings(float parameter)
    {
        var count = _distanceSettings.Length;
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
        ref var setting0 = ref _distanceSettings[index0];
        ref var setting1 = ref _distanceSettings[index1];

        var minmax = math.lerp(setting0.distanceMinMax, setting1.distanceMinMax, t);
        var (z, dz) = ((minmax.x + minmax.y) * -0.5f, (minmax.y - minmax.x) * 0.5f);

        _distanceBase.localPosition = new Vector3(0, 0, z);
        _distanceMotion.positionAmount = new Vector3(0, 0, dz);
        _targetMotion.positionAmount = math.float3(math.lerp(setting0.slideRange, setting1.slideRange, t), 0).xzy;
    }

    void ApplyLerpedAngleSettings(int index0, int index1, float t)
    {
        ref var setting0 = ref _angleSettings[index0];
        ref var setting1 = ref _angleSettings[index1];

        var baseAngle = math.lerp(setting0.baseAngle, setting1.baseAngle, t);
        var swingRange = math.lerp(setting0.swingRange, setting1.swingRange, t);

        _rotationBase.localRotation = Quaternion.AngleAxis(90 - baseAngle, Vector3.right);
        _rotationMotion.rotationAmount = swingRange;
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
      => _targetBase.position = Vector3.zero; // FIXME: Just for suppressing the warning.

    void Update()
    {
        ApplyDistanceSettings(DistanceParameter);
        ApplyAngleSettings(AngleParameter);
    }

    #endregion
}

} // namespace Fluo
