using System.Runtime.InteropServices;

namespace Fluo {

// Fluo input state blittable struct
[StructLayout(LayoutKind.Sequential)]
public unsafe struct InputState
{
    public ushort Buttons;
    public ushort Toggles;
    public fixed byte Knobs[16];
}

} // namespace Fluo
