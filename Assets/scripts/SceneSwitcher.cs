using Packages.Rider.Editor.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public string targetSceneName; // Name of the scene that its gonna switch to
    public string playerTag = "Player"; // Tag of the player GameObject

    private void OnTriggerEnter(Collider other)
    {
        // Checks if the entering object has the player tag
        if (other.CompareTag(playerTag))
        {
            // Switches to the target scene
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
