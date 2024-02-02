using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float detectionRange = 5f;
    public LayerMask playerLayer;

    private Transform player;
    private bool playerInCover;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("Player not found!");
        }
    }

    void Update()
    {
        // Check if player is in cover
        playerInCover = Physics2D.Raycast(transform.position, Vector2.right, detectionRange, playerLayer);

        if (playerInCover)
        {
            // If player is in cover, look towards the player
            LookAtPlayer();
        }
    }

    void LookAtPlayer()
    {
        Vector3 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }
}


