using GLTFast.Schema;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class SCR_ShipControls : NetworkBehaviour
{

    public bool onWheel = false;
    private GameObject ship;
    private Rigidbody shipRb;

    [SerializeField] private float shipAcceleration;
    [SerializeField] private float shipTurnSpeed;
    private float rotationSpeed;
    private float shipCurrentSpeed;
    [SerializeField] float shipMaxSpeed;

    [SerializeField] public NetworkVariable<Vector3> shipPos;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ship == null)
        {
            ship = GameObject.Find("PRE-Airship");
        }
        updatePosServerRPC(ship.transform.right * shipAcceleration * Time.deltaTime);
        if (onWheel == true)
        {
            Debug.Log("straight wheelin");
            //Forward
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Debug.Log("Shiftin");
                shipAcceleration += 10;
            }
            //Backward
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                shipAcceleration -= 10;
            }
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
                updateRollRotServerRPC(shipTurnSpeed * Time.deltaTime);
            }
            //Roll Left
            if (Input.GetKey(KeyCode.Q))
            {
                updateRollRotServerRPC(-shipTurnSpeed * Time.deltaTime);
            }
            //Pitch up
            if (Input.GetKey(KeyCode.W))
            {
                updatePitchRotServerRPC(shipTurnSpeed * Time.deltaTime);
            }
            //Roll Left
            if (Input.GetKey(KeyCode.S))
            {
                updatePitchRotServerRPC(-shipTurnSpeed * Time.deltaTime);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void updatePosServerRPC(Vector3 newPos)
    {
        updatePosClientRPC(newPos);
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
    private void updatePitchRotServerRPC(float RotationSpeed)
    {
        Vector3 newRot = GameObject.Find("PRE-Airship").transform.rotation.eulerAngles + new Vector3(0, 0, RotationSpeed);
        GameObject.Find("PRE-Airship").transform.rotation = Quaternion.Euler(newRot);
        updateRotClientRPC(new Vector3(GameObject.Find("PRE-Airship").transform.rotation.x, GameObject.Find("PRE-Airship").transform.rotation.y, GameObject.Find("PRE-Airship").transform.rotation.z));

        updateRotClientRPC(newRot);
    }

    [ClientRpc]
    private void updatePosClientRPC(Vector3 newPos)
    {
        GameObject.Find("PRE-Airship").transform.position += newPos;
    }

    [ClientRpc]
    private void updateRotClientRPC(Vector3 newRot)
    {
        Debug.Log("Rotating client rpc");
        GameObject.Find("PRE-Airship").transform.rotation = Quaternion.Euler(newRot);
    }
}
