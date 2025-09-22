using UnityEngine;
using UnityEngine.UIElements;
using VJUITK;

namespace Fluo {

[System.Serializable]
public struct WebcamConfig
{
    public string DeviceName;
    public string UIName;
    public bool HFlip;
    public bool VFlip;
}

public sealed class WebcamController : MonoBehaviour
{
    [SerializeField] UIDocument _inputSource = null;
    [SerializeField] RenderTexture _target = null;
    [SerializeField] WebcamConfig[] _configs = null;

    WebCamTexture _webcam;
    WebcamConfig _config;

    void SelectCamera(WebcamConfig config)
    {
        _config = config;
        if (_webcam != null) Destroy(_webcam);
        _webcam = new WebCamTexture(_config.DeviceName);
        _webcam.Play();
    }

    async Awaitable Start()
    {
        var root = _inputSource.rootVisualElement;

        // Camera button callbacks
        foreach (var cfg in _configs)
            root.Q<VJButton>(cfg.UIName).Clicked += () => SelectCamera(cfg);

        // Webcam activation
        await Application.RequestUserAuthorization(UserAuthorization.WebCam);

        // Default camera
        SelectCamera(_configs[0]);
    }

    void Update()
    {
        // Webcam state check
        if (_webcam == null || _webcam.width < 32) return;

        // Crop and copy
        var srcRatio = (float)_webcam.width / _webcam.height;
        var dstRatio = (float)_target.width / _target.height;
        var yScale = (_config.VFlip ? -1 : 1) * srcRatio / dstRatio;
        var scale = new Vector2(_config.HFlip ? -1 : 1, yScale);
        var offs = scale * -0.5f + Vector2.one * 0.5f;
        Graphics.Blit(_webcam, _target, scale, offs);
    }
}

} // namespace Fluo
