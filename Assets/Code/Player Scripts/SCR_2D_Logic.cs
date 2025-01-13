using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SCR_2D_Logic : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }


    [ServerRpc]
    public void PassAlongQuestIndexServerRpc(int questIndex)
    {
        GameObject.Find("MainMenuCanvas").GetComponent<SCR_MenuManager>().ChangeQuestIndexServerRpc(questIndex);

    }
}
