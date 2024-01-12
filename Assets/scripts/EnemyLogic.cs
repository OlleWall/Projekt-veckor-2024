using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    public int status = 0; // 0 = passive, 1 = searching, 2 = chasing

    [SerializeField, Range(0, 5)]
    float speed = 2.5f;

    [SerializeField, Range(0, 5)]
    float patrolWaitTime = 2;

    float patrolWaitTimer;

    [SerializeField]
    Vector2 patrolArea; // x = högra area slut, y = vänstra area slut

    Vector2 patrolPoint;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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
        // om enemy är mindre än 0.75 units ifrån patrolPoint och timern har gåt ut ska den skapa en ny
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

        // kallar på funktionen för att gå mot patrolpointen
        MoveTowardsTarget(patrolPoint, speed);
    }

    Vector2 MakePatrolPoint(Vector2 area)
    {
        // ta fram en random distans att gå sen randomly gå den distansen åt höger eller vänster

        // skapar en random punkt inom arean
        Vector2 pos = new Vector2(Random.Range(area.y, area.x), 0);

        // om pos är mindre än en 1/3 av arean bort från enemy ska den skapa en ny tills det inte är så 
        if (Vector2.Distance(pos, transform.position) < (area.x - area.y / 3))
        {
            do
            {
                pos = new Vector2(Random.Range(area.y, area.x), 0);
            } while (Vector2.Distance(pos, transform.position) < (area.x - area.y / 3));
        }
        
        return pos;
    }

    void MoveTowardsTarget(Vector3 target, float localSpeed)
    {
        // om targets position på x är större ska den röra sig åt höger
        if (target.x > transform.position.x)
        {
            transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        }
        // om targets position på x är mindre ska den röra sig åt vänster
        else if (target.x < transform.position.x)
        {
            transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);
        }
    }

    void Searching()
    {

    }

    void Chasing()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(patrolArea.x, transform.position.y + 3, 0), new Vector3(patrolArea.x, transform.position.y - 3, 0));
        Gizmos.DrawLine(new Vector3(patrolArea.y, transform.position.y + 3, 0), new Vector3(patrolArea.y, transform.position.y - 3, 0));
    }
}
