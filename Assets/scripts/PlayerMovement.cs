using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(1, 10)]
    float speed = 5; // den vanliga hastigheten

    float liveSpeed; // hastigheten som anv�nds i movment

    [SerializeField, Range(1, 10)]
    float crouchSpeed = 5; // hastigheten n�r man crouchar

    [SerializeField, Range(1, 100)]
    float jumpForce = 5; // kraften man hoppar med

    [SerializeField, Range(0.01f, 1f)]
    float coyoteTime = 0.05f; // hur l�nge innan man f�rlorar sitt hopp efter spelarn slutar nudda marken

    [SerializeField]
    BoxCollider2D crouchCollider; // collidern som ska disableas n�r man crouchar

    Rigidbody2D rb2D;

    float inputX;

    bool crouching = false;

    public LayerMask groundMask; // layermasken f�r marken

    bool airJump; // om man kan hoppa


    void Start()
    {
        // h�mtar rigidbody componenten
        rb2D = GetComponent<Rigidbody2D>();

        // h�mtar boxcollidern som d� �r den �vre
        crouchCollider = GetComponent<BoxCollider2D>();

        // definerar livespeed
        liveSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        // uppdaterar inputX med inputen i horizontella riktningen
        inputX = Input.GetAxisRaw("Horizontal");

        // om spelaren �r p� marken ska spelaren r�ra sig snabbare och t�tare annars ska den �ndra hastighet l�ngsamt
        if (GroundCheck())
        {
            rb2D.velocity = new Vector2(liveSpeed * inputX, rb2D.velocity.y);
        }
        // kollar s� spelaren bara accelererar n�r den �r under hur snabbt man g�r p� marken
        else if (rb2D.velocity.x < inputX * speed)
        {
            rb2D.AddForce(new Vector2(speed * 0.2f * inputX, 0), ForceMode2D.Force);
        }
        
        // kollar om W trycks ned
        if (Input.GetKey(KeyCode.W))
        {
            // kollar om spelaren inte redan har hoppat sedan man nuddade marken, det var 0.15 sekunder sedan man hoppa och om spelaren inte flyter
            if (airJump == true)
            {
                // stannar spelaren p� y axeln och trycker upp spelaren
                rb2D.velocity = new Vector2(rb2D.velocity.x, 0);
                rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
                // s�ger att man har hoppat i luften
                airJump = false;
            }
        }

        // n�r v�nster ctrl trycks ned och man inte �r crouchar under ett tak s� byter den crouch boolen till motsatta
        if (Input.GetKeyDown(KeyCode.LeftControl) && !CelingCheck())
        {
            crouching = !crouching;
        }

        // n�r crouching �r true disableas crouchCollider som �r den �vre collidern och liveSpeed byts till crouchSpeed som �r l�ngsammare
        if (crouching)
        {
            crouchCollider.enabled = false;
            liveSpeed = crouchSpeed;
        }
        // �ndrar tillbaka allt n�r crouching �r false
        else
        {
            crouchCollider.enabled = true;
            liveSpeed = speed;
        }

        // kollar functionen GroundCheck och f�r ett true false v�rde tillbaka
        if (GroundCheck())
        {
            // s�ger att spelaren kan hoppa
            airJump = true;
        }
    }

    bool GroundCheck()
    {
        // skapar en raycast som b�rjar i spelaren, �ker ner�t och kollar om den tr�ffar n�got i Floor layer och sparar detta i en hit varibael
        RaycastHit2D hit = Physics2D.CircleCast(transform.position - new Vector3(0, 0.95f, 0), 0.15f, -Vector2.up, 1f, groundMask);

        // om raycasten har tr�ffat n�gontig och punkten d�r den tr�ffa �r mindre �n 0.6 units fr�n spelaren
        if (hit.transform != null && hit.distance < 0.025F)
        {
            // skickar den true
            return true;
        }
        // om det inte �r sant
        else
        {
            // skickar den false
            return false;
        }
    }

    bool CelingCheck()
    {
        RaycastHit2D[] hit = Physics2D.BoxCastAll(transform.position + new Vector3(0, 0.5f, 0), new Vector2(0.9f, 1), 0, Vector2.up, 0, groundMask);

        foreach (RaycastHit2D item in hit)
        {
            if (item.transform != null)
            {
                return true;
            }
        }

        return false;
    }

    bool WallCheck()
    {
        RaycastHit2D[] hit = Physics2D.BoxCastAll(transform.position + new Vector3(0.01f * inputX, 0, 0), new Vector2(1f, 1.9f), 0, Vector2.up);

        foreach (RaycastHit2D item in hit)
        {
            if (item.collider.gameObject.layer != gameObject.layer)
            {
                print(item.collider.gameObject.layer);
                return true;
            }
        }
        print("hello");
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, 0.5f, 0), new Vector2(0.9f, 1));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + new Vector3(0.01f * inputX, 0, 0), new Vector2(1.1f, 1.9f));

    }
}
