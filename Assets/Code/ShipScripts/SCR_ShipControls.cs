using GLTFast.Schema;
using Gravitas;
using JetBrains.Annotations;
using System;
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

    [SerializeField] private Animator wheelAnimator;
    private GameObject[] wheelPiecesToRotate;
    private GameObject[] wheelRimPieces;
    public GameObject shipWheel;
    private GameObject wheelCentrePost;

    [SerializeField] float maxInput = 1f;
    [SerializeField] float smoothTime = 5f;
    [SerializeField] float currentSteer = 0f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        LoadShip();
    }

    private void LoadShip()
    {
        shipWheel = GameObject.Find("ShipWheel");
        ship = GameObject.Find("PRE-Airship");
        shipMovement = ship.GetComponent<SCR_ShipMovement>();
    }


    private void Update()
    {
        if (ship == null) { LoadShip(); return; }
        if (shipMovement == null) { if (ship != null) { LoadShip(); } }
        if (!IsOwner) { return; }
        if (!onWheel) { return; }

        if (Input.GetKeyDown(KeyCode.LeftShift) && shipMovement.shipAcceleration.Value < shipMovement.shipAccelerationBound)
        {
            shipMovement.increaseAccelerationServerRPC();
            //Debug.Log("speed up: " + shipMovement.shipAcceleration.Value);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && shipMovement.shipAcceleration.Value > -shipMovement.shipAccelerationBound/2)
        {
            shipMovement.decreaseAccelerationServerRPC();
            //Debug.Log("speed down: " + shipMovement.shipAcceleration.Value);
        }

        //Yaw Right
        if (Input.GetKey(KeyCode.D) == true && !Input.GetKey(KeyCode.A))
        {
            shipMovement.updateYawRotServerRPC("Right", OwnerClientId);
            RotateWheelRightServerRPC();
            //RotateWheelServerRPC();
        }
        //Yaw Left
        if (Input.GetKey(KeyCode.A) == true && !Input.GetKey(KeyCode.D))
        {
            shipMovement.updateYawRotServerRPC("Left", OwnerClientId);
            RotateWheelLeftServerRPC();
            //RotateWheelServerRPC();
        }
        if(Input.GetKey(KeyCode.D) == false && Input.GetKey(KeyCode.A) == false) 
        {
            CentreWheelServerRPC();
        }
        if (Input.GetKey(KeyCode.E))
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
        //Roll Left -  && !(ship.transform.rotation.eulerAngles.x < 180 && ship.transform.rotation.eulerAngles.x > shipMovement.autoCorrectLimit)
        if (Input.GetKey(KeyCode.Q))
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
        //Pitch up -  && !(ship.transform.rotation.eulerAngles.z < 180 && ship.transform.rotation.eulerAngles.z > shipMovement.autoCorrectLimit)
        if (Input.GetKey(KeyCode.S))
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
        //Pitch Down -  && !(ship.transform.rotation.eulerAngles.z > 180 && ship.transform.rotation.eulerAngles.z < 360 - shipMovement.autoCorrectLimit)
        if (Input.GetKey(KeyCode.W))
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

    [ServerRpc(RequireOwnership = false)]
    private void RotateWheelRightServerRPC()
    {
        RotateWheelRightClientRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    private void CentreWheelServerRPC()
    {
        CentreWheelClientRPC();
    }

    [ClientRpc(RequireOwnership = false)]
    private void CentreWheelClientRPC()
    {
        Vector3 desiredRotation = new Vector3(shipWheel.transform.localEulerAngles.x, shipWheel.transform.localEulerAngles.y, 0);
        shipWheel.transform.localEulerAngles = Vector3.Lerp(shipWheel.transform.localEulerAngles, desiredRotation, Time.deltaTime);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RotateWheelLeftServerRPC()
    {
        RotateWheelLeftClientRPC();
    }

    [ClientRpc(RequireOwnership = false)]
    private void RotateWheelLeftClientRPC()
    {
        float rotationSpeed = 100f;
        float z = shipWheel.transform.localEulerAngles.z;
        //179
        if(z > 180) { z -= 360; }
        if(z >= 145)
        {
            shipWheel.transform.localEulerAngles = new Vector3(0, 90, 145);
            return;
        }
        shipWheel.transform.Rotate(0,0, rotationSpeed * Time.deltaTime);
    }

    [ClientRpc(RequireOwnership = false)]
    private void RotateWheelRightClientRPC()
    {
        float rotationSpeed = -100f;
        float z = shipWheel.transform.localEulerAngles.z;
        if (z > 180) { z -= 360; }
        if (z <= -145)
        {
            shipWheel.transform.localEulerAngles = new Vector3(0, 90, -145);
            return;
        }
        shipWheel.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180) angle -= 360;
        return angle;
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
