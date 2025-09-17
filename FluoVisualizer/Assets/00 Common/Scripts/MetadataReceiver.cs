using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fluo {

public sealed class MetadataReceiver : MonoBehaviour
{
    void Update()
    {
        // NDI receiver existence
        var recv = GetComponent<Klak.Ndi.NdiReceiver>();
        if (recv == null) return;

        // Deserialization
        var xml = recv.metadata;
        if (xml == null || xml.Length == 0) return;
        var bin = Metadata.Deserialize(xml);

        // Update RemoteInputDevice via InputSystem
        if (RemoteInputDevice.current != null)
        {
            var inputState = bin.InputState;
            var bytes = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref inputState, 1));
            var remoteState = MemoryMarshal.Read<RemoteInputState>(bytes);
            InputSystem.QueueStateEvent(RemoteInputDevice.current, remoteState);
        }
    }
}

} // namespace Fluo
