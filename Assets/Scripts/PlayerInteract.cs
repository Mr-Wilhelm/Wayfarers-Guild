using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public bool canInteract;

    private void Update()
    {
        if(canInteract)
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("interact");
            }
        }
    }
}
