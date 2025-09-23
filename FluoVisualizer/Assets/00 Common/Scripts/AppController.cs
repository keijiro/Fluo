using UnityEngine;

public sealed class FrameRateConfig : MonoBehaviour
{
    void Start()
      => Application.targetFrameRate = 60;
}
