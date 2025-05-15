using Gravitas.Demo;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class SCR_BallistaLogic : NetworkBehaviour
{

    [SerializeField] public NetworkVariable<bool> ballistaLoaded = new NetworkVariable<bool>(false);
    [SerializeField] public NetworkVariable<bool> ballistaOccupied = new NetworkVariable<bool>(false);
    [SerializeField] public ulong currentPlayerOnBallistaID;
    [SerializeField] private LayerMask mothHitBoxLayer;

    [SerializeField] private float turnSpeed = 20.0f;
    [SerializeField] private float turnLimit = 180.0f;
    [SerializeField] private int BallistaRange = 1000;

    [SerializeField] private KeyCode reloadKey = KeyCode.R;
    [SerializeField] private KeyCode fireKey = KeyCode.Mouse0;

    public bool playerHasBolt;
    private Camera occupant = null;

    public GameObject ballista;
    public GameObject ballistaHousing;
    public GameObject ballistaFirePoint;
    public GameObject ballistaBolt;

    [Header("Audio source and clips")]
    public SCR_AudioHelper audioHelper;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        audioHelper = GameObject.Find("AudioHelperOBJ").GetComponent<SCR_AudioHelper>();
    }
    public void setOccupant(Camera playerCam)
    {
        occupant = playerCam;
        occupyServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    public void occupyServerRPC()
    {
        ballistaOccupied.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void leaveServerRPC()
    {
        occupant = null;
        ballistaOccupied.Value = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (occupant != null)
        {

            ballista.transform.LookAt(occupant.transform.position + (-occupant.transform.forward * 30));

            //ballista.transform.localEulerAngles =  new Vector3(ballista.transform.localEulerAngles.x, occupant.transform.parent.localEulerAngles.y+270, ballista.transform.localEulerAngles.z);
            if (ballista.transform.localEulerAngles.x < 180)
            {
                ballista.transform.localEulerAngles = new Vector3(0.5f, ballista.transform.localEulerAngles.y, 0);
            }
            else if (ballista.transform.localEulerAngles.x < 300)
            {
                ballista.transform.localEulerAngles = new Vector3(300, ballista.transform.localEulerAngles.y, 0);
            }

            ballistaHousing.transform.localEulerAngles = new Vector3(0, ballista.transform.localEulerAngles.y, 0);

            if (Input.GetKeyDown(reloadKey))
            {
                ReloadBallista();
            }

            if (Input.GetKeyDown(fireKey))
            {
                FireBallista();
            }

        }
    }

    private void ReloadBallista()
    {
        if (!playerHasBolt) { Debug.Log("Player does not have bolt"); return; }
        audioHelper.PlayAudioClipAcrossNetwork("ballistaReload");
        playerHasBolt = false;
        BallistaLoadServerRPC();
        ballistaBolt.SetActive(true);
        RemoveBoltFromPlayerServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    private void BallistaLoadServerRPC()
    {
        ballistaLoaded.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void BallistaUnLoadServerRPC()
    {
        ballistaLoaded.Value = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemoveBoltFromPlayerServerRPC()
    {
        GameObject playerOnBallista = FindNetworkObject(currentPlayerOnBallistaID);
        Debug.Log($"Remove ballista bolt from player: {playerOnBallista.gameObject.name}");

        SCR_NewInteract interactScriptRef = playerOnBallista.GetComponent<SCR_NewInteract>();
        playerOnBallista.GetComponent<GravitasFirstPersonPlayerSubject>().hasItem = false;
        interactScriptRef.objectBeingHeld = "";
        interactScriptRef.craigHoldItemMesh.SetActive(false);
        interactScriptRef.craigStandardMesh.SetActive(true);
        interactScriptRef.ballistaBoltMesh.SetActive(false);

        RemoveBoltFromPlayerClientRPC(new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { currentPlayerOnBallistaID }
            }
        });
    }

    [ClientRpc(RequireOwnership = false)]
    private void RemoveBoltFromPlayerClientRPC(ClientRpcParams clientRpcParams = default)
    {
        GameObject player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().gameObject;
        if (player != null)
        {
            SCR_NewInteract interactScriptRef = player.GetComponent<SCR_NewInteract>();
            if (interactScriptRef != null)
            {
                interactScriptRef.objectBeingHeld = "";
                interactScriptRef.craigHoldItemMesh.SetActive(false);
                interactScriptRef.craigStandardMesh.SetActive(true);
                interactScriptRef.ballistaBoltMesh.SetActive(false);
            }
            GravitasFirstPersonPlayerSubject playerSubject = player.GetComponent<GravitasFirstPersonPlayerSubject>();
            if (playerSubject != null)
            {
                playerSubject.hasItem = false;
            }
        }
    }

    private void FireBallista()
    {
        if(!ballistaLoaded.Value) { Debug.Log("Ballista not loaded"); return; }
        audioHelper.PlayAudioClipAcrossNetwork("ballistaFire");
        Debug.Log("Attempting to fire ballsita");
        ballistaBolt.SetActive(false);
        BallistaUnLoadServerRPC();
        RaycastHit[] hits = Physics.RaycastAll(ballistaFirePoint.transform.position, occupant.transform.forward, BallistaRange);
        Debug.Log("Hits length is: " + hits.Length);
        {
            foreach(var hit in hits)
            {
                Debug.Log("Hit objects layer is: " + LayerMask.LayerToName(hit.collider.gameObject.layer));

                //if ((mothHitBoxLayer & (1 << hit.collider.gameObject.layer)) != 0)
                //{
                //    Debug.Log("Hit: " + hit.collider.gameObject.name);
                //    ulong enemyID = hit.collider.gameObject.transform.root.GetComponent<NetworkObject>().NetworkObjectId;
                //    KillEnemyServerRPC(enemyID);
                //    Debug.Log("Hit enemy with ID: " + enemyID);
                //}

                NetworkObject hitObj = hit.collider.GetComponentInParent<NetworkObject>();

                if(hitObj != null)
                {
                    if (hitObj.gameObject.tag == "Huberto")
                    {
                        hitObj.GetComponent<SCR_HubertoLogic>().takeDamageServerRPC();
                    }
                    else
                    {
                        KillEnemyServerRPC(hitObj.NetworkObjectId);
                    }
                }
                else
                {
                    Debug.Log("Nothing Hit");
                }
            }
        }
    }

    private GameObject FindNetworkObject(ulong idOfNetworkObj)
    {
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(idOfNetworkObj, out NetworkObject networkOBJ))
        {
            return networkOBJ.gameObject;
        }
        return null;
    }

    [ServerRpc(RequireOwnership = false)]
    private void KillEnemyServerRPC(ulong enemyToDestryID)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(enemyToDestryID, out NetworkObject networkOBJ))
        {
            networkOBJ.Despawn();
        }
        else
        {
            Debug.Log("Nothing FOund");
        }
    }

}
