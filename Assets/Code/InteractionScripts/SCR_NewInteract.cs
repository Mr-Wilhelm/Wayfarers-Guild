using Gravitas;
using Gravitas.Demo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class SCR_NewInteract : NetworkBehaviour
{
    public bool interacting = false;

    [SerializeField] private float interactionRange;
    [SerializeField] public NetworkVariable<bool> canInteract;
    [SerializeField]
    private NetworkVariable<bool> bookOpen = new NetworkVariable<bool>(
    false,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server
);
    bool otherPlayerCanInteract = false;

    public Camera playerCam;
    private GameObject ship;

    public GameUIScript gameUI;

    [SerializeField] private KeyCode InteractKey = KeyCode.F;
    [SerializeField] private KeyCode DropKey = KeyCode.G;

    [SerializeField] private LayerMask Wheel;
    [SerializeField] private LayerMask BallistaBolt;
    [SerializeField] private LayerMask EngineFuel;
    [SerializeField] private LayerMask PickUp;
    [SerializeField] private LayerMask ActualPickUp;
    [SerializeField] private GravitasFirstPersonPlayerSubject playerScriptReference;

    [SerializeField] public GameObject craigStandardMesh;
    [SerializeField] public GameObject craigHoldItemMesh;
    [SerializeField] public GameObject craigHoldOneHandMesh;
    [SerializeField] private GameObject engineFoodMesh;
    [SerializeField] public GameObject ballistaBoltMesh;
    [SerializeField] public GameObject fuseMesh;
    public string objectBeingHeld = string.Empty;

    [SerializeField] private GameObject ballistaBoltPrefab;
    [SerializeField] private GameObject engineFoodPrefab;
    [SerializeField] private GameObject fusePrefab;
    [SerializeField] private GameObject dropPosition;

    private bool inBallista = false;

    public bool hasHadFuelBefore = false;
    public bool hasHadAmmoBefore = false;
    public bool hasUsedWheelBefore;
    public bool hasUsedBallistaBefore = false;

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
        if (!IsOwner) { enabled = false; return; }

        gameUI = GameObject.Find("MainUICanvas").GetComponent<GameUIScript>();

        Ray lookAtRay = new Ray(playerCam.transform.position, playerCam.transform.forward);


        if (Physics.Raycast(lookAtRay, out RaycastHit lookInfo, interactionRange, PickUp))
        {
            string lookAtTag = lookInfo.collider.gameObject.tag;

            gameUI.lookingAtWheel = false;
            gameUI.lookingAtHatch = false;
            gameUI.lookingAtBallistaStorage = false;
            gameUI.lookingAtEngine = false;
            gameUI.lookingAtFuelStorage = false;
            gameUI.lookingAtDroppedBallista = false;
            gameUI.lookingAtDroppedFuel = false;
            gameUI.lookingAtCompendium = false;
            gameUI.lookingAtFuseBox = false;
            gameUI.lookingAtFuseShelf = false;

            switch (lookAtTag)
            {
                case ("Wheel"):
                    gameUI.lookingAtWheel = true;
                    break;
                case ("BallistaHatch"):
                    gameUI.lookingAtHatch = true;
                    break;
                case ("Ballista Storage"):
                    gameUI.lookingAtBallistaStorage = true;
                    break;
                case ("Engine"):
                    gameUI.lookingAtEngine = true;
                    break;
                case ("Fuel Storage"):
                    gameUI.lookingAtFuelStorage = true;
                    break;
                case ("Ballista Bolt"):
                    gameUI.lookingAtDroppedBallista = true;
                    break;
                case ("Engine Fuel"):
                    gameUI.lookingAtDroppedFuel = true;
                    break;
                case ("Compendium"):
                    gameUI.lookingAtCompendium = true;
                    break;
                case ("FuseBox"):
                    gameUI.lookingAtFuseBox = true;
                    break;
                case ("Fuse Storage"):
                    gameUI.lookingAtFuseShelf = true;
                    break;
            }
        }
        else
        {
            gameUI.lookingAtWheel = false;
            gameUI.lookingAtHatch = false;
            gameUI.lookingAtBallistaStorage = false;
            gameUI.lookingAtEngine = false;
            gameUI.lookingAtFuelStorage = false;
            gameUI.lookingAtDroppedBallista = false;
            gameUI.lookingAtDroppedFuel = false;
            gameUI.lookingAtCompendium = false;
            gameUI.lookingAtFuseBox = false;
            gameUI.lookingAtFuseShelf = false;
        }

        if (Input.GetKeyDown(InteractKey))
        {

            if (GameObject.FindGameObjectsWithTag("Player").Length != 1)
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
            if (inBallista)
            {
                GameObject ballistaHatch = GameObject.FindGameObjectWithTag("BallistaHatch");
                Debug.Log("Interact with ballista");

                GameObject.FindGameObjectWithTag("Ballista").GetComponent<SCR_BallistaLogic>().leaveServerRPC();

                GetComponent<GravitasBody>().unLockPosition();

                inBallista = false;
                playerScriptReference.playerOnBallista = false;

                UpdateCanInteractBoolServerRpc(true);

                interacting = false;

                if (playerScriptReference.hasItem)
                {
                    if (objectBeingHeld == "Engine Food")
                    {
                        Debug.Log("Give back food");
                        engineFoodMesh.SetActive(true);
                    }
                    else if (objectBeingHeld == "Ballista Bolt")
                    {
                        Debug.Log("Give back ballista bolt");
                        ballistaBoltMesh.SetActive(true);
                    }
                }
                gameUI.HideBallistaControls();
                gameUI.HideControlsPrompt();

            }
            else if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hitInfo, interactionRange, PickUp))
            {
                if (interacting)
                {
                    playerScriptReference.playerOnWheel = false;
                    UpdateCanInteractBoolServerRpc(true);
                    interacting = false;
                    gameObject.GetComponent<SCR_ShipControls>().onWheel = false;
                    gameUI.HideControlsPrompt();
                }
                else if (hitInfo.collider.gameObject.CompareTag("Wheel") && !interacting && otherPlayerCanInteract && !playerScriptReference.hasItem)
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
                    if (!hasUsedWheelBefore)
                    {
                        StartCoroutine(gameUI.ShowWheelControls());
                        gameUI.HideBallistaControls();
                        gameUI.HideAmmoPrompt();
                        gameUI.HideFuelPrompt();
                    }
                    else if (hasUsedWheelBefore)
                    {
                        gameUI.ShowControlsPrompt();
                        gameUI.HideBallistaControls();
                        gameUI.HideAmmoPrompt();
                        gameUI.HideFuelPrompt();
                    }
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
                        if (!hasHadAmmoBefore)
                        {
                            StartCoroutine(gameUI.ShowAmmoPrompt());
                            gameUI.HideWheelControls();
                            gameUI.HideBallistaControls();
                            gameUI.HideFuelPrompt();
                        }
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
                else if (hitInfo.collider.gameObject.CompareTag("BallistaHatch"))
                {
                    GameObject ballista = GameObject.FindGameObjectWithTag("Ballista");
                    Debug.Log("Interact with ballista hatch");
                    if (!ballista.GetComponent<SCR_BallistaLogic>().ballistaOccupied.Value)
                    {
                        GetComponent<GravitasBody>().lockPosition(ballista);
                        if (objectBeingHeld == "Ballista Bolt")
                        {
                            Debug.Log("Entering with bolt, YIPEEEEEEEEEE");
                            ballista.GetComponent<SCR_BallistaLogic>().playerHasBolt = true;
                        }
                        else
                        {
                            Debug.Log("Entering without bolt, SADDDDDDDDDD");
                        }
                        ballistaBoltMesh.SetActive(false);
                        engineFoodMesh.SetActive(false);
                        ballista.GetComponent<SCR_BallistaLogic>().setOccupant(playerCam);
                        ballista.GetComponent<SCR_BallistaLogic>().currentPlayerOnBallistaID = gameObject.GetComponent<NetworkObject>().NetworkObjectId;
                        inBallista = true;
                        playerScriptReference.playerOnBallista = true;
                        interacting = true;
                        if (!hasUsedBallistaBefore)
                        {
                            StartCoroutine(gameUI.ShowBallistaControls());
                            gameUI.HideWheelControls();
                            gameUI.HideAmmoPrompt();
                            gameUI.HideFuelPrompt();
                        }
                        else if (hasUsedBallistaBefore)
                        {
                            gameUI.ShowControlsPrompt();
                            gameUI.HideWheelControls();
                            gameUI.HideAmmoPrompt();
                            gameUI.HideFuelPrompt();
                        }
                    }
                }
                else if (hitInfo.collider.gameObject.CompareTag("Fuel Storage"))
                {
                    if (playerScriptReference.hasItem == false)
                    {
                        pickUpItem("Engine Food", false, null);
                        playerScriptReference.hasItem = true;
                        if (!hasHadFuelBefore)
                        {
                            StartCoroutine(gameUI.ShowFuelPrompt());
                            gameUI.HideBallistaControls();
                            gameUI.HideWheelControls();
                            gameUI.HideAmmoPrompt();
                        }
                    }
                }
                else if (hitInfo.collider.gameObject.CompareTag("Ballista Bolt"))
                {
                    Debug.Log("Found bolt on ground");
                    pickUpItem("Ballista Bolt", true, hitInfo.collider.transform.root.gameObject);
                    playerScriptReference.hasItem = true;
                }
                else if (hitInfo.collider.gameObject.CompareTag("Engine Fuel"))
                {
                    Debug.Log("Found scran on ground");
                    pickUpItem("Engine Food", true, hitInfo.collider.transform.root.gameObject);
                    playerScriptReference.hasItem = true;
                }
                else if (hitInfo.collider.gameObject.CompareTag("Compendium"))
                {
                    Debug.Log("Interacting with compendium");
                    Debug.Log($"bookOpen network var: {hitInfo.collider.gameObject.GetComponent<SCR_Book>().bookOpen.Value} ");
                    if (hitInfo.collider.gameObject.GetComponent<SCR_Book>().bookOpen.Value == false)
                    {

                        if (hitInfo.collider.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Closing")) { Debug.Log("Book closing, please wait"); return; }

                        OpenBookGoBetweenServerRPC();
                    }
                    else
                    {

                        if (hitInfo.collider.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Opening")) { Debug.Log("Book opening, please wait"); return; }

                        CloseBookGoBetweenServerRPC();
                    }
                }
                else if (hitInfo.collider.gameObject.CompareTag("FuseBox"))
                {
                    SCR_FuseBox fuseBoxRef = hitInfo.collider.gameObject.GetComponent<SCR_FuseBox>();
                    if(hitInfo.collider.gameObject.GetComponent<SCR_FuseBox>().fuseBlown)
                    {
                        if (objectBeingHeld == "Fuse")
                        {
                            Debug.Log("Fixing fuse");
                            fuseBoxRef.FixFuseServerRPC();
                            dropItem(true);
                        }
                        else { Debug.Log("Need fuse to replace this blown one"); }
                    }
                    else
                    {
                        Debug.Log("Fuse is fine");
                    }
                }
                else if (hitInfo.collider.gameObject.CompareTag("Fuse Storage"))
                {
                    Debug.Log("Trying to grab fuse");
                    pickUpItem("Fuse", false, null);
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
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inBallista && interacting)
            {
                GameObject ballistaHatch = GameObject.FindGameObjectWithTag("BallistaHatch");
                Debug.Log("Interact with ballista");

                GameObject.FindGameObjectWithTag("Ballista").GetComponent<SCR_BallistaLogic>().leaveServerRPC();

                GetComponent<GravitasBody>().unLockPosition();

                inBallista = false;
                playerScriptReference.playerOnBallista = false;

                UpdateCanInteractBoolServerRpc(true);

                interacting = false;

                if (playerScriptReference.hasItem)
                {
                    if (objectBeingHeld == "Engine Food")
                    {
                        Debug.Log("Give back food");
                        engineFoodMesh.SetActive(true);
                    }
                    else if (objectBeingHeld == "Ballista Bolt")
                    {
                        Debug.Log("Give back ballista bolt");
                        ballistaBoltMesh.SetActive(true);
                    }
                    else if (objectBeingHeld == "Fuse")
                    {
                        Debug.Log("Give back fuse");
                        fuseMesh.SetActive(true);
                    }
                }
            }
            else if (gameObject.GetComponent<SCR_ShipControls>().onWheel = true && interacting)
            {
                gameObject.GetComponent<GravitasFirstPersonPlayerSubject>().playerOnWheel = false;
                UpdateCanInteractBoolServerRpc(true);
                interacting = false;
                gameObject.GetComponent<SCR_ShipControls>().onWheel = false;
                gameUI.HideControlsPrompt();
            }
            else if (gameUI.showCompendium == true)
            {
                CloseBookGoBetweenServerRPC();
                StartCoroutine(gameUI.HideCompendium());
            }
        }
        else if (Input.GetKeyDown(KeyCode.C) && interacting)
        {
            if(inBallista)
            {
                gameUI.ToggleBallistaControlsOn();
                gameUI.HideControlsPrompt();
            }
            else if(gameObject.GetComponent<GravitasFirstPersonPlayerSubject>().playerOnWheel)
            {
                gameUI.ToggleWheelControlsOn();
                gameUI.HideControlsPrompt();
            }
        }
        else if (Input.GetKeyUp(KeyCode.C) && interacting)
        {
            Debug.Log("RELEASE!");
            gameUI.ToggleBallistaControlsOff();
            gameUI.ToggleWheelControlsOff();
            gameUI.ShowControlsPrompt();
        }


        if (Input.GetKeyDown(DropKey))
        {
            if (playerScriptReference.hasItem == false || inBallista) { Debug.Log("Cannot drop"); }
            else
            {
                dropItem();
            }
        }

        if(Input.GetKeyDown(KeyCode.Z))
        {
            SCR_FuseBox fuseBoxInstance = GameObject.Find("BridgeFuseBox").GetComponent<SCR_FuseBox>();
            fuseBoxInstance.BlowFuseServerRPC();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void OpenBookGoBetweenServerRPC()
    {
        GameObject bookRef = GameObject.FindGameObjectWithTag("Compendium");
        bookRef.GetComponent<SCR_Book>().bookOpen.Value = true;
        OpenBookClientRPC();
        gameUI.showCompendium = true;
    }


    [ServerRpc(RequireOwnership = false)]
    private void CloseBookGoBetweenServerRPC()
    {
        GameObject bookRef = GameObject.FindGameObjectWithTag("Compendium");
        bookRef.GetComponent<SCR_Book>().bookOpen.Value = false;
        CloseBookClientRPC();
        gameUI.showCompendium = false;
    }

    [ClientRpc(RequireOwnership = false)]
    private void OpenBookClientRPC()
    {
        Debug.Log($"Player: {gameObject.GetComponent<NetworkObject>().NetworkObjectId} trying to open book");
        GameObject bookRef = GameObject.FindGameObjectWithTag("Compendium");
        bookRef.GetComponent<Animator>().SetTrigger("OpenTrigger");
        gameUI.showCompendium = true;
    }

    [ClientRpc(RequireOwnership = false)]
    private void CloseBookClientRPC()
    {
        Debug.Log($"Player: {gameObject.GetComponent<NetworkObject>().NetworkObjectId} trying to close book");
        GameObject bookRef = GameObject.FindGameObjectWithTag("Compendium");
        bookRef.GetComponent<Animator>().SetTrigger("CloseTrigger");
        gameUI.showCompendium = false;
    }

    [ServerRpc(RequireOwnership = false)]
    void SetPlayerPositionServerRPC(Vector3 newPosition)
    {
        transform.position = newPosition; // Update position on server
        UpdatePositionOnClientsClientRPC(newPosition); // Update position on all clients
    }

    // This function synchronizes the position to all clients
    [ClientRpc(RequireOwnership = false)]
    void UpdatePositionOnClientsClientRPC(Vector3 updatedPosition)
    {
        transform.position = updatedPosition; // Update position on clients
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateCanInteractBoolServerRpc(bool newValue) 
    {
        canInteract.Value = newValue;
        gameUI.HideWheelControls();
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
        if(objectBeingHeld == "Fuse")
        {
            if(!itemBeingDeleted) { SpawnFuseServerRPC(); }
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
        else if(itemToPickUp == "Fuse")
        {
            objectBeingHeld = "Fuse";
            PickUpFuseServerRPC();
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
        craigStandardMesh.SetActive(false);
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
        craigStandardMesh.SetActive(false);
        engineFoodMesh.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickUpFuseServerRPC()
    {
        PickUpFuseClientRPC();
    }

    [ClientRpc]
    private void PickUpFuseClientRPC()
    {
        craigHoldOneHandMesh.SetActive(true);
        craigStandardMesh.SetActive(false);
        fuseMesh.SetActive(true);
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
        craigHoldOneHandMesh.SetActive(false);
        craigStandardMesh.SetActive(true);
        ballistaBoltMesh.SetActive(false);
        engineFoodMesh.SetActive(false);
        fuseMesh.SetActive(false);
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

    [ServerRpc(RequireOwnership = false)]
    private void SpawnFuseServerRPC()
    {
        var instance = Instantiate(fusePrefab, dropPosition.transform.position, dropPosition.transform.rotation);
        var instanceNetworkOBJ = instance.GetComponent<NetworkObject>();
        instanceNetworkOBJ.Spawn();
    }

}
