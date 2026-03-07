using Core;
using Farming;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Environment;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private GameObject seedPopupPanel;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject seedButtonPrefab;
    [SerializeField] private GameObject seedAmountPrefab;
    [SerializeField] private TMP_Text popupHintText;

    private FarmTile selectedTile;

    void Awake()
    {
        Instance = this;
        seedPopupPanel.SetActive(false);
    }

    private void OnDisable()
    {
        if (seedPopupPanel != null)
        {
            seedPopupPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    public void OpenSeedPopUp(FarmTile tile)
    {
        if (GameManager.Instance.GetTotalSeeds() <= 0)
        {
            Debug.Log("No seeds available, cannot open popup.");
            return;
        }

        selectedTile = tile;
        seedPopupPanel.SetActive(true);
        Time.timeScale = 0f;

        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        SeasonManager.Season activeSeason = SeasonManager.Instance != null
            ? SeasonManager.Instance.CurrentSeason
            : SeasonManager.Season.Spring;

        bool hasAvailableSeed = false;

        foreach (SeedData seed in GameManager.Instance.avaiableSeeds)
        {
            GameObject btnObj = Instantiate(seedButtonPrefab, buttonContainer);
            TMP_Text buttonText = btnObj.GetComponentInChildren<TMP_Text>();
            bool ownsSeed = GameManager.Instance.HasSeed(seed);
            bool inSeason = seed.IsAvailableInSeason(activeSeason);
            bool canPlant = ownsSeed && inSeason;

            if (buttonText != null)
            {
                buttonText.text = seed.seedName;
            }

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                SeedData capturedSeed = seed;
                btn.interactable = canPlant;
                btn.onClick.AddListener(() =>
                {
                    SelectSeed(capturedSeed);
                });
            }
            else
            {
                Debug.LogError("Seed Button prefab does not have a Button component!");
            }

            if (seedAmountPrefab != null)
            {
                GameObject amountObj = Instantiate(seedAmountPrefab, buttonContainer);
                TMP_Text amountText = amountObj.GetComponent<TMP_Text>();
                if (amountText != null)
                {
                    amountText.text = $"Count: {GameManager.Instance.GetSeedCount(seed)} | {seed.GetSeasonSummary()} | {(inSeason ? "Ready" : "Out of Season")}";
                }
            }

            hasAvailableSeed |= canPlant;
        }

        if (popupHintText != null)
        {
            popupHintText.text = hasAvailableSeed
                ? $"Pick a seed for {activeSeason}."
                : $"No owned seeds can be planted during {activeSeason}.";
        }
    }
    public void SelectSeed(SeedData seed)
    {
        if (!GameManager.Instance.HasSeed(seed))
            return;

        if (selectedTile != null && selectedTile.PlantSelectedSeed(seed))
        {
            GameManager.Instance.selectedSeed = seed;
            GameManager.Instance.UseSeed(seed);
        }

        Debug.Log("SelectSeed was called: " + seed.seedName);
        
        seedPopupPanel.SetActive(false);
        Time.timeScale = 1f;
        selectedTile = null;
    }
    public void ClosePopup()
    {
        seedPopupPanel.SetActive(false);
        Time.timeScale = 1f;
        selectedTile = null;
    }
}
