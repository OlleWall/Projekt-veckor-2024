using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Olle
public class GrabbableLogic : MonoBehaviour
{
    public PlayerMovement playerInventory;

    void Start()
    {
        playerInventory = FindObjectOfType<PlayerMovement>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PickedUp();
        }
    }

    public virtual void PickedUp()
    {
        
    }
}
