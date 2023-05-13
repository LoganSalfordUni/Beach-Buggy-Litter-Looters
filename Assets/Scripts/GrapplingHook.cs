using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrapplingHook : MonoBehaviour
{
    [SerializeField] float maxRange;
    [SerializeField] float minDistance;
    [SerializeField] float chargeSpeed;
    [SerializeField] float travelSpeed;

    [SerializeField] float grabRadius;

    float travelDistance;
    Vector3 startingPosition;
    Vector3 targetPosition;
    [SerializeField] Transform grapplingHookHead;
    [SerializeField] Transform buggyBody;
    [SerializeField] BuggyMovement buggyMovement;

    [SerializeField] Image chargeImage;
    [SerializeField] Image chargeImageFilled;
    [SerializeField] Image chargeMinDistance;


    [SerializeField] LayerMask onlyLitter;

    float chargeAmount;
    bool grappleActive;//dont allow you to charge 
    bool goingOut;

    Plane plane;
    int planeLayerMask;
    private void Start()
    {
        chargeAmount = 0f;
        grappleActive = false;
        travelDistance = 0f;

        chargeMinDistance.fillAmount = minDistance / maxRange;

        plane = new Plane(Vector3.up, buggyBody.transform.position.y);
        planeLayerMask = LayerMask.GetMask("Plane");
    }
    private void Update()
    {
        //if the grapple hook isnt active, the travel distance of the hook should be reset to 0
        if (grappleActive == false)
            travelDistance = 0f;

        if (!Input.GetMouseButton(0))
            chargeImage.gameObject.SetActive(false);

        //Whilst holding the mouse button, and whilst the grapple hook isnt active. Charge it up
        if (Input.GetMouseButton(0) && grappleActive == false)
        {
            chargeAmount += Time.deltaTime * chargeSpeed;

            chargeImage.gameObject.SetActive(true);
            chargeImageFilled.fillAmount = (chargeAmount / maxRange);
        }
        else if (chargeAmount > minDistance)
        {
            //once the mouse button is released, if the charge amount is high enough
            grappleActive = true;
            goingOut = true;
            travelDistance = 0f;

            if (chargeAmount > maxRange)
                chargeAmount = maxRange;

            startingPosition = new Vector3(buggyBody.transform.position.x, 0f, buggyBody.transform.position.z);
            //targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out float distance))
            {
                targetPosition = ray.GetPoint(distance);
                targetPosition = new Vector3(targetPosition.x, 0f, targetPosition.z);
            }
            else
            {
                Debug.Log("ERROR. Target position not found");
                targetPosition = new Vector3(0f, 0f, 0f);
            }

            


            //used chat gpt for this bit
            Vector3 direction = targetPosition - startingPosition;
            float currentDistance = direction.magnitude;
            float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

            targetPosition = startingPosition + direction.normalized * chargeAmount;
            Debug.Log("Final target: " + targetPosition);
        }
        else if (grappleActive == false)//if the charge amount is too low to actually send out the grappling hook, reset charge amount to 0
            chargeAmount = 0f;

        if (grappleActive)
        {
            chargeAmount = 0f;

            if (goingOut)
            {
                travelDistance += travelSpeed * Time.deltaTime;
                grapplingHookHead.position = Vector3.Lerp(buggyBody.transform.position, targetPosition, travelDistance);
                if (travelDistance >= 1f)
                    goingOut = false;
            }
            else
            {
                travelDistance -= travelSpeed * Time.deltaTime;
                grapplingHookHead.position = Vector3.Lerp(buggyBody.transform.position, targetPosition, travelDistance);
                if (travelDistance <= 0f)
                    grappleActive = false;
            }


            //Pick up trash
            //RaycastHit[] pickUp = Physics.SphereCastAll(grapplingHookHead.position, grabRadius, Vector3.forward, Mathf.Infinity, onlyLitter);
            Collider[] pickUp = Physics.OverlapSphere(grapplingHookHead.position, grabRadius, onlyLitter);
            foreach (Collider litterPiece in pickUp)
            {
                litterPiece.transform.parent = grapplingHookHead;
                litterPiece.transform.localPosition = new Vector3(0f, 0f, 0f);
            }

            if (grappleActive == false)
            {
                foreach (Collider litterPiece in pickUp)
                {
                    float fuel = litterPiece.transform.GetComponent<Litter>().FuelValue;

                    buggyMovement.AddFuel(fuel);
                    Scoring.instance.IncreaseScore(fuel);

                    Destroy(litterPiece.transform.gameObject);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 x = ray.GetPoint(distance);
            Gizmos.DrawSphere(x, 0.5f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(grapplingHookHead.position, grabRadius);
    }
}
