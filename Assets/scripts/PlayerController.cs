using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public KeyCode switchLayerKey = KeyCode.Space;  // The key to switch between layers
    public SpriteRenderer playerSpriteRenderer;     // Reference to the player's sprite renderer
     string frontSortingLayer = "PlayerForeground"; // Sorting layer for in front of the wall
     string behindSortingLayer = "PlayerBackground"; // Sorting layer for behind the wall

    private bool isBehindWall = false; // Flag to track whether the player is behind the wall

    void Update()
    {
        // Check if the switchLayerKey is pressed
        if (Input.GetKeyDown(switchLayerKey))
        {
            // Toggle between behind and in front of the wall
            isBehindWall = !isBehindWall;

            // Update the sorting layer accordingly
            if (isBehindWall)
            {
                
                playerSpriteRenderer.sortingLayerName = behindSortingLayer;
            }
            else
            {
                playerSpriteRenderer.sortingLayerName = frontSortingLayer;
            }
        }
    }
}
