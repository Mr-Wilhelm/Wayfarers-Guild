using GLTFast.Schema;
using Gravitas;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

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
        bool beep = false;
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


            //if(!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            //{
            //    resetRollRotServerRPC();
            //}
            //if(!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            //{
            //    Debug.Log("No pitching");
            //    resetPitchRotServerRPC();
            //}
            if (!beep)
            {
                // Roll Auto-level
                if (ship.transform.rotation.eulerAngles.x < 2 || ship.transform.rotation.eulerAngles.x > 358)
                {
                    //Debug.Log("SweetSpotBabeeeeeey : " + ship.transform.rotation.eulerAngles.x);
                }
                else if (ship.transform.rotation.eulerAngles.x < 180)
                {

                    //Debug.Log(ship.transform.rotation.eulerAngles.x);
                    updateRollRotServerRPC(-shipTurnSpeed * Time.deltaTime);
                }
                else if (ship.transform.rotation.eulerAngles.x > 180)
                {
                    //Debug.Log(ship.transform.rotation.eulerAngles.x);
                    updateRollRotServerRPC(shipTurnSpeed * Time.deltaTime);
                }

                // Pitch Auto-level
                if (ship.transform.rotation.eulerAngles.z < 2 || ship.transform.rotation.eulerAngles.z > 358)
                {
                    //Debug.Log("SweetSpotBabeeeeeey : " + ship.transform.rotation.eulerAngles.z);
                }
                else if (ship.transform.rotation.eulerAngles.z < 180)
                {

                    //Debug.Log(ship.transform.rotation.eulerAngles.z);
                    updatePitchRotServerRPC(-shipTurnSpeed * Time.deltaTime);
                }
                else if (ship.transform.rotation.eulerAngles.z > 180)
                {
                    //Debug.Log(ship.transform.rotation.eulerAngles.z);
                    updatePitchRotServerRPC(shipTurnSpeed * Time.deltaTime);
                }

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
        Vector3 newRot = GameObject.Find("PRE-Airship").transform.rotation.eulerAngles + new Vector3(0, RotationSpeed, 0);
        GameObject.Find("PRE-Airship").transform.rotation = Quaternion.Euler(newRot);
        updateRotClientRPC(new Vector3(GameObject.Find("PRE-Airship").transform.rotation.x, GameObject.Find("PRE-Airship").transform.rotation.y, GameObject.Find("PRE-Airship").transform.rotation.z));

        updateRotClientRPC(newRot);
    }

    [ServerRpc(RequireOwnership = false)]
    private void updateRollRotServerRPC(float RotationSpeed)
    {
        Vector3 newRot = GameObject.Find("PRE-Airship").transform.rotation.eulerAngles + new Vector3(RotationSpeed, 0, 0);
        GameObject.Find("PRE-Airship").transform.rotation = Quaternion.Euler(newRot);
        updateRotClientRPC(new Vector3(GameObject.Find("PRE-Airship").transform.rotation.x, GameObject.Find("PRE-Airship").transform.rotation.y, GameObject.Find("PRE-Airship").transform.rotation.z));

        updateRotClientRPC(newRot);
    }

    [ServerRpc(RequireOwnership = false)]
    private void resetRollRotServerRPC()
    {
        Quaternion currentRotation = GameObject.Find("PRE-Airship").transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, currentRotation.eulerAngles.y, currentRotation.eulerAngles.z); // Reset roll to 0
        Quaternion newRotation = Quaternion.Lerp(currentRotation, targetRotation, pitchRollResetSpeed * Time.deltaTime);

        GameObject.Find("PRE-Airship").transform.rotation = newRotation;
        updateRotClientRPC(new Vector3(newRotation.x, newRotation.y, newRotation.z));
    }

    [ServerRpc(RequireOwnership = false)]
    private void updatePitchRotServerRPC(float RotationSpeed)
    {
        Vector3 newRot = GameObject.Find("PRE-Airship").transform.rotation.eulerAngles + new Vector3(0, 0, RotationSpeed);
        GameObject.Find("PRE-Airship").transform.rotation = Quaternion.Euler(newRot);
        updateRotClientRPC(new Vector3(GameObject.Find("PRE-Airship").transform.rotation.x, GameObject.Find("PRE-Airship").transform.rotation.y, GameObject.Find("PRE-Airship").transform.rotation.z));

        updateRotClientRPC(newRot);
    }

    [ServerRpc(RequireOwnership = false)]
    private void resetPitchRotServerRPC()
    {
        Quaternion currentRotation = GameObject.Find("PRE-Airship").transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y, 0); // Reset roll to 0
        Quaternion newRotation = Quaternion.Lerp(currentRotation, targetRotation, pitchRollResetSpeed * Time.deltaTime);

        GameObject.Find("PRE-Airship").transform.rotation = newRotation;
        updateRotClientRPC(new Vector3(newRotation.x, newRotation.y, newRotation.z));
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
        GameObject.Find("PRE-Airship").transform.rotation = Quaternion.Euler(newRot);
    }
}
