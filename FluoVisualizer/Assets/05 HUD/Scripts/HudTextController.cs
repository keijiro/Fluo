using Klak.Ndi;
using Klak.TestTools;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Profiling;

namespace Fluo {

public sealed class HudTextController : MonoBehaviour
{
    [SerializeField] MetadataReceiver _metadataReceiver = null;
    [SerializeField] ImageSource _imageSource = null;

    static readonly string[] _spinner = { "|", "/", "-", "\\", "*" };

    (Label e1, Label e2, Label e3) _labels;
    int _count;
    float _eofTime;

    async Awaitable Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _labels.e1 = root.Q<Label>("label1");
        _labels.e2 = root.Q<Label>("label2");
        _labels.e3 = root.Q<Label>("label3");

        while (true)
        {
            var t1 = System.DateTime.Now.Ticks;
            await Awaitable.EndOfFrameAsync();
            var t2 = System.DateTime.Now.Ticks;
            _eofTime = (float)(t2 - t1) / TimeSpan.TicksPerMillisecond;
            await Awaitable.NextFrameAsync();
        }
    }

    void Update()
    {
        _labels.e1.text = GenerateLabel1();
        _labels.e2.text = GenerateLabel2();
        _labels.e3.text = GenerateLabel3();
    }

    string GenerateLabel1()
    {
        var spinner = _spinner[(_count++ / 8) % 5];

        var ndi = _imageSource.GetComponent<NdiReceiver>();
        if (ndi == null) return $"* Awaiting Link... [{spinner}]";

        var metadata = _metadataReceiver.LastReceived;
        return $"* Link Established [{spinner}]\n" +
               $"* Endpoint ID [{ndi.ndiName}]\n" +
               $"* Sync Timecode [{metadata.FrameCount}:{metadata.FrameTime:.00}]";
    }

    static string GenerateLabel2Line(ReadOnlySpan<byte> s)
        => $"{s[0]:X2} {s[1]:X2} {s[2]:X2} {s[3]:X2} {s[4]:X2} {s[5]:X2} {s[6]:X2}";

    string GenerateLabel2()
    {
        var data = _metadataReceiver.LastReceived;
        var span = MemoryMarshal.CreateSpan(ref data, 1);
        var bytes = MemoryMarshal.AsBytes(span);
        return GenerateLabel2Line(bytes.Slice( 0, 7)) + "\n" +
               GenerateLabel2Line(bytes.Slice( 7, 7)) + "\n" +
               GenerateLabel2Line(bytes.Slice(14, 7)) + "\n" +
               GenerateLabel2Line(bytes.Slice(21, 7));
    }

    string GenerateLabel3()
      => $"Core Runtime Index [{_eofTime:.00}]\n" +
         $"Visual Sync Rate [{1.0f / Time.deltaTime:.00}]\n" +
         $"Primary Occupancy [{Profiler.GetTotalAllocatedMemoryLong():N0}]\n" +
         $"Secondary Occupancy [{Profiler.GetMonoUsedSizeLong():N0}]";
}

} // namespace Fluo
