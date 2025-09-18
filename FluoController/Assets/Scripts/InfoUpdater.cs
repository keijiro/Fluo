using UnityEngine;
using UnityEngine.UIElements;

namespace Fluo {

public sealed class InfoUpdater : MonoBehaviour
{
    Label _clockLabel;

    void Start()
      => _clockLabel = GetComponent<UIDocument>().rootVisualElement.Q<Label>("clock");

    void Update()
      => _clockLabel.text = System.DateTime.Now.ToString("HH:mm:ss");
}

} // namespace Fluo
