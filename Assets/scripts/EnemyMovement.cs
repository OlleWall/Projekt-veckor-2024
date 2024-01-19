using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    public GameObject player;

    public float enemySpeed = 2f;
    public Rigidbody2D rb;

    public bool playerIsToTheRight;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMovement>().gameObject;

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Update()
    {
        if(player.transform.position.x < transform.position.x)
        {
            playerIsToTheRight = true;
        }
        else
        {
            playerIsToTheRight = false;
        }

        if (playerIsToTheRight)
        {
            rb.velocity = new Vector2(-enemySpeed, 0);
        }
        if (!playerIsToTheRight)
        {
            rb.velocity = new Vector2(enemySpeed, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject == player)
        {
            print("switch scene");
            SceneManager.LoadScene("MainMenu");
            Destroy(player);
        }
    }
}
