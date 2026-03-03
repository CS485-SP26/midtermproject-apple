using System.Collections.Generic;
using UnityEngine;
using Environment;
using Farming;
using Core;

namespace Farming 
{
    public class FarmTile : MonoBehaviour
    {
        public enum Condition { Grass, Tilled, Watered, Planted}

        [SerializeField] private Condition tileCondition = Condition.Grass; 

        [Header("Visuals")]
        [SerializeField] private Material grassMaterial;
        [SerializeField] private Material tilledMaterial;
        [SerializeField] private Material wateredMaterial;
        //[SerializeField] private GameObject plantPrefab;
        private MeshRenderer tileRenderer;

        [Header("Audio")]
        [SerializeField] private AudioSource stepAudio;
        [SerializeField] private AudioSource tillAudio;
        [SerializeField] private AudioSource waterAudio;

        private readonly List<Material> materials = new List<Material>();
        private Farmer farmer;
        private Plant currentPlant;

        private int daysSinceLastInteraction = 0;
        private bool plantWateredToday = false;
        public FarmTile.Condition GetCondition { get { return tileCondition; } } // TODO: Consider what the set would do?

        void Start()
        {
            tileRenderer = GetComponent<MeshRenderer>();
            Debug.Assert(tileRenderer, "FarmTile requires a MeshRenderer");
            farmer = FindFirstObjectByType<Farmer>();

            materials.Clear();
            materials.Capacity = transform.childCount;

            foreach (Transform edge in transform)
            {
                if (edge.TryGetComponent<MeshRenderer>(out var edgeRenderer))
                {
                    materials.Add(edgeRenderer.material);
                }
            }

            // Load saved condition on start
            if (PlayerPrefs.HasKey(gameObject.name + "_condition"))
            {
                tileCondition = (Condition)PlayerPrefs.GetInt(gameObject.name + "_condition");
                UpdateVisual();
            }

            // Load saved plant on start
            if (PlayerPrefs.HasKey(gameObject.name + "_has_plant") && PlayerPrefs.GetInt(gameObject.name + "_has_plant") == 1)
            {
                // Load the seed type that was saved (you may need to save selectedSeedName as string)
                string seedName = PlayerPrefs.GetString(gameObject.name + "_selected_seed", null);
                SeedData seedData = null;

                foreach (SeedData s in GameManager.Instance.avaiableSeeds)
                {
                    if (s.seedName == seedName)
                    {
                        seedData = s;
                        break;
                    }
                }

                if (seedData != null)
                {
                    Vector3 spawnPos = transform.position;
                    GameObject plantObj = Instantiate(seedData.plantedModel, spawnPos, Quaternion.identity);
                    plantObj.transform.parent = transform;
                    currentPlant = plantObj.GetComponent<Plant>();
                    if (currentPlant != null && PlayerPrefs.HasKey(gameObject.name + "_plant_state"))
                    {
                        currentPlant.currentState = (Plant.PlantState)PlayerPrefs.GetInt(gameObject.name + "_plant_state");
                        currentPlant.PlantSeed(seedData); // Assign SeedData to Plant
                    }
                }
            }
        }

        public void Interact()
        {
            /*
            Debug.Log("Tile: " + gameObject.name + 
          " | InstanceID: " + GetInstanceID() + 
          " | Condition: " + tileCondition);
          */
          if(currentPlant != null && currentPlant.IsMature())
            {
                Harvest();
                return;
            }
            switch(tileCondition)
            {
                case FarmTile.Condition.Grass: Till(); break;
                case FarmTile.Condition.Tilled: Water();break;
                case FarmTile.Condition.Watered:PlantSeed();break;
                case FarmTile.Condition.Planted: Water();break;
            }
            Debug.Log("Condition AFTER: " + tileCondition);
            
        }

        public void Till()
        {
            tileCondition = FarmTile.Condition.Tilled;
            UpdateVisual();
            daysSinceLastInteraction = 0;
            tillAudio?.Play();
            // Set the tile's condition in PlayerPrefs so it persists across sessions
            PlayerPrefs.SetInt(gameObject.name + "_condition", (int)tileCondition);
            
        }

        public void Water()
        {
            tileCondition = FarmTile.Condition.Watered;
            daysSinceLastInteraction = 0;
            if (currentPlant != null)
            {
                plantWateredToday = true;
                tileCondition = Condition.Watered;
                UpdateVisual();
            }
            else
            {
                tileCondition = Condition.Watered;
                UpdateVisual();
            }
            
            waterAudio?.Play();
            // Set the tile's condition in PlayerPrefs so it persists across sessions
            PlayerPrefs.SetInt(gameObject.name + "_condition", (int)tileCondition);
        }
        private void PlantSeed()
        {
            // Check if player has seeds
            if (GameManager.Instance.seeds <= 0)
            {
                Debug.Log("No seeds available to plant!");
                Farmer farmer = FindFirstObjectByType<Farmer>();
                if(farmer != null) farmer.DisplayLowSeed();
                return;
                
            }
            if (currentPlant != null) return;
            UIManager.Instance.OpenSeedPopUp(this);
        }
        public void PlanetSelectedSeed(SeedData seed)
        {
            if (seed == null) return;
            if (!GameManager.Instance.HasSeed(seed)) return;

            
            Vector3 spawnPos = transform.position;
            GameObject plantObj = Instantiate(seed.plantPrefab, spawnPos, Quaternion.identity);
            plantObj.transform.parent = transform;

            currentPlant = plantObj.GetComponent<Plant>();
            if (currentPlant != null)
            {
                currentPlant.PlantSeed(seed);
                //GameManager.Instance.UseSeed(seed);
            }
            else
            {
                Debug.LogError("Planted model prefab does not have a Plant script!");
                return;
            }
            
            //plantWateredToday = true;
            //tileCondition = Condition.Planted;
            
            //UpdateVisual();

            
            PlayerPrefs.SetInt(gameObject.name + "_has_plant", 1);
            PlayerPrefs.SetInt(gameObject.name + "_plant_state", (int)currentPlant.currentState);
            PlayerPrefs.SetString(gameObject.name + "_selected_seed", seed.seedName);
        
        }
        public bool HasMaturePlant()
        {
            return currentPlant != null && currentPlant.IsMature();
        }
        private void Harvest()
        {
            if (currentPlant == null) return;

            Debug.Log("Harvesting plant on " + gameObject.name);

            Destroy(currentPlant.gameObject);
            currentPlant = null;

            // Remove saved plant data
            PlayerPrefs.DeleteKey(gameObject.name + "_has_plant");
            PlayerPrefs.DeleteKey(gameObject.name + "_plant_state");

            GameManager.Instance.AddHarvest(1);

        }

        private void UpdateVisual()
        {
            if(tileRenderer == null) return;
            
            switch(tileCondition)
            {
                case FarmTile.Condition.Grass: tileRenderer.material = grassMaterial; break;
                case FarmTile.Condition.Tilled: tileRenderer.material = tilledMaterial; break;
                case FarmTile.Condition.Watered: tileRenderer.material = wateredMaterial; break;
            }
            
        }

        public void SetHighlight(bool active)
        {
            foreach (Material m in materials)
            {
                if (active)
                {
                    m.EnableKeyword("_EMISSION");
                } 
                else 
                {
                    m.DisableKeyword("_EMISSION");
                }
            }
            if (active) stepAudio.Play();
        }
        public bool IsEffectivelyWatered()
        {
            // Counts as watered if tile itself is watered
            if (tileCondition == Condition.Watered)
                return true;

            // Or if the plant is alive
            if (currentPlant != null)
                return currentPlant.currentState != Plant.PlantState.Whithered;

            return false;
        }
        public bool IsEffectivelyPlanted()
        {
            // A tile counts as planted if it’s visually planted or has a plant
            return tileCondition == Condition.Planted || currentPlant != null;
        }
        

        public void OnDayPassed()
        {
            daysSinceLastInteraction++;
            //bool wasWatered = tileCondition == Condition.Watered || plantWateredToday;
            bool wasWatered = currentPlant != null && (tileCondition == Condition.Watered || plantWateredToday);
            if(daysSinceLastInteraction >= 2)
            {
                if(tileCondition == FarmTile.Condition.Watered) tileCondition = FarmTile.Condition.Tilled;
                else if(tileCondition == FarmTile.Condition.Tilled) tileCondition = FarmTile.Condition.Grass;
            }
            //water Info
            if(currentPlant != null)
            {
                currentPlant.OnDayPassed(wasWatered);
                plantWateredToday = false;
                // Save updated plant state
                PlayerPrefs.SetInt(gameObject.name + "_plant_state", (int)currentPlant.currentState);
            }
            farmer?.CheckTilesResetToGrass();

            UpdateVisual();
            // Set the tile's condition in PlayerPrefs so it persists across sessions
            PlayerPrefs.SetInt(gameObject.name + "_condition", (int)tileCondition);
        }
        
        public void ResetToTilled()
        {
            tileCondition = Condition.Tilled;
            UpdateVisual();
            // Set the tile's condition in PlayerPrefs so it persists across sessions
            PlayerPrefs.SetInt(gameObject.name + "_condition", (int)tileCondition);
        }
    }
}