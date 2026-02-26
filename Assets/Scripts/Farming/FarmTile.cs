using System.Collections.Generic;
using UnityEngine;
using Environment;
using Farming;

namespace Farming 
{
    public class FarmTile : MonoBehaviour
    {
        public enum Condition { Grass, Tilled, Watered }

        [SerializeField] private Condition tileCondition = Condition.Grass; 

        [Header("Visuals")]
        [SerializeField] private Material grassMaterial;
        [SerializeField] private Material tilledMaterial;
        [SerializeField] private Material wateredMaterial;
        [SerializeField] private GameObject plantPrefab;
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
        }

        public void Interact()
        {
            Debug.Log("Tile: " + gameObject.name + 
          " | InstanceID: " + GetInstanceID() + 
          " | Condition: " + tileCondition);
            switch(tileCondition)
            {
                case FarmTile.Condition.Grass: Till(); break;
                case FarmTile.Condition.Tilled: Water(); break;
                case FarmTile.Condition.Watered: PlantSeed(); break;
            }
            Debug.Log("Condition AFTER: " + tileCondition);
            
        }

        public void Till()
        {
            tileCondition = FarmTile.Condition.Tilled;
            UpdateVisual();
            daysSinceLastInteraction = 0;
            tillAudio?.Play();
            
        }

        public void Water()
        {
            tileCondition = FarmTile.Condition.Watered;
            daysSinceLastInteraction = 0;
            if (currentPlant != null)
            {
                plantWateredToday = true;
                currentPlant.OnDayPassed(true); // immediately water the plant
            }
            else
            {
                tileCondition = Condition.Watered;
                UpdateVisual();
            }
            waterAudio?.Play();
        }
        private void PlantSeed()
        {
            Debug.Log("PlantSeed called on " + gameObject.name);
            if (currentPlant != null) return;
            if (plantPrefab == null)
            {
                Debug.LogWarning("No plantPrefab assigned on tile: " + gameObject.name);
                return;
            }
            
            Vector3 spawnPos = transform.position + new Vector3(0f, 0f, 0f);
            GameObject plantObj = Instantiate(plantPrefab, spawnPos, Quaternion.identity);
            plantObj.transform.parent = transform;
            currentPlant = plantObj.GetComponent<Plant>();
            if (currentPlant == null)
                Debug.LogWarning("Plant prefab does not have Plant.cs attached!");
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

        public void OnDayPassed()
        {
            daysSinceLastInteraction++;
            bool wasWatered = tileCondition == Condition.Watered || plantWateredToday;
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
            }
            farmer?.CheckTilesResetToGrass();

            UpdateVisual();
        }
        
    }
}