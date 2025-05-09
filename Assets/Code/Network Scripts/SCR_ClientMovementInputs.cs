using Gravitas;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class SCR_ClientMovementInputs : NetworkBehaviour
{
    Vector3 TESTVEL = Vector3.left;
    float moveSpeed = 1f;
    float jumpForce = 10f;
    Vector2 keyInput;
    float verticalInput;


    public NetworkVariable<Vector2> ClientInputHorizontal = new NetworkVariable<Vector2>();
    public NetworkVariable<float> ClientInputVertical = new NetworkVariable<float>();

    SCR_GravBridge ServerGravBridge;
    bool foundNetworking = false;
    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
        {
            if (IsOwner)
            {

                //Transform t = this.transform;

                //// Vertical input
                //if (Input.GetKey(KeyCode.Space))
                //    verticalInput = 1; // Up
                //else
                //    verticalInput = 0; // None

                keyInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

                // Vertical input
                if (Input.GetKey(KeyCode.Space))
                    verticalInput = 1; // Up
                else
                    verticalInput = 0; // None

                //Vector3 velocity = Vector3.zero;

                ////Left-Right movement
                //float xForce = moveSpeed;
                //Vector3 velocityX = keyInput.x * xForce * t.right;

                //// Up-Down movement
                //float yForce = jumpForce;
                //Vector3 velocityY = verticalInput * yForce * t.up;

                ////Checks to sort of floating point numbers issue
                //if (velocityY.y > 0f)
                //{
                //    if (velocityY.x + velocityY.z <= 0.1f)
                //    {
                //        velocityY.x = 0f;
                //        velocityY.z = 0f;
                //    }
                //}

                ////Forward-Back movement
                //float zForce = moveSpeed;
                //Vector3 velocityZ = keyInput.y * zForce * t.forward;


                ////Adding all velocity Vectors together
                //velocity = velocityX + velocityY + velocityZ;
                SetCLientVelServerRpc(keyInput);
                SetClientInputVerticalServerRpc(verticalInput);
            }

        }
    }

    private void Awake()
    {
        foundNetworking = true;
    }

    [Rpc(SendTo.Server)]
    void SetCLientVelServerRpc(Vector3 newClientVel)
    {
        ClientInputHorizontal.Value = newClientVel;
    }

    [Rpc(SendTo.Server)]
    void SetClientInputVerticalServerRpc(float ClientVertInput)
    {
        ClientInputVertical.Value = ClientVertInput;
    }










}
