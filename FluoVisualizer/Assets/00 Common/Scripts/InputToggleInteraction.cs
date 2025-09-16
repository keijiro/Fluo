using UnityEngine;
using UnityEngine.InputSystem;

namespace Fluo {

#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
#endif
public sealed class InputToggleInteraction : IInputInteraction
{
    bool _state;

    public void Reset()
      => _state = false;

    public void Process(ref InputInteractionContext context)
    {
        if (!context.ControlIsActuated()) return;
        if (context.isStarted) return;

        _state = !_state;

        if (_state)
        {
            context.Started();
            context.PerformedAndStayPerformed();
        }
        else
        {
            context.Canceled();
        }
    }

#if UNITY_EDITOR
    static InputToggleInteraction()
      => InputSystem.RegisterInteraction<InputToggleInteraction>();
#else
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
      => InputSystem.RegisterInteraction<InputToggleInteraction>();
#endif
}

} // namespace Fluo