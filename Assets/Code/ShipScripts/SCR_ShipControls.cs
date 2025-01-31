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

    [SerializeField] private float rollResetThreshold = 5f;
    [SerializeField] private float pitchResetThreshold = 5f;

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
        //updatePosServerRPC(ship.transform.right * shipAcceleration * Time.deltaTime);
        Vector3 forceToAdd = (ship.transform.right * shipAcceleration);
        updatePosServerRPC(forceToAdd);
        //shipRb.AddForce(forceToAdd, ForceMode.Acceleration);
        Vector3 currentSpeed = shipRb.Velocity;
        if (currentSpeed.x >= shipMaxSpeed)
        {
            //Debug.Log("Capping speed");
            currentSpeed.x = shipMaxSpeed;  
            shipRb.Velocity = currentSpeed;
        }
        bool changingShipRot = false;
        if (onWheel == true)
        {
            //Yaw Right
            if (Input.GetKey(KeyCode.D))
            {
                updateYawRotServerRPC(shipTurnSpeed * Time.deltaTime);
                //Vector3 rotation = new Vector3(ship.transform.rotation.eulerAngles.x, ship.transform.rotation.eulerAngles.y + shipTurnSpeed * Time.deltaTime, ship.transform.rotation.eulerAngles.z);
                //updateRotServerRPC(rotation);
            }
            //Yaw Left
            if (Input.GetKey(KeyCode.A))
            {
                updateYawRotServerRPC(-shipTurnSpeed * Time.deltaTime);
                //Vector3 rotation = new Vector3(ship.transform.rotation.eulerAngles.x, ship.transform.rotation.eulerAngles.y - shipTurnSpeed * Time.deltaTime, ship.transform.rotation.eulerAngles.z);
                //updateRotServerRPC(rotation);
            }
            //Roll Right
            if(Input.GetKey(KeyCode.E))
            {
                changingShipRot = true;
                updateRollRotServerRPC(-shipTurnSpeed * Time.deltaTime);
            }
            //Roll Left
            if (Input.GetKey(KeyCode.Q))
            {
                changingShipRot = true;
                updateRollRotServerRPC(shipTurnSpeed * Time.deltaTime);
            }
            //Pitch up
            if (Input.GetKey(KeyCode.W))
            {
                changingShipRot = true;
                updatePitchRotServerRPC(shipTurnSpeed * Time.deltaTime);
            }
            //Roll Left
            if (Input.GetKey(KeyCode.S))
            {
                changingShipRot = true;
                updatePitchRotServerRPC(-shipTurnSpeed * Time.deltaTime);
            }     
        }
        if (!changingShipRot)
        {
            // Roll Auto-level
            if (ship.transform.rotation.eulerAngles.x < 2 || ship.transform.rotation.eulerAngles.x > 358)
            {
                //Debug.Log("SweetSpotBabeeeeeey : " + ship.transform.rotation.eulerAngles.x);
            }
            else if (ship.transform.rotation.eulerAngles.x < 180)
            {
                //Debug.Log(ship.transform.rotation.eulerAngles.x);
                resetRollRotServerRPC();
            }
            else if (ship.transform.rotation.eulerAngles.x > 180)
            {
                //Debug.Log(ship.transform.rotation.eulerAngles.x);
                resetRollRotServerRPC();
            }

            // Pitch Auto-level
            if (ship.transform.rotation.eulerAngles.z < 2 || ship.transform.rotation.eulerAngles.z > 358)
            {
                //Debug.Log("SweetSpotBabeeeeeey : " + ship.transform.rotation.eulerAngles.z);
            }
            else if (ship.transform.rotation.eulerAngles.z < 180)
            {

                //Debug.Log(ship.transform.rotation.eulerAngles.z);
                resetPitchRotServerRPC();
            }
            else if (ship.transform.rotation.eulerAngles.z > 180)
            {
                //Debug.Log(ship.transform.rotation.eulerAngles.z);
                resetPitchRotServerRPC();
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
        //Vector3 newRot = GameObject.Find("PRE-Airship").transform.rotation.eulerAngles + new Vector3(0, RotationSpeed, 0);
        //GameObject.Find("PRE-Airship").transform.rotation = Quaternion.Euler(newRot);
        //updateRotClientRPC(new Vector3(GameObject.Find("PRE-Airship").transform.rotation.x, GameObject.Find("PRE-Airship").transform.rotation.y, GameObject.Find("PRE-Airship").transform.rotation.z));

        updateRotClientRPC(torque);
        //updateRotClientRPC(newRot);
    }

    [ServerRpc(RequireOwnership = false)]
    private void updateRollRotServerRPC(float RotationSpeed)
    {
        Vector3 torque = new Vector3(RotationSpeed, 0, 0);
        //Vector3 newRot = GameObject.Find("PRE-Airship").transform.rotation.eulerAngles + new Vector3(RotationSpeed, 0, 0);
        //GameObject.Find("PRE-Airship").transform.rotation = Quaternion.Euler(newRot);
        //updateRotClientRPC(new Vector3(GameObject.Find("PRE-Airship").transform.rotation.x, GameObject.Find("PRE-Airship").transform.rotation.y, GameObject.Find("PRE-Airship").transform.rotation.z));

        updateRotClientRPC(torque);
        //updateRotClientRPC(newRot);
    }

    [ServerRpc(RequireOwnership = false)]
    private void updatePitchRotServerRPC(float RotationSpeed)
    {
        Vector3 torque = new Vector3(0,0, RotationSpeed);   
        updateRotClientRPC(torque);
    }

    [ServerRpc(RequireOwnership = false)]
    private void resetRollRotServerRPC()
    {
        Quaternion currentRotation = ship.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, currentRotation.eulerAngles.y, currentRotation.eulerAngles.z);
        Quaternion newRotation = Quaternion.Lerp(currentRotation, targetRotation, pitchRollResetSpeed * Time.deltaTime);

        ship.transform.rotation = newRotation;
        resetRotClientRPC(new Vector3(newRotation.x, newRotation.y, newRotation.z));
    }


    [ServerRpc(RequireOwnership = false)]
    private void resetPitchRotServerRPC()
    {
        Quaternion currentRotation =ship.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y, 0);
        Quaternion newRotation = Quaternion.Lerp(currentRotation, targetRotation, pitchRollResetSpeed * Time.deltaTime);

        ship.transform.rotation = newRotation;
        resetRotClientRPC(new Vector3(newRotation.x, newRotation.y, newRotation.z));
    }

    [ClientRpc]
    private void updatePosClientRPC(Vector3 forceToAdd)
    {
        //GameObject.Find("PRE-Airship").transform.position += newPos;
        shipRb.AddForce(forceToAdd, ForceMode.Acceleration);
    }

    [ClientRpc]
    private void updateRotClientRPC(Vector3 newRot)
    {
        shipRb.AddTorque(newRot, ForceMode.Acceleration);
        //GameObject.Find("PRE-Airship").transform.rotation = Quaternion.Euler(newRot);
    }

    [ClientRpc]
    private void resetRotClientRPC(Vector3 newRot)
    {
        ship.transform.rotation = Quaternion.Euler(newRot);
    }
}
