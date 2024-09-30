using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PlatformMovement : MonoBehaviour
{
    [SerializeField] bool PlatformRandomMovement = false;

    private float RandomCloseness = 10f;

    // Start is called before the first frame update
    void Start()
    {
        if (PlatformRandomMovement)
        {
            StartCoroutine(moveObject(RandomClsoeLocation()));

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
            (CurrentPosition.x+Random.Range(-RandomCloseness,RandomCloseness), 
            CurrentPosition.y + Random.Range(-RandomCloseness, RandomCloseness), 
            CurrentPosition.z + Random.Range(-RandomCloseness, RandomCloseness));

        return NewPos;
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
}
