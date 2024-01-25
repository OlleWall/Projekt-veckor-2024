using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // Reference to the player's transform
    public float smoothSpeed = 0.125f;  // Smoothness of camera movement
    public Vector3 offset = new Vector3(0f, 4.2f, -9.2f); // Offset for the camera
    float playerStartY; // y positionen av spelaren i början

    void Start()
    {
        // definerar variablen
        playerStartY = player.transform.position.y;
    }

    void LateUpdate()
    {
        transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z); //simon

        // skapar en temporär variable för att inte tappa normala offseten. Olle
        Vector3 tempOffset = offset;

        // tar bort hur högt upp spelaren är för att inte kameran ska röra sig uppåt. Olle
        tempOffset.y -= player.position.y - playerStartY;

        // om offseten blir mindre än 0 ska den vara 0 detta så kameran inte åker över spelaren. Olle
        if (tempOffset.y < 0)
        {
            tempOffset = new Vector3(tempOffset.x, 0, tempOffset.z);
        }

        print(tempOffset);

        if (player != null)
        {
            Vector3 desiredPosition = player.position + tempOffset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
        }
    }
}