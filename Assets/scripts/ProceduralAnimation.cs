using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnimation : MonoBehaviour
{
    #region Variabler
    [Header("Debug checks")]
    public bool isBalanced;
    public bool isRunning;

    public bool leftFootCanMove;
    public bool rightFootCanMove;   


    [Header("References")]
    public GameObject player;
    
    PlayerMovement movementCode;
    
    public GameObject leftTarget;
    public GameObject rightTarget;

    public float moveDirection;


    [Header("Other values")]
    public float targetRaycastBuffer;

    

    public float rightLerp = 0.02f  ;
    public float leftLerp = 0;


    private Vector3 leftStartPos; //start positionen för steget
    private Vector3 rightStartPos;

    private Vector3 leftMidPos; //mitten positionen för steget (för att animationen ska se bättre ut)
    private Vector3 rightMidPos;

    private Vector3 leftEndPos; //slut positionen för steget
    private Vector3 rightEndPos;


    public float extraStepDis;

    public float runStepMulti; //variabel för att ändra multiplier i unity
    private float runningStepDistanceMultiplier; //variabeln som faktiskt används av koden, olika för att komma ihåg värdet

    public float stepSpeed;

    [Header("Feet placement from character center")]
    public float leftFootDistanceFromCenter;
    public float rightFootDistanceFromCenter;
    #endregion

    private void Start()
    {
        movementCode = GetComponentInParent<PlayerMovement>();

        leftStartPos = leftEndPos = leftTarget.transform.position;
        rightStartPos = rightEndPos = rightTarget.transform.position;
    }

    private void Update()
    {
        UpdateBalance();

        isRunning = Input.GetKey(KeyCode.LeftShift);
        if (isRunning)
        {
            runningStepDistanceMultiplier = runStepMulti;
        }
        else
        {
            runningStepDistanceMultiplier = 1;
        }

        moveDirection = movementCode.facingRight;

        //räknar upp lerp variablerna och kollar vilken fot som borde röra sig
        #region Lerp & FootCanMove 
        rightLerp += Time.deltaTime * stepSpeed;
        leftLerp += Time.deltaTime * stepSpeed;

        rightFootCanMove = leftLerp > 1 && rightLerp > leftLerp;
        leftFootCanMove = rightLerp > 1 && leftLerp > rightLerp;
        #endregion 

        if (!isBalanced && rightFootCanMove || !isBalanced && leftFootCanMove)
        {
            CalculateStep();
        }

        //variabler för att göra steget mer smooth
        //float rightEasedLerp;
        //float leftEasedLerp;

        //ändrar targets positioner från startpunkt till slutpunkt över tid
        rightTarget.transform.position = Vector3.Lerp(rightStartPos, rightEndPos, rightLerp);
        leftTarget.transform.position = Vector3.Lerp(leftStartPos, leftEndPos, leftLerp);
    }

    private void CalculateStep() //räknar ut var fötterna ska placeras, har försökt anpassa det så att det blir rätt vid sprint
    {
        if (rightFootCanMove)
        {
            rightStartPos = rightTarget.transform.position;

            rightLerp = 0f;

            RaycastHit2D ray = Physics2D.Raycast(new Vector2(rightTarget.transform.position.x + (1 + extraStepDis * runningStepDistanceMultiplier * moveDirection), rightTarget.transform.position.y + targetRaycastBuffer), Vector2.down, 200);
            Debug.DrawRay(new Vector3(rightTarget.transform.position.x + (1 + extraStepDis * runningStepDistanceMultiplier * moveDirection), rightTarget.transform.position.y + targetRaycastBuffer, 0), Vector3.down, Color.green, 3f);

            rightEndPos = ray.point;
        }

        else
        {
            leftStartPos = leftTarget.transform.position;

            leftLerp = 0f;

            RaycastHit2D ray = Physics2D.Raycast(new Vector2(leftTarget.transform.position.x + (1 + extraStepDis * runningStepDistanceMultiplier * moveDirection), leftTarget.transform.position.y + targetRaycastBuffer), Vector2.down, 200);
            Debug.DrawRay(new Vector3(leftTarget.transform.position.x + (1 + extraStepDis * runningStepDistanceMultiplier * moveDirection), leftTarget.transform.position.y + targetRaycastBuffer, 0), Vector3.down, Color.red, 3f);

            leftEndPos = ray.point;
        }
    }

    private void UpdateBalance()
    {
        //karaktärens x position
        float CenterOfMass = player.transform.position.x;

        //isBalanced blir true om karaktärens kropp är mellan båda targets
        isBalanced = IsFloatInRange(CenterOfMass, leftTarget.transform.position.x, rightTarget.transform.position.x);
    } //kollar om karaktären är "i balans"

    bool IsFloatInRange(float value, float bound1, float bound2)
    {
        float minValue = Mathf.Min(bound1, bound2);
        float maxValue = Mathf.Max(bound1, bound2);
        return value > minValue && value < maxValue;
    } //ger true om value är mellan bound1 och bound2
}