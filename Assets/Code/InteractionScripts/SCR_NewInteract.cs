using Gravitas.Demo;
using System;
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
    [SerializeField] private LayerMask BallistaBolt;
    [SerializeField] private LayerMask EngineFuel;
    [SerializeField] private LayerMask PickUp;
    [SerializeField] private LayerMask ActualPickUp;
    [SerializeField] private GravitasFirstPersonPlayerSubject playerScriptReference;

    [SerializeField] private GameObject craigBodyMesh;
    [SerializeField] private GameObject craigClothesMesh;
    [SerializeField] private GameObject craigHoldItemMesh;
    [SerializeField] private GameObject engineFoodMesh;
    [SerializeField] private GameObject ballistaBoltMesh;
    private string objectBeingHeld = string.Empty;

    [SerializeField] private GameObject ballistaBoltPrefab;
    [SerializeField] private GameObject engineFoodPrefab;
    [SerializeField] private GameObject dropPosition;

    private void Start()
    {
        UpdateCanInteractBoolServerRpc(true);
        playerScriptReference = GetComponent<GravitasFirstPersonPlayerSubject>();
        craigHoldItemMesh.SetActive(false);
        engineFoodMesh.SetActive(false);
        ballistaBoltMesh.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) { enabled = false; return; }
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
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hitInfo, interactionRange, PickUp))
            {
                if (interacting)
                {
                    playerScriptReference.playerOnWheel = false;
                    UpdateCanInteractBoolServerRpc(true);
                    interacting = false;
                    gameObject.GetComponent<SCR_ShipControls>().onWheel = false;
                }
                else if (hitInfo.collider.gameObject.CompareTag("Wheel") && !interacting && otherPlayerCanInteract)
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
                else if (hitInfo.collider.gameObject.CompareTag("Ballista Storage"))
                {
                    //Checks if player already has item
                    if (playerScriptReference.hasItem == false)
                    {
                        //Picks up ballista bolt from storage
                        pickUpItem("Ballista Bolt", false, null);
                        Debug.Log(hitInfo.collider.gameObject.name);
                        playerScriptReference.hasItem = true;
                    }
                }
                else if (hitInfo.collider.gameObject.CompareTag("Engine"))
                {
                    if (objectBeingHeld == "Engine Food")
                    {
                        Debug.Log("Interact with engine");
                        dropItem(true);
                        if (ship == null) { ship = GameObject.Find("PRE-Airship"); }
                        ship.GetComponent<SCR_ShipMovement>().boostSpeedServerRPC();
                    }
                }
                else if (hitInfo.collider.gameObject.CompareTag("Fuel Storage"))
                {
                    if(playerScriptReference.hasItem == false)
                    {
                        pickUpItem("Engine Food", false, null);
                        playerScriptReference.hasItem = true;
                    }
                }
                else if(hitInfo.collider.gameObject.CompareTag("Ballista Bolt"))
                {
                    Debug.Log("Found bolt on ground");
                    pickUpItem("Ballista Bolt", true, hitInfo.collider.transform.root.gameObject);
                    playerScriptReference.hasItem = true;
                }
                else if(hitInfo.collider.gameObject.CompareTag("Engine Fuel"))
                {
                    Debug.Log("Found scran on ground");
                    pickUpItem("Engine Food", true, hitInfo.collider.transform.root.gameObject);
                    playerScriptReference.hasItem = true;
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

    private void dropItem(bool itemBeingDeleted = false)
    {
        playerScriptReference.hasItem = false;
        DropItemServerRPC();
        if(objectBeingHeld == "Ballista Bolt")
        {
            if (!itemBeingDeleted) { SpawnBallistaBoltServerRPC(); }
        }
        else if(objectBeingHeld == "Engine Food")
        {
            if (!itemBeingDeleted) { SpawnEngineFoodServerRPC(); }
        }
        objectBeingHeld = "";
    }

    private void pickUpItem(string itemToPickUp, bool pickingUpFromGround, GameObject objToPickUp)
    {
        if(pickingUpFromGround)
        {
            deleteItemServerRPC(objToPickUp.GetComponent<NetworkObject>().NetworkObjectId);
        }
        if (itemToPickUp == "Ballista Bolt")
        {
            objectBeingHeld = "Ballista Bolt";
            PickUpBallistaBoltServerRPC();
        }
        else if(itemToPickUp == "Engine Food")
        {
            objectBeingHeld = "Engine Food";
            PickUpEngineFoodServerRPC();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void deleteItemServerRPC(ulong objToPickUp)
    {
        NetworkObject objToDestroy = GetNetworkObject(objToPickUp);
        objToDestroy.Despawn();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickUpBallistaBoltServerRPC()
    {
        PickUpBallistaBoltClientRPC();
    }

    [ClientRpc]
    private void PickUpBallistaBoltClientRPC()
    {
        craigHoldItemMesh.SetActive(true);
        craigBodyMesh.SetActive(false);
        craigClothesMesh.SetActive(false);
        ballistaBoltMesh.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickUpEngineFoodServerRPC()
    {
        PickUpEngineFoodClientRPC();
    }

    [ClientRpc]
    private void PickUpEngineFoodClientRPC()
    {
        craigHoldItemMesh.SetActive(true);
        craigBodyMesh.SetActive(false);
        craigClothesMesh.SetActive(false);
        engineFoodMesh.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DropItemServerRPC()
    {
        DropItemClientRPC();
    }

    [ClientRpc]
    private void DropItemClientRPC()
    {
        craigHoldItemMesh.SetActive(false);
        craigBodyMesh.SetActive(true);
        craigClothesMesh.SetActive(true);
        ballistaBoltMesh.SetActive(false);
        engineFoodMesh.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnBallistaBoltServerRPC()
    {
        var instance = Instantiate(ballistaBoltPrefab, dropPosition.transform.position, (dropPosition.transform.rotation * Quaternion.Euler(0, 90, 0)));
        var instanceNetworkOBJ = instance.GetComponent<NetworkObject>();
        instanceNetworkOBJ.Spawn(); 
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnEngineFoodServerRPC()
    {
        var instance = Instantiate(engineFoodPrefab, dropPosition.transform.position, dropPosition.transform.rotation);
        var instanceNetworkOBJ = instance.GetComponent<NetworkObject>();
        instanceNetworkOBJ.Spawn();
    }

}
