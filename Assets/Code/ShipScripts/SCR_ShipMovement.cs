using GLTFast.Schema;
using Gravitas;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class SCR_ShipMovement : NetworkBehaviour
{
    public bool onWheel = false;

    [SerializeField]
    private GameObject ship;

    [SerializeField]
    private GravitasBody shipRb;

    [SerializeField] private float shipAcceleration;
    [SerializeField] private float shipTurnSpeed;

    public float shipHealth = 10.0f;

    [SerializeField] float shipMaxSpeed;

    [SerializeField] public NetworkVariable<Vector3> shipPos;

    [SerializeField] private float pitchRollResetSpeed;
    private float rotationSpeed;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 forceToAdd = (gameObject.transform.right * shipAcceleration);
        updatePosServerRPC(forceToAdd);
        Vector3 currentSpeed = shipRb.Velocity;
        if (currentSpeed.x >= shipMaxSpeed)
        {
            //Debug.Log("Capping speed");
            currentSpeed.x = shipMaxSpeed;  
            shipRb.Velocity = currentSpeed;
        }
        // Roll Auto-level
        if (ship.transform.rotation.eulerAngles.x < 2 || ship.transform.rotation.eulerAngles.x > 358)
        {
            //Debug.Log("SweetSpotBabeeeeeey : " + ship.transform.rotation.eulerAngles.x);
        }
        else if (ship.transform.rotation.eulerAngles.x < 180)
        {
            updateRollRotServerRPC("Left");
        }
        else if (ship.transform.rotation.eulerAngles.x > 180)
        {
            updateRollRotServerRPC("Right");
        }
        // Pitch Auto-level
        if (ship.transform.rotation.eulerAngles.z < 2 || ship.transform.rotation.eulerAngles.z > 358)
        {
            //Debug.Log("SweetSpotBabeeeeeey : " + ship.transform.rotation.eulerAngles.z);
        }
        else if (ship.transform.rotation.eulerAngles.z < 180)
        {
            updatePitchRotServerRPC("Left");
        }
        else if (ship.transform.rotation.eulerAngles.z > 180)
        {
            updatePitchRotServerRPC("Right");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void updatePosServerRPC(Vector3 forceToAdd)
    {
        updatePosClientRPC(forceToAdd);
    }

    [ServerRpc(RequireOwnership = false)]
    public void updateYawRotServerRPC(string LeftOrRight)
    {
        if(LeftOrRight == "Left") { rotationSpeed = (-shipTurnSpeed * Time.deltaTime); }
        else { rotationSpeed = shipTurnSpeed * Time.deltaTime; }
        Vector3 torque = new Vector3(0, rotationSpeed, 0);
        updateRotClientRPC(torque);
    }

    [ServerRpc(RequireOwnership = false)]
    public void updateRollRotServerRPC(string LeftOrRight)
    {
        if (LeftOrRight == "Left") { rotationSpeed = (-shipTurnSpeed * Time.deltaTime); }
        else { rotationSpeed = shipTurnSpeed * Time.deltaTime; }
        Vector3 torque = new Vector3(rotationSpeed, 0, 0);
        updateRotClientRPC(torque);
    }

    [ServerRpc(RequireOwnership = false)]
    public void updatePitchRotServerRPC(string LeftOrRight)
    {
        if (LeftOrRight == "Left") { rotationSpeed = (-shipTurnSpeed * Time.deltaTime); }
        else { rotationSpeed = shipTurnSpeed * Time.deltaTime; }
        Vector3 torque = new Vector3(0,0, rotationSpeed);   
        updateRotClientRPC(torque);
    }

    [ClientRpc]
    private void updatePosClientRPC(Vector3 forceToAdd)
    {
        shipRb.AddForce(forceToAdd, ForceMode.Acceleration);
    }

    [ClientRpc]
    private void updateRotClientRPC(Vector3 newRot)
    {
        shipRb.AddTorque(newRot, ForceMode.Acceleration);
    }
}
