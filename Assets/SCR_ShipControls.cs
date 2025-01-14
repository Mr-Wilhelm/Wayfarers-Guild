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
        if (onWheel == true)
        {
            if (ship == null)
            {
                ship = GameObject.Find("PRE-Airship");
            }
            if (Input.GetKey(KeyCode.W))
            {
                updatePosServerRPC(ship.transform.right * shipAcceleration * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.S))
            {
                updatePosServerRPC(-(ship.transform.right) * shipAcceleration * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                updateRotServerRPC(shipTurnSpeed * Time.deltaTime);
                //Vector3 rotation = new Vector3(ship.transform.rotation.eulerAngles.x, ship.transform.rotation.eulerAngles.y + shipTurnSpeed * Time.deltaTime, ship.transform.rotation.eulerAngles.z);
                //updateRotServerRPC(rotation);
            }
            if (Input.GetKey(KeyCode.A))
            {
                updateRotServerRPC(-shipTurnSpeed * Time.deltaTime);
                //Vector3 rotation = new Vector3(ship.transform.rotation.eulerAngles.x, ship.transform.rotation.eulerAngles.y - shipTurnSpeed * Time.deltaTime, ship.transform.rotation.eulerAngles.z);
                //updateRotServerRPC(rotation);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void updatePosServerRPC(Vector3 newPos)
    {
        updatePosClientRPC(newPos);
    }

    //Updates the owner's rot and sends it to the network
    [ServerRpc(RequireOwnership = false)]
    private void updateRotServerRPC(float RotationSpeed)
    {
        Debug.Log("Rotating server rpc");

        Vector3 newRot = GameObject.Find("PRE-Airship").transform.rotation.eulerAngles + new Vector3(0, RotationSpeed, 0);
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
