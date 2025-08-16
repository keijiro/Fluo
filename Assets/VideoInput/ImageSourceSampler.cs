using UnityEngine;
using Klak.TestTools;

public sealed class ImageSourceSampler : MonoBehaviour
{
    [field:SerializeField] public float Interval { get; set; } = 0.5f;

    [SerializeField] ImageSource _source = null;
    [SerializeField] RenderTexture _destination = null;

    async void Start()
    {
        await Awaitable.WaitForSecondsAsync(0.5f);

        while (true)
        {
            Graphics.Blit(_source.AsTexture, _destination);
            await Awaitable.WaitForSecondsAsync(Interval);
        }
    }
}
