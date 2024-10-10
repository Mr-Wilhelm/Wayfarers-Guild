using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public bool canInteract;

    public bool canDoSpeedMinigame;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && canInteract && canDoSpeedMinigame)
        {
            Debug.Log("Do Speed Minigame");
        }
    }
}
