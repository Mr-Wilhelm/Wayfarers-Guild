using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableScript : MonoBehaviour
{
    [SerializeField]
    private PlayerInteract playerInteractScript;

    private void Update()
    {
        if (playerInteractScript == null)
        {
            playerInteractScript = Object.FindFirstObjectByType<PlayerInteract>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            playerInteractScript.canInteract = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            playerInteractScript.canInteract = false;
        }
    }
}
