using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SCR_FuseBox : NetworkBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject fuseStatusConsole;

    [SerializeField] private GameObject bridgeFuseBox;
    [SerializeField] private GameObject cargoHoldFuseBox;
    [SerializeField] private GameObject engineRoomFuseBox;

    public bool fuseBlown;

    [ServerRpc(RequireOwnership = false)]
    public void FixFuseServerRPC()
    {
        FixFuseClientRPC();
    }

    [ClientRpc(RequireOwnership = false)]
    public void FixFuseClientRPC()
    {
        if (fuseBlown)
        {
            Debug.Log("Repairing fuse");
            fuseBlown = false;
            if (!bridgeFuseBox.GetComponent<SCR_FuseBox>().fuseBlown && !cargoHoldFuseBox.GetComponent<SCR_FuseBox>().fuseBlown && !engineRoomFuseBox.GetComponent<SCR_FuseBox>().fuseBlown)
            {
                Debug.Log("Enabling lights");
                fuseStatusConsole.GetComponent<SCR_FuseManager>().EnableLights();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void BlowFuseServerRPC()
    {
        BlowFuseClientRPC();
        //fuseBlown = true;
        //DisableLights();
    }

    [ClientRpc(RequireOwnership = false)]
    private void BlowFuseClientRPC()
    {
        fuseBlown = true;
        DisableLights();
    }

    private void DisableLights()
    {
        SCR_FuseManager fuseManagerInstance = fuseStatusConsole.GetComponent<SCR_FuseManager>();
        fuseManagerInstance.DisableBridge();
        fuseManagerInstance.DisableCargoHold();
        fuseManagerInstance.DisableEngineRoom();
        Debug.Log("Disabling lights");
    }

}
