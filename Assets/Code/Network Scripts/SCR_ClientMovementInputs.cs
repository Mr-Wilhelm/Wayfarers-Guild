using Gravitas;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SCR_ClientMovementInputs : NetworkBehaviour
{
    Vector3 TESTVEL = Vector3.left;
    float clientTurnspeed = 100f;

    Vector2 keyInput;
    float verticalInput;
    Vector2 mouseInput;
    float angleX;


    public NetworkVariable<Vector2> ClientInputHorizontal = new NetworkVariable<Vector2>();
    public NetworkVariable<float> ClientInputVertical = new NetworkVariable<float>();
    public NetworkVariable<float> ClientInputMouseX = new NetworkVariable<float>();

    SCR_GravBridge ServerGravBridge;
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

                mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));


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

                //Transform t = this.transform; // Reference to either the player or the player's proxy transform
                //t.rotation *= Quaternion.AngleAxis(mouseInput.x * 1, Vector3.up);

                ////Adding all velocity Vectors together
                //velocity = velocityX + velocityY + velocityZ;
                SetCLientVelServerRpc(keyInput);
                SetClientInputVerticalServerRpc(verticalInput);
                SetClientInputMouseServerRpc(mouseInput.x);




                Camera playerCamera = GameObject.Find("Player_1").transform.Find("Camera").GetComponent<Camera>();

                angleX += -mouseInput.y * clientTurnspeed *Time.deltaTime;
                angleX = Mathf.Clamp(angleX, -90, 90);
                playerCamera.transform.localRotation = Quaternion.Euler(angleX, 0, 0);

                //Debug.Log(playerCamera.transform.localRotation + " Key 2");


            }

        }
    }

    private void Awake()
    {

        if (!IsServer)
        {
            //Capping FPS because uncapped FPS causes a visual jitter or teleportation when ship is moving, due to how the networking and gravitas are fighting
            Application.targetFrameRate = 60;
        }
        else if (IsServer)
        {
            Application.targetFrameRate = 120;
        }
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


    [Rpc(SendTo.Server)]
    void SetClientInputMouseServerRpc(float ClientMouseInput)
    {
        ClientInputMouseX.Value = ClientMouseInput;
    }









}
