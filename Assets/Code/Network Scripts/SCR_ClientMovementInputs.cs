using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class SCR_ClientMovementInputs : NetworkBehaviour
{
    Vector3 TESTVEL = Vector3.forward;

    [SerializeField] Vector3 ClientVelocity;
    SCR_GravBridge ServerGravBridge;
    bool foundNetworking = false;
    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            if (ServerGravBridge==null)
            {
                ServerGravBridge = this.GetComponent<SCR_GravBridge>();
            }
            else
            {

                //if (ClientVelocity == null) { ClientVelocity = Vector3.zero; }
     
                //ServerGravBridge.RecieveClientVelocity(ClientVelocity);
            }
        }
        if (!IsServer)
        {
            ClientDataHandOverRpc(TESTVEL);
            Debug.Log(TESTVEL);
        }
        


        //SCR_GravBridge.replaceWithRpc(TESTVEL);
    }


    [Rpc(SendTo.Server)]
    void ClientDataHandOverRpc(Vector3 clientVel)
    {
        RecieveDataHandOverServerRpc(clientVel);
    }

    [ServerRpc]
    void RecieveDataHandOverServerRpc(Vector3 clientVel)
    {
        ClientVelocity = clientVel;
        Debug.Log("Recieved on Server");
        ServerGravBridge.RecieveClientVelocity(ClientVelocity);
    }





    


}
