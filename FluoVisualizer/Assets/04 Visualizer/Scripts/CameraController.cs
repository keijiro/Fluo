using UnityEngine;
using Unity.Mathematics;
using Klak.Math;
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

    [field:SerializeField]
    public float TweenSpeed { get; set; } = 1;

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
        public float baseOffset;
        public Vector3 swingRange;
    }

    [Space]
    [SerializeField] DistanceSetting[] _distanceSettings = null;
    [SerializeField] AngleSetting[] _angleSettings = null;

    #endregion

    #region Parameter application

    (int i0, int i1, float t) GetLerpParams(float parameter, int count)
    {
        parameter *= count - 1;
        var i0 = (int)math.floor(parameter);
        var i1 = math.min(i0 + 1, count - 1);
        return (i0, i1, parameter - i0);
    }

    void ApplyDistanceSettings(float parameter)
    {
        var p = GetLerpParams(parameter, _distanceSettings.Length);

        ref var s0 = ref _distanceSettings[p.i0];
        ref var s1 = ref _distanceSettings[p.i1];

        var range = math.lerp(s0.distanceMinMax, s1.distanceMinMax, p.t);
        var slide = math.lerp(s0.slideRange, s1.slideRange, p.t);

        var  z = (range.x + range.y) * -0.5f;
        var dz = (range.y - range.x) *  0.5f;

        _targetMotion.positionAmount = math.float3(slide, 0).xzy;
        _distanceBase.localPosition = new Vector3(0, 0, z);
        _distanceMotion.positionAmount = new Vector3(0, 0, dz);
    }

    void ApplyAngleSettings(float parameter)
    {
        var p = GetLerpParams(parameter, _angleSettings.Length);

        ref var s0 = ref _angleSettings[p.i0];
        ref var s1 = ref _angleSettings[p.i1];

        var baseAngle = math.lerp(s0.baseAngle, s1.baseAngle, p.t);
        var baseOffset = math.lerp(s0.baseOffset, s1.baseOffset, p.t);
        var swingRange = math.lerp(s0.swingRange, s1.swingRange, p.t);

        _targetBase.localPosition = new Vector3(0, 0, -baseOffset);
        _rotationBase.localRotation = Quaternion.AngleAxis(90 - baseAngle, Vector3.right);
        _rotationMotion.rotationAmount = swingRange;
    }

    #endregion

    #region Parameters for tweening

    (float x, float v) _distanceParam;
    (float x, float v) _angleParam;

    #endregion

    #region MonoBehaviour implementation

    void Update()
    {
        _distanceParam = CdsTween.Step(_distanceParam, DistanceParameter, TweenSpeed);
        _angleParam = CdsTween.Step(_angleParam, AngleParameter, TweenSpeed);

        ApplyDistanceSettings(_distanceParam.x);
        ApplyAngleSettings(_angleParam.x);
    }

    #endregion
}

} // namespace Fluo
