using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Other networking happening in:
///     -PlayerNetworkManager.cs
/// </summary>
public class NetworkLogic : NetworkBehaviour
{
    [SerializeField]
    public GameObject playerOne;

    [SerializeField]
    public GameObject playerTwo;

    private void DisableNonOwner()
    {

    }
}
