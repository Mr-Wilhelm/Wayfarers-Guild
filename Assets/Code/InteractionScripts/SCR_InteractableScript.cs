using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_InteractableScript : MonoBehaviour
{
    [SerializeField]
    protected SCR_PlayerInteract playerInteractScript;

    private void Update()
    {
        if (playerInteractScript == null)
        {
            StartCoroutine(JoeBalls());
            
        }
    }

    private IEnumerator JoeBalls()
    {
        yield return 0;
        playerInteractScript = Object.FindFirstObjectByType<SCR_PlayerInteract>();
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
