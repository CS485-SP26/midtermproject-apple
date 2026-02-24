using UnityEngine;
using UnityEngine.UI;
using Character; 
using Core;

public class ButtonTrigger : MonoBehaviour
{
    [SerializeField] private GameObject ButtonUI;
    [SerializeField] private string sceneToLoad;
    private Button button;

    private void Start()
    {
        ButtonUI.SetActive(false);
        // Get the Button component
        button = ButtonUI.GetComponentInChildren<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.LoadScenebyName(sceneToLoad);
                else
                    Debug.LogWarning("GameManager instance is null!");
            });
        }
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out _))
        {
            ButtonUI.SetActive(true);
        }
    
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out _))
        {
            ButtonUI.SetActive(false);
        }
    }
}
