using System;
using System.Runtime.InteropServices;

namespace Fluo {

// Fluo Metadata struct
[StructLayout(LayoutKind.Sequential)]
public unsafe readonly struct Metadata
{
    #region Data members

    // Control input state
    public readonly InputState InputState;

    // Constructor
    public Metadata(InputState inputState)
      => InputState = inputState;

    #endregion

    #region Serialization/deserialization

    public string Serialize()
    {
        ReadOnlySpan<Metadata> data = stackalloc Metadata[] { this };
        var bytes = MemoryMarshal.AsBytes(data).ToArray();
        return "<![CDATA[" + System.Convert.ToBase64String(bytes) + "]]>";
    }

    public static Metadata Deserialize(string xml)
    {
        var base64 = xml.Substring(9, xml.Length - 9 - 3);
        var data = System.Convert.FromBase64String(base64);
        return MemoryMarshal.Read<Metadata>(new Span<byte>(data));
    }

    #endregion
}

} // namespace Fluo
