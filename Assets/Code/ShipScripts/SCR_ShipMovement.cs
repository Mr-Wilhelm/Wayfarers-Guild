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
    [SerializeField] private GameObject ship;
    [SerializeField] private GravitasBody shipRb;

    [SerializeField] private float shipAcceleration;
    [SerializeField] private float shipTurnSpeed;

    [SerializeField] float shipMaxSpeed;

    [SerializeField] public NetworkVariable<Vector3> shipPos;

    [SerializeField] private float pitchRollResetSpeed;
    private float rotationSpeed;

    [SerializeField] private float autoCorrectLimit;


    private Vector3 rotation;

    private NetworkVariable<ulong> controllingPlayer = new NetworkVariable<ulong>(ulong.MaxValue, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!IsServer) return;

        rotation = Vector3.zero;

        Vector3 forceToAdd = (gameObject.transform.right * shipAcceleration);
        shipRb.AddForce(forceToAdd, ForceMode.Acceleration);
        //updatePosServerRPC(gameObject.transform.position);
        Vector3 currentSpeed = shipRb.Velocity;
        if (currentSpeed.x >= shipMaxSpeed)
        {
            //Debug.Log("Capping speed");
            currentSpeed.x = shipMaxSpeed;  
            shipRb.Velocity = currentSpeed;
        }

        

        //AutoLevel();
    }

    private void LateUpdate()
    {
        if (!IsServer) return;

        Vector3 currentEuler = transform.eulerAngles;
        Vector3 targetEuler = currentEuler;

        // Auto-level pitch if no pitch input
        if (Mathf.Abs(rotation.x) < autoCorrectLimit)
            targetEuler.x = Mathf.LerpAngle(currentEuler.x, 0, Time.fixedDeltaTime * (shipTurnSpeed / 2));

        // Auto-level roll if no roll input
        if (Mathf.Abs(rotation.z) < autoCorrectLimit)
            targetEuler.z = Mathf.LerpAngle(currentEuler.z, 0, Time.fixedDeltaTime * (shipTurnSpeed / 2));

        Quaternion targetRotation = Quaternion.Euler(targetEuler);
        shipRb.MoveRotation(Quaternion.Slerp(shipRb.rotation(), targetRotation, Time.fixedDeltaTime * (shipTurnSpeed / 2)));
    }


    [ServerRpc(RequireOwnership = false)]
    public void SetControllingPlayerServerRPC(ulong playerID)
    {
        controllingPlayer.Value = playerID;
    }    

    [ServerRpc(RequireOwnership = false)]
    private void updatePosServerRPC(Vector3 forceToAdd)
    {
        updatePosClientRPC(forceToAdd);
    }

    [ServerRpc(RequireOwnership = false)]
    public void updateYawRotServerRPC(string LeftOrRight, ulong senderID)
    {
        //if (senderID != ulong.MaxValue && controllingPlayer.Value != senderID && controllingPlayer.Value != ulong.MaxValue) return;

        if(LeftOrRight == "Left") { rotationSpeed = (-shipTurnSpeed * Time.deltaTime); }
        else { rotationSpeed = shipTurnSpeed * Time.deltaTime; }
        Vector3 torque = new Vector3(0, rotationSpeed, 0);
        updateRotClientRPC(torque);
    }

    [ServerRpc(RequireOwnership = false)]
    public void updateRollRotServerRPC(string LeftOrRight, ulong senderID, float multiplier = 1)
    {
        //if (senderID != ulong.MaxValue && controllingPlayer.Value != senderID && controllingPlayer.Value != ulong.MaxValue) return;

        if (LeftOrRight == "Left") { rotationSpeed = (-shipTurnSpeed * multiplier * Time.deltaTime); }
        else { rotationSpeed = shipTurnSpeed * Time.deltaTime; }
        Vector3 torque = new Vector3(rotationSpeed, 0, 0);
        updateRotClientRPC(torque);
    }

    [ServerRpc(RequireOwnership = false)]
    public void updatePitchRotServerRPC(string LeftOrRight, ulong senderID, float multiplier = 1)
    {
        //if (senderID != ulong.MaxValue && controllingPlayer.Value != senderID && controllingPlayer.Value != ulong.MaxValue) return;

        if (LeftOrRight == "Left") { rotationSpeed = (-shipTurnSpeed * multiplier * Time.deltaTime); }
        else { rotationSpeed = shipTurnSpeed * Time.deltaTime; }
        Vector3 torque = new Vector3(0,0, rotationSpeed);   
        updateRotClientRPC(torque);
    }

    [ClientRpc]
    private void updatePosClientRPC(Vector3 newPos)
    {
        //ship.transform.position = newPos;
    }

    [ClientRpc]
    private void updateRotClientRPC(Vector3 newRot)
    {
        rotation += newRot;
        shipRb.AddTorque(newRot, ForceMode.Acceleration);
    }

    private void AutoLevel()
    {

        AutoLevelUsingServer();

        //ulong playerID = controllingPlayer.Value;
        
        //if(playerID == ulong.MaxValue)
        //{
        //    AutoLevelUsingServer();
        //}
        //else
        //{
        //    AutoLevelWithPlayer(playerID);
        //}
    }

    private void AutoLevelWithPlayer(ulong playerID)
    {
        // Roll Auto-level
        if (ship.transform.rotation.eulerAngles.x < 2 || ship.transform.rotation.eulerAngles.x > 358)
        {
            //Debug.Log("SweetSpotBabeeeeeey : " + ship.transform.rotation.eulerAngles.x);
        }
        else if (ship.transform.rotation.eulerAngles.x < 180)
        {
            updateRollRotServerRPC("Left", controllingPlayer.Value);
        }
        else if (ship.transform.rotation.eulerAngles.x > 180)
        {
            updateRollRotServerRPC("Right", controllingPlayer.Value);
        }
        // Pitch Auto-level
        if (ship.transform.rotation.eulerAngles.z < 2 || ship.transform.rotation.eulerAngles.z > 358)
        {
            //Debug.Log("SweetSpotBabeeeeeey : " + ship.transform.rotation.eulerAngles.z);
        }
        else if (ship.transform.rotation.eulerAngles.z < 180)
        {
            updatePitchRotServerRPC("Left", controllingPlayer.Value);
        }
        else if (ship.transform.rotation.eulerAngles.z > 180)
        {
            updatePitchRotServerRPC("Right", controllingPlayer.Value);
        }
    }

    private void AutoLevelUsingServer()
    {
        // Roll Auto-level
        if (ship.transform.rotation.eulerAngles.x < autoCorrectLimit || ship.transform.rotation.eulerAngles.x > 360 - autoCorrectLimit)
        {
            //Debug.Log("SweetSpotBabeeeeeey : " + ship.transform.rotation.eulerAngles.x);
        }
        else if (ship.transform.rotation.eulerAngles.x < 180)
        {
            //updateRollRotServerRPC("Left", ulong.MaxValue, 0.5f);
        }
        else if (ship.transform.rotation.eulerAngles.x > 180)
        {
            //updateRollRotServerRPC("Right", ulong.MaxValue, 0.5f);
        }
        // Pitch Auto-level
        if (ship.transform.rotation.eulerAngles.z < autoCorrectLimit || ship.transform.rotation.eulerAngles.z > 360 - autoCorrectLimit)
        {
            //Debug.Log("SweetSpotBabeeeeeey : " + ship.transform.rotation.eulerAngles.z);
        }
        else if (ship.transform.rotation.eulerAngles.z < 180)
        {
            //updatePitchRotServerRPC("Left", ulong.MaxValue, 0.5f);
        }
        else if (ship.transform.rotation.eulerAngles.z > 180)
        {
            //updatePitchRotServerRPC("Right", ulong.MaxValue, 0.5f);
        }
    }

    private void AutoLevelEdward()
    {
        
    }

}
