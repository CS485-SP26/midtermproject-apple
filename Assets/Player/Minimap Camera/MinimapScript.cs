using UnityEngine;

public class MinimapScript : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    void LateUpdate()
    {
        Vector3 newPosition = player.position; // Get the player's position
        newPosition.y = transform.position.y; // Keep the minimap camera's height constant
        transform.position = newPosition; // Update the minimap camera's position
    }
}
