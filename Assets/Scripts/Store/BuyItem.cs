using UnityEngine;
using Character;
using Core;
public class BuyItem : MonoBehaviour
{
    [SerializeField] private GameObject ButtonUI;
    [SerializeField] private GameObject Seed;

    [SerializeField] private int seedPrice = 50;

    
    private bool playerInside = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ButtonUI.SetActive(false);
        Seed.SetActive(true);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out _))
        {
            playerInside = true;
            ButtonUI.SetActive(true);
        }
    
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out _))
        {
            playerInside = false;
            ButtonUI.SetActive(false);
        }
    }

    public void BuyItem_Object()
    {
        if (playerInside)
        {
            if(seedPrice <= GameManager.Instance.funds) 
            {
                GameManager.Instance.AddFunds(0-seedPrice);
                GameManager.Instance.AddSeeds(1);
                Seed.SetActive(false);
                ButtonUI.SetActive(false);
            }
            else
            {
                Debug.Log("Unable to purchase seeds, not enough funds.");
            }
        }
    }

    // public void storeBuyFruit()
    // {
    //     if(playerInside)
    //     {
    //         if(player)
    // }
}
