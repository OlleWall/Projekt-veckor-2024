using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Olle
public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(1, 15)]
    float runningSpeed = 10; // hastigheten n�r man springer

    [SerializeField, Range(1, 10)]
    float speed = 5; // den vanliga hastigheten

    public float liveSpeed; // hastigheten som anv�nds i movment

    [SerializeField, Range(1, 10)]
    float crouchSpeed = 5; // hastigheten n�r man crouchar

    [SerializeField, Range(0, 10)]
    float climbingSpeed = 3; // hastigheten n�r man kl�ttrar

    [SerializeField, Range(1, 100)]
    public float jumpForce = 5; // kraften man hoppar med

    [SerializeField, Range(0, 1)]
    float airSpeedMultiplier = 0.3f;

    [SerializeField, Range(0.01f, 1f)]
    float coyoteTime = 0.05f; // hur l�nge innan man f�rlorar sitt hopp efter spelarn slutar nudda marken

    float coyoteTimer; // sj�lva variabeln som r�knar upp

    bool canJump; // om man kan hoppa

    BoxCollider2D crouchCollider; // collidern som ska disableas n�r man crouchar

    public Rigidbody2D rb2D;

    public float inputX;

    public bool crouching = false;

    public bool hiding = false;

    string frontSortingLayer = "PlayerLayer"; // Sorting layer for in front of the wall. Marcus

    string behindSortingLayer = "PlayerHidingLayer"; // Sorting layer for behind the wall. Marcus

    [SerializeField]
    LayerMask groundMask; // layermasken f�r marken

    [SerializeField]
    LayerMask celingMask; // layermasken f�r saker man inte ska kunna st� under

    [SerializeField]
    LayerMask interactMask; // layermasken f�r stegar

    bool climbing = false;

    public int facingRight; // 1 = kollar h�ger -1 = v�nster

    float gravity; // vanliga gravitationen

    SpriteRenderer[] playerSpriteRenderers;

    public List<int> inventory;

    [SerializeField]
    Vector3 celingCheckOffsett = new Vector3(0, 0.5f, 0);

    [SerializeField]
    Vector2 celingCheckSize = new Vector2(0.9f, 1);

    [SerializeField]
    Vector3 groundCheckOffsett = new Vector3(0, -0.95f, 0);

    [SerializeField]
    Vector2 groundCheckSize = new Vector2(0.6f, 0.3f);

    [SerializeField]
    Vector3 interactCheckOffsett = new Vector3(0, 0, 0);

    [SerializeField]
    Vector2 interactCheckSize = new Vector2(1, 1.9f);
    
    //CameraFollow cameraScript;
    void Start()
    {
        // h�mtar rigidbody componenten
        rb2D = GetComponent<Rigidbody2D>();

        // s�tter gravity till gravitationen p� rigidbodyn
        gravity = rb2D.gravityScale;

        // h�mtar boxcollidern som d� �r den �vre
        crouchCollider = GetComponent<BoxCollider2D>();

        // s�tter liveSpeed till den vanliga hastigheten
        liveSpeed = speed;

        playerSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject interactedObject = InteractCheck();

        #region Movement och kl�ttring
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

        // bara n�r man inte kl�ttrar ska man kunna g�ra dessa saker
        if (!climbing && !hiding)
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

            // om spelaren �r p� marken ska spelaren r�ra sig snabbare och t�tare annars ska den �ndra hastighet l�ngsamt
            if (GroundCheck())
            {
                rb2D.velocity = new Vector2(liveSpeed * inputX, rb2D.velocity.y);
            }
            // kollar s� spelaren bara accelererar n�r den �r under hur snabbt man g�r p� marken
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
        // om man kl�ttrar
        else
        {
            crouching = false;

            if (interactedObject != null && interactedObject.tag == "Ladder")
            {
                transform.position = new Vector3(interactedObject.gameObject.transform.position.x, transform.position.y, transform.position.z);
            }

            rb2D.gravityScale = 0;

            #region LadderMovement
            // om man trycker p� W ska man �ka upp om S �ker man ned annars st�r man still
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
            #endregion

            // n�r man trycker p� space s� hoppar man av �t de h�llet man kollar
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb2D.velocity = new Vector2(speed * facingRight, jumpForce);
                //rb2D.gravityScale = gravity;
                climbing = false;
            }
            // n�r man slutar nudda stegen s� hoppar man fram och upp
            else if (InteractCheck() ==  null)
            {
                climbing = false;
                rb2D.velocity = new Vector2(crouchSpeed * facingRight, 5);              
            }
        }
        #endregion

        #region Crouching
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
        #endregion

        #region Jumping
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
        #endregion

        #region Hiding
        // Update the sorting layer accordingly. Marcus
        if (hiding)
        {
            rb2D.Sleep(); // st�nger av rigidbodyn n�r man g�mmer sig
            foreach (SpriteRenderer spriteRenderer in playerSpriteRenderers)
            {
                spriteRenderer.sortingLayerName = behindSortingLayer; 
            }
        }
        else
        {
            rb2D.WakeUp(); // s�tter ig�ng rigidbodyn n�r man inte g�mmer sig
            foreach (SpriteRenderer spriteRenderer in playerSpriteRenderers)
            {
                spriteRenderer.sortingLayerName = frontSortingLayer;
            }
        }
        #endregion

        if (interactedObject != null && Input.GetKeyDown(KeyCode.E))
        {
            print("interacted with a " + interactedObject.name);
            // om objectets tag �r ladder ska den sluta kl�ttra om den kl�ttrar och b�rja om den inte
            if (interactedObject.tag == "Ladder")
            {
                climbing = !climbing;
            }
            else if (interactedObject.tag == "Door")
            {
                interactedObject.GetComponent<DoorLogic>().Open(inventory);
            }
            else if (interactedObject.tag == "Hider")
            {
                hiding = !hiding;
            }
        }
    }

    public bool GroundCheck()
    {
        // skapar en raycast som b�rjar i spelaren, kollar om den tr�ffar n�got i Ground layer och sparar detta i en hit variabeln
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + groundCheckOffsett, groundCheckSize, 0, Vector2.up, 0, groundMask);

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
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + celingCheckOffsett, celingCheckSize, 0, Vector2.up, 0, celingMask);

        // om hit har en transform skickar den true 
        if (hit.transform != null)
        {
            return true;
        }

        // annars false
        return false;
    }

    GameObject InteractCheck()
    {
        // skapar en BoxCast som kollar efter object i interactable layer och sparar den i hit
        RaycastHit2D hit = Physics2D.BoxCast
        (
            new Vector2(transform.position.x + interactCheckOffsett.x * facingRight, transform.position.y + interactCheckOffsett.y),
            interactCheckSize, 0, Vector2.up, 0, interactMask
        );

        // om hit har en transform skickar den gameobjectet
        if (hit.transform != null)
        {
            return hit.transform.gameObject;
        }

        // annars skickar den null
        return null;
    }

    private void OnDrawGizmos()
    {
        // Celingcheck raycast gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + celingCheckOffsett, celingCheckSize);

        // GroundCheck raycast gizmo
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + groundCheckOffsett, groundCheckSize);

        // Interactivecheck raycast gizmo
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + interactCheckOffsett.x * facingRight, transform.position.y + interactCheckOffsett.y), interactCheckSize);
    }
}
