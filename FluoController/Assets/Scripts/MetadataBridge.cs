using UnityEngine;
using Klak.Ndi;

namespace Fluo {

public sealed class MetadataBridge : MonoBehaviour
{
    InputHandle _input;
    NdiSender _sender;

    Metadata MetadataFromInput
      => new Metadata(_input.InputState);

    void Start()
    {
        _input = GetComponent<InputHandle>();
        _sender = GetComponent<NdiSender>();
    }

    void Update()
      => _sender.metadata = MetadataFromInput.Serialize();
}

} // namespace Fluo
