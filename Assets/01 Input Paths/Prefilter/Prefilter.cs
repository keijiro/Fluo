using UnityEngine;
using Klak.TestTools;
using BodyPix;

namespace Fluo {

public sealed class Prefilter : MonoBehaviour
{
    #region Public properties

    [field:SerializeField] public float Interval { get; set; } = 1;

    #endregion

    #region Editable attributes

    [SerializeField] ImageSource _source = null;
    [SerializeField] ResourceSet _resources = null;
    [SerializeField] Texture3D _lutTexture = null;
    [SerializeField] RenderTexture _output = null;

    #endregion

    #region Project asset references

    [SerializeField, HideInInspector] Shader _shader = null;

    #endregion

    #region Private members

    BodyDetector _detector;
    Material _material;
    float _timer;

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _detector = new BodyDetector(_resources, 512, 384);
        _material = new Material(_shader);
    }

    void OnDestroy()
    {
        _detector.Dispose();
        Destroy(_material);
    }

    void LateUpdate()
    {
        if ((_timer -= Time.deltaTime) > 0) return;

        _detector.ProcessImage(_source.AsTexture);
        _material.SetTexture(ShaderID.BodyPixTex, _detector.MaskTexture);
        _material.SetTexture(ShaderID.LutTex, _lutTexture);
        Graphics.Blit(_source.AsTexture, _output, _material);

        _timer += Interval;
    }

    #endregion
}

} // namespace Dcam2
