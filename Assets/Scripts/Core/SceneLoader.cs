using UnityEngine;
using Core;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        GameManager.Instance.LoadScenebyName(sceneName);
    }
}