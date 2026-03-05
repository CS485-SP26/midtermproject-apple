using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using Farming;

namespace Core
{
    public class GameManager:MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private SeedData startingSeed; // Tomato
        public int funds = 100;
        public int seedBags = 1;
        public int harvest = 0;

        public int currentDay = 1;

        //Seed Data
        public SeedData selectedSeed;
        public SeedData[] avaiableSeeds;

        // Stores how many of each seed the player owns
        private Dictionary<SeedData, int> seedInventory = new Dictionary<SeedData, int>();
        // Tracks how many full bags each seed type has
        private Dictionary<SeedData, int> seedBagsPerType = new Dictionary<SeedData, int>();

        private const int seedPerBag = 16;


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
            // Ensure currentDay matches GameManager
            currentDay = GameManager.Instance.currentDay;
            InitializeSeeds();
            
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
            seedBags += amount;
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
                //int totalSeeds = 0;
                /*
                foreach (SeedData seed in avaiableSeeds)
                    seedBags += seedInventory[seed]; // total seeds remaining in all types
                */
                seedsText.SetText("Seeds: {0}", seedBags);
            if (harvestText != null)
                harvestText.SetText("Harvest: {0}", harvest);
            if(dayText != null)
                dayText.SetText("Days: {0}", currentDay);
        }
        
        public void LoadScenebyName(string name)
        {
            SceneManager.LoadScene(name);
        }
        // ---------------- Seed Inventory Helpers ----------------
        public void InitializeSeeds()
        {
            foreach (SeedData seed in avaiableSeeds)
            {
                seedInventory[seed] = 0;
                seedBagsPerType[seed] = 0;
            }
            seedInventory[startingSeed] = seedPerBag;
            seedBagsPerType[startingSeed] = seedBags;
        }

        public int GetSeedCount(SeedData seed)
        {
            if (seedInventory.ContainsKey(seed))
            {
                return seedInventory[seed];
            }
            return 0;
        }

        // Check if player has at least 1 seed
        public bool HasSeed(SeedData seed)
        {
            return GetSeedCount(seed) > 0;
        }

        // Reduce seed by 1 when planting
        public void UseSeed(SeedData seed)
        {
            if (!HasSeed(seed))
                return;

            seedInventory[seed]--;
            if(seedInventory[seed] <= 0)
            {
                seedInventory[seed] = 0;
                if(seedBagsPerType[seed] > 0)
                {
                    seedBagsPerType[seed]--;
                    seedBags = seedBagsPerType[seed];
                }
                
            }
            UpdateUI();
        }
        public int GetTotalSeeds()
        {
            int total = 0;
            foreach (SeedData seed in avaiableSeeds)
            {
                total += GetSeedCount(seed);
            }
            return total;
        }
        public void BuySeedBag(SeedData seed)
        {
            // Increment bag count for this seed type
            if (!seedBagsPerType.ContainsKey(seed))
                seedBagsPerType[seed] = 0;

            seedBagsPerType[seed]++;
            seedBags = seedBagsPerType[seed];
            
            // Refill seeds for this type
            seedInventory[seed] += seedPerBag;

            // Refresh UI
            UpdateUI();
        }
        /*
        public bool HasSeed(SeedData seed)
        {
            // Simple check: if player has seeds > 0
            return seeds > 0;
        }
        public void UseSeed(SeedData seed)
        {
            seeds = Mathf.Max(0, seeds - 1);
            UpdateUI();
        }
        */

        public void ResetHarvest(Farming.FarmTile tile)
        {
            tile.ResetToTilled();
        }



    }
    
}