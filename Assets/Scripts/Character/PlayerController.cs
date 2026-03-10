using UnityEngine;
using UnityEngine.InputSystem;
using Farming;

namespace Character 
{
    [RequireComponent(typeof(PlayerInput))] // Input is required and we don't store a reference
    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(AnimatedController))]
    //[RequireComponent(typeof(Farmer))]
    public class PlayerController : MonoBehaviour
    {
        
        //[SerializeField] private GameObject gardenHoe;
        
        //[SerializeField] private GameObject waterCan;

        private MovementController moveController;
        private AnimatedController animatedController;
        private Farmer farmer;

        private void Awake()
        {
            moveController = GetComponent<MovementController>();
            animatedController = GetComponent<AnimatedController>();
            farmer = GetComponent<Farmer>();

            // TODO: Consider Debug.Assert vs RequireComponent(typeof(...))
            Debug.Assert(animatedController, "PlayerController requires an AnimatedController");
            Debug.Assert(moveController, "PlayerController requires a MovementController");
            //Debug.Assert(tileSelector, "PlayerController requires a TileSelector.");
        }

        public void OnMove(InputValue inputValue)
        {
            if (moveController == null)
            {
                return;
            }

            Vector2 inputVector = inputValue.Get<Vector2>();
            moveController.Move(inputVector);
        }

        public void OnJump(InputValue inputValue)
        {
            if (moveController == null)
            {
                return;
            }

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
            if (farmer == null)
            {
                return;
            }

            Debug.Log("Interact");
            farmer.TryTileInteraction();
        }
    }
}