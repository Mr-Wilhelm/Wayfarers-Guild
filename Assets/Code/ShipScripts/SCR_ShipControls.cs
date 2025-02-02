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
    private GameObject ship;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ship == null)
        {
            ship = GameObject.Find("PRE-Airship");
        }
        if (!IsOwner)
        {
            enabled = false; return;
        }
        if (onWheel == true)
        {
            //Yaw Right
            if (Input.GetKey(KeyCode.D))
            {
                ship.GetComponent<SCR_ShipMovement>().updateYawRotServerRPC("Right");
            }
            //Yaw Left
            if (Input.GetKey(KeyCode.A))
            {
                ship.GetComponent<SCR_ShipMovement>().updateYawRotServerRPC("Left");
            }
            //Roll Right
            if(Input.GetKey(KeyCode.E))
            {
                ship.GetComponent<SCR_ShipMovement>().updateRollRotServerRPC("Left");
            }
            //Roll Left
            if (Input.GetKey(KeyCode.Q))
            {
                ship.GetComponent<SCR_ShipMovement>().updateRollRotServerRPC("Right");
            }
            //Pitch up
            if (Input.GetKey(KeyCode.W))
            {
                ship.GetComponent<SCR_ShipMovement>().updatePitchRotServerRPC("Right");
            }
            //Roll Left
            if (Input.GetKey(KeyCode.S))
            {
                ship.GetComponent<SCR_ShipMovement>().updatePitchRotServerRPC("Left");
            }
        }
    }
}
