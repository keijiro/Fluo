using UnityEngine;
using UnityEngine.UIElements;
using Klak.Ndi;
using VJUITK;

namespace Fluo {

[RequireComponent(typeof(NdiSender))]
public sealed class MetadataUpdater : MonoBehaviour
{
    #region Scene object references

    [SerializeField] UIDocument _inputSource = null;

    #endregion

    #region Private members

    NdiSender _sender;

    VJButton[] _buttons;
    VJToggle[] _toggles;
    VJKnob[] _knobs;

    unsafe RemoteInputState CalculateInputState()
    {
        var (bdata , tdata) = ((ushort)0, (ushort)0);

        for (var i = 0; i < _buttons.Length; i++)
            if (_buttons[i]?.value ?? false) bdata |= (ushort)(1 << i);

        for (var i = 0; i < _toggles.Length; i++)
            if (_toggles[i]?.value ?? false) tdata |= (ushort)(1 << i);

        var state = new RemoteInputState(){ Buttons = bdata, Toggles = tdata };

        for (var i = 0; i < _knobs.Length; i++)
            state.Knobs[i] = (byte)((_knobs[i]?.value ?? 0) * 255);

        return state;
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _sender = GetComponent<NdiSender>();

        _buttons = new VJButton[RemoteInputState.ButtonCount];
        _toggles = new VJToggle[RemoteInputState.ToggleCount];
        _knobs = new VJKnob[RemoteInputState.KnobCount];

        var root = _inputSource.rootVisualElement;

        for (var i = 0; i < _buttons.Length; i++)
            _buttons[i] = root.Q<VJButton>($"button-{i}");

        for (var i = 0; i < _toggles.Length; i++)
            _toggles[i] = root.Q<VJToggle>($"toggle-{i}");

        for (var i = 0; i < _knobs.Length; i++)
            _knobs[i] = root.Q<VJKnob>($"knob-{i}");
    }

    void Update()
      => _sender.metadata = new Metadata(CalculateInputState()).Serialize();

    #endregion
}

} // namespace Fluo
