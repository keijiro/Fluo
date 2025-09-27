using System;
using System.Runtime.InteropServices;

namespace Fluo {

public readonly struct Metadata
{
    #region Data members

    public readonly RemoteInputState InputState;
    public readonly int FrameCount;
    public readonly float FrameTime;

    #endregion

    #region Constructors

    public Metadata(RemoteInputState inputState, int frameCount, float frameTime)
      => (InputState, FrameCount, FrameTime) = (inputState, frameCount, frameTime);

    #endregion

    #region Serialization/deserialization

    public string Serialize()
    {
        ReadOnlySpan<Metadata> data = stackalloc Metadata[] { this };
        var bytes = MemoryMarshal.AsBytes(data).ToArray();
        return "<![CDATA[" + System.Convert.ToBase64String(bytes) + "]]>";
    }

    public static unsafe Metadata Deserialize(string xml)
    {
        var base64 = xml.Substring(9, xml.Length - 9 - 3);
        var buf = (Span<byte>)(stackalloc byte[Marshal.SizeOf<Metadata>()]);
        Convert.TryFromBase64String(base64, buf, out int written);
        return MemoryMarshal.Read<Metadata>(buf);
    }

    #endregion
}

} // namespace Fluo
