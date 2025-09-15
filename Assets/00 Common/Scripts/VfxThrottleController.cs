using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

namespace Fluo {

public sealed class VfxThrottleController : MonoBehaviour
{
    [SerializeField] VisualEffect _target = null;
    [Space, SerializeField] InputAction _throttleSource = null;
    [Space, SerializeField] InputAction _toggleButton = null;

    bool _toggleState;

    void OnPerformed(InputAction.CallbackContext context)
      => _toggleState = !_toggleState;

    void OnEnable()
    {
        _throttleSource.Enable();

        _toggleButton.performed += OnPerformed;
        _toggleButton.Enable();
    }

    void OnDisable()
    {
        _throttleSource.Disable();

        _toggleButton.Disable();
        _toggleButton.performed -= OnPerformed;
    }

    void Update()
    {
        var t = _toggleState ? 1.0f : _throttleSource.ReadValue<float>();
        _target.SetFloat("Throttle", t);
    }
}

} // namespace Fluo
