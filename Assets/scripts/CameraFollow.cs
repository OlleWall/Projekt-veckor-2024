using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // Reference to the player's transform
    public float smoothSpeed = 0.125f;  // Smoothness of camera movement
    public Vector3 offset = new Vector3(0f, 4.2f, -9.2f); // Offset for the camera


    void LateUpdate()
    {
        // skapar en temporär variable för att inte tappa normala offseten. Olle
        Vector3 tempOffset = offset;

        // om spelaren är över 4 units upp. Olle
        if (player.position.y >= 4)
        {
            // tar bort hur högt upp spelaren är för att inte kameran ska röra sig uppåt
            // tar bort 4 för att den inte ska hoppa upp när spelaren är över 4 units. Olle
            tempOffset.y -= player.position.y - 4;
            
            // om offseten blir mindre än 0 ska den vara noll detta så kameran inte åker över spelaren. Olle
            if (tempOffset.y < 0)
            {
                tempOffset = new Vector3(tempOffset.x, 0, tempOffset.z);
            }
        }

        if (player != null)
        {
            Vector3 desiredPosition = player.position + tempOffset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
            
        }
    }
}


