using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public bool canInteract;

    [SerializeField]
    private GameObject[] interactableObjects;

    [SerializeField]
    private Vector3 distanceToObject;

    private void Start()
    {
        interactableObjects = GameObject.FindGameObjectsWithTag("Interactable");
    }

    private void Update()
    {
        //find nearest interact

        foreach (var obj in interactableObjects)
        {
            distanceToObject = new Vector3(
                gameObject.transform.position.x - obj.transform.position.x, 
                gameObject.transform.position.y - obj.transform.position.y,
                gameObject.transform.position.z - obj.transform.position.z);
        }

        if(canInteract)
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("Interact with the thing");
            }
        }
    }
}
