using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class CounterDisplay : MonoBehaviour
{
    public Text counterText; // Reference to the Text component

    private int counter = 0;

    void Start()
    {
        // Ensure the Text component is assigned in the Inspector
        if (counterText == null)
        {
            Debug.LogError("Counter Text component is not assigned!");
        }
        else
        {
            // Update the initial text
            UpdateCounterText();
        }
    }

    void Update()
    {
        // Example: Increment the counter every frame (you can modify this as needed)
        counter++;

        // Update the displayed text
        UpdateCounterText();
    }

    void UpdateCounterText()
    {
        // Display the counter value in the Text component
        counterText.text = "Counter: " + counter;
    }
}

