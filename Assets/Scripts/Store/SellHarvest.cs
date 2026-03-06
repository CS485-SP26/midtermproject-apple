using UnityEngine;
using Character;
using TMPro;
using System.Collections.Generic; 
using Core;

public class SellHarvest : MonoBehaviour
{
    [SerializeField] private GameObject SellButtonUI;
    //[SerializeField] private int harvestPrice = 10;
    //UI Sell Panel
    [SerializeField] private GameObject sellPanel;           // The popup panel
    [SerializeField] private Transform fruitListParent;      // Parent for the fruit entries
    [SerializeField] private GameObject fruitEntryPrefab;    // Prefab for one fruit
    //[SerializeField] private TMP_Text totalSellText;         // Optional: show total value

     private bool playerInside = false;

    void Start()
    {
        sellPanel.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out _))
        {
            playerInside = true;
            Debug.Log("Player inside Trigger");
            //SellUI.SetActive(true);
            OpenSellPanel();
            
        }
    
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out _))
        {
            playerInside = false;
            //SellUI.SetActive(false);
            CloseSellPanel();
            
        }
    }
    public void OpenSellPanel()
    {
        sellPanel.SetActive(true);
         // Clear old entries
        foreach (Transform child in fruitListParent)
            Destroy(child.gameObject);

        int totalValue = 0;

        // Populate UI
        foreach (var kvp in GameManager.Instance.harvestInventory)
        {
            
            PlantType plant = kvp.Key;
            int amount = kvp.Value;

            if (amount <= 0) continue;

            GameObject entry = Instantiate(fruitEntryPrefab, fruitListParent);
            TMP_Text entryText = entry.GetComponentInChildren<TMP_Text>();
            entryText.text = $"{plant} x{amount}";

            totalValue += amount * GameManager.Instance.GetPlantPrice(plant);
            Debug.Log($"Adding UI entry: {plant} x{amount}");
        }

    }
    public void CloseSellPanel()
    {
        sellPanel.SetActive(false);
    }

    public void SellHarvest_Object()
    {
        if (!playerInside) return;

        int totalValue = 0;

        foreach (var kvp in new Dictionary<PlantType, int>(GameManager.Instance.harvestInventory))
        {
            PlantType plant = kvp.Key;
            int amount = kvp.Value;

            // Use GameManager's method to sell
            GameManager.Instance.SellHarvest(plant, amount);
            totalValue += amount * GameManager.Instance.GetPlantPrice(plant);
            GameManager.Instance.AddFunds(totalValue);
            GameManager.Instance.ResetHarvest();
        }

        Debug.Log($"Sold all crops for ${totalValue}");
        GameManager.Instance.UpdateUI();
        CloseSellPanel();
        /*
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
        */
    }
    
}
