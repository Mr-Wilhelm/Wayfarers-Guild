using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PlatformMovement : MonoBehaviour
{
    [SerializeField] bool RandomMovement = false;
    [SerializeField] bool RandomRotation = false;

    private float wanderDistance = 10f;
    private float rotationAmount = 180f;

    // Start is called before the first frame update
    void Start()
    {
        if (RandomMovement)
        {
            StartCoroutine(MoveObject(GetRandomLocation()));

        }
        if (RandomRotation)
        {
            StartCoroutine(RotateObject(GetRandomRotation()));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private Vector3 GetRandomLocation()
    {
        Vector3 CurrentPosition = this.transform.position;
        
        Vector3 NewPos = new Vector3 
            (CurrentPosition.x+Random.Range(-wanderDistance,wanderDistance), 
            CurrentPosition.y + Random.Range(-wanderDistance, wanderDistance), 
            CurrentPosition.z + Random.Range(-wanderDistance, wanderDistance));

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
    
}
