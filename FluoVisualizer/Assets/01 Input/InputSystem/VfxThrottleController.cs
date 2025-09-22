using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

namespace Fluo {

public sealed class VfxThrottleController : MonoBehaviour
{
    [SerializeField] VisualEffect _target = null;
    [SerializeField] string _propertyName = "Throttle";
    [Space, SerializeField] InputAction _throttleSource = null;
    [Space, SerializeField] InputAction _toggleButton = null;

    bool _toggleState;

    void OnThrottled(InputAction.CallbackContext context)
      => _target.SetFloat(_propertyName, context.ReadValue<float>());

    void OnToggled(InputAction.CallbackContext context)
      => _target.SetFloat(_propertyName, (_toggleState = !_toggleState) ? 1 : 0);

    void OnEnable()
    {
        _throttleSource.started += OnThrottled;
        _throttleSource.performed += OnThrottled;
        _throttleSource.canceled += OnThrottled;
        _throttleSource.Enable();

        _toggleButton.performed += OnToggled;
        _toggleButton.Enable();
    }

    void OnDisable()
    {
        _throttleSource.started -= OnThrottled;
        _throttleSource.performed -= OnThrottled;
        _throttleSource.canceled -= OnThrottled;
        _throttleSource.Disable();

        _toggleButton.Disable();
        _toggleButton.performed -= OnToggled;
    }
}

} // namespace Fluo
