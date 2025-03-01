using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SCR_BallistaLogic : NetworkBehaviour
{

    [SerializeField] public NetworkVariable<bool> ballistaLoaded = new NetworkVariable<bool>(false);
    [SerializeField] public NetworkVariable<bool> ballistaOccupied = new NetworkVariable<bool>(false);

    [SerializeField] private float turnSpeed = 20.0f;
    [SerializeField] private float turnLimit = 180.0f;

    private Camera occupant = null;

    public GameObject ballista;
    public GameObject ballistaHousing;

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
        }
    }
}
