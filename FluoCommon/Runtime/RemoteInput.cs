using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Fluo {

// Remote input device definition for Unity Input System

// Input state struct
public unsafe struct RemoteInputState : IInputStateTypeInfo
{
    public FourCC format => new FourCC('F', 'L', 'U', 'O');

    public const int ButtonCount = 16;
    public const int ToggleCount = 16;
    public const int KnobCount = 16;

    [InputControl(name = "button0",  layout = "Button", bit = 0)]
    [InputControl(name = "button1",  layout = "Button", bit = 1)]
    [InputControl(name = "button2",  layout = "Button", bit = 2)]
    [InputControl(name = "button3",  layout = "Button", bit = 3)]
    [InputControl(name = "button4",  layout = "Button", bit = 4)]
    [InputControl(name = "button5",  layout = "Button", bit = 5)]
    [InputControl(name = "button6",  layout = "Button", bit = 6)]
    [InputControl(name = "button7",  layout = "Button", bit = 7)]
    [InputControl(name = "button8",  layout = "Button", bit = 8)]
    [InputControl(name = "button9",  layout = "Button", bit = 9)]
    [InputControl(name = "button10", layout = "Button", bit = 10)]
    [InputControl(name = "button11", layout = "Button", bit = 11)]
    [InputControl(name = "button12", layout = "Button", bit = 12)]
    [InputControl(name = "button13", layout = "Button", bit = 13)]
    [InputControl(name = "button14", layout = "Button", bit = 14)]
    [InputControl(name = "button15", layout = "Button", bit = 15)]
    public ushort Buttons;

    [InputControl(name = "toggle0",  layout = "Button", bit = 0)]
    [InputControl(name = "toggle1",  layout = "Button", bit = 1)]
    [InputControl(name = "toggle2",  layout = "Button", bit = 2)]
    [InputControl(name = "toggle3",  layout = "Button", bit = 3)]
    [InputControl(name = "toggle4",  layout = "Button", bit = 4)]
    [InputControl(name = "toggle5",  layout = "Button", bit = 5)]
    [InputControl(name = "toggle6",  layout = "Button", bit = 6)]
    [InputControl(name = "toggle7",  layout = "Button", bit = 7)]
    [InputControl(name = "toggle8",  layout = "Button", bit = 8)]
    [InputControl(name = "toggle9",  layout = "Button", bit = 9)]
    [InputControl(name = "toggle10", layout = "Button", bit = 10)]
    [InputControl(name = "toggle11", layout = "Button", bit = 11)]
    [InputControl(name = "toggle12", layout = "Button", bit = 12)]
    [InputControl(name = "toggle13", layout = "Button", bit = 13)]
    [InputControl(name = "toggle14", layout = "Button", bit = 14)]
    [InputControl(name = "toggle15", layout = "Button", bit = 15)]
    public ushort Toggles;

    [InputControl(name = "knob0",  layout = "Axis", format = "BYTE", sizeInBits = 8)]
    [InputControl(name = "knob1",  layout = "Axis", format = "BYTE", sizeInBits = 8, offset = 1)]
    [InputControl(name = "knob2",  layout = "Axis", format = "BYTE", sizeInBits = 8, offset = 2)]
    [InputControl(name = "knob3",  layout = "Axis", format = "BYTE", sizeInBits = 8, offset = 3)]
    [InputControl(name = "knob4",  layout = "Axis", format = "BYTE", sizeInBits = 8, offset = 4)]
    [InputControl(name = "knob5",  layout = "Axis", format = "BYTE", sizeInBits = 8, offset = 5)]
    [InputControl(name = "knob6",  layout = "Axis", format = "BYTE", sizeInBits = 8, offset = 6)]
    [InputControl(name = "knob7",  layout = "Axis", format = "BYTE", sizeInBits = 8, offset = 7)]
    [InputControl(name = "knob8",  layout = "Axis", format = "BYTE", sizeInBits = 8, offset = 8)]
    [InputControl(name = "knob9",  layout = "Axis", format = "BYTE", sizeInBits = 8, offset = 9)]
    [InputControl(name = "knob10", layout = "Axis", format = "BYTE", sizeInBits = 8, offset = 10)]
    [InputControl(name = "knob11", layout = "Axis", format = "BYTE", sizeInBits = 8, offset = 11)]
    [InputControl(name = "knob12", layout = "Axis", format = "BYTE", sizeInBits = 8, offset = 12)]
    [InputControl(name = "knob13", layout = "Axis", format = "BYTE", sizeInBits = 8, offset = 13)]
    [InputControl(name = "knob14", layout = "Axis", format = "BYTE", sizeInBits = 8, offset = 14)]
    [InputControl(name = "knob15", layout = "Axis", format = "BYTE", sizeInBits = 8, offset = 15)]
    public fixed byte Knobs[16];
}

// Input device class
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[InputControlLayout(stateType = typeof(RemoteInputState))]
public class RemoteInputDevice : InputDevice
{
    public static RemoteInputDevice current { get; private set; }

    public override void MakeCurrent()
    {
        base.MakeCurrent();
        current = this;
    }

    protected override void OnRemoved()
    {
        base.OnRemoved();
        if (current == this) current = null;
    }

#if UNITY_EDITOR

    static RemoteInputDevice()
    {
        InputSystem.RegisterLayout<RemoteInputDevice>();
        if (current == null) InputSystem.AddDevice<RemoteInputDevice>();
    }

#else

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        InputSystem.RegisterLayout<RemoteInputDevice>();
        if (current == null) InputSystem.AddDevice<RemoteInputDevice>();
    }

#endif
}

} // namespace Fluo
