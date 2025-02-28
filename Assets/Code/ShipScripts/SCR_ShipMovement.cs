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

    //[SerializeField] public float shipMaxSpeed;
    //[SerializeField] public float shipAcceleration;
    //[SerializeField] public float shipTurnSpeed;

    [SerializeField] public NetworkVariable<float> shipMaxSpeed;
    [SerializeField] public NetworkVariable<float> shipAcceleration;
    [SerializeField] public NetworkVariable<float> shipTurnSpeed;

    public float shipHealth = 10.0f;

    [SerializeField] public float boostedShipAcceleration;
    [SerializeField] public float boostedShipMaxSpeed;
    [SerializeField] public float boostedShipTurnSpeed;

    [SerializeField] public float unBoostedShipAcceleration;
    [SerializeField] public float unBoostedShipMaxSpeed;
    [SerializeField] public float unBoostedShipTurnSpeed;

    [SerializeField] private float boostDuaration = 30.0f;

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

        Vector3 forceToAdd = (gameObject.transform.right * shipAcceleration.Value);
        shipRb.AddForce(forceToAdd, ForceMode.Acceleration);
        //updatePosServerRPC(gameObject.transform.position);
        Vector3 currentSpeed = shipRb.Velocity;
        if (currentSpeed.x >= shipMaxSpeed.Value)
        {
            //Debug.Log("Capping speed");
            currentSpeed.x = shipMaxSpeed.Value;  
            shipRb.Velocity = currentSpeed;
        }

        

        //AutoLevel();
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

        if(LeftOrRight == "Left") { rotationSpeed = (-shipTurnSpeed.Value * Time.deltaTime); }
        else { rotationSpeed = shipTurnSpeed.Value * Time.deltaTime; }
        Vector3 torque = new Vector3(0, rotationSpeed, 0);
        updateRotClientRPC(torque);
    }

    [ServerRpc(RequireOwnership = false)]
    public void updateRollRotServerRPC(string LeftOrRight, ulong senderID, float multiplier = 1)
    {
        //if (senderID != ulong.MaxValue && controllingPlayer.Value != senderID && controllingPlayer.Value != ulong.MaxValue) return;

        if (LeftOrRight == "Left") { rotationSpeed = (-shipTurnSpeed.Value * Time.deltaTime); }
        else { rotationSpeed = shipTurnSpeed.Value * Time.deltaTime; }
        Vector3 torque = new Vector3(rotationSpeed, 0, 0);
        updateRotClientRPC(torque);
    }

    [ServerRpc(RequireOwnership = false)]
    public void updatePitchRotServerRPC(string LeftOrRight, ulong senderID, float multiplier = 1)
    {
        //if (senderID != ulong.MaxValue && controllingPlayer.Value != senderID && controllingPlayer.Value != ulong.MaxValue) return;

        if (LeftOrRight == "Left") { rotationSpeed = (-shipTurnSpeed.Value * Time.deltaTime); }
        else { rotationSpeed = shipTurnSpeed.Value * Time.deltaTime; }
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
        shipRb.AddRelativeTorque(newRot, ForceMode.Acceleration);
    }

    //private void AutoLevel()
    //{
    //    ulong playerID = controllingPlayer.Value;

    //    if(playerID == ulong.MaxValue)
    //    {
    //        AutoLevelUsingServer();
    //    }
    //    else
    //    {
    //        AutoLevelWithPlayer(playerID);
    //    }
    //}

    //private void AutoLevelWithPlayer(ulong playerID)
    //{
    //    // Roll Auto-level
    //    if (ship.transform.rotation.eulerAngles.x < 2 || ship.transform.rotation.eulerAngles.x > 358)
    //    {
    //        //Debug.Log("SweetSpotBabeeeeeey : " + ship.transform.rotation.eulerAngles.x);
    //    }
    //    else if (ship.transform.rotation.eulerAngles.x < 180)
    //    {
    //        updateRollRotServerRPC("Left", controllingPlayer.Value);
    //    }
    //    else if (ship.transform.rotation.eulerAngles.x > 180)
    //    {
    //        updateRollRotServerRPC("Right", controllingPlayer.Value);
    //    }
    //    // Pitch Auto-level
    //    if (ship.transform.rotation.eulerAngles.z < 2 || ship.transform.rotation.eulerAngles.z > 358)
    //    {
    //        //Debug.Log("SweetSpotBabeeeeeey : " + ship.transform.rotation.eulerAngles.z);
    //    }
    //    else if (ship.transform.rotation.eulerAngles.z < 180)
    //    {
    //        updatePitchRotServerRPC("Left", controllingPlayer.Value);
    //    }
    //    else if (ship.transform.rotation.eulerAngles.z > 180)
    //    {
    //        updatePitchRotServerRPC("Right", controllingPlayer.Value);
    //    }
    //}

    private void AutoLevelUsingServer()
    {
        // Roll Auto-level
        if (ship.transform.rotation.eulerAngles.x < 2 || ship.transform.rotation.eulerAngles.x > 358)
        {
            //Debug.Log("SweetSpotBabeeeeeey : " + ship.transform.rotation.eulerAngles.x);
        }
        else if (ship.transform.rotation.eulerAngles.x < 180)
        {
            //updateRollRotServerRPC("Left", ulong.MaxValue);
        }
        else if (ship.transform.rotation.eulerAngles.x > 180)
        {
            //updateRollRotServerRPC("Right", ulong.MaxValue);
        }
        // Pitch Auto-level
        if (ship.transform.rotation.eulerAngles.z < 2 || ship.transform.rotation.eulerAngles.z > 358)
        {
            //Debug.Log("SweetSpotBabeeeeeey : " + ship.transform.rotation.eulerAngles.z);
        }
        else if (ship.transform.rotation.eulerAngles.z < 180)
        {
            //updatePitchRotServerRPC("Left", ulong.MaxValue);
        }
        else if (ship.transform.rotation.eulerAngles.z > 180)
        {
            //updatePitchRotServerRPC("Right", ulong.MaxValue);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void boostSpeedServerRPC()
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
        boostSpeed();
    }

    private void boostSpeed()
    {
        Debug.Log("Boosting speed");
        shipAcceleration.Value = boostedShipAcceleration;
        shipMaxSpeed.Value = boostedShipMaxSpeed;
        shipTurnSpeed.Value = boostedShipTurnSpeed;
        CancelInvoke(nameof(unBoostSpeed));
        Invoke(nameof(unBoostSpeed), boostDuaration);
    }

    public void unBoostSpeed()
    {
        Debug.Log("UnBoosting speed");
        shipAcceleration.Value = unBoostedShipAcceleration;
        shipMaxSpeed.Value = unBoostedShipMaxSpeed;
        shipTurnSpeed.Value = unBoostedShipTurnSpeed;
    }

    private void AutoLevelEdward()
    {
        
    }

}
