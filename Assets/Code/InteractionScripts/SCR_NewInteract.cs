using Gravitas.Demo;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SCR_NewInteract : NetworkBehaviour
{

    private bool interacting = false;

    [SerializeField] private float interactionRange;
    [SerializeField] NetworkVariable<bool> canInteract;

    public Camera playerCam;

    private void Start()
    {
        UpdateCanInteractBoolServerRpc(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Interacting");
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hitInfo, interactionRange))
            {
                if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Wheel") && !interacting && canInteract.Value)
                {
                    Debug.Log("Taking ship wheel");
                    gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    interacting = true;
                    UpdateCanInteractBoolServerRpc(false);
                    gameObject.GetComponent<GravitasFirstPersonPlayerSubject>().playerOnWheel = false;
                    //gameObject.GetComponent<GravitasFirstPersonPlayerSubject>().enabled = false;
                }
                else if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Wheel") && interacting && !canInteract.Value)
                {
                    Debug.Log("Getting off ship wheel");
                    gameObject.GetComponent<GravitasFirstPersonPlayerSubject>().playerOnWheel = true;
                    //gameObject.GetComponent<GravitasFirstPersonPlayerSubject>().enabled = true;
                    interacting = false;
                    UpdateCanInteractBoolServerRpc(true);
                }
            }
        }
    }

    [ServerRpc]
    private void UpdateCanInteractBoolServerRpc(bool newValue)
    {
        canInteract.Value = newValue;
    }
}
