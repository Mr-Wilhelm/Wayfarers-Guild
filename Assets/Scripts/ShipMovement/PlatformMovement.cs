using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PlatformMovement : MonoBehaviour
{
    [SerializeField] bool RandomMovement = false;
    [SerializeField] bool RandomRotation = false;

    private float WanderDistance = 10f;
    private float RotationDistance = 90;

    // Start is called before the first frame update
    void Start()
    {
        if (RandomMovement)
        {
            StartCoroutine(moveObject(RandomClsoeLocation()));

        }
        if (RandomRotation)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3 RandomClsoeLocation()
    {
        Vector3 CurrentPosition = this.transform.position;
        
        Vector3 NewPos = new Vector3 
            (CurrentPosition.x+Random.Range(-WanderDistance,WanderDistance), 
            CurrentPosition.y + Random.Range(-WanderDistance, WanderDistance), 
            CurrentPosition.z + Random.Range(-WanderDistance, WanderDistance));

        return NewPos;
    }
    private Vector3 GetRandomRotation()
    {
        Vector3 CurrentRotation = this.transform.rotation.eulerAngles;

        //Vector3 NewRot = new Vector3(CurrentRotation.x + (Random.Range(-RotationDistance, +RotationDistance)));
           

        return CurrentRotation;
    }

    public IEnumerator moveObject(Vector3 Destination)
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
        StartCoroutine(moveObject(RandomClsoeLocation()));
    }
    public IEnumerator RotateObject(Vector3 Destination)
    {
        Vector3 Origin = this.transform.position;
        float totalRotationTime = 5f; //the amount of time you want the movement to take
        float currentRotationTime = 0f;//The amount of time that has passed
        while (Vector3.Distance(transform.localPosition, Destination) > 0)
        {
            currentRotationTime += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(Origin, Destination, currentRotationTime / totalRotationTime);
            yield return null;
        }
        StartCoroutine(RotateObject(RandomClsoeLocation()));
    }
}
