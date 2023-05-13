using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuggyMovement : MonoBehaviour
{
    [Header("Speed Values")]
    [SerializeField] float startingMaxSpeed;
    [SerializeField, Tooltip("How long does the buggy take to reach max speed")] float accelerationTime = 0.5f;
    [SerializeField] float turnSpeed;
    [Space(1)]
    [SerializeField, Tooltip("How far do you travel to reach a distance milestone")] float distanceBetwenSpeedIncreases;
    [SerializeField, Tooltip("How much faster does the car get per distance milestones")] float speedIncreasePerMilestone;
    [SerializeField, Tooltip("The max speed can never go beyond this value")] float absoluteMaxSpeed;

    //oaef fuel
    //fuel should give you one travel unit
    [Header("Travel and Fuel")]
    [SerializeField, Tooltip("How far is a meter. affects score and fuel consumption")] float meterLength = 10f;
    [SerializeField, Tooltip("How much fuel you start with. You burn 1 per distance unit")] float startingFuel;
    [SerializeField, Tooltip("Whilst beyond fuel capacity, move at double speed and burn double fuel")] float fuelCapacity;
    //[SerializeField, Tooltip("Once fuel reaches this point. The buggy will gradually get slower until it hits 0")] float slowDownFuelAmount;
    float fuelAmount;


    [Header("Editor")]
    [SerializeField] CharacterController controller;
    [SerializeField, Tooltip("When turning into the sides of the map, the buggy starts to slow down. how far does the buggy start slowing")] float slowTowardsWallsDistance;
    [SerializeField, Tooltip("Detect only the invisible walls at the sides of the map")] LayerMask slowDownLayerMask;


    float currentMaxSpeed;
    float currentSpeed;
    float milestoneDistance;

    float distanceTravelled;


    private void Start()
    {
        distanceTravelled = 0f;
        milestoneDistance = distanceBetwenSpeedIncreases;
        currentMaxSpeed = startingMaxSpeed;
        fuelAmount = startingFuel;
    }



    void Update()
    {
        //forward momentum
        if (currentSpeed < currentMaxSpeed)
        {
            currentSpeed += (100/accelerationTime) * Time.deltaTime;
            if (currentSpeed > currentMaxSpeed)
            {
                currentSpeed = currentMaxSpeed;
            }
        }

        float effectiveSpeed = currentSpeed;
        float estimatedFuelUse = (effectiveSpeed * Time.deltaTime) / meterLength;

        if (fuelAmount - estimatedFuelUse > fuelCapacity)
        {
            effectiveSpeed *= 1.5f;
            estimatedFuelUse *= 2f;
        }
        else if (fuelAmount > fuelCapacity)
        {
            //oaef Okay so, i want the fuel use to scale down as you approach capacity, but the maths is complicated. so im just setting it for now
            fuelAmount = fuelCapacity + estimatedFuelUse;
        }
        if (fuelAmount <= 0f)
        {
            effectiveSpeed = 0f;
            //game is over at this point. but give a little timer of like 0.5 seconds so you can recover.   
        }

        float travelDistance = (effectiveSpeed * Time.deltaTime) / meterLength;
        fuelAmount -= estimatedFuelUse;
        //display fuel to the screen


        //sideways momentum
        float turnDirection = Input.GetAxis("Horizontal") * turnSpeed;
        //oaef raycast in the turn direction. if apporaching an edge, slow turn direction down a bit
        RaycastHit hit;
        Physics.Raycast(this.transform.position, new Vector3(0f, 0f, turnDirection), out hit, slowTowardsWallsDistance, slowDownLayerMask);
        if (hit.collider != null)
            turnDirection = turnDirection * (hit.distance / slowTowardsWallsDistance);


        //moving
        Vector3 moveDir = new Vector3(currentSpeed, 0f, turnDirection);
        controller.Move(moveDir * Time.deltaTime);


        Scoring.instance.IncreaseScore(travelDistance);
        distanceTravelled += travelDistance;
        if (distanceTravelled >= milestoneDistance)
        {
            TriggerDistanceMilestone();
        }
        //call the scoring script. add to the score 
    }

    public void AddFuel(float fuelValue)
    {
        fuelAmount += fuelValue;
    }

    void TriggerDistanceMilestone()
    {
        Debug.Log("I have travelled: " + distanceTravelled + "out of: " + milestoneDistance);

        currentMaxSpeed += speedIncreasePerMilestone;
        if (currentMaxSpeed > absoluteMaxSpeed)
            currentMaxSpeed = absoluteMaxSpeed;

        milestoneDistance += distanceBetwenSpeedIncreases;
    }
}
