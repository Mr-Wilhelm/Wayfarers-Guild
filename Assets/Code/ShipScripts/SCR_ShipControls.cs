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
        if(Input.GetKey(KeyCode.E) && !(ship.transform.rotation.eulerAngles.x > 180 && ship.transform.rotation.eulerAngles.x < 360 - shipMovement.autoCorrectLimit) )
        {

            if (ship.transform.rotation.eulerAngles.x < 180)
            {
                shipMovement.updateRollRotServerRPC("Left", OwnerClientId, 2f);
            }
            else
            {
                shipMovement.updateRollRotServerRPC("Left", OwnerClientId);
            }

            
            shipMovement.autoLevelRollActive = false;
            CancelInvoke(nameof(startAutoLevelRoll));
            Invoke(nameof(startAutoLevelRoll), 0.5f);
        }
        //Roll Left
        if (Input.GetKey(KeyCode.Q) && !(ship.transform.rotation.eulerAngles.x < 180 && ship.transform.rotation.eulerAngles.x > shipMovement.autoCorrectLimit))
        {
            if (ship.transform.rotation.eulerAngles.x > 180)
            {
                shipMovement.updateRollRotServerRPC("Right", OwnerClientId, 2f);
            }
            else
            {
                shipMovement.updateRollRotServerRPC("Right", OwnerClientId);
            }
                
            shipMovement.autoLevelRollActive = false;
            CancelInvoke(nameof(startAutoLevelRoll));
            Invoke(nameof(startAutoLevelRoll), 0.5f);
        }
        //Pitch up
        if (Input.GetKey(KeyCode.S) && !(ship.transform.rotation.eulerAngles.z < 180 && ship.transform.rotation.eulerAngles.z > shipMovement.autoCorrectLimit))
        {
            if (ship.transform.rotation.eulerAngles.z > 180)
            {
                shipMovement.updatePitchRotServerRPC("Right", OwnerClientId, 2f);
            }
            else
            {
                shipMovement.updatePitchRotServerRPC("Right", OwnerClientId);
            }
                
            shipMovement.autoLevelPitchActive = false;
            CancelInvoke(nameof(startAutoLevelPitch));
            Invoke(nameof(startAutoLevelPitch), 0.5f);
        }
        //Pitch Down
        if (Input.GetKey(KeyCode.W) && !(ship.transform.rotation.eulerAngles.z > 180 && ship.transform.rotation.eulerAngles.z < 360 - shipMovement.autoCorrectLimit))
        {
            if (ship.transform.rotation.eulerAngles.z < 180)
            {
                shipMovement.updatePitchRotServerRPC("Left", OwnerClientId, 2f);
            }
            else
            {
                shipMovement.updatePitchRotServerRPC("Left", OwnerClientId);
            }
                
            shipMovement.autoLevelPitchActive = false;
            CancelInvoke(nameof(startAutoLevelPitch));
            Invoke(nameof(startAutoLevelPitch), 0.5f);
        }
    }

    private void startAutoLevelRoll()
    {
        shipMovement.autoLevelRollActive = true;
    }
    private void startAutoLevelPitch()
    {
        shipMovement.autoLevelPitchActive = true;
    }
}
