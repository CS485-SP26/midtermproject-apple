using UnityEngine;
using Character;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Core;
namespace Farming
{
    [RequireComponent(typeof(AnimatedController))]
    public class Farmer : MonoBehaviour
    {
        [SerializeField] private TileSelector tileSelector;
        [SerializeField] private GameObject gardenHoe;
        [SerializeField] private GameObject waterCan;
        [SerializeField] private GameObject waterSourceObject;
        [SerializeField] private ProgressBar waterLevelUI;
        [SerializeField] private float waterLevel = 1f;
        [SerializeField] private float waterPerUse = 0.1f;

        //MovementController moveController;
        AnimatedController animatedController;

        [SerializeField] private List<FarmTile> farmTiles; // List of all farm tiles

        [SerializeField] private float rewardAmount = 50f; // Amount to award when all tiles are watered

        private bool rewardGiven = false;

        [SerializeField] private TMP_Text congratulationsText; // TMP Text to display the congratulations message

        private float congratulationsDuration = 3f; // Duration to show the congratulations message (in seconds)
        [SerializeField] private TMP_Text waterRefillText;

        void Start()
        {
            //moveController = GetComponent<MovementController>();
            
            // TODO: Consider Debug.Assert vs RequireComponent(typeof(...))
             //Debug.Log(animatedController);
            if (gardenHoe != null) gardenHoe.SetActive(false);
            if (waterCan != null) waterCan.SetActive(false);
            if(animatedController == null)
                animatedController = GetComponentInChildren<AnimatedController>();

            if(animatedController == null)
                Debug.LogError("Farmer requires an AnimatedController! Animation will not play.");
            Debug.Assert(waterLevelUI, "Farmer requires an waterLevel");
            
            animatedController = GetComponentInChildren<AnimatedController>();
            waterLevelUI.setText("Water Level");

            // Collect all tiles in the scene
            farmTiles = new List<FarmTile>(Object.FindObjectsByType<FarmTile>(FindObjectsSortMode.None));
        }
        
        public void SetTool(string tool)
        {
            if (gardenHoe != null) gardenHoe.SetActive(false);
            if (waterCan != null) waterCan.SetActive(false);
            switch (tool)
            {
                case "GardenHoe": gardenHoe.SetActive(true); break;
                case "WaterCan": waterCan.SetActive(true); break;
            }
        }
       
        public void TryTileInteraction()
        {
            Debug.Log("TryTileInteraction called");
            FarmTile tile = tileSelector.GetSelectedTile();
            if(tile == null) return;
            // updates the condition, play the anim after
            switch (tile.GetCondition)
            {
                case FarmTile.Condition.Grass:
                    animatedController.SetTrigger("Till");
                    tile.Interact();
                    break;
                case FarmTile.Condition.Tilled: 
                    if(waterLevel > waterPerUse)
                    {
                        animatedController.SetTrigger("Water");
                        tile.Interact();
                        waterLevel -= waterPerUse;
                        waterLevelUI.Fill = waterLevel;
                        if(waterLevel <= 0.1)
                        {
                            DisplayWaterLow();
                        }
                    }
                    else
                    {
                        DisplayWaterLow();
                    }
                    break;
                default: break;
            }
            // Check if all tiles are watered
            CheckWinCondition();
        }

        public void DisplayWaterLow()
        {
            waterRefillText.text = "Water low";
            waterRefillText.gameObject.SetActive(true);
            StartCoroutine(HideWaterMessage());
        }
        public void DisplayWaterRefilled()
        {
            waterRefillText.text = "Water Refilled";
            waterRefillText.gameObject.SetActive(true);
            StartCoroutine(HideWaterMessage());
        }
        private IEnumerator HideWaterMessage()
        {
            // Wait for the specified duration
            yield return new WaitForSeconds(congratulationsDuration);

            // Hide the message after the wait
            waterRefillText.gameObject.SetActive(false);
        }

        public void OnTriggerEnter(Collider other)
        {
            if ((other.gameObject == waterSourceObject))
            {
                
                if (waterLevel < 1f)
                {
                    waterLevel = 1f;
                    waterLevelUI.Fill = waterLevel;
                    DisplayWaterRefilled();  
 
                }
            }
        }
        public void OnTriggerExit(Collider other)
        {
            if ((other.gameObject == waterSourceObject))
            {
                Debug.Log("Left water source.");
            }
        }
        
        private void CheckWinCondition()
        {
            if (rewardGiven) return;

            bool allWatered = true;
            foreach (var tile in farmTiles)
            {
                if (tile.GetCondition != FarmTile.Condition.Watered)
                {
                    allWatered = false;
                    break;
                }
            }

            if (allWatered)
            {
                DisplayWinMessage();
                AwardFunds();
            }
        }

        private void DisplayWinMessage()
        {
            // Display the congratulations message in the UI (TMP)
            congratulationsText.text = "Congratulations! All tiles are watered.";
            congratulationsText.gameObject.SetActive(true); // Make sure the message is visible

            // Start the coroutine to hide the message after a few seconds
            StartCoroutine(HideCongratulationsMessage());
        }

        private IEnumerator HideCongratulationsMessage()
        {
            // Wait for the specified duration
            yield return new WaitForSeconds(congratulationsDuration);

            // Hide the message after the wait
            congratulationsText.gameObject.SetActive(false);
        }

        private void AwardFunds()
        {
            // Award funds to the player, only once
            if (!rewardGiven)
            {
                rewardGiven = true;
                GameManager.Instance.AddFunds(50);
                Debug.Log($"You have been awarded {rewardAmount} funds!");
            }
        }

        // This method will be called to reset the reward condition when all tiles are back to grass
        public void CheckTilesResetToGrass()
        {
            bool allGrass = true;
            foreach (var tile in farmTiles)
            {
                if (tile.GetCondition != FarmTile.Condition.Grass)
                {
                    allGrass = false;
                    break;
                }
            }

            if (allGrass)
            {
                rewardGiven = false; // Reset the reward condition once all tiles are grass again
            }
        }
    }
}

