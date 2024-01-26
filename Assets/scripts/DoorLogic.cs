using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Olle
public class DoorLogic : MonoBehaviour
{
    [SerializeField]
    int keyNumber;
    
    public void Open(List<int> keys)
    {
        foreach (int key in keys)
        {
            if (key == keyNumber)
            {
                gameObject.GetComponent<Collider2D>().isTrigger = !gameObject.GetComponent<Collider2D>().isTrigger;

                gameObject.GetComponent<SpriteRenderer>().enabled = !gameObject.GetComponent<SpriteRenderer>().enabled;
            }
        }
    }
}
