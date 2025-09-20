using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Fluo {

public sealed class InputPressToEvent : MonoBehaviour
{
    [Space, SerializeField] InputAction _action = null;
    [Space, SerializeField] UnityEvent _onEvent = null;
    [Space, SerializeField] UnityEvent _offEvent = null;

    void OnTriggered(InputAction.CallbackContext context)
      => _onEvent.Invoke();

    void OnCanceled(InputAction.CallbackContext context)
      => _offEvent.Invoke();

    void OnEnable()
    {
        _action.performed += OnTriggered;
        _action.canceled += OnCanceled;
        _action.Enable();
    }

    void OnDisable()
    {
        _action.Disable();
        _action.performed -= OnTriggered;
        _action.canceled -= OnCanceled;
    }
}

} // namespace Fluo
