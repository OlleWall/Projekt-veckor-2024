using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProceduralAnimation : MonoBehaviour
{
    #region InformationAboutScriptValues
    /*
     * 
    */
    #endregion

    #region Variabler
    [Header("Debug checks")]
    #region DebugChecks
    public bool isBalanced;
    public bool isRunning;
    public bool isJumping;
    public bool isCrouching;

    public bool isTakingStep = false;


    public bool leftLegCanMove;
    public bool leftArmCanMove;

    public bool rightLegCanMove;   
    public bool rightArmCanMove;


    public bool leftLegMovedLast;
    public bool rightLegMovedLast;

    public bool facingRight;
    #endregion

    [Header("References")]
    #region References
    public GameObject player;
    private GameObject playerSprite;

    public GameObject lowerBackBone;
    public GameObject upperBackBone;
    
    PlayerMovement movementCode;
    
    public GameObject leftLegTarget;
    public GameObject leftArmTarget;

    public GameObject rightLegTarget;
    public GameObject rightArmTarget;

    public Transform rightLegObject;
    public Transform leftLegObject;

    public float moveDirection;
    #endregion

    [Header("Other values")]
    #region Other values
    public float targetRaycastBuffer;

    public float extraStepDis;

    public float timeBackToIdle = 0.15f;

    public float crouchHeight;
    public float crouchLowerBackRotation;
    public float crouchUpperBackRotation;

    float jumpForceOriginalValue;

    public float stepHeight;
    public float stepSpeed;
    public float stepCooldown;

    public float runStepMulti; //variabel för att ändra multiplier i unity
    private float runningStepDistanceMultiplier; //variabeln som faktiskt används av koden, olika för att komma ihåg 

    public float crouchStepMulti; //oanvänd för tillfället
    private float crouchingStepDistanceMultiplier = 1f; //oanvänd för tillfället

    #region Properties
    public float _stepTime;
    public float stepTime
    {
        get
        {
            return _stepTime;
        }

        set 
        {     
            _stepTime = value;
            if(stepTime > 1)
            {
                stepTime = 1;
                //isTakingStep = false;
            }
        }
    }


    public float _rightLerp = 0.02f;
    public float rightLerp { 
     get{return _rightLerp;} 

      set{ 
            _rightLerp = value;
            if (rightLerp > 1.5f)
            {

                rightLerp = 1f;
            }
        }
    }
    #endregion

    #region TargetPositions
    #region LeftLegTargetPositions
    private Vector3 leftLegStartPos;
    private Vector3 leftLegMidPos;
    private Vector3 leftLegEndPos;
    private Vector3 leftLegIdlePosition;
    #endregion

    #region RightLegTargetPositions
    private Vector3 rightLegStartPos;
    private Vector3 rightLegMidPos;
    private Vector3 rightLegEndPos;
    private Vector3 rightLegIdlePosition;
    #endregion

    #region RightArmTargetPositions
    private Vector3 rightArmStartPos;
    private Vector3 rightArmMidPos;
    private Vector3 rightArmEndPos;
    private Vector3 rightArmIdlePosition;
    #endregion

    #region LeftArmTargetPositions
    private Vector3 leftArmStartPos;
    private Vector3 leftArmMidPos;
    private Vector3 leftArmEndPos;
    private Vector3 leftArmIdlePosition;
    #endregion
    #endregion
    #endregion

    [Header("Bodyparts placement from character center")]
    #region BodypartsPlacementFromCharacterCenter
    public float leftLegDistanceFromCenter = 0.13f;
    public float leftArmDistanceFromCenter = 0.33f;

    public float rightLegDistanceFromCenter = -0.105f;
    public float rightArmDistanceFromCenter = -0.33f;

    public float armHeightDifferenceFromCenter;
    public float feetPlacementInAir;
    #endregion
    #endregion


    private void Start()
    {
        movementCode = GetComponentInParent<PlayerMovement>();
        playerSprite = player.transform.GetChild(0).gameObject;

        leftLegStartPos = leftLegEndPos = leftLegObject.transform.position;
        rightLegStartPos = rightLegEndPos = rightLegObject.transform.position;

        jumpForceOriginalValue = movementCode.jumpForce;
    }

    private void Update()
    {
        #region RightArmTargetPlacements
        rightArmTarget.transform.position = new Vector3(rightArmTarget.transform.position.x, player.transform.position.y - armHeightDifferenceFromCenter, rightArmTarget.transform.position.z);
        leftArmTarget.transform.position = new Vector3(leftArmTarget.transform.position.x, player.transform.position.y - armHeightDifferenceFromCenter, leftArmTarget.transform.position.z);
        #endregion

        #region FlipSprite 
        //flippar sprite så den tittar dit du rör dig

        if (moveDirection == 1 && !facingRight)
        {
            playerSprite.transform.localScale = new Vector3(-playerSprite.transform.localScale.x, playerSprite.transform.localScale.y, playerSprite.transform.localScale.z);
            playerSprite.transform.localPosition = new Vector3(-playerSprite.transform.localPosition.x, playerSprite.transform.localPosition.y, playerSprite.transform.localPosition.z);

            //lowerBackBone.transform.eulerAngles = new Vector3(lowerBackBone.transform.eulerAngles.x, lowerBackBone.transform.eulerAngles.y, 90);

            facingRight = true;
        }

        else if (moveDirection == -1 && facingRight)
        {
            playerSprite.transform.localScale = new Vector3(-playerSprite.transform.localScale.x, playerSprite.transform.localScale.y, playerSprite.transform.localScale.z);
            playerSprite.transform.localPosition = new Vector3(-playerSprite.transform.localPosition.x, playerSprite.transform.localPosition.y, playerSprite.transform.localPosition.z);

            facingRight = false;
        }
        #endregion

        #region Idle
        if (movementCode.inputX == 0 && !isTakingStep && movementCode.rb2D.velocity.magnitude < 0.1f && !isJumping)
        {
            if (!facingRight)
            {
                rightArmIdlePosition = new Vector3(player.transform.position.x + rightArmDistanceFromCenter, rightArmTarget.transform.position.y, rightArmTarget.transform.position.z);
                rightLegIdlePosition = new Vector3(player.transform.position.x + rightLegDistanceFromCenter, rightLegTarget.transform.position.y, rightLegTarget.transform.position.z);

                leftArmIdlePosition = new Vector3(player.transform.position.x + leftArmDistanceFromCenter, leftArmTarget.transform.position.y, leftArmTarget.transform.position.z);
                leftLegIdlePosition = new Vector3(player.transform.position.x + leftLegDistanceFromCenter, leftLegTarget.transform.position.y, leftLegTarget.transform.position.z);
            }

            if (facingRight)
            {
                rightArmIdlePosition = new Vector3(player.transform.position.x - rightArmDistanceFromCenter, rightArmTarget.transform.position.y, rightArmTarget.transform.position.z);
                rightLegIdlePosition = new Vector3(player.transform.position.x + leftLegDistanceFromCenter, rightLegTarget.transform.position.y, rightLegTarget.transform.position.z);

                leftArmIdlePosition = new Vector3(player.transform.position.x - leftArmDistanceFromCenter, leftArmTarget.transform.position.y, leftArmTarget.transform.position.z);
                leftLegIdlePosition = new Vector3(player.transform.position.x + rightLegDistanceFromCenter, leftLegTarget.transform.position.y, leftLegTarget.transform.position.z);
            }

            #region TargetsBackToIdle
            rightArmTarget.transform.position = Vector3.Lerp(rightArmTarget.transform.position, rightArmIdlePosition, timeBackToIdle);
            rightLegTarget.transform.position = Vector3.Lerp(rightLegTarget.transform.position, rightLegIdlePosition, timeBackToIdle);

            leftArmTarget.transform.position = Vector3.Lerp(leftArmTarget.transform.position, leftArmIdlePosition, timeBackToIdle);
            leftLegTarget.transform.position = Vector3.Lerp(leftLegTarget.transform.position, leftLegIdlePosition, timeBackToIdle);
            #endregion
        } //för idle skulle jag kunna lägga empties i spelaren som alltid har samma plats på spelaren, och sätta varje target till sin specifika idle empty.position
        #endregion

        #region Walking & Running
        if ((movementCode.inputX != 0 && !isJumping) || (isTakingStep && !isJumping))
        {
            //moveDirection 1 = höger, -1 = vänster
            moveDirection = movementCode.facingRight;

            UpdateBalance();

            #region CalculateStepCall
            if (!isBalanced && !isTakingStep)
                {
                    CalculateStep();
                }
                #endregion


            #region RunningValues
                isRunning = Input.GetKey(KeyCode.LeftShift);
                if (isRunning)
                {
                    runningStepDistanceMultiplier = runStepMulti;
                }
                else
                {
                    runningStepDistanceMultiplier = 1;
                }
                #endregion


            #region Lerp & CanMove 
                //räknar upp lerp variablerna och kollar vilken fot som borde röra sig

                rightLerp += Time.deltaTime * stepSpeed * (Mathf.Pow(runningStepDistanceMultiplier, 2));
                stepTime += Time.deltaTime * stepSpeed;            
                

                rightArmCanMove = !rightLegCanMove;
                leftArmCanMove = rightLegCanMove;
                #endregion
        }
        #endregion

        #region Jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isTakingStep = false;
            isJumping = true;

            if (facingRight)
            {
                rightLegTarget.transform.position = new Vector3(player.transform.position.x + leftLegDistanceFromCenter, player.transform.position.y - feetPlacementInAir, rightLegTarget.transform.position.z);
                leftLegTarget.transform.position = new Vector3(player.transform.position.x + leftLegDistanceFromCenter, player.transform.position.y - feetPlacementInAir, leftLegTarget.transform.position.z);

                //right arm
                //left arm
            }
            else
            {
                rightLegTarget.transform.position = new Vector3(player.transform.position.x + rightLegDistanceFromCenter, player.transform.position.y - feetPlacementInAir, rightLegTarget.transform.position.z);
                leftLegTarget.transform.position = new Vector3(player.transform.position.x + rightLegDistanceFromCenter, player.transform.position.y - feetPlacementInAir, leftLegTarget.transform.position.z);
                
                //right arm
                //left arm
            }

            
        }
        else if (movementCode.GroundCheck())
        {
            isJumping = false;
        }
        #endregion

        #region Crouching
        if (movementCode.crouching)
        {
            movementCode.jumpForce = 4f;
            crouchingStepDistanceMultiplier = crouchStepMulti;

            if (facingRight)
            {
                lowerBackBone.transform.localEulerAngles = new Vector3(lowerBackBone.transform.localEulerAngles.x, lowerBackBone.transform.localEulerAngles.y, 90);
                lowerBackBone.transform.localEulerAngles = new Vector3(lowerBackBone.transform.localEulerAngles.x, lowerBackBone.transform.localEulerAngles.y, lowerBackBone.transform.localEulerAngles.z + crouchLowerBackRotation);

                upperBackBone.transform.localEulerAngles = new Vector3(upperBackBone.transform.localEulerAngles.x, upperBackBone.transform.localEulerAngles.y, 90);
                upperBackBone.transform.localEulerAngles = new Vector3(upperBackBone.transform.localEulerAngles.x, upperBackBone.transform.localEulerAngles.y, crouchUpperBackRotation);
            }
            else
            {
                lowerBackBone.transform.localEulerAngles = new Vector3(lowerBackBone.transform.localEulerAngles.x, lowerBackBone.transform.localEulerAngles.y, 90);
                lowerBackBone.transform.localEulerAngles = new Vector3(lowerBackBone.transform.localEulerAngles.x, lowerBackBone.transform.localEulerAngles.y, lowerBackBone.transform.localEulerAngles.z + crouchLowerBackRotation);

                upperBackBone.transform.localEulerAngles = new Vector3(upperBackBone.transform.localEulerAngles.x, upperBackBone.transform.localEulerAngles.y, 90);
                upperBackBone.transform.localEulerAngles = new Vector3(upperBackBone.transform.localEulerAngles.x, upperBackBone.transform.localEulerAngles.y, crouchUpperBackRotation);
            }
        }
        else
        {
            movementCode.jumpForce = jumpForceOriginalValue;
            crouchingStepDistanceMultiplier = 1;

            lowerBackBone.transform.localEulerAngles = new Vector3(lowerBackBone.transform.localEulerAngles.x, lowerBackBone.transform.localEulerAngles.y, 90);
            upperBackBone.transform.localEulerAngles = new Vector3(upperBackBone.transform.localEulerAngles.x, upperBackBone.transform.localEulerAngles.y, 0);
        }
        #endregion

        #region ChangeTargetsPositions

        if (isTakingStep)
        {
            //ändrar targets positioner från startpunkt till slutpunkt över tid

            #region TargetPositions
            rightLegMidPos = new Vector3((rightLegStartPos.x + rightLegEndPos.x) / 2, rightLegStartPos.y + stepHeight, rightLegStartPos.z);
            leftLegMidPos = new Vector3((leftLegStartPos.x + leftLegEndPos.x) / 2, leftLegStartPos.y + stepHeight, leftLegStartPos.z);

            rightArmStartPos = new Vector3(leftLegStartPos.x, rightArmTarget.transform.position.y, rightArmTarget.transform.position.z);
            leftArmStartPos = new Vector3(rightLegStartPos.x, leftArmTarget.transform.position.y, leftArmTarget.transform.position.z);

            rightArmMidPos = new Vector3((leftLegStartPos.x + leftLegEndPos.x) / 2, rightArmTarget.transform.position.y - stepHeight, rightArmTarget.transform.position.z);
            leftArmMidPos = new Vector3((rightLegStartPos.x + rightLegEndPos.x) / 2, leftArmTarget.transform.position.y - stepHeight, leftArmTarget.transform.position.z);

            rightArmEndPos = new Vector3(leftLegEndPos.x, rightArmTarget.transform.position.y, rightArmTarget.transform.position.z);
            leftArmEndPos = new Vector3(rightLegEndPos.x, leftArmTarget.transform.position.y, leftArmTarget.transform.position.z);
            #endregion

            if (stepTime >= 1)
            {
                rightLegCanMove = !rightLegCanMove;
            }

            if (rightLegMovedLast)
            {
                rightLegTarget.transform.position = (Mathf.Pow(1 - stepTime, 2) * rightLegStartPos) + (2 * (1 - stepTime) * stepTime * rightLegMidPos) + (Mathf.Pow(stepTime, 2) * rightLegEndPos);
                leftArmTarget.transform.position = (Mathf.Pow(1 - stepTime, 2) * leftArmStartPos) + (2 * (1 - stepTime) * stepTime * leftArmMidPos) + (Mathf.Pow(stepTime, 2) * leftArmEndPos);
                isTakingStep = stepTime >= 1f ? false : true;
            }

            else
            {
                leftLegTarget.transform.position = (Mathf.Pow(1 - stepTime, 2) * leftLegStartPos) + (2 * (1 - stepTime) * stepTime * leftLegMidPos) + (Mathf.Pow(stepTime, 2) * leftLegEndPos);
                rightArmTarget.transform.position = (Mathf.Pow(1 - stepTime, 2) * rightArmStartPos) + (2 * (1 - stepTime) * stepTime * rightArmMidPos) + (Mathf.Pow(stepTime, 2) * rightArmEndPos);
                isTakingStep = stepTime >= 1f ? false : true;
            }
        }
        #endregion
    }


    private void CalculateStep() //räknar ut var fötterna ska placeras, har försökt anpassa det så att det blir rätt vid sprint
    {
        //!isbalanced && leg furthest away from me should move
        
        if (rightLegCanMove)
        {
            rightLegStartPos = rightLegTarget.transform.position;

            rightLerp = 0f;
            stepTime = 0f;

            RaycastHit2D ray = Physics2D.Raycast(new Vector2((rightLegMovedLast ? rightLegObject.transform.position.x : leftLegObject.transform.position.x) + (extraStepDis * runningStepDistanceMultiplier * moveDirection), transform.position.y + targetRaycastBuffer), Vector2.down, 200);

            rightLegEndPos = ray.point;

            isTakingStep = true;

            rightLegMovedLast = true;
            leftLegMovedLast = false;
        }

        else
        {
            leftLegStartPos = leftLegTarget.transform.position;

            rightLerp = 0f;
            stepTime = 0f;

            RaycastHit2D ray = Physics2D.Raycast(new Vector2((rightLegMovedLast ? rightLegObject.transform.position.x : leftLegObject.transform.position.x) + (extraStepDis * runningStepDistanceMultiplier * moveDirection), transform.position.y + targetRaycastBuffer), Vector2.down, 200);

            leftLegEndPos = ray.point;

            isTakingStep = true;

            rightLegMovedLast = false;
            leftLegMovedLast = true;
        }
    }

    private void UpdateBalance()
    {
        //karaktärens x position
        float CenterOfMass = player.transform.position.x;

        //isBalanced blir true om karaktärens kropp är mellan båda targets
        isBalanced = IsFloatInRange(CenterOfMass, leftLegObject.transform.position.x, rightLegObject.transform.position.x);
    } //kollar om karaktären är "i balans"

    bool IsFloatInRange(float value, float bound1, float bound2)
    {
        float minValue = Mathf.Min(bound1, bound2);
        float maxValue = Mathf.Max(bound1, bound2);
        return value > minValue && value < maxValue;
    } //ger true om value är mellan bound1 och bound2
}

#region savedCode
/*
//variabler för att göra steget mer smooth
//float rightEasedLerp;
//float leftEasedLerp;


//RaycastHit2D ray = Physics2D.Raycast(new Vector2((moveDirection == 1 ? leftLegTarget.transform.position.x : rightLegTarget.transform.position.x) + (extraStepDis * runningStepDistanceMultiplier * moveDirection), (moveDirection == 1 ? leftLegTarget.transform.position.y : rightLegTarget.transform.position.y) + targetRaycastBuffer), Vector2.down, 200);
//Debug.DrawRay(new Vector3((moveDirection == 1 ? leftLegTarget.transform.position.x : rightLegTarget.transform.position.x) + (extraStepDis * runningStepDistanceMultiplier * moveDirection), rightLegTarget.transform.position.y + targetRaycastBuffer, 0), Vector3.down, Color.red, 10f);

//RaycastHit2D ray = Physics2D.Raycast(new Vector2((moveDirection == -1 ? rightLegTarget.transform.position.x : leftLegTarget.transform.position.x) + (extraStepDis * runningStepDistanceMultiplier * moveDirection), (moveDirection == -1 ? rightLegTarget.transform.position.y : leftLegTarget.transform.position.y) + targetRaycastBuffer), Vector2.down, 200);
//Debug.DrawRay(new Vector3((moveDirection == -1 ? rightLegTarget.transform.position.x : leftLegTarget.transform.position.x) + (extraStepDis * runningStepDistanceMultiplier * moveDirection), leftLegTarget.transform.position.y + targetRaycastBuffer, 0), Vector3.down, Color.red, 10f);

//RaycastHit2D ray = Physics2D.Raycast(new Vector2((rightLegMovedLast ? rightLegTarget.transform.position.x : leftLegTarget.transform.position.x) + (extraStepDis * runningStepDistanceMultiplier * moveDirection), (rightLegMovedLast ? rightLegTarget.transform.position.y : leftLegTarget.transform.position.y) + targetRaycastBuffer), Vector2.down, 200);


rightLegTarget.transform.position = Vector3.Lerp(rightLegStartPos, rightLegEndPos, rightLerp);
leftLegTarget.transform.position = Vector3.Lerp(leftLegStartPos, leftLegEndPos, leftLerp);


om rightLerp < 0.5 
rightLegTarget.transform.position = Vector3.Lerp(rightLegStartPos, rightLegMidPos, rightLerp);
annars den här
rightLegTarget.transform.position = Vector3.Lerp(rightLegMidPos, rightLegEndPos, rightLerp);

Debug.DrawRay(new Vector3(rightLegObject.transform.position.x + (extraStepDis * runningStepDistanceMultiplier * moveDirection), transform.position.y + targetRaycastBuffer), Vector3.down * 200, Color.green, 10f);
Debug.DrawRay(new Vector3((rightLegMovedLast ? rightLegTarget.transform.position.x : leftLegTarget.transform.position.x) + (extraStepDis * runningStepDistanceMultiplier * moveDirection), (rightLegMovedLast ? rightLegTarget.transform.position.y : leftLegTarget.transform.position.y) + targetRaycastBuffer, 0), Vector3.down, Color.green, 10f);
Debug.DrawRay(new Vector3(rightLegObject.transform.position.x + (extraStepDis * runningStepDistanceMultiplier * moveDirection), transform.position.y + targetRaycastBuffer), Vector3.down * 200, Color.green, 10f);

//RaycastHit2D ray = Physics2D.Raycast(new Vector2(rightLegObject.transform.position.x + (extraStepDis * runningStepDistanceMultiplier * moveDirection), transform.position.y + targetRaycastBuffer), Vector2.down, 200);

//RaycastHit2D ray = Physics2D.Raycast(new Vector2((leftLegMovedLast ? leftLegObject.transform.position.x : rightLegObject.transform.position.x) + (extraStepDis * runningStepDistanceMultiplier * moveDirection), transform.position.y + targetRaycastBuffer), Vector2.down, 200);
        
//RaycastHit2D ray = Physics2D.Raycast(new Vector2(leftLegObject.transform.position.x + (extraStepDis * runningStepDistanceMultiplier * moveDirection), transform.position.y + targetRaycastBuffer), Vector2.down, 200);



                if (!leftLegMovedLast && !rightLegMovedLast && beforeFirstStep)
                {
                    beforeFirstStep = false;
                    if (moveDirection == 1 && rightLerp > leftLerp)
                    {
                        float temp;
                        temp = leftLerp;
                        leftLerp = rightLerp;
                        rightLerp = temp;
                    } //om vi kollar höger och höger ben ska röra sig först, så byts värdena så vänster ben rör sig först

                    else if (moveDirection == -1 && leftLerp > rightLerp)
                    {
                        float temp;
                        temp = rightLerp;
                        rightLerp = leftLerp;
                        leftLerp = temp;
                    } //samma sak åt andra hållet
                } //vilket ben som ska röra sig först, ändrar det benets lerp till det större värdet och sker bara innan första steget
                
*/
#endregion