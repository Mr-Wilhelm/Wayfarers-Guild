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

    [SerializeField]
    private GravitasBody shipRb;

    [SerializeField] private float shipAcceleration;
    [SerializeField] private float shipTurnSpeed;
    private float rotationSpeed;
    private float shipCurrentSpeed;

    public float shipHealth = 10.0f;

    [SerializeField] float shipMaxSpeed;

    [SerializeField] public NetworkVariable<Vector3> shipPos;

    [SerializeField] private float pitchRollResetSpeed;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsOwner)
        {
            enabled = false; return;
        }
        if (ship == null)
        {
            ship = GameObject.Find("PRE-Airship");
            shipRb = ship.GetComponent<GravitasBody>();
        }
        Vector3 forceToAdd = (ship.transform.right * shipAcceleration);
        updatePosServerRPC(forceToAdd);
        Vector3 currentSpeed = shipRb.Velocity;
        if (currentSpeed.x >= shipMaxSpeed)
        {
            //Debug.Log("Capping speed");
            currentSpeed.x = shipMaxSpeed;  
            shipRb.Velocity = currentSpeed;
        }
        bool beep = false;
        if (onWheel == true)
        {
            //Yaw Right
            if (Input.GetKey(KeyCode.D))
            {
                updateYawRotServerRPC(shipTurnSpeed * Time.deltaTime);
            }
            //Yaw Left
            if (Input.GetKey(KeyCode.A))
            {
                updateYawRotServerRPC(-shipTurnSpeed * Time.deltaTime);
            }
            //Roll Right
            if(Input.GetKey(KeyCode.E))
            {
                beep = true;
                updateRollRotServerRPC(-shipTurnSpeed * Time.deltaTime);
            }
            //Roll Left
            if (Input.GetKey(KeyCode.Q))
            {
                beep = true;
                updateRollRotServerRPC(shipTurnSpeed * Time.deltaTime);
            }
            //Pitch up
            if (Input.GetKey(KeyCode.W))
            {
                beep = true;
                updatePitchRotServerRPC(shipTurnSpeed * Time.deltaTime);
            }
            //Roll Left
            if (Input.GetKey(KeyCode.S))
            {
                beep = true;
                updatePitchRotServerRPC(-shipTurnSpeed * Time.deltaTime);
            }
        }
        if (!beep)
        {
            // Roll Auto-level
            if (ship.transform.rotation.eulerAngles.x < 2 || ship.transform.rotation.eulerAngles.x > 358)
            {
                //Debug.Log("SweetSpotBabeeeeeey : " + ship.transform.rotation.eulerAngles.x);
            }
            else if (ship.transform.rotation.eulerAngles.x < 180)
            {
                updateRollRotServerRPC(-shipTurnSpeed * Time.deltaTime);
            }
            else if (ship.transform.rotation.eulerAngles.x > 180)
            {
                updateRollRotServerRPC(shipTurnSpeed * Time.deltaTime);
            }

            // Pitch Auto-level
            if (ship.transform.rotation.eulerAngles.z < 2 || ship.transform.rotation.eulerAngles.z > 358)
            {
                //Debug.Log("SweetSpotBabeeeeeey : " + ship.transform.rotation.eulerAngles.z);
            }
            else if (ship.transform.rotation.eulerAngles.z < 180)
            {
                updatePitchRotServerRPC(-shipTurnSpeed * Time.deltaTime);
            }
            else if (ship.transform.rotation.eulerAngles.z > 180)
            {
                updatePitchRotServerRPC(shipTurnSpeed * Time.deltaTime);
            }

        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void updatePosServerRPC(Vector3 forceToAdd)
    {
        updatePosClientRPC(forceToAdd);
    }

    [ServerRpc(RequireOwnership = false)]
    private void updateYawRotServerRPC(float RotationSpeed)
    {
        Vector3 torque = new Vector3(0, RotationSpeed, 0);
        updateRotClientRPC(torque);
    }

    [ServerRpc(RequireOwnership = false)]
    private void updateRollRotServerRPC(float RotationSpeed)
    {
        Vector3 torque = new Vector3(RotationSpeed, 0, 0);
        updateRotClientRPC(torque);
    }

    [ServerRpc(RequireOwnership = false)]
    private void updatePitchRotServerRPC(float RotationSpeed)
    {
        Vector3 torque = new Vector3(0,0, RotationSpeed);   
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
