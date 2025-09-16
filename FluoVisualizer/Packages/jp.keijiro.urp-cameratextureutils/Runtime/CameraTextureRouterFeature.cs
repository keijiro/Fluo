using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace CameraTextureUtils {

sealed class CameraTextureRouterPass : ScriptableRenderPass
{
    // Per-pass data transferred to the render function
    class PassData
    {
        public TextureHandle cameraDepth;
        public TextureHandle motionVector;
        public Material material;
        public int depthEncoding;
        public int motionEncoding;
        public int passIndex; // 0=Depth, 1=Motion, 2=MRT
    }

    Material _material;

    public CameraTextureRouterPass(Material material)
    {
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Motion);

        _material = material;
    }

    public void Cleanup()
      => _material = null;

    public override void RecordRenderGraph(RenderGraph graph, ContextContainer frameData)
    {
        if (_material == null) return;

        // Fetch targets/encoding from the per-camera component
        var cam = frameData.Get<UniversalCameraData>().camera;
        var controller = cam != null ? cam.GetComponent<CameraTextureRouter>() : null;
        if (controller == null || !controller.enabled || !controller.IsReady) return;

        var resrc = frameData.Get<UniversalResourceData>();
        var (depthHandle, depthInfo) = controller.DepthOutput;
        var (motionHandle, motionInfo) = controller.MotionOutput;

        // Start a raster pass and attach 1 or 2 color targets depending on availability
        using var builder = graph.AddRasterRenderPass<PassData>("Camera Texture Router", out var passData);

        var (passNum, targetIndex) = (0, 0);

        // Depth output (optional)
        if (depthHandle != null)
        {
            var src = resrc.cameraDepthTexture;
            var dest = graph.ImportTexture(depthHandle, depthInfo);
            passNum |= 1;

            passData.cameraDepth = src;
            builder.UseTexture(src, AccessFlags.Read);
            builder.SetRenderAttachment(dest, targetIndex++, AccessFlags.Write);
        }

        // Motion output (optional)
        if (motionHandle != null)
        {
            var src = resrc.motionVectorColor;
            var dest = graph.ImportTexture(motionHandle, motionInfo);
            passNum |= 2;

            passData.motionVector = src;
            builder.UseTexture(src, AccessFlags.Read);
            builder.SetRenderAttachment(dest, targetIndex++, AccessFlags.Write);
        }

        passData.material = _material;
        passData.depthEncoding = (int)controller.GetDepthEncoding();
        passData.motionEncoding = (int)controller.GetMotionEncoding();
        passData.passIndex = passNum - 1; // Map bitmask to pass index

        builder.SetRenderFunc<PassData>(ExecutePass);
    }

    static void ExecutePass(PassData data, RasterGraphContext ctx)
    {
        data.material.SetInt("_DepthEncoding", data.depthEncoding);
        data.material.SetInt("_MotionEncoding", data.motionEncoding);
        data.material.SetTexture("_CameraDepthTexture", data.cameraDepth);
        data.material.SetTexture("_MotionVectorTexture", data.motionVector);
        CoreUtils.DrawFullScreen(ctx.cmd, data.material, null, data.passIndex);
    }
}

public sealed class CameraTextureRouterFeature : ScriptableRendererFeature
{
    [SerializeField, HideInInspector] Shader _shader = null;

    Material _material;
    CameraTextureRouterPass _pass;

    void OnDestroy()
    {
        _pass?.Cleanup();
        if (_material != null) CoreUtils.Destroy(_material);
        (_material, _pass) = (null, null);
    }

    public override void Create()
    {
        _material = CoreUtils.CreateEngineMaterial(_shader);
        _pass = new CameraTextureRouterPass(_material);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData data)
      => renderer.EnqueuePass(_pass);
}

} // namespace CameraTextureUtils
