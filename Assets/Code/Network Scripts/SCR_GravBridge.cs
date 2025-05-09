using Gravitas.Demo;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class SCR_GravBridge : GravitasFirstPersonPlayerSubject
{
    private NetworkTransform _transform;
    private Vector3 _clientVel;

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
        if (_transform == null)
        {
            _transform = GetComponent<NetworkTransform>();
            return;
        }
        if (_transform.IsServer) 
        {

            SCR_ClientMovementInputs ClientInputValues = this.GetComponent<SCR_ClientMovementInputs>();
            Debug.Log("Louis Smellz: " + ClientInputValues.ClientInputHorizontal.Value);
            //Debug.Log(_clientVel + "PickedUP");
        
        }

        //if (_transform.IsServer)  server makes the cleint rotate to the hosts inputs
        if (_transform.IsOwner)
        {
            base.OnSubjectUpdate();
        }
        else
        {

           // replaceWithRPCSend(base.GetInputVelocity());
        }
    }



    protected override void OnSubjectFixedUpdate()
    {
        if (_transform != null && _transform.IsServer)
        {
            base.OnSubjectFixedUpdate();
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
            SCR_ClientMovementInputs ClientInputValue = this.GetComponent<SCR_ClientMovementInputs>();
            //return Scripty.ClientVelocity.Value;

            Transform t = gravitasBody.CurrentTransform;
            if (!playerOnWheel)
            {
                Vector3 velocity = Vector3.zero;

                //Left-Right movement
                float xForce = 10;
                Vector3 velocityX = ClientInputValue.ClientInputHorizontal.Value.x * xForce * t.right;

                // Up-Down movement
                float yForce = 10;
                Vector3 velocityY = ClientInputValue.ClientInputVertical.Value * yForce * t.up;

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
                Vector3 velocityZ = ClientInputValue.ClientInputHorizontal.Value.y * zForce * t.forward;


                //Adding all velocity Vectors together
                velocity = velocityX + velocityY + velocityZ;

                //if (keyInput != Vector2.zero)
                //{
                //    Walking = true;
                //    playerAnimator.SetBool("Walking", true);
                //}
                //else
                //{
                //    Walking = false;
                //    playerAnimator.SetBool("Walking", false);
                //}
                return velocity;



            }
            return Vector3.zero;

        }
    }

        //[Rpc(SendTo.Server)]
        //void replaceWithRPCSend(Vector3 myPos) 
        //{

        //}
    public void RecieveClientVelocity(Vector3 clientVel)
    {
        _clientVel = clientVel;
    }



}
