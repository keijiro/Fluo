using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Fluo {

public sealed class InputAxisToFloatEvent : MonoBehaviour
{
    [SerializeField] AnimationCurve _curve = AnimationCurve.Linear(0, 0, 1, 1);
    [Space, SerializeField] InputAction _action = null;
    [Space, SerializeField] UnityEvent<float> _event = null;

    void OnUpdate(InputAction.CallbackContext context)
      => _event.Invoke(_curve.Evaluate(context.ReadValue<float>()));

    void OnEnable()
    {
        _action.started += OnUpdate;
        _action.performed += OnUpdate;
        _action.canceled += OnUpdate;
        _action.Enable();
    }

    void OnDisable()
    {
        _action.Disable();
        _action.started -= OnUpdate;
        _action.performed -= OnUpdate;
        _action.canceled -= OnUpdate;
    }
}

} // namespace Fluo
