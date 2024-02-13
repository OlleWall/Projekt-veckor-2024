using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CounterScript : MonoBehaviour
{
    // Reference to the Text component for displaying the counter
    public Text counterText;

    // Counter variable
    private int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the counter text
        UpdateCounterText();
    }

    // Update is called once per frame
    void Update()
    {
        // Update the counter value
        counter++;

        // Update the counter text
        UpdateCounterText();
    }

    // Method to update the counter text
    void UpdateCounterText()
    {
        // Update the counter text with the current counter value
        counterText.text = "Counter: " + counter;
    }
}
