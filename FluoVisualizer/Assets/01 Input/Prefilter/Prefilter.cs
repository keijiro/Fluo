using UnityEngine;
using Klak.TestTools;
using BodyPix;

namespace Fluo {

public sealed class Prefilter : MonoBehaviour
{
    #region Public properties

    [field:SerializeField, Range(0, 1)]
    public float LutBlend { get; set; } = 1;

    #endregion

    #region Editable attributes

    [SerializeField] ImageSource _source = null;
    [SerializeField] ResourceSet _resources = null;
    [SerializeField] Texture3D _lutTexture = null;
    [SerializeField] RenderTexture _multiplexOut = null;
    [SerializeField] RenderTexture _blurOut = null;

    #endregion

    #region Project asset references

    [SerializeField, HideInInspector] Shader _shader = null;

    #endregion

    #region Private members

    BodyDetector _detector;
    Material _material;

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
        _detector.ProcessImage(_source.AsTexture);

        _material.SetTexture(ShaderID.BodyPixTex, _detector.MaskTexture);
        _material.SetTexture(ShaderID.LutTex, _lutTexture);
        _material.SetFloat(ShaderID.LutBlend, LutBlend);

        var fxcol = Pugrad.Hsluv.ToRgb(Time.time * Mathf.PI * 2 / 270, 120, 60);
        _material.SetColor(ShaderID.EffectColor, fxcol);

        // Multiplexing: Color grading and human stencil
        Graphics.Blit(_source.AsTexture, _multiplexOut, _material, 0);

        // Separable Gaussian blur x2
        var tmp = RenderTexture.GetTemporary(_blurOut.width, _blurOut.height);
        Graphics.Blit(_multiplexOut, tmp, _material, 1);
        Graphics.Blit(tmp, _blurOut, _material, 2);
        Graphics.Blit(_blurOut, tmp, _material, 1);
        Graphics.Blit(tmp, _blurOut, _material, 2);
        RenderTexture.ReleaseTemporary(tmp);
    }

    #endregion
}

} // namespace Fluo
