using Gravitas.Demo;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SCR_NewInteract : NetworkBehaviour
{

    private bool interacting = false;

    [SerializeField] private float interactionRange;
    [SerializeField] public NetworkVariable<bool> canInteract;
    bool otherPlayerCanInteract = false;

    public Camera playerCam;

    private void Start()
    {
        UpdateCanInteractBoolServerRpc(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if(GameObject.FindGameObjectsWithTag("Player").Length != 1)
            {
                otherPlayerCanInteract = false;
                foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (!player.GetComponent<SCR_PlayerNetworkManager>().IsOwner)
                    {
                        otherPlayerCanInteract = player.GetComponent<SCR_NewInteract>().canInteract.Value;
                    }
                }
            }
            else
            {
                otherPlayerCanInteract = true;
            }
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hitInfo, interactionRange))
            {
                Debug.Log(hitInfo.collider.gameObject.name);
                if (interacting)
                {
                    gameObject.GetComponent<GravitasFirstPersonPlayerSubject>().playerOnWheel = false;
                    UpdateCanInteractBoolServerRpc(true);
                    interacting = false;
                    gameObject.GetComponent<SCR_ShipControls>().onWheel = false;
                }
                else if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Wheel") && !interacting && otherPlayerCanInteract)
                {
                    gameObject.transform.position = GameObject.Find("WheelPos").transform.position;
                    interacting = true;
                    UpdateCanInteractBoolServerRpc(false);
                    gameObject.GetComponent<GravitasFirstPersonPlayerSubject>().playerOnWheel = true;
                    gameObject.GetComponent<SCR_ShipControls>().onWheel = true;
                }
                else
                {
                    Debug.Log("Wheel hit, dont work tho");
                    Debug.Log(interacting);
                    Debug.Log(otherPlayerCanInteract);
                }
            }
            else if (interacting)
            {
                gameObject.GetComponent<GravitasFirstPersonPlayerSubject>().playerOnWheel = false;
                UpdateCanInteractBoolServerRpc(true);
                interacting = false;
                gameObject.GetComponent<SCR_ShipControls>().onWheel = false;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateCanInteractBoolServerRpc(bool newValue)
    {
        canInteract.Value = newValue;
    }
}
