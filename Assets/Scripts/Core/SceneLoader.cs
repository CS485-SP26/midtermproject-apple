using UnityEngine;
using Core;

public class SceneLoader : MonoBehaviour
{
    // This method can be called to load a scene by name instead of using UnityEngine.SceneManagement directly
    public void LoadScene(string sceneName)
    {
        GameManager.Instance.LoadScenebyName(sceneName);
    }
}