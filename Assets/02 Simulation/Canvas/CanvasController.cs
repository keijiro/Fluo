using UnityEngine;

namespace Fluo {

public sealed class CanvasController : MonoBehaviour
{
    [field:SerializeField] public float AlphaDecay { get; set; } = 1;

    void Update()
      => Shader.SetGlobalFloat(ShaderID.FluoCanvasAlphaDecay, AlphaDecay);
}

} // namespace Fluo
