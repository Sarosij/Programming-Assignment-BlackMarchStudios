using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// <summary>
// This script handles the raycasting from the mouse position to detect tiles in a grid and updates UI text with the tile's position.
// </summary>

public class MouseRayCast : MonoBehaviour
{
    public Text infoText; // Reference to the UI Text component for displaying tile information
    void Update()
    {
        // Create a ray from the camera through the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Performing the raycast and check if it hits a collider
        if (Physics.Raycast(ray, out hit))
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null)
            {
                infoText.text = $"Tile Position: ({tile.x}, {tile.y})"; // Update the UI Text with the position of the tile
            }
        }
        else
        {
            infoText.text = ""; // Clearing the UI Text if no tile is hit
        }
    }
}
