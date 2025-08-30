using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace CameraTextureUtils {

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public sealed class CameraTextureRouter : MonoBehaviour
{
    #region Serialized Fields

    public enum DepthEncoding { RawBuffer = 0, Linear01 = 1, LinearEye = 2 }
    public enum MotionEncoding { Signed = 0, Centered01 = 1 }

    [SerializeField] RenderTexture _depthTarget = null;
    [SerializeField] RenderTexture _motionTarget = null;

    [SerializeField] DepthEncoding _depthEncoding = DepthEncoding.RawBuffer;
    [SerializeField] MotionEncoding _motionEncoding = MotionEncoding.Signed;

    #endregion

    #region Public API

    public RenderTexture DepthTarget { get => _depthTarget; set => SetDepthTarget(value); }
    public RenderTexture MotionTarget { get => _motionTarget; set => SetMotionTarget(value); }

    public (RTHandle, RenderTargetInfo) DepthOutput => _depth;
    public (RTHandle, RenderTargetInfo) MotionOutput => _motion;

    public bool IsReady => DepthTarget != null || MotionTarget != null;

    public DepthEncoding GetDepthEncoding() => _depthEncoding;
    public MotionEncoding GetMotionEncoding() => _motionEncoding;

    #endregion

    #region Runtime State

    (RTHandle handle, RenderTargetInfo info) _depth;
    (RTHandle handle, RenderTargetInfo info) _motion;

    #endregion

    #region Property Setters

    void SetDepthTarget(RenderTexture target)
    {
        if (_depthTarget == target) return;
        _depthTarget = target;
        UpdateDepthOutput();
    }

    void SetMotionTarget(RenderTexture target)
    {
        if (_motionTarget == target) return;
        _motionTarget = target;
        UpdateMotionOutput();
    }

    void UpdateDepthOutput()
    {
        _depth.handle?.Release();
        _depth = CreateOutput(_depthTarget, "DepthOutput");
    }

    void UpdateMotionOutput()
    {
        _motion.handle?.Release();
        _motion = CreateOutput(_motionTarget, "MotionOutput");
    }

    #endregion

    #region Unity Lifecycle

    void OnEnable() => UpdateOutputs();
    void OnDisable() => ReleaseOutputs();
    void OnDestroy() => ReleaseOutputs();
    void OnValidate() { ReleaseOutputs(); UpdateOutputs(); }

    #endregion

    #region Internal Helpers

    void UpdateOutputs()
    {
        UpdateDepthOutput();
        UpdateMotionOutput();
    }

    void ReleaseOutputs()
    {
        _depth.handle?.Release();
        _depth = (null, default);
        _motion.handle?.Release();
        _motion = (null, default);
    }

    static (RTHandle handle, RenderTargetInfo info)
      CreateOutput(RenderTexture target, string name)
    {
        if (target == null) return (null, default);
        var handle = RTHandles.Alloc(target, name);
        var info = new RenderTargetInfo
        {
            format = target.graphicsFormat,
            width = target.width,
            height = target.height,
            volumeDepth = target.volumeDepth,
            msaaSamples = 1,
            bindMS = target.bindTextureMS
        };
        return (handle, info);
    }

    #endregion
}

} // namespace CameraTextureUtils
