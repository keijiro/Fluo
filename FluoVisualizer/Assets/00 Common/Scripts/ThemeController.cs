using UnityEngine;

namespace Fluo {

public sealed class ThemeController : MonoBehaviour
{
    [field:SerializeField, Range(0, 1)] public float Hue { get; set; }
    [field:SerializeField, Range(0, 1)] public float Saturation { get; set; }

    void Update()
      => Shader.SetGlobalColor(ShaderID.FluoThemeColor, Color.HSVToRGB(Hue, Saturation, 1));
}

} // namespace Fluo
