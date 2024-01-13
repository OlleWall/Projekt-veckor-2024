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


    private Vector3 leftStartPos; //start positionen f�r steget
    private Vector3 rightStartPos;

    private Vector3 leftMidPos; //mitten positionen f�r steget (f�r att animationen ska se b�ttre ut)
    private Vector3 rightMidPos;

    private Vector3 leftEndPos; //slut positionen f�r steget
    private Vector3 rightEndPos;


    public float extraStepDis;

    public float runStepMulti; //variabel f�r att �ndra multiplier i unity
    private float runningStepDistanceMultiplier; //variabeln som faktiskt anv�nds av koden, olika f�r att komma ih�g v�rdet

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

        //r�knar upp lerp variablerna och kollar vilken fot som borde r�ra sig
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

        //variabler f�r att g�ra steget mer smooth
        //float rightEasedLerp;
        //float leftEasedLerp;

        //�ndrar targets positioner fr�n startpunkt till slutpunkt �ver tid
        rightTarget.transform.position = Vector3.Lerp(rightStartPos, rightEndPos, rightLerp);
        leftTarget.transform.position = Vector3.Lerp(leftStartPos, leftEndPos, leftLerp);
    }

    private void CalculateStep() //r�knar ut var f�tterna ska placeras, har f�rs�kt anpassa det s� att det blir r�tt vid sprint
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
        //karakt�rens x position
        float CenterOfMass = player.transform.position.x;

        //isBalanced blir true om karakt�rens kropp �r mellan b�da targets
        isBalanced = IsFloatInRange(CenterOfMass, leftTarget.transform.position.x, rightTarget.transform.position.x);
    } //kollar om karakt�ren �r "i balans"

    bool IsFloatInRange(float value, float bound1, float bound2)
    {
        float minValue = Mathf.Min(bound1, bound2);
        float maxValue = Mathf.Max(bound1, bound2);
        return value > minValue && value < maxValue;
    } //ger true om value �r mellan bound1 och bound2
}