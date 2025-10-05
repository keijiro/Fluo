using UnityEngine;

namespace Fluo {

static class ShaderID
{
    public static readonly int Alternate = Shader.PropertyToID("_Alternate");
    public static readonly int BodyPixTex = Shader.PropertyToID("_BodyPixTex");
    public static readonly int Effect = Shader.PropertyToID("_Effect");
    public static readonly int FluoCanvasAlphaDecay = Shader.PropertyToID("_Fluo_CanvasAlphaDecay");
    public static readonly int FluoMonitorEffect = Shader.PropertyToID("_Fluo_MonitorEffect");
    public static readonly int LutBlend = Shader.PropertyToID("_LutBlend");
    public static readonly int LutTex = Shader.PropertyToID("_LutTex");
}

} // namespace Fluo
