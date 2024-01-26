using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Olle
public class KeyID : GrabbableLogic
{
    [SerializeField]
    public int keyNumber;

    public override void PickedUp()
    {
        playerInventory.inventory.Add(keyNumber);

        gameObject.SetActive(false);
    }
}
