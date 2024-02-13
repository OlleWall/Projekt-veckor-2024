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

    public bool followingPlayer = false;
    public bool moveRight;

    public float playerDetectionTime;
    public float timeBeforeLosingPlayer;

    float moveOrIdleFloat;
    public bool moving;
    public bool idling;

    public float minMoveTime;
    public float maxMoveTime;

    public float timeToMove;

    float _stateTimer;
    public float stateTimer
    {
        get
        {
            return _stateTimer;
        }

        private set
        {
            _stateTimer = value;
            if(stateTimer >= timeToMove)
            {
                stateTimer = 0;
                timeToMove = Random.Range(minMoveTime, maxMoveTime);
                moveOrIdleFloat = Random.value;
                directionDecider = true;
            }
        }
    }

    public bool playerIsToTheRight;


    bool directionDecider = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMovement>().gameObject;

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        timeToMove = Random.Range(minMoveTime, maxMoveTime);
        moveOrIdleFloat = Random.value;
        
        
    }

    private void Update()
    {
        #region OldMovement
        if (player.transform.position.x < transform.position.x)
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
        #endregion

        stateTimer += Time.deltaTime;

        if (moveOrIdleFloat <= 0.70)
        {
            idling = false;
            moving = true;

            if (directionDecider)
            {
                directionDecider = false;
                float leftorright = Random.value;
                moveRight = leftorright <= 0.5f ? true : false;
            }
        }
        else if(moveOrIdleFloat > 0.70)
        {
            moving = false;
            idling = true;
        }

        //visual cone kod, om spelaren är i visual cone, starta en playerdetectiontimer, om den når sitt värde så ska followingPlayer = true
        //om spelaren var i visual cone och går ut, starta losingplayertimer

        //tänker kanske att visual cone kan vara en cirkelsektor från fiendet utåt i "radius = visualrange" typ

        //sebastian lague tutorial/video om visual cone

        if (moveRight)
        {
            //riktad åt höger
        }
        else
        {
            //riktad åt vänster
        }

        if (moving)
        {
            if (followingPlayer)
            {
                //fiende chase kod (rör sig mot spelaren)
            }
            else
            {
                //fiende random movement kod
                if (moveRight)
                {
                    rb.velocity = new Vector2(enemySpeed, 0);
                }
                else
                {
                    rb.velocity = new Vector2(-enemySpeed, 0);
                }
            }
        }
        else if (idling)
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject == player)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
