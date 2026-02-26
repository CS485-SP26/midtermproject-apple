using UnityEngine;
using UnityEngine.InputSystem;
using Farming;

namespace Character 
{
    [RequireComponent(typeof(PlayerInput))] // Input is required and we don't store a reference
    //[RequireComponent(typeof(Farmer))]
    public class PlayerController : MonoBehaviour
    {
        
        //[SerializeField] private GameObject gardenHoe;
        
        //[SerializeField] private GameObject waterCan;

        MovementController moveController;
        AnimatedController animatedController;
        Farmer farmer;

        void Start()
        {
            farmer = GetComponent<Farmer>();
            moveController = GetComponent<MovementController>();
    
            animatedController = GetComponent<AnimatedController>();

            // TODO: Consider Debug.Assert vs RequireComponent(typeof(...))
            Debug.Assert(animatedController, "PlayerController requires an animatedController");
            Debug.Assert(moveController, "PlayerController requires a MovementController");
            if (farmer != null)
                Debug.Log("Farmer type: " + farmer.GetType());
            //Debug.Assert(tileSelector, "PlayerController requires a TileSelector.");
        }
        public void OnMove(InputValue inputValue)
        {
            Vector2 inputVector = inputValue.Get<Vector2>();
            moveController.Move(inputVector);
        }

        public void OnJump(InputValue inputValue)
        {
            moveController.Jump();
        }

        /*
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
        */

        public void OnInteract(InputValue value)
        { 
            Debug.Log("Interact");
            farmer.TryTileInteraction();
            /*
            FarmTile tile = tileSelector.GetSelectedTile();
            if (tile != null)
            {
                tile.Interact(); // updates the condition, play the anim after
                switch (tile.GetCondition)
                {
                    case FarmTile.Condition.Tilled: animatedController.SetTrigger("Till"); break;
                    case FarmTile.Condition.Watered: animatedController.SetTrigger("Water"); break;
                    default: break;
                }
            }
            */
        }
    }
}