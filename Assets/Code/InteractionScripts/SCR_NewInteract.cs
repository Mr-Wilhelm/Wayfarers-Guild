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
    private GameObject ship;

    [SerializeField] private KeyCode InteractKey = KeyCode.F;
    [SerializeField] private KeyCode DropKey = KeyCode.G;

    [SerializeField] private LayerMask Wheel;
    [SerializeField] private LayerMask Ballista;
    [SerializeField] private GravitasFirstPersonPlayerSubject playerScriptReference;

    private void Start()
    {
        UpdateCanInteractBoolServerRpc(true);
        playerScriptReference = GetComponent<GravitasFirstPersonPlayerSubject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(InteractKey))
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
                    playerScriptReference.playerOnWheel = false;
                    UpdateCanInteractBoolServerRpc(true);
                    interacting = false;
                    gameObject.GetComponent<SCR_ShipControls>().onWheel = false;
                }
                else if (hitInfo.collider.gameObject.layer == Wheel && !interacting && otherPlayerCanInteract)
                {
                    if (ship == null)
                    {
                        ship = GameObject.Find("PRE-Airship");
                    }
                    gameObject.transform.position = GameObject.Find("WheelPos").transform.position;
                    interacting = true;
                    UpdateCanInteractBoolServerRpc(false);
                    playerScriptReference.playerOnWheel = true;
                    gameObject.GetComponent<SCR_ShipControls>().onWheel = true;
                }
                else if (hitInfo.collider.gameObject.layer == Ballista)
                {
                    Debug.Log("Interacting with ballsita");
                    if (playerScriptReference.hasItem == false)
                    {
                        Debug.Log("Picking up ballista");
                        playerScriptReference.hasItem = true;
                    }
                    else { Debug.Log("Already have item"); }
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
        if(Input.GetKeyDown(DropKey))
        {
            if(playerScriptReference.hasItem == false) { Debug.Log("No item to drop"); }
            else
            {
                dropItem();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateCanInteractBoolServerRpc(bool newValue)
    {
        canInteract.Value = newValue;
    }

    private void dropItem()
    {
        playerScriptReference.hasItem = false;
        Debug.Log("Dropping item");
    }
}
