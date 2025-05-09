using Gravitas.Demo;
using System.Collections;
using System.Collections.Generic;
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
            // TODO sync from client
            return _clientVel;
        }
    }

    //void replaceWithRPCSend(Vector3 myPos);

    //void replaceWithRPCRecv(Vector3 theirVel)
    //{
    //    _clientVel = theirVel; 
    //}

}
