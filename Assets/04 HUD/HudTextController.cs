using UnityEngine;
using UnityEngine.UIElements;

public sealed class HudTextController : MonoBehaviour
{
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

        if (Random.value < 0.1f) _labels.e2.text = GenerateLabel2();
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

    string GenerateLabel2()
    {
        var text = "";
        for (var i = 0; i < 8; i++)
        {
            text += $" {Random.Range(0, 255):X2}";
        }
        return text;
    }

    string GenerateLabel3()
      => $"Packet Stream: {Random.Range(0, 99):00}%\nNode Status: Stable\nPattern Recognized";
}
