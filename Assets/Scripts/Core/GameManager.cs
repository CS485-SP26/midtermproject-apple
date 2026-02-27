using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Core
{
    public class GameManager:MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public int funds = 100;
        public int seeds = 1;
        public int harvest = 0;

        public int currentDay = 1;

        private TMP_Text fundsText;
        private TMP_Text seedsText;
        private TMP_Text harvestText;
        private TMP_Text dayText;


        private void Awake()
        {
            // Singleton protection
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Reset tile and plant states on game launch
            for (int i = 0; i < 16; i++)
            {
                PlayerPrefs.DeleteKey("Farm Tile " + i + "_condition");
                PlayerPrefs.DeleteKey("Farm Tile " + i + "_has_plant");
                PlayerPrefs.DeleteKey("Farm Tile " + i + "_plant_state");
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Find UI in the newly loaded scene
            fundsText = GameObject.Find("FundsText")?.GetComponent<TMP_Text>();
            seedsText = GameObject.Find("SeedsText")?.GetComponent<TMP_Text>();
            harvestText = GameObject.Find("HarvestText")?.GetComponent<TMP_Text>();
            dayText = GameObject.Find("DayLabel")?.GetComponent<TMP_Text>();

            UpdateUI();
        }

        public void AddFunds(int amount)
        {
            funds += amount;
            UpdateUI();
        }

        public void AddSeeds(int amount)
        {
            seeds += amount;
            UpdateUI();
        }

        public void AddHarvest(int amount)
        {
            harvest += amount;
            UpdateUI();
        }

        // Reset the stored harvest to zero and refresh UI
        public void ResetHarvest()
        {
            harvest = 0;
            UpdateUI();
        }
        
        // variable amount to sell
        public void SellHarvest(int amount)
        {
            // Ensure we don't sell more than we have
            int amountToSell = Mathf.Min(amount, harvest);

            // if we have something to sell, reduce harvest and add funds
            if (amountToSell > 0)
            {
                harvest -= amountToSell;
                AddFunds(amountToSell * 10);
            }
        }

        // You can call this from a "Sell All" button in Unity
        public void SellAll() 
        {
            SellHarvest(harvest);
        }
        public void SpendFunds(int amount)
        {
            funds -= amount;
            UpdateUI();
        }
        public void SetDay(int day)
        {
            currentDay = day;
            PlayerPrefs.SetInt("CurrentDay", day); // save immediately
            PlayerPrefs.Save();
        }

        public void LoadDay()
        {
            if (PlayerPrefs.HasKey("CurrentDay"))
                currentDay = PlayerPrefs.GetInt("CurrentDay");
            else
                currentDay = 1;
        }

        private void UpdateUI()
        {
            if (fundsText != null)
                fundsText.SetText("Funds: ${0}", funds);

            if (seedsText != null)
                seedsText.SetText("Seeds: {0}", seeds);
            if (harvestText != null)
                harvestText.SetText("Harvest: {0}", harvest);
            if(dayText != null)
                dayText.SetText("Days: {0}", currentDay);
        }
        
        public void LoadScenebyName(string name)
        {
            SceneManager.LoadScene(name);
        }
        /*
        void Awake()
        {
            if(GameManager.instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
                Debug.Log("GameManager set through Awake");
            }
            else
            {
                Debug.Log("Duplicate GameManager attempted. Deleting new attempt");
                Destroy(this);
            }
        }
        */
    }
}