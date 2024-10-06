using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWorld : MonoBehaviour
{
    [SerializeField] bool randomRotation = false;
    [SerializeField] float rotationAmount = 45f;

    private bool randomRotationIsActive = false;
    // Start is called before the first frame update
    void Start()
    {
        if (randomRotation){StartCoroutine(RotateObject(GetRandomRotation()));}

    }

    // Update is called once per frame
    void Update()
    {
        if (!randomRotationIsActive && randomRotation)
        { StartCoroutine(RotateObject(GetRandomRotation())); randomRotationIsActive = true; }

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


}
