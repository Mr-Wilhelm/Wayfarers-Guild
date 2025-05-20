using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBoundaries : MonoBehaviour
{
    [SerializeField]
    private GameObject airship;

    [SerializeField]
    private bool isXLocked, isYLocked, isZLocked;

    private void Awake()
    {
        airship = GameObject.Find("PRE-Airship");
    }

    private void Update()
    {
        if(isXLocked && !isYLocked && !isZLocked)
        {
            transform.position = new Vector3(transform.position.x, airship.transform.position.y, airship.transform.position.z);
        }
        else if(isYLocked && !isXLocked && !isZLocked)
        {
            transform.position = new Vector3(airship.transform.position.x, transform.position.y, airship.transform.position.z);
        }
        else if(isZLocked && !isXLocked && !isYLocked)
        {
            transform.position = new Vector3(airship.transform.position.x, airship.transform.position.y, transform.position.z);
        }
        else
        {
            Debug.LogWarning("World boundary has no assigned axis lock, or too many assigned axis locks, please assign only one axis lock per world boundary" + gameObject.name);
        }
    }
}
