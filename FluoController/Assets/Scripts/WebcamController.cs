using UnityEngine;
using UnityEngine.UIElements;
using VJUITK;

namespace Fluo {

public sealed class WebcamController : MonoBehaviour
{
    [SerializeField] UIDocument _inputSource = null;
    [SerializeField] RenderTexture _target = null;

    static readonly
      (string uiName, string deviceName, bool hflip, bool vflip)[] DeviceDefs =
        { ("camera-telephoto", "Back Dual Camera",      false, true),
          ("camera-wide",      "Back Dual Wide Camera", false, true),
          ("camera-ultrawide", "Back Triple Camera",    false, true),
          ("camera-front",     "Front Camera",          true, false) };

    WebCamTexture _webcam;
    (bool h, bool v) _flip;

    void SelectCamera(int index)
    {
        ref var def = ref DeviceDefs[index];

        if (_webcam != null) Destroy(_webcam);
        _webcam = new WebCamTexture(def.deviceName);
        _webcam.Play();

        _flip = (def.hflip, def.vflip);
    }

    async Awaitable Start()
    {
        var root = _inputSource.rootVisualElement;

        // Camera button callbacks
        for (var i = 0; i < DeviceDefs.Length; i++)
        {
            var temp = i;
            root.Q<VJButton>(DeviceDefs[temp].uiName).Clicked += () => SelectCamera(temp);
        }

        // Webcam activation
        await Application.RequestUserAuthorization(UserAuthorization.WebCam);

        // Initial camera (wide)
        SelectCamera(1);
    }

    void Update()
    {
        // Webcam state check
        if (_webcam == null || _webcam.width < 32) return;

        // Crop and copy
        var srcRatio = (float)_webcam.width / _webcam.height;
        var dstRatio = (float)_target.width / _target.height;
        var scale = new Vector2(_flip.h ? -1 : 1, (_flip.v ? -1 : 1) * srcRatio / dstRatio);
        var offs = new Vector2(_flip.h ? 1 : 0, -0.5f * scale.y + 0.5f);
        Graphics.Blit(_webcam, _target, scale, offs);
    }
}

} // namespace Fluo
