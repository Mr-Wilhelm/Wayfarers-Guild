using Gravitas.Demo;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class SCR_GravBridge : GravitasFirstPersonPlayerSubject
{
    private NetworkTransform _transform;
    private Vector3 _clientVel;

    public Camera PlayerCamera => playerCamera;
    [SerializeField] private Camera playerCamera; // The camera used by the player, typically a child of the player


    SCR_ClientMovementInputs ClientInputValues;
    SCR_NewInteract clientOnWheelStatus;

    private float angleX; // Stored camera pitch value
    private float turnSpeed = 5f;
    private float clientTurnSpeed = 100f;


    private void Update()
    {
        base.Update();
        if (_transform.IsServer)
        {
            this.transform.position = gravitasBody.CurrentTransform.position;
        }
    }
    protected override void OnSubjectUpdate()
    {
        //Since Awake and start happen before all the network stuff, we need to slap this in update until we have a better solution
        if (_transform == null)
        {
            _transform = GetComponent<NetworkTransform>();
            return;
        }
        if(ClientInputValues == null)
        {
            ClientInputValues = this.GetComponent<SCR_ClientMovementInputs>();
        }
        if(clientOnWheelStatus == null)
        {
            clientOnWheelStatus = this.GetComponent<SCR_NewInteract>();
        }

        //if (_transform.IsServer)  server makes the cleint rotate to the hosts inputs
        if (_transform.IsOwner)
        {
            base.OnSubjectUpdate();
        }
        else
        {
            float mouseInput = ClientInputValues.ClientInputMouseX.Value;
            angleX = mouseInput;

            //This is where Horizontal Player rotation is handled,   Vertical Player rotation is handled locally in the script thats in ClientInputValues due to conflicts with networking and player controller packages
            Transform t = gravitasBody.CurrentTransform; // Reference to either the player or the player's proxy transform

            mouseInput = angleX;

            t.rotation *= Quaternion.AngleAxis(mouseInput * clientTurnSpeed * Time.deltaTime, Vector3.up);

        }
    }



    protected override Vector3 GetInputVelocity()
    {
        if (_transform == null)
        {
            return Vector3.zero;
        }

        if (_transform.IsOwner)
        {
            return base.GetInputVelocity();
        }
        else
        {
            //// TODO sync from client
            //SCR_ClientMovementInputs ClientInputValue = this.GetComponent<SCR_ClientMovementInputs>();
            //SCR_NewInteract clientOnWheelStatus = this.GetComponent<SCR_NewInteract>();
            //return Scripty.ClientVelocity.Value;

            Transform t = gravitasBody.CurrentTransform;
            //Debug.Log(clientOnWheelStatus.clientOnWheel.Value + " Key 3");
            if (!clientOnWheelStatus.clientOnWheel.Value)
            {
                Vector3 velocity = Vector3.zero;

                //Left-Right movement
                float xForce = 10;
                Vector3 velocityX = ClientInputValues.ClientInputHorizontal.Value.x * xForce * t.right;

                // Up-Down movement
                float yForce = 10;
                Vector3 velocityY = ClientInputValues.ClientInputVertical.Value * yForce * t.up;

                //Checks to sort of floating point numbers issue
                if (velocityY.y > 0f)
                {
                    if (velocityY.x + velocityY.z <= 0.1f)
                    {
                        velocityY.x = 0f;
                        velocityY.z = 0f;
                    }
                }

                //Forward-Back movement
                float zForce = 10;
                Vector3 velocityZ = ClientInputValues.ClientInputHorizontal.Value.y * zForce * t.forward;


                //Adding all velocity Vectors together
                velocity = velocityX + velocityY + velocityZ;

                if (ClientInputValues.ClientInputHorizontal.Value != Vector2.zero)
                {
                    Walking = true;
                    playerAnimator.SetBool("Walking", true);
                }
                else
                {
                    Walking = false;
                    playerAnimator.SetBool("Walking", false);
                }
                return velocity;



            }
            return Vector3.zero;

        }
    }

}
