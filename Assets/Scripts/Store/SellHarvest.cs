using UnityEngine;
using Character;
using Core;

public class SellHarvest : MonoBehaviour
{
    [SerializeField] private GameObject SellUI;
    [SerializeField] private int harvestPrice = 10;

     private bool playerInside = false;

    void Start()
    {
        SellUI.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out _))
        {
            playerInside = true;
            SellUI.SetActive(true);
            
        }
    
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out _))
        {
            playerInside = false;
            SellUI.SetActive(false);
            
        }
    }

    public void SellHarvest_Object()
    {
        if (playerInside)
        {
            if (GameManager.Instance.harvest > 0)
            {
                int amount = GameManager.Instance.harvest;

                int totalSellValue = amount * harvestPrice;

                GameManager.Instance.AddFunds(totalSellValue);

                GameManager.Instance.ResetHarvest();

                Debug.Log("Sold crops for $" + totalSellValue);
                SellUI.SetActive(false);
            }
            else
            {
                Debug.Log("Nothing to sell.");
            }
        }
    }
}
