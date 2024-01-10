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
        //hämtar rigidbody componenten
        rb2D = GetComponent<Rigidbody2D>();

        crouchCollider = GetComponent<BoxCollider2D>();

        liveSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        //om rör spelaren åt höger när D knappen trycks ner och spelaren dashar inte
        if (Input.GetKey(KeyCode.D))
        {
            facingRight = 1;
            if (WallForward == false)
            {
                rb2D.velocity = new Vector2(0, rb2D.velocity.y);
                transform.eulerAngles = new Vector3(0, 0, 0);
                transform.position += new Vector3(liveSpeed, 0, 0) * Time.deltaTime;
            }
        }

        //om rör spelaren åt vänster när A trycks ner, spelaren dashar inte och kollar om spelaren är mer än -10 units.x från 0
        if (Input.GetKey(KeyCode.A) && transform.position.x > -10)
        {
            facingRight = -1;
            if (WallForward == false)
            {
                rb2D.velocity = new Vector2(0, rb2D.velocity.y);
                transform.eulerAngles = new Vector3(0, 180, 0);
                transform.position -= new Vector3(liveSpeed, 0, 0) * Time.deltaTime;
            }
        }

        //kollar om W trycks ned
        if (Input.GetKey(KeyCode.W))
        {
            //kollar om spelaren inte redan har hoppat sedan man nuddade marken, det var 0.15 sekunder sedan man hoppa och om spelaren inte flyter
            if (airJump == true)
            {
                //stannar spelaren på y axeln och trycker upp spelaren
                rb2D.velocity = new Vector2(rb2D.velocity.x, 0);
                rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
                //säger att man har hoppat i luften
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

        //kollar functionen GroundCheck och får ett true false värde tillbaka
        if (GroundCheck())
        {
            //säger att spelaren kan hoppa
            airJump = true;
        }
    }

    bool GroundCheck()
    {
        //skapar en raycast som börjar i spelaren, åker neråt och kollar om den träffar något i Floor layer och sparar detta i en hit varibael
        RaycastHit2D hit = Physics2D.CircleCast(transform.position - new Vector3(0, 0.95f, 0), 0.15f, -Vector2.up, 1f, mask);

        //om raycasten har träffat någontig och punkten där den träffa är mindre än 0.6 units från spelaren
        if (hit.transform != null && hit.distance < 0.025F)
        {
            //sickar den true
            return true;
        }
        //om det inte är sant
        else
        {
            //sickar den false
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, 0.5f, 0), new Vector2(0.9f, 1));
    }
}
