using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class TeddyBear : GrabbableLogic
{
    // Reference to the GameManager script
    private WinCollisions winCollisions;

    void Start()
    {

        winCollisions = FindObjectOfType<WinCollisions>();

        if (winCollisions == null)
        {
            Debug.LogError("WinCollision script not found in the scene.");
        }
    }

    public override void PickedUp()
    {
        winCollisions.TeddyBear++;
    }




    static void Main()
    {
        // Declares and initializeces a counter variable
        int counter = 0;

       
        for (int i = 0; i < 5; i++)
        {
            
            counter++;

            // Display the current value of the counter
            Console.WriteLine("Counter: " + counter);
        }
    }
}



  
   
        
    








