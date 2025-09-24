using UnityEngine;

namespace Fluo {

public sealed class MonitorController : MonoBehaviour
{
    [field:SerializeField, Range(0, 1)]
    public float EffectIntensity { get; set; } = 0;

    void Update()
      => Shader.SetGlobalFloat(ShaderID.FluoMonitorEffect, EffectIntensity);
}

} // namespace Fluo
