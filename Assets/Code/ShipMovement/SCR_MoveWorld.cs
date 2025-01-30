using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class SCR_MoveWorld : MonoBehaviour
{

    [SerializeField] bool randomRotation = false;
    [SerializeField] bool randomMovement = false;
    [SerializeField] float rotationAmount = 45f;
    [SerializeField] float randomMovementAmount = 10.0f;

    [SerializeField] GameObject worldCentrePosition;

    private bool randomRotationIsActive = false;
    private bool randomMovementIsActive = false;
    // Start is called before the first frame update
    void Start()
    {
        //get the gameobject that the world moves with.
        if (worldCentrePosition == null)
        {
            worldCentrePosition=transform.GetChild(0).gameObject;
        }
        if (randomMovement) { }
        if (randomRotation){StartCoroutine(RotateObject(GetRandomRotation()));}

    }

    // Update is called once per frame
    void Update()
    {
        if (!randomRotationIsActive && randomRotation)
        { StartCoroutine(RotateObject(GetRandomRotation())); randomRotationIsActive = true; }

        if (!randomMovementIsActive && randomMovement)
        {
            StartCoroutine(MovePlatform(GetRandomPosition())); randomMovementIsActive = true;
        }

    }


    private Vector3 GetRandomPosition()
    {
        Vector3 currentPos = worldCentrePosition.transform.localPosition;


        Vector3 newPos = new Vector3(
            currentPos.x + (Random.Range(-randomMovementAmount, +randomMovementAmount)),
            currentPos.y + (Random.Range(-randomMovementAmount, +randomMovementAmount)),
            currentPos.z + (Random.Range(-randomMovementAmount, +randomMovementAmount)));

        return newPos;
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

    public IEnumerator MovePlatform(Vector3 Destination)
    {
        Vector3 Origin = worldCentrePosition.transform.localPosition;
        float totalMoveTime = 5f; //the amount of time you want the movement to take
        float currentMoveTime = 0f;//The amount of time that has passed
        while (worldCentrePosition.transform.localPosition != Destination)
        {
            currentMoveTime += Time.deltaTime;
            worldCentrePosition.transform.position = Vector3.Lerp(Origin, Destination, currentMoveTime / totalMoveTime);
            yield return null;

        }
        StartCoroutine(MovePlatform(GetRandomPosition()));
    }


}
