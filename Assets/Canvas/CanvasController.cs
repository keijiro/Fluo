using UnityEngine;

namespace Fluo {

public sealed class CanvasController : MonoBehaviour
{
    [SerializeField] CustomRenderTexture _target = null;

    void Start()
      => _target.Initialize();

    void Update()
      => _target.Update();
}

} // namespace Fluo
