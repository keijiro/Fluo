using System.Runtime.InteropServices;

namespace Fluo {

// Fluo input state blittable struct
[StructLayout(LayoutKind.Sequential)]
public unsafe struct InputState
{
    #region Data members

    fixed byte Buttons[2];
    fixed byte Toggles[2];
    fixed byte Knobs[16];

    #endregion

    #region Public accessor methods

    public byte GetButtonData(int offset)
      => Buttons[offset];

    public void SetButtonData(int offset, int data)
      => Buttons[offset] = (byte)data;

    public byte GetToggleData(int offset)
      => Toggles[offset];

    public void SetToggleData(int offset, int data)
      => Toggles[offset] = (byte)data;

    public byte GetKnobData(int offset)
      => Knobs[offset];

    public void SetKnobData(int offset, int value)
      => Knobs[offset] = (byte)value;

    #endregion
}

} // namespace Fluo
