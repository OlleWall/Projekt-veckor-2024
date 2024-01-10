using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(1, 10)]
    float speed = 5;

    float liveSpeed;

    [SerializeField, Range(1, 10)]
    float crouchSpeed = 5;

    [SerializeField, Range(1, 100)]
    float jumpForce = 5;

    [SerializeField]
    BoxCollider2D crouchCollider;

    bool crounching = false;

    public int facingRight;
    public LayerMask mask;
    bool airJump;
    public bool WallForward;

    Rigidbody2D rb2D;

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
        // om spelaren �r p� marken ska man kunna r�ra sig p� detta s�ttet
        if (GroundCheck())
        {
            // n�r D knappen trycks ned r�rs spelaren �t h�ger
            if (Input.GetKey(KeyCode.D))
            {
                facingRight = 1;
                rb2D.velocity = new Vector2(liveSpeed, rb2D.velocity.y);
            }
            // n�r A knappen trycks ned r�rs spelaren �t v�nster
            else if (Input.GetKey(KeyCode.A))
            {
                facingRight = -1;
                rb2D.velocity = new Vector2(-liveSpeed, rb2D.velocity.y);
            }
            // om inget trycks ned s� stannar spelarn
            else
            {
                rb2D.velocity = new Vector2(0, rb2D.velocity.y);
            }
        }
        else
        {
            rb2D.AddForce(new Vector2(speed * 0.1f, 0), ForceMode2D.Force);
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

        if (Input.GetKeyDown(KeyCode.LeftControl) && !CelingCheck())
        {
            crounching = !crounching;
        }

        if (crounching)
        {
            crouchCollider.isTrigger = true;
            liveSpeed = crouchSpeed;
        }
        else
        {
            crouchCollider.isTrigger = false;
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
        RaycastHit2D hit = Physics2D.CircleCast(transform.position - new Vector3(0, 0.95f, 0), 0.15f, -Vector2.up, 1f, mask);

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
        RaycastHit2D[] hit = Physics2D.BoxCastAll(transform.position + new Vector3(0, 0.5f, 0), new Vector2(0.9f, 1), 0, Vector2.up, 0, mask);

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
        RaycastHit2D[] hit = Physics2D.BoxCastAll(transform.position + new Vector3(0.01f * facingRight, 0, 0), new Vector2(1f, 1.9f), 0, Vector2.up);

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
        Gizmos.DrawWireCube(transform.position + new Vector3(0.01f * facingRight, 0, 0), new Vector2(1.1f, 1.9f));

    }
}
