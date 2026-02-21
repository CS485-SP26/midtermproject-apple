using UnityEngine;
using Character; 

public class ButtonTrigger : MonoBehaviour
{
    [SerializeField] private GameObject ButtonUI;

    private void Start()
    {
        ButtonUI.SetActive(false);
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
