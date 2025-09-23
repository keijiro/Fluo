using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fluo {

public sealed class AppController : MonoBehaviour
{
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) SceneManager.LoadScene(0);
    }
}

} // namespace Fluo
