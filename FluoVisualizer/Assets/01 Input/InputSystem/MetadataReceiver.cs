using UnityEngine;
using UnityEngine.InputSystem;

namespace Fluo {

public sealed class MetadataReceiver : MonoBehaviour
{
    public Metadata LastReceived { get; private set; }

    void Update()
    {
        // NDI receiver existence
        var recv = GetComponent<Klak.Ndi.NdiReceiver>();
        if (recv == null) return;

        // Deserialization
        var xml = recv.metadata;
        if (xml == null || xml.Length == 0) return;
        LastReceived = Metadata.Deserialize(xml);

        // Update RemoteInputDevice via InputSystem
        if (RemoteInputDevice.current != null)
            InputSystem.QueueStateEvent
              (RemoteInputDevice.current, LastReceived.InputState);
    }
}

} // namespace Fluo
