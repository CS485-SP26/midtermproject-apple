using UnityEngine;
using Character;


namespace Farming
{
    [RequireComponent(typeof(AnimatedController))]
    public class Farmer : MonoBehaviour
    {
        //[SerializeField] private TileSelector tileSelector;
        [SerializeField] private GameObject gardenHoe;
        [SerializeField] private GameObject waterCan;
        [SerializeField] private GameObject waterSourceObject;
        [SerializeField] private ProgressBar waterLevelUI;
        [SerializeField] private float waterLevel = 1f;
        [SerializeField] private float waterPerUse = 0.1f;

        //MovementController moveController;
        AnimatedController animatedController;

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
       
        public void TryTileInteraction(FarmTile tile)
        {
            Debug.Log("TryTileInteraction called");
            //FarmTile tile = tileSelector.GetSelectedTile();
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
                    }
                    break;
                default: break;
            }
            
        }

        public void OnTriggerEnter(Collider other)
        {
            if ((other.gameObject == waterSourceObject))
            {
                
                if (waterLevel < 1f)
                {
                    waterLevel = 1f;
                    waterLevelUI.Fill = waterLevel;  
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
        
    }
}

