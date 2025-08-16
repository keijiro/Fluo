using UnityEngine;
using StableFluids;

namespace Fluo {

public sealed class MarblingController : MonoBehaviour
{
    #region Public properties

    [field:SerializeField] public float Viscosity { get; set; } = 1e-6f;
    [field:SerializeField] public float PointForce { get; set; } = 300;
    [field:SerializeField] public float PointFalloff { get; set; } = 200;

    #endregion

    #region Editable attributes

    [SerializeField] RenderTexture _velocityField = null;

    #endregion

    #region Project asset references

    [SerializeField, HideInInspector] Shader _kernelsShader = null;

    #endregion

    #region Private members

    FluidSimulation _simulation;
    MarblingInputHandler _input;

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _simulation = new FluidSimulation(_velocityField, _kernelsShader);
        _input = new MarblingInputHandler(_velocityField);
    }

    void OnDestroy()
      => _simulation?.Dispose();

    void Update()
    {
        _input.Update();

        // Simulation pre-step (advection + diffusion)
        _simulation.Viscosity = Viscosity;
        _simulation.PreStep();

        // Apply forces based on input
        if (_input.RightPressed)
        {
            var force = Random.insideUnitCircle * PointForce * 0.025f;
            _simulation.ApplyPointForce(_input.Position, force, PointFalloff);
        }
        else if (_input.LeftPressed)
        {
            var force = _input.Velocity * PointForce;
            _simulation.ApplyPointForce(_input.Position, force, PointFalloff);
        }

        // Simulation post-step (projection)
        _simulation.PostStep();
    }

    #endregion
}

} // namespace Fluo