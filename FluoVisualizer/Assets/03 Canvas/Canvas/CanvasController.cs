using UnityEngine;

namespace Fluo {

public sealed class CanvasController : MonoBehaviour
{
    [SerializeField] CustomRenderTexture _target = null;

    [field:SerializeField] public float AlphaDecay { get; set; } = 1;

    void Start()
      => _target.Initialize();

    void Update()
    {
        Shader.SetGlobalFloat(ShaderID.FluoCanvasAlphaDecay, AlphaDecay);
        _target.Update();
    }
}

} // namespace Fluo
