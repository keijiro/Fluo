using UnityEngine;
using UnityEngine.InputSystem;

namespace Fluo {

public sealed class MonitorSwitcher : MonoBehaviour
{
    [SerializeField] Camera _targetCamera = null;
    [SerializeField] Renderer _targetRenderer = null;
    [SerializeField] InputAction _switchInput = null;

    async Awaitable WaitSwitchAsync()
    {
        await Awaitable.NextFrameAsync();
        while (!_switchInput.triggered) await Awaitable.NextFrameAsync();
    }

    async Awaitable Start()
    {
        var props = new MaterialPropertyBlock();

        _switchInput.Enable();

        while (true)
        {
            await WaitSwitchAsync();

            props.SetInteger("_Alternate", 0);
            _targetRenderer.SetPropertyBlock(props);
            _targetCamera.enabled = true;

            await WaitSwitchAsync();

            props.SetInteger("_Alternate", 1);
            _targetRenderer.SetPropertyBlock(props);

            await WaitSwitchAsync();

            _targetCamera.enabled = false;
        }
    }
}

} // namespace Fluo
