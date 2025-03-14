using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SCR_Book : NetworkBehaviour
{
    [SerializeField] public NetworkVariable<bool> bookOpen = new NetworkVariable<bool>(false,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server);

    [SerializeField] GameObject crystal;

    // Start is called before the first frame update
    public void DisableCrystal()
    {
        crystal.SetActive(false);
    }

    public void EnableCrystal() 
    {
        crystal.SetActive(true);
    }
}
