using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Fluo {

public sealed class HudTextController : MonoBehaviour
{
    [SerializeField] MetadataReceiver _metadataReceiver = null;

    static readonly string[] _spinner = { "|", "/", "-", "\\", "*" };

    (Label e1, Label e2, Label e3) _labels;
    float _timer;
    int _count;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _labels.e1 = root.Q<Label>("label1");
        _labels.e2 = root.Q<Label>("label2");
        _labels.e3 = root.Q<Label>("label3");
    }

    void Update()
    {
        if ((_timer += Time.deltaTime) > 0.05f)
        {
            _labels.e1.text = GenerateLabel1();
            _timer -= 0.05f;
        }

        _labels.e2.text = GenerateLabel2();
        if (Random.value < 0.1f) _labels.e3.text = GenerateLabel3();
    }

    string GenerateLabel1()
    {
        var n1 = Time.time % 100;
        var n2 = (Time.time * 11) % 100;
        var s = _spinner[_count++ % 5];
        var text = $"* System Link Established ({n1:00}:{n2:00})\n";
        text += $"* Core Sync Active\n";
        text += $"* Target Lock Pending [{s}]";
        return text;
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
      => $"Packet Stream: {Random.Range(0, 99):00}%\nNode Status: Stable\nPattern Recognized";
}

} // namespace Fluo
