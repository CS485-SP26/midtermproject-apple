using System;
using UnityEngine;

namespace Character 
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] public GameObject player;
        [SerializeField] private Vector3 offset = new(0f, 0f, -3f);
        private Transform playerTransform;

        void Start()
        {
            Debug.Assert(player, "CameraFollow requires a player (GameObject).");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        void LateUpdate()
        {
            if (playerTransform == null) return;
            transform.position = playerTransform.position + offset;         
        }
    }
}
