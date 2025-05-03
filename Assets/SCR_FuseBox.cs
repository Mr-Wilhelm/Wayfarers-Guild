using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_FuseBox : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject fuseStatusConsole;

    [SerializeField] private GameObject bridgeFuseBox;
    [SerializeField] private GameObject cargoHoldFuseBox;
    [SerializeField] private GameObject engineRoomFuseBox;

    public bool fuseBlown;

    public void FixFuse()
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

    public void BlowFuse()
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
