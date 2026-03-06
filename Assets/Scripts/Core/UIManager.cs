using Core;
using Farming;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private GameObject seedPopupPanel;
    [SerializeField] private Transform buttonContainer; // parent for buttons
    [SerializeField] private GameObject seedButtonPrefab;
    [SerializeField] private GameObject seedAmountPrefab;

    private FarmTile selectedTile;

    void Awake()
    {
        Instance = this;
        seedPopupPanel.SetActive(false);
    }

    public void OpenSeedPopUp(FarmTile tile)
    {
        // Check if player has any seeds
        if (GameManager.Instance.GetTotalSeeds() <= 0)
        {
            Debug.Log("No seeds available, cannot open popup.");
            return;
        }
        selectedTile = tile;
        seedPopupPanel.SetActive(true);
        Time.timeScale = 0f;

        // Clear old buttons
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }
            
        foreach (SeedData seed in GameManager.Instance.avaiableSeeds)
        {
            //Create Button
            GameObject btnObj = Instantiate(seedButtonPrefab, buttonContainer);
            btnObj.GetComponentInChildren<TMP_Text>().text = seed.seedName;
            Debug.Log("Created button for: " + seed.seedName);

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() =>
                {
                    SelectSeed(seed);
                });
            }
            else
            {
                Debug.LogError("Seed Button prefab does not have a Button component!");
            }
            GameObject amountObj = Instantiate(seedAmountPrefab, buttonContainer);
            TMP_Text amountText = amountObj.GetComponent<TMP_Text>();
            amountText.text = "Amount: "+ GameManager.Instance.GetSeedCount(seed);

            // Disable button if no seeds in inventory
            if (!GameManager.Instance.HasSeed(seed))
                btnObj.GetComponent<Button>().interactable = false;
            
        }
    }
    public void SelectSeed(SeedData seed)
    {
        if (!GameManager.Instance.HasSeed(seed))
            return;
        if(selectedTile != null)
        {
            selectedTile.PlanetSelectedSeed(seed);
        }
        GameManager.Instance.UseSeed(seed);
        Debug.Log("SelectSeed was called: " + seed.seedName);
        Debug.Log("SELECT SEED CLICKED");
        
        seedPopupPanel.SetActive(false);
        Time.timeScale = 1f;
        //GameManager.Instance.selectedSeed = seed;
        if(selectedTile != null)
        {
            selectedTile.PlanetSelectedSeed(seed);
        }
    }
    public void ClosePopup()
    {
        seedPopupPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
