using UnityEngine;
using Lasp;

namespace Fluo {

public sealed class AudioToTransform : MonoBehaviour
{
    [field:SerializeField, Range(0, 1)]
    public float Amplitude { get; set; } = 1;

    [SerializeField] AudioLevelTracker _tracker = null;

    void Update()
      => transform.localPosition = Vector3.one * _tracker.normalizedLevel * Amplitude;
}

} // namespace Fluo
