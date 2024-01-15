using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProceduralAnimation : MonoBehaviour
{
    #region Variabler
    [Header("Debug checks")]
    public bool isBalanced;
    public bool isRunning;

    public bool leftLegCanMove;
    public bool leftArmCanMove;

    public bool rightLegCanMove;   
    public bool rightArmCanMove;

    public bool facingRight;

    [Header("References")]
    public GameObject player;
    private GameObject playerSprite;
    
    PlayerMovement movementCode;
    
    public GameObject leftLegTarget;
    public GameObject leftArmTarget;

    public GameObject rightLegTarget;
    public GameObject rightArmTarget;

    public float moveDirection;


    [Header("Other values")]
    public float targetRaycastBuffer;

    public float rightLerp = 0;
    public float leftLerp = 0;

    #region LeftLegTargetPositions
    private Vector3 leftLegStartPos;
    private Vector3 leftLegMidPos;
    private Vector3 leftLegEndPos;
    #endregion

    #region RightLegTargetPositions
    private Vector3 rightLegStartPos;
    private Vector3 rightLegMidPos;
    private Vector3 rightLegEndPos;
    #endregion

    #region RightArmTargetPositions
    private Vector3 rightArmStartPos;
    private Vector3 rightArmMidPos;
    private Vector3 rightArmEndPos;
    #endregion

    #region LeftArmTargetPositions
    private Vector3 leftArmStartPos;
    private Vector3 leftArmMidPos;
    private Vector3 leftArmEndPos;
    #endregion

    public float extraStepDis;

    public float runStepMulti; //variabel för att ändra multiplier i unity
    private float runningStepDistanceMultiplier; //variabeln som faktiskt används av koden, olika för att komma ihåg värdet

    public float stepSpeed;

    [Header("Feet placement from character center")]
    public float leftLegDistanceFromCenter = 0.13f;
    public float leftArmDistanceFromCenter = 0.33f;

    public float rightLegDistanceFromCenter = -0.105f;
    public float rightArmDistanceFromCenter = -0.33f;
    #endregion

    private void Start()
    {
        movementCode = GetComponentInParent<PlayerMovement>();
        playerSprite = player.transform.GetChild(0).gameObject;

        leftLegStartPos = leftLegEndPos = leftLegTarget.transform.position;
        rightLegStartPos = rightLegEndPos = rightLegTarget.transform.position;
    }

    private void Update()
    {
        if (movementCode.inputX == 0)
        {
            rightArmTarget.transform.position = new Vector3(player.transform.position.x + rightArmDistanceFromCenter, rightArmTarget.transform.position.y, rightArmTarget.transform.position.z);
            rightLegTarget.transform.position = new Vector3(player.transform.position.x + rightLegDistanceFromCenter, rightLegTarget.transform.position.y, rightLegTarget.transform.position.z);

            leftArmTarget.transform.position = new Vector3(player.transform.position.x + leftArmDistanceFromCenter, leftArmTarget.transform.position.y, leftArmTarget.transform.position.z);
            leftLegTarget.transform.position = new Vector3(player.transform.position.x + leftLegDistanceFromCenter, leftLegTarget.transform.position.y, leftLegTarget.transform.position.z);
        }

        if (movementCode.inputX != 0)
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

            //moveDirection 1 = höger, -1 = vänster
            moveDirection = movementCode.facingRight;

            //flippar sprite så den tittar dit du rör dig
            #region FlipSprite
            if (moveDirection == 1 && !facingRight)
            {
                playerSprite.transform.localScale = new Vector3(-playerSprite.transform.localScale.x, playerSprite.transform.localScale.y, playerSprite.transform.localScale.z);
                playerSprite.transform.localPosition = new Vector3(-playerSprite.transform.localPosition.x, playerSprite.transform.localPosition.y, playerSprite.transform.localPosition.z);
                
                rightArmTarget.transform.position = new Vector3(-rightArmTarget.transform.position.x, rightArmTarget.transform.position.y, rightArmTarget.transform.position.z);
                rightLegTarget.transform.position = new Vector3(-rightLegTarget.transform.position.x, rightLegTarget.transform.position.y, rightLegTarget.transform.position.z);
                leftArmTarget.transform.position = new Vector3(-leftArmTarget.transform.position.x, leftArmTarget.transform.position.y, leftArmTarget.transform.position.z);
                leftLegTarget.transform.position = new Vector3(-leftLegTarget.transform.position.x, leftLegTarget.transform.position.y, leftLegTarget.transform.position.z);
                
                /*
                rightArmTarget.transform.position = new Vector3(rightArmTarget.transform.position.x - rightArmDistanceFromCenter, rightArmTarget.transform.position.y, rightArmTarget.transform.position.z);
                leftArmTarget.transform.position = new Vector3(leftArmTarget.transform.position.x - leftArmDistanceFromCenter, leftArmTarget.transform.position.y, leftArmTarget.transform.position.z);
                rightLegTarget.transform.position = new Vector3(rightLegTarget.transform.position.x - rightLegDistanceFromCenter, rightLegTarget.transform.position.y, rightLegTarget.transform.position.z);
                leftLegTarget.transform.position = new Vector3(leftLegTarget.transform.position.x - leftLegDistanceFromCenter, leftLegTarget.transform.position.y, leftLegTarget.transform.position.z);
                */

                facingRight = true;
            }

            else if (moveDirection == -1 && facingRight)
            {
                playerSprite.transform.localScale = new Vector3(-playerSprite.transform.localScale.x, playerSprite.transform.localScale.y, playerSprite.transform.localScale.z);
                playerSprite.transform.localPosition = new Vector3(-playerSprite.transform.localPosition.x, playerSprite.transform.localPosition.y, playerSprite.transform.localPosition.z);
                rightArmTarget.transform.position = new Vector3(-rightArmTarget.transform.position.x, rightArmTarget.transform.position.y, rightArmTarget.transform.position.z);
                rightLegTarget.transform.position = new Vector3(-rightLegTarget.transform.position.x, rightLegTarget.transform.position.y, rightLegTarget.transform.position.z);
                leftArmTarget.transform.position = new Vector3(-leftArmTarget.transform.position.x, leftArmTarget.transform.position.y, leftArmTarget.transform.position.z);
                leftLegTarget.transform.position = new Vector3(-leftLegTarget.transform.position.x, leftLegTarget.transform.position.y, leftLegTarget.transform.position.z);
                facingRight = false;
            }
            #endregion

            //räknar upp lerp variablerna och kollar vilken fot som borde röra sig
            #region Lerp & CanMove 
            rightLerp += Time.deltaTime * stepSpeed;
            leftLerp += Time.deltaTime * stepSpeed;
            //om movedirection är -1 / 1 kan jag ändra så att antingen leftLerp eller rightLerp är störst
            //vilket ändrar vilken fot som rör sig först


            rightLegCanMove = leftLerp > 1 && rightLerp > leftLerp;
            leftLegCanMove = rightLerp > 1 && leftLerp > rightLerp;

            rightArmCanMove = leftLegCanMove;
            leftArmCanMove = rightLegCanMove;
            #endregion

            if (!isBalanced && rightLegCanMove || !isBalanced && leftLegCanMove)
            {
                CalculateStep();
            }

            //variabler för att göra steget mer smooth
            //float rightEasedLerp;
            //float leftEasedLerp;

            //ändrar targets positioner från startpunkt till slutpunkt över tid
            rightLegTarget.transform.position = Vector3.Lerp(rightLegStartPos, rightLegEndPos, rightLerp);
            leftLegTarget.transform.position = Vector3.Lerp(leftLegStartPos, leftLegEndPos, leftLerp);

            rightArmTarget.transform.position = new Vector3(leftLegTarget.transform.position.x, leftArmTarget.transform.position.y, 0);
            leftArmTarget.transform.position = new Vector3(rightLegTarget.transform.position.x, rightArmTarget.transform.position.y, 0);
        }
    }

    private void CalculateStep() //räknar ut var fötterna ska placeras, har försökt anpassa det så att det blir rätt vid sprint
    {
        
        if (rightLegCanMove)
        {
            rightLegStartPos = rightLegTarget.transform.position;
            
            rightLerp = 0f;
             
            RaycastHit2D ray = Physics2D.Raycast(new Vector2((moveDirection == 1 ? leftLegTarget.transform.position.x : rightLegTarget.transform.position.x) + (extraStepDis * runningStepDistanceMultiplier * moveDirection), rightLegTarget.transform.position.y + targetRaycastBuffer), Vector2.down, 200);
            Debug.DrawRay(new Vector3((moveDirection == 1 ? leftLegTarget.transform.position.x : rightLegTarget.transform.position.x) + (extraStepDis * runningStepDistanceMultiplier * moveDirection), rightLegTarget.transform.position.y + targetRaycastBuffer, 0), Vector3.down, Color.green, 3f);

            rightLegEndPos = ray.point;
        }

        if(leftLegCanMove)
        {
            leftLegStartPos = leftLegTarget.transform.position;

            leftLerp = 0f;

            RaycastHit2D ray = Physics2D.Raycast(new Vector2((moveDirection == -1 ? rightLegTarget.transform.position.x : leftLegTarget.transform.position.x) + (extraStepDis * runningStepDistanceMultiplier * moveDirection), leftLegTarget.transform.position.y + targetRaycastBuffer), Vector2.down, 200);
            Debug.DrawRay(new Vector3((moveDirection == -1 ? rightLegTarget.transform.position.x : leftLegTarget.transform.position.x) + (extraStepDis * runningStepDistanceMultiplier * moveDirection), leftLegTarget.transform.position.y + targetRaycastBuffer, 0), Vector3.down, Color.red, 3f);

            leftLegEndPos = ray.point;
        }
    }

    private void UpdateBalance()
    {
        //karaktärens x position
        float CenterOfMass = player.transform.position.x;

        //isBalanced blir true om karaktärens kropp är mellan båda targets
        isBalanced = IsFloatInRange(CenterOfMass, leftLegTarget.transform.position.x, rightLegTarget.transform.position.x);
    } //kollar om karaktären är "i balans"

    bool IsFloatInRange(float value, float bound1, float bound2)
    {
        float minValue = Mathf.Min(bound1, bound2);
        float maxValue = Mathf.Max(bound1, bound2);
        return value > minValue && value < maxValue;
    } //ger true om value är mellan bound1 och bound2
} 