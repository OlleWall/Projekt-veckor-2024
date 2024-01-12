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
        // skapar en tempor�r variable f�r att inte tappa normala offseten. Olle
        Vector3 tempOffset = offset;

        // om spelaren �r �ver 4 units upp. Olle
        if (player.position.y >= 4)
        {
            // tar bort hur h�gt upp spelaren �r f�r att inte kameran ska r�ra sig upp�t
            // tar bort 4 f�r att den inte ska hoppa upp n�r spelaren �r �ver 4 units. Olle
            tempOffset.y -= player.position.y - 4;
            
            // om offseten blir mindre �n 0 ska den vara noll detta s� kameran inte �ker �ver spelaren. Olle
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


