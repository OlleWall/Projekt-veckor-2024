using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Olle
public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(1, 15)]
    float runningSpeed = 10; // hastigheten när man springer

    [SerializeField, Range(1, 10)]
    float speed = 5; // den vanliga hastigheten

    float liveSpeed; // hastigheten som används i movment

    [SerializeField, Range(1, 10)]
    float crouchSpeed = 5; // hastigheten när man crouchar

    [SerializeField, Range(0, 10)]
    float climbingSpeed = 3; // hastigheten när man klättrar

    [SerializeField, Range(1, 100)]
    float jumpForce = 5; // kraften man hoppar med

    [SerializeField, Range(0, 1)]
    float airSpeedMultiplier = 0.3f;

    [SerializeField, Range(0.01f, 1f)]
    float coyoteTime = 0.05f; // hur länge innan man förlorar sitt hopp efter spelarn slutar nudda marken

    float coyoteTimer; // själva variabeln som räknar upp

    bool canJump; // om man kan hoppa

    BoxCollider2D crouchCollider; // collidern som ska disableas när man crouchar

    Rigidbody2D rb2D;

    float inputX;

    bool crouching = false;

    [SerializeField]
    LayerMask groundMask; // layermasken för marken

    [SerializeField]
    LayerMask ladderMask; // layermasken för stegar

    public bool canInteract; // om san ska något visas att spelaren kan interacta med något typ text över huvudet

    public bool climbing = false;

    int facingRight; // 1 = kollar höger -1 = vänster

    float gravity;

    [SerializeField]
    Vector3 celingCheckOffsett = new Vector3(0, 0.5f, 0);

    [SerializeField]
    Vector2 celingCheckSize = new Vector2(0.9f, 1);

    [SerializeField]
    Vector3 groundCheckOffsett = new Vector3(0, -0.95f, 0);

    [SerializeField]
    Vector2 groundCheckSize = new Vector2(0.6f, 0.3f);

    [SerializeField]
    Vector3 ladderCheckOffsett = new Vector3(0, 0, 0);

    [SerializeField]
    Vector2 ladderCheckSize = new Vector2(1, 1.9f);

    //CameraFollow cameraScript;

    void Start()
    {
        // hämtar rigidbody componenten
        rb2D = GetComponent<Rigidbody2D>();

        // sätter gravity till gravityn på rigidbodyn
        gravity = rb2D.gravityScale;

        // hämtar boxcollidern som då är den övre
        crouchCollider = GetComponent<BoxCollider2D>();

        // reffererar till cameraFollow scriptet på cameran
        //cameraScript = GetComponent<CameraFollow>();

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

        GameObject ladder = LadderCheck();

        // bara när man inte klättrar ska man kunna göra dessa saker
        if (!climbing)
        {
            rb2D.gravityScale = gravity;

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
                rb2D.AddForce(new Vector2(speed * airSpeedMultiplier * inputX, 0), ForceMode2D.Force);
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
        // om man klättrar
        else
        {
            crouching = false;

            canInteract = false;

            if (ladder != null)
            {
                transform.position = new Vector3(ladder.gameObject.transform.position.x, transform.position.y, transform.position.z);
            }

            rb2D.gravityScale = 0;

            // om man trycker på W ska man åka upp om S åker man ned annars står man still
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

            // när man trycker på space så hoppar man av åt de hållet man kollar
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb2D.velocity = new Vector2(speed * facingRight, jumpForce);
                rb2D.gravityScale = gravity;
                climbing = false;
            }
            // när man slutar nudda stegen så hoppar man fram och upp
            else if (LadderCheck() ==  null)
            {
                climbing = false;
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

        if (ladder != null)
        {
            // ska visa att om spelaren trycker på E börjar den klättra på stegen
            if (!climbing)
            {
                canInteract = true;
            }    

            // bara om spelaren trycker på E ska den börja klättra
            if (Input.GetKeyDown(KeyCode.E))
            {
                climbing = !climbing;
            }      
        }
        else
        {
            canInteract = false;
        }
    }

    bool GroundCheck()
    {
        // skapar en raycast som börjar i spelaren, kollar om den träffar något i Ground layer och sparar detta i en hit variabeln
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + groundCheckOffsett, groundCheckSize, 0, Vector2.up, 0, groundMask);

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
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + celingCheckOffsett, celingCheckSize, 0, Vector2.up, 0, groundMask);

        // om hit har en transform skickar den true 
        if (hit.transform != null)
        {
            return true;
        }

        // annars false
        return false;
    }

    GameObject LadderCheck()
    {
        // skapar en BoxCast som kollar efter object i ground layer och sparar den i hit
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + ladderCheckOffsett, ladderCheckSize, 0, Vector2.up, 0, ladderMask);

        // om hit har en transform skickar den gameobjectet
        if (hit.transform != null)
        {
            return hit.transform.gameObject;
        }

        // annars skickar den null
        return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //när spelaren nuddar ett objekt med taggen CameraGraber så säger den åt kammeran att röra sig åt höger
        if (collision.gameObject.tag == "CamerGraber")
        {
            //cameraScript.GrabCamera();
        }
    }

    private void OnDrawGizmos()
    {
        // Celingcheck raycast gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + celingCheckOffsett, celingCheckSize);

        // GroundCheck raycast gizmo
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + groundCheckOffsett, groundCheckSize);

        // Laddercheck raycast gizmo
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + ladderCheckOffsett, ladderCheckSize);
    }
}
