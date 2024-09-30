using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PlatformMovement : MonoBehaviour
{
    [SerializeField] bool randomMovement = false;
    [SerializeField] bool randomRotation = false;

    [SerializeField] float wanderDistance = 10f;
    [SerializeField] float rotationAmount = 180f;

    private bool randomIsMovementActive = false;
    private bool randomRotationIsActive = false;

    [SerializeField] List<GameObject> playersOnShip = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

        if (randomMovement) {StartCoroutine(MoveObject(GetRandomLocation()));}

        if (randomRotation){StartCoroutine(RotateObject(GetRandomRotation()));}
    }

    // Update is called once per frame
    void Update()
    {
        if (!randomIsMovementActive && randomMovement)
        {StartCoroutine(MoveObject(GetRandomLocation())); randomIsMovementActive=true;}

        if (!randomRotationIsActive && randomRotation)
        { StartCoroutine (RotateObject(GetRandomRotation())); randomRotationIsActive=true;}

    }
    private void FixedUpdate()
    {
        CorrectPlayerGravity();
    }

    private void CorrectPlayerGravity()
    {
        Physics.gravity = this.transform.up * -1;

        //depedning on player controller for network will have to change
        foreach (GameObject go in playersOnShip) 
        {
            go.transform.up = this.transform.up;
            //go.transform.forward = this.transform.forward;
        }


    }
    private Vector3 GetRandomLocation()
    {
        Vector3 CurrentPosition = this.transform.position;
        
        Vector3 NewPos = new Vector3 
            (CurrentPosition.x+Random.Range(-wanderDistance,wanderDistance), 
            CurrentPosition.y + Random.Range(-wanderDistance, wanderDistance), 
            CurrentPosition.z + Random.Range(-wanderDistance, wanderDistance));
        if(NewPos.y < 0) { NewPos.y = 0; }
        return NewPos;
    }
    private Vector3 GetRandomRotation()
    {
        Vector3 CurrentRotation = this.transform.rotation.eulerAngles;

        Vector3 NewRot = new Vector3
            (CurrentRotation.x + (Random.Range(-rotationAmount, +rotationAmount)),
            CurrentRotation.y + (Random.Range(-rotationAmount, +rotationAmount)),
            CurrentRotation.z + (Random.Range(-rotationAmount, +rotationAmount)));
           
        return NewRot;
    }

    public IEnumerator MoveObject(Vector3 Destination)
    {
        Vector3 Origin = this.transform.position;
        float totalMovementTime = 5f; //the amount of time you want the movement to take
        float currentMovementTime = 0f;//The amount of time that has passed
        while (Vector3.Distance(transform.localPosition, Destination) > 0)
        {
            currentMovementTime += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(Origin, Destination, currentMovementTime / totalMovementTime);
            yield return null;
        }
        StartCoroutine(MoveObject(GetRandomLocation()));
    }
    public IEnumerator RotateObject(Vector3 Destination)
    {
        Vector3 Origin = this.transform.rotation.eulerAngles;
        float totalRotationTime = 5f; //the amount of time you want the movement to take
        float currentRotationTime = 0f;//The amount of time that has passed
        while (this.transform.rotation != Quaternion.Euler(Destination))
        {
            currentRotationTime += Time.deltaTime;
            transform.localEulerAngles = Vector3.Lerp(Origin, Destination, currentRotationTime / totalRotationTime);
            yield return null;

        }
        StartCoroutine(RotateObject(GetRandomRotation()));
        
    }


    //change off ontriggerenter and sett the players to automatically be in the ship
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other + "entered trigger");
        if (other.name == "Player")
        {
            playersOnShip.Add(other.transform.gameObject);
            other.transform.parent = this.transform;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            try
            {   playersOnShip.Remove(other.gameObject);
                other.transform.parent=null;}
            catch 
            {
                Debug.Log("ERRORRE");
            }

        }
    }

}


