using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Fluo {

public sealed class TriggerToEvent : MonoBehaviour
{
    [Space, SerializeField] InputAction _action = null;
    [Space, SerializeField] UnityEvent _event = null;

    void OnEnable()
    {
        _action.performed += OnTriggered;
        _action.Enable();
    }

    void OnDisable()
    {
        _action.Disable();
        _action.performed -= OnTriggered;
    }

    void OnTriggered(InputAction.CallbackContext context)
      => _event.Invoke();
}

} // namespace Fluo
