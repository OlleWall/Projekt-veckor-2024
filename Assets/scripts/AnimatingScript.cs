using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatingScript : MonoBehaviour
{
    //player object
    public GameObject player;

    //IK target
    public Transform legTarget;

    //reference to the other foot/leg
    public AnimatingScript otherLeg;

    public bool isBalanced;

    [Header("Make a step forward")]
    // used to lerp the foot from its current position to target position
    public float lerp;

    // the start and end position of a step
    private Vector3 startPos;
    private Vector3 endPos;

    //how far we should anticipate a step (because we place our foots in front of our center of mass)
    public float overShootFactor = 0.5f;

    //how fast the foot moves
    public float stepSpeed = 3f;

    //the foot's displacement from the body's center on the X axis
    public float footDisplacementOnX;


    private void Start()
    {
        startPos = endPos = legTarget.position;
    }
    private void Update()
    {
        UpdateBalance();

        //this foot can only move when: (1) the other foot finishes moving, (2) the other foot made the last step
        bool thisFootCanMove = otherLeg.lerp > 1 && lerp > otherLeg.lerp;

        //if the body is not balanced AND this foot has finished its previous step (we don't want to calculate new steps in the process of moving a foot)
        if(!isBalanced && lerp > 1 && thisFootCanMove)
        {
            CalculateNewStep();
        }

        //using ease in/ease out value will make the animation look more natural
        float easedLerp = EaseInOutCubic(lerp);

        legTarget.position = Vector3.Lerp(startPos, endPos, easedLerp);
        lerp += Time.deltaTime * stepSpeed;
    }

    //smoothly ease in and ease out the input using sigmoid function
    private float EaseInOutCubic(float x)
    {
        return 1f / (1 + Mathf.Exp(-10 * (x - 0.5f)));
    }

    //calculate where the new step should be made
    private void CalculateNewStep()
    {
        //set starting position
        startPos = legTarget.position;

        //this will make the foot start moving to its target position starting from next frame
        lerp = 0;

        //find where the foot should land without considering overshoot
        RaycastHit2D ray = Physics2D.Raycast(player.transform.position + new Vector3(footDisplacementOnX, 0, 0), Vector2.down, 10);

        //consider the overshoot factor
        Vector3 posDiff = ((Vector3)ray.point - legTarget.position).normalized * (1 + overShootFactor);
        //posDiff = new Vector3(posDiff.x , ray.point.y, 0);

        //find end target position
        endPos = legTarget.position + posDiff;
    }

    private void UpdateBalance()
    {
        //center of mass position
        float centerOfMass = player.transform.parent.transform.position.x;

        //balanced is true if center of mass is between both feet
        isBalanced = IsFloatInRange(centerOfMass, legTarget.position.x, otherLeg.legTarget.position.x);
    }

    // returns true if "value" is between "bound1" and "bound2" 
    bool IsFloatInRange(float value, float bound1, float bound2)
    {
        float minValue = Mathf.Min(bound1, bound2);
        float maxValue = Mathf.Max(bound1, bound2);
        return value > minValue && value < maxValue;
    }
}
