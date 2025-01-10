using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_MoveCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform cameraPosition;

    private void Update()
    {
        transform.position = cameraPosition.position;
    }
}
