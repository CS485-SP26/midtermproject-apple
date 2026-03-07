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
        public SeedData[] availableSeeds;

        // Stores how many of each seed the player owns
        private Dictionary<SeedData, int> seedInventory = new Dictionary<SeedData, int>();
        // Tracks how many full bags each seed type has
        private Dictionary<SeedData, int> seedBagsPerType = new Dictionary<SeedData, int>();
        public Dictionary<PlantType, int> harvestInventory = new Dictionary<PlantType, int>();

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

        public void AddHarvest(PlantType plant, int amount)
        {
            //harvest += amount;
            if (!harvestInventory.ContainsKey(plant))
                harvestInventory[plant] = 0;
            harvestInventory[plant] += amount;
            UpdateUI();
        }

        public int GetPlantPrice(PlantType plant)
        {
            switch (plant)
            {
                case PlantType.Tomato: return 5;
                case PlantType.Onion: return 7;
                case PlantType.Pepper: return 10;
                case PlantType.Special: return 100;
                default: return 0;

            }
        }

        // Reset the stored harvest to zero and refresh UI
        public void ResetHarvest()
        {
            harvest = 0;
            harvestInventory.Clear();
            UpdateUI();
        }
        
        // variable amount to sell
        public void SellHarvest(PlantType plant, int amount)
        {
            if(!harvestInventory.ContainsKey(plant))return;
            int currentAmount = harvestInventory[plant];
            // Ensure we don't sell more than we have
            int amountToSell = Mathf.Min(amount, harvest);

            // if we have something to sell, reduce harvest and add funds
            if (amountToSell > 0)
            {
                harvestInventory[plant] -= amountToSell;
                int pricePerUnit = GetPlantPrice(plant);
                AddFunds(amountToSell * pricePerUnit);
            }
             // Remove plant type from dictionary if amount reaches 0
            if (harvestInventory[plant] == 0)
                harvestInventory.Remove(plant);
        }

        // You can call this from a "Sell All" button in Unity
        public void SellAll() 
        {
            //SellHarvest(harvest);
            foreach (var kvp in new Dictionary<PlantType, int>(harvestInventory)) // copy to avoid modification during iteration
            {
                PlantType plant = kvp.Key;
                int amount = kvp.Value;

                SellHarvest(plant, amount);
            }
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

        public void UpdateUI()
        {
            if (fundsText != null)
            {
                fundsText.SetText("Funds: ${0}", funds);
            }

            if (seedsText != null)
            {
                seedsText.SetText("Seeds: {0}", GetTotalSeeds());
            }

            if (harvestText != null)
            {
                int totalHarvest = 0;

                foreach (var kvp in harvestInventory)
                    totalHarvest += kvp.Value;

                harvestText.SetText("Harvest: {0}", totalHarvest);
            }

            if (dayText != null)
            {
                dayText.SetText("Days: {0}", currentDay);
            }
        }
        
        public void LoadScenebyName(string name)
        {
            SceneManager.LoadScene(name);
        }
        // ---------------- Seed Inventory Helpers ----------------
        public void InitializeSeeds()
        {
            foreach (SeedData seed in availableSeeds)
            {
                seedInventory[seed] = 0;
                seedBagsPerType[seed] = 0;
            }
            if (startingSeed != null)
            {
                seedInventory[startingSeed] = seedPerBag;
                seedBagsPerType[startingSeed] = 1;  // 1 bag
                seedBags = seedPerBag;  // Update total to reflect 16 seeds
            }
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
                    // Recalculate total seed bags from all types
                    seedBags = 0;
                    foreach (var bagCount in seedBagsPerType.Values)
                        seedBags += bagCount;
                }
                
            }
            UpdateUI();
        }
        public int GetTotalSeeds()
        {
            int total = 0;
            foreach (SeedData seed in availableSeeds)
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
            if (!seedInventory.ContainsKey(seed))
                seedInventory[seed] = 0;

            seedBagsPerType[seed]++;
            
            // Refill seeds for this type
            seedInventory[seed] += seedPerBag;
            seedBags = 0;
            foreach (var bagCount in seedBagsPerType.Values)
                seedBags += bagCount;

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