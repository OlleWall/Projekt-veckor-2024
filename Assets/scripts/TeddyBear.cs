using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeddyBear : GrabbableLogic

{
    // Reference to the GameManager script
    private  WinCollisions winCollisions;

    void Start()
    {

        winCollisions = FindObjectOfType<WinCollisions>();

        if (winCollisions == null)
        {
            Debug.LogError("Wincollision script not found in the scene.");
        }
    }

    public override void PickedUp()
    {
        winCollisions.TeddyBear++;
    }
}


