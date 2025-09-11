using UnityEngine;
using UnityEngine.InputSystem;

namespace Fluo {

public sealed class DebugViewSwitcher : MonoBehaviour
{
    [SerializeField] InputAction _switchInput = null;

    async Awaitable WaitSwitchAsync()
    {
        await Awaitable.NextFrameAsync();
        while (!_switchInput.triggered) await Awaitable.NextFrameAsync();
    }

    async Awaitable Start()
    {
        var renderer = GetComponent<Renderer>();
        var props = new MaterialPropertyBlock();

        _switchInput.Enable();

        while (true)
        {
            await WaitSwitchAsync();

            props.SetInteger(ShaderID.Alternate, 0);
            renderer.SetPropertyBlock(props);
            renderer.enabled = true;

            await WaitSwitchAsync();

            props.SetInteger(ShaderID.Alternate, 1);
            renderer.SetPropertyBlock(props);

            await WaitSwitchAsync();

            renderer.enabled = false;
        }
    }
}

} // namespace Fluo
