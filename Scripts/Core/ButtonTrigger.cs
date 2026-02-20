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
        if((other.GetComponent<PlayerController>() != null))
        {
            ButtonUI.SetActive(true);
        }
    
    }
    public void OnTriggerExit(Collider other)
    {
        if((other.GetComponent<PlayerController>() != null))
        {
            ButtonUI.SetActive(false);
        }
    }
}
