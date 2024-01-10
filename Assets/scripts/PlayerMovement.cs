using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(1, 15)]
    float runningSpeed = 10;

    [SerializeField, Range(1, 10)]
    float speed = 5; // den vanliga hastigheten

    float liveSpeed; // hastigheten som anv�nds i movment

    [SerializeField, Range(1, 10)]
    float crouchSpeed = 5; // hastigheten n�r man crouchar

    [SerializeField, Range(1, 100)]
    float jumpForce = 5; // kraften man hoppar med

    [SerializeField, Range(0.01f, 1f)]
    float coyoteTime = 0.05f; // hur l�nge innan man f�rlorar sitt hopp efter spelarn slutar nudda marken

    float coyoteTimer; // sj�lva variabeln som r�knar upp

    bool canJump; // om man kan hoppa

    [SerializeField]
    BoxCollider2D crouchCollider; // collidern som ska disableas n�r man crouchar

    Rigidbody2D rb2D;

    float inputX;

    bool crouching = false;

    [SerializeField]
    LayerMask groundMask; // layermasken f�r marken

    [SerializeField]
    LayerMask ladderMask; // layermasken f�r stegar

    public bool climbing = false;

    [SerializeField, Range(0, 10)]
    float climbingSpeed = 3; // hastigheten n�r man kl�trar

    int facingRight;

    float gravity;

    void Start()
    {
        // h�mtar rigidbody componenten
        rb2D = GetComponent<Rigidbody2D>();

        // s�tter gravity till gravityn p� rigidbodyn
        gravity = rb2D.gravityScale;

        // h�mtar boxcollidern som d� �r den �vre
        crouchCollider = GetComponent<BoxCollider2D>();

        // s�tter liveSpeed till den vanliga hastigheten
        liveSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        // uppdaterar inputX med inputen i horizontella riktningen
        inputX = Input.GetAxisRaw("Horizontal");

        // uppdaterar facingRight genom att om input blir st�rre eller mindre �n 0 ska den s�tta v�rdet p� facingRight efter det.
        if (inputX > 0)
        {
            facingRight = 1;
        }
        else if (inputX < 0)
        {
            facingRight = -1;
        }

        // bara n�r man inte kl�trar ska man kunna g�ra dessa saker
        if (!climbing)
        {
            if (Input.GetKey(KeyCode.LeftShift) && !crouching)
            {
                liveSpeed = runningSpeed;
            }
            else if (!crouching)
            {
                liveSpeed = speed;
            }

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

            // kollar om W eller space trycks ned
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))
            {
                // kollar om spelaren inte redan har hoppat sedan man nuddade marken, det var 0.15 sekunder sedan man hoppa och om spelaren inte flyter
                if (canJump == true)
                {
                    // trycker upp spelaren
                    rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
                    // s�ger att man har hoppat och inte can l�ngre
                    canJump = false;
                }
            }

            // n�r v�nster ctrl trycks ned och man inte �r crouchar under ett tak s� byter den crouch boolen till motsatta
            if (Input.GetKeyDown(KeyCode.LeftControl) && !CelingCheck())
            {
                crouching = !crouching;
            }
        }       
        else
        {
            crouching = false;

            if (Input.GetKey(KeyCode.W))
            {
                rb2D.velocity = new Vector2(0, climbingSpeed);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                rb2D.velocity = new Vector2(0, -climbingSpeed);
            }
            else
            {
                rb2D.velocity = new Vector2(0, 0);
            }

            if (Input.GetKey(KeyCode.Space))
            {
                rb2D.velocity = new Vector2(speed * -facingRight, jumpForce);
                facingRight *= -1;
                
            }
            else if (!LadderCheck())
            {
                // n�r man slutar nudda stegen s� hoppar man fram och upp
                rb2D.velocity = new Vector2(crouchSpeed * facingRight, 5);
            }
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

        // anv�nder GroundCheck f�r att kolla om spelaren �r p� marken och om den �r det s�tts coyoteTimer till 0
        if (GroundCheck())
        {
            coyoteTimer = 0;
        }
        // om spelaren inte �r p� marken s� ska den r�kna upp p� coyoteTimer
        else
        {
            coyoteTimer += Time.deltaTime;
        }
        
        // om coyoteTimer �r st�rre �n coyoteTime s� ska canJump bli false
        if (coyoteTime <= coyoteTimer)
        {
            canJump = false;
        }
        // annars ska canJump vara true
        else
        {
            canJump = true;
        }

        if (LadderCheck())
        {
            climbing = true;
            rb2D.gravityScale = 0;
        }
        else
        {
            climbing = false;
            rb2D.gravityScale = gravity;
        }
    }

    bool GroundCheck()
    {
        // skapar en raycast som b�rjar i spelaren, kollar om den tr�ffar n�got i Ground layer och sparar detta i en hit variabeln
        RaycastHit2D hit = Physics2D.BoxCast(transform.position - new Vector3(0, 0.95f, 0), new Vector2(0.6f, 0.3f), 0, Vector2.up, 0, groundMask);

        // om raycasten har tr�ffat n�gontig och punkten d�r den tr�ffa �r mindre �n 0.6 units fr�n spelaren
        if (hit.transform != null && hit.distance < 0.025F)
        {
            // skickar den true
            return true;
        }
        // annar false
        return false;
    }

    bool CelingCheck()
    {
        // skapar en BoxCast som kollar efter object i ground layer och sparar den i hit
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + new Vector3(0, 0.5f, 0), new Vector2(0.9f, 1), 0, Vector2.up, 0, groundMask);

        // om hit har en transform skickar den true 
        if (hit.transform != null)
        {
            return true;
        }

        // annars false
        return false;
    }

    bool LadderCheck()
    {
        // skapar en BoxCast som kollar efter object i ground layer och sparar den i hit
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + new Vector3(0.1f * facingRight, 0, 0), new Vector2(1f, 1.9f), 0, Vector2.up, 0, ladderMask);

        // om hit har en transform skickar den true 
        if (hit.transform != null)
        {
            return true;
        }

        // annars false
        return false;
    }

    private void OnDrawGizmos()
    {
        // Crouchcheck raycast gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, 0.5f, 0), new Vector2(0.9f, 1));

        // GroundCheck raycast gizmo
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position - new Vector3(0, 0.95f, 0), new Vector3(0.6f, 0.3f));

        // Laddercheck raycast gizmo
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + new Vector3(0.1f * facingRight, 0, 0), new Vector2(1f, 1.9f));
    }
}
