using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    public int status = 0; // 0 = passive, 1 = searching, 2 = chasing

    [SerializeField]
    Transform player;

    [SerializeField, Range(0, 5)]
    float speed = 2.5f;

    [SerializeField, Range(0, 5)]
    float patrolWaitTime = 2;

    float patrolWaitTimer;

    [SerializeField]
    LayerMask mask;

    [SerializeField, Range(0, 20)]
    float spotDistance = 5;

    [SerializeField]
    Vector2 patrolArea; // x = v�nster area slut, y = h�ger area slut

    Vector2 patrolPoint;

    PlayerMovement playerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CanSee())
        {
            status = 2;
        }
        else
        {
            status = 0;
        }

        switch (status)
        {
            case 0:
                Passive();
                break;
            case 1:
                Searching();
                break;
            case 2:
                Chasing();
                break;
        }
    }

    void Passive()
    {
        // om det inte finns en patrolPoint skapar den en
        if (patrolPoint == new Vector2(0, 0))
        {
            patrolPoint = MakePatrolPoint(patrolArea);
        }
        // om enemy �r mindre �n 0.75 units ifr�n patrolPoint och timern har g�t ut ska den skapa en ny
        else if (Vector2.Distance(patrolPoint, transform.position) < 0.75f)
        {
            if (patrolWaitTimer > patrolWaitTime)
            {
                patrolPoint = MakePatrolPoint(patrolArea);
                patrolWaitTimer = 0;
            }
            else
            {
                patrolWaitTimer += Time.deltaTime;
            }
        }

        // kallar p� funktionen f�r att g� mot patrolpointen
        MoveTowardsTarget(patrolPoint, speed);
    }

    Vector2 MakePatrolPoint(Vector2 area)
    {
        // ta fram en random distans att g� sen randomly g� den distansen �t h�ger eller v�nster

        //print("skapar en ny patrol point");

        // r�knar ut hur avst�ndet till slutet av patrol areaen
        float leftArea = Mathf.Abs(area.x - transform.position.x);
        float rightArea = Mathf.Abs(area.y - transform.position.x);

        Vector2 pos = new Vector2(0, 0);

        // v�ljer en random punkt p� den sidan som �r st�rst
        if (rightArea > leftArea)
        {
            pos = new Vector2(Random.Range(transform.position.x, area.y), transform.position.y);
        }
        else
        {
            pos = new Vector2(Random.Range(transform.position.x, area.x), transform.position.y);
        }
        
        return pos;
    }

    void MoveTowardsTarget(Vector3 target, float localSpeed)
    {
        // om targets position p� x �r st�rre ska den r�ra sig �t h�ger
        if (target.x > transform.position.x)
        {
            transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        }
        // om targets position p� x �r mindre ska den r�ra sig �t v�nster
        else if (target.x < transform.position.x)
        {
            transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);
        }
    }

    void Searching()
    {
        status = 0;
    }

    void Chasing()
    {
        //print("i attacking you");
    }

    public bool CanSee()
    {
        Vector2 top = new Vector2(player.position.x, player.position.y + (player.localScale.y / 2));
        Vector2 bottom = new Vector2(player.position.x, player.position.y - (player.localScale.y / 2));

        RaycastHit2D[] hits =
        {
            Physics2D.Raycast(transform.position, -1 * new Vector2(transform.position.x - top.x, transform.position.y - top.y), Mathf.Infinity, mask),
            Physics2D.Raycast(transform.position, -1 * new Vector2(transform.position.x - bottom.x, transform.position.y - bottom.y), Mathf.Infinity, mask)
        };

        Debug.DrawRay(transform.position, -1 * new Vector2(transform.position.x - top.x, transform.position.y - top.y), Color.red);
        Debug.DrawRay(transform.position, -1 * new Vector2(transform.position.x - bottom.x, transform.position.y - bottom.y), Color.red);

        foreach (RaycastHit2D x in hits)
        {
            if (x.transform != null)
            {
                if (x.transform.gameObject.tag == "Player" && x.distance <= spotDistance)
                {
                    return true;
                }
            }           
        }

        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(patrolArea.y, transform.position.y + 3, 0), new Vector3(patrolArea.y, transform.position.y - 3, 0));
        Gizmos.DrawLine(new Vector3(patrolArea.x, transform.position.y + 3, 0), new Vector3(patrolArea.x, transform.position.y - 3, 0));

        /*Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, new Vector2(player.position.x, player.position.y + (player.localScale.y / 2)));
        Gizmos.DrawLine(transform.position, new Vector2(player.position.x, player.position.y - (player.localScale.y / 2)));*/
    }
}
