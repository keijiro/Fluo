using UnityEngine;
using Lasp;

namespace Fluo {

public sealed class AudioToShaderProps : MonoBehaviour
{
    [field:SerializeField, Range(0, 1)]
    public float Amplitude { get; set; } = 1;

    [SerializeField] AudioLevelTracker _trackerX = null;
    [SerializeField] AudioLevelTracker _trackerY = null;
    [SerializeField] AudioLevelTracker _trackerZ = null;
    [SerializeField] AudioLevelTracker _trackerW = null;

    void Update()
    {
        var x = _trackerX.normalizedLevel;
        var y = _trackerY.normalizedLevel;
        var z = _trackerZ.normalizedLevel;
        var w = _trackerW.normalizedLevel;
        Shader.SetGlobalVector(ShaderID.FluoAudioLevel, new Vector4(x, y, z, w) * Amplitude);
    }
}

} // namespace Fluo
