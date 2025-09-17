using UnityEngine;
using UnityEngine.UIElements;
using VJUITK;

namespace Fluo {

[RequireComponent(typeof(UIDocument))]
[RequireComponent(typeof(InputHandle))]
public sealed class InputLinker : MonoBehaviour
{
    #region Private members

    VJButton[] _buttons;
    VJToggle[] _toggles;
    VJKnob[] _knobs;

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _buttons = new VJButton[InputHandle.ButtonCount];
        _toggles = new VJToggle[InputHandle.ToggleCount];
        _knobs = new VJKnob[InputHandle.KnobCount];

        var root = GetComponent<UIDocument>().rootVisualElement;

        for (var i = 0; i < InputHandle.ButtonCount; i++)
            _buttons[i] = root.Q<VJButton>($"button-{i}");

        for (var i = 0; i < InputHandle.ToggleCount; i++)
            _toggles[i] = root.Q<VJToggle>($"toggle-{i}");

        for (var i = 0; i < InputHandle.KnobCount; i++)
            _knobs[i] = root.Q<VJKnob>($"knob-{i}");
    }

    void LateUpdate()
    {
        var handle = GetComponent<InputHandle>();

        for (var i = 0; i < InputHandle.ButtonCount; i++)
            handle.SetButton(i, _buttons[i]?.value ?? false);

        for (var i = 0; i < InputHandle.ToggleCount; i++)
            handle.SetToggle(i, _toggles[i]?.value ?? false);

        for (var i = 0; i < InputHandle.KnobCount; i++)
            handle.SetKnob(i, _knobs[i]?.value ?? 0);
    }

    #endregion
}

} // namespace Fluo
