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
        if (shipMovement == null) { if (ship != null) { shipMovement = ship.GetComponent<SCR_ShipMovement>(); } }
        if (!IsOwner){ return; }
        if (!onWheel) { return; }
        //Yaw Right

        Debug.Log("x: " + ship.transform.rotation.eulerAngles.x);
        Debug.Log("y: " + ship.transform.rotation.eulerAngles.y);
        Debug.Log("z: " + ship.transform.rotation.eulerAngles.z);

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
        if(Input.GetKey(KeyCode.E) && (ship.transform.rotation.eulerAngles.x <= shipMovement.autoCorrectLimit || ship.transform.rotation.eulerAngles.x >= 360 - shipMovement.autoCorrectLimit) )
        {
            shipMovement.updateRollRotServerRPC("Left", OwnerClientId);
        }
        //Roll Left
        if (Input.GetKey(KeyCode.Q) && (ship.transform.rotation.eulerAngles.x <= shipMovement.autoCorrectLimit || ship.transform.rotation.eulerAngles.x >= 360 - shipMovement.autoCorrectLimit))
        {
            shipMovement.updateRollRotServerRPC("Right", OwnerClientId);
        }
        //Pitch up
        if (Input.GetKey(KeyCode.W) && (ship.transform.rotation.eulerAngles.z <= shipMovement.autoCorrectLimit || ship.transform.rotation.eulerAngles.z >= 360 - shipMovement.autoCorrectLimit))
        {
            shipMovement.updatePitchRotServerRPC("Right", OwnerClientId);
        }
        //Roll Left
        if (Input.GetKey(KeyCode.S) && (ship.transform.rotation.eulerAngles.z <= shipMovement.autoCorrectLimit || ship.transform.rotation.eulerAngles.z >= 360 - shipMovement.autoCorrectLimit))
        {
            shipMovement.updatePitchRotServerRPC("Left", OwnerClientId);
        }
    }
}
