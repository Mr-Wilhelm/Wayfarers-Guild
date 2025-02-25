using GLTFast.Schema;
using Gravitas;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class SCR_ShipControls : NetworkBehaviour
{
    public bool onWheel = false;

    [SerializeField]
    public GameObject ship;
    public SCR_ShipMovement shipMovement;

    private void Start()
    {
        Invoke("LoadShip", 1);
    }

    private void LoadShip()
    {
        ship = GameObject.Find("PRE-Airship");
        shipMovement = ship.GetComponent<SCR_ShipMovement>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ship == null) { ship = GameObject.Find("PRE-Airship"); return; }
        if (shipMovement == null) { shipMovement = ship.GetComponent<SCR_ShipMovement>(); return; }
        if (!IsOwner){ return; }
        if (!onWheel) { return; }
        //Yaw Right
        if (Input.GetKey(KeyCode.D))
        {
            shipMovement.updateYawRotServerRPC("Right", OwnerClientId);
        }
        //Yaw Left
        if (Input.GetKey(KeyCode.A))
        {
            shipMovement.updateYawRotServerRPC("Left", OwnerClientId);
        }
        //Roll Right
        if(Input.GetKey(KeyCode.E))
        {
            shipMovement.updateRollRotServerRPC("Left", OwnerClientId);
        }
        //Roll Left
        if (Input.GetKey(KeyCode.Q))
        {
            shipMovement.updateRollRotServerRPC("Right", OwnerClientId);
        }
        //Pitch up
        if (Input.GetKey(KeyCode.W))
        {
            shipMovement.updatePitchRotServerRPC("Right", OwnerClientId);
        }
        //Roll Left
        if (Input.GetKey(KeyCode.S))
        {
            shipMovement.updatePitchRotServerRPC("Left", OwnerClientId);
        }
    }
}
