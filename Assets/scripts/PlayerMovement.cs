using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(1, 15)]
    float runningSpeed = 10;

    [SerializeField, Range(1, 10)]
    float speed = 5; // den vanliga hastigheten

    float liveSpeed; // hastigheten som används i movment

    [SerializeField, Range(1, 10)]
    float crouchSpeed = 5; // hastigheten när man crouchar

    [SerializeField, Range(1, 100)]
    float jumpForce = 5; // kraften man hoppar med

    [SerializeField, Range(0.01f, 1f)]
    float coyoteTime = 0.05f; // hur länge innan man förlorar sitt hopp efter spelarn slutar nudda marken

    float coyoteTimer; // själva variabeln som räknar upp

    bool canJump; // om man kan hoppa

    [SerializeField]
    BoxCollider2D crouchCollider; // collidern som ska disableas när man crouchar

    Rigidbody2D rb2D;

    float inputX;

    bool crouching = false;

    [SerializeField]
    LayerMask groundMask; // layermasken för marken

    [SerializeField]
    LayerMask ladderMask; // layermasken för stegar

    public bool climbing = false;

    [SerializeField, Range(0, 10)]
    float climbingSpeed = 3; // hastigheten när man klätrar

    int facingRight;

    float gravity;

    void Start()
    {
        // hämtar rigidbody componenten
        rb2D = GetComponent<Rigidbody2D>();

        // sätter gravity till gravityn på rigidbodyn
        gravity = rb2D.gravityScale;

        // hämtar boxcollidern som då är den övre
        crouchCollider = GetComponent<BoxCollider2D>();

        // sätter liveSpeed till den vanliga hastigheten
        liveSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        // uppdaterar inputX med inputen i horizontella riktningen
        inputX = Input.GetAxisRaw("Horizontal");

        // uppdaterar facingRight genom att om input blir större eller mindre än 0 ska den sätta värdet på facingRight efter det.
        if (inputX > 0)
        {
            facingRight = 1;
        }
        else if (inputX < 0)
        {
            facingRight = -1;
        }

        // bara när man inte klätrar ska man kunna göra dessa saker
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

            // om spelaren är på marken ska spelaren röra sig snabbare och tätare annars ska den ändra hastighet långsamt
            if (GroundCheck())
            {
                rb2D.velocity = new Vector2(liveSpeed * inputX, rb2D.velocity.y);
            }
            // kollar så spelaren bara accelererar när den är under hur snabbt man går på marken
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
                    // säger att man har hoppat och inte can längre
                    canJump = false;
                }
            }

            // när vänster ctrl trycks ned och man inte är crouchar under ett tak så byter den crouch boolen till motsatta
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
                // när man slutar nudda stegen så hoppar man fram och upp
                rb2D.velocity = new Vector2(crouchSpeed * facingRight, 5);
            }
        }
        // när crouching är true disableas crouchCollider som är den övre collidern och liveSpeed byts till crouchSpeed som är långsammare
        if (crouching)
        {
            crouchCollider.enabled = false;
            liveSpeed = crouchSpeed;
        }
        // ändrar tillbaka allt när crouching är false
        else
        {
            crouchCollider.enabled = true;
            liveSpeed = speed;
        }

        // använder GroundCheck för att kolla om spelaren är på marken och om den är det sätts coyoteTimer till 0
        if (GroundCheck())
        {
            coyoteTimer = 0;
        }
        // om spelaren inte är på marken så ska den räkna upp på coyoteTimer
        else
        {
            coyoteTimer += Time.deltaTime;
        }
        
        // om coyoteTimer är större än coyoteTime så ska canJump bli false
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
        // skapar en raycast som börjar i spelaren, kollar om den träffar något i Ground layer och sparar detta i en hit variabeln
        RaycastHit2D hit = Physics2D.BoxCast(transform.position - new Vector3(0, 0.95f, 0), new Vector2(0.6f, 0.3f), 0, Vector2.up, 0, groundMask);

        // om raycasten har träffat någontig och punkten där den träffa är mindre än 0.6 units från spelaren
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
