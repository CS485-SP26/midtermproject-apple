using UnityEngine;
using Character;

public class BuyItem : MonoBehaviour
{
    [SerializeField] private GameObject ButtonUI;
    [SerializeField] private GameObject Seed;
     private bool playerInside = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ButtonUI.SetActive(false);
        Seed.SetActive(true);
    }

    public void OnTriggerEnter(Collider other)
    {
        if((other.GetComponent<PlayerController>() != null))
        {
            playerInside = true;
            ButtonUI.SetActive(true);
        }
    
    }
    public void OnTriggerExit(Collider other)
    {
        if((other.GetComponent<PlayerController>() != null))
        {
            playerInside = false;
            ButtonUI.SetActive(false);
        }
    }

    public void BuyItem_Object()
    {
        if (playerInside)
        {
            Seed.SetActive(false);
            ButtonUI.SetActive(false);
        }
    }
}
