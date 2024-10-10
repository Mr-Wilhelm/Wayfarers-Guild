using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public bool canInteract;

    public bool canDoSpeedMinigame;

    [SerializeField]
    private bool isInteracting;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && canInteract && canDoSpeedMinigame && !isInteracting)
        {
            isInteracting = true;
        }

        else if (Input.GetKeyDown(KeyCode.F) && isInteracting)
        {
            isInteracting = false;
        }
    }
}
