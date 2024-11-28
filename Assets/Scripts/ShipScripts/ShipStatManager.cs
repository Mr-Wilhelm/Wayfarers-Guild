using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Runtime.CompilerServices;

public class ShipStatManager : MonoBehaviour
{
    public ShipHealth shipHealthScript;

    public float shipHealth;

    public float shipHealthMax;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        shipHealthScript = GameObject.Find("TEMPShip").GetComponent<ShipHealth>();
        shipHealthMax = shipHealthScript.maxHealth;
    }

    private void Update()
    {
        shipHealth = shipHealthScript.currentHealthNetworked.Value;
    }
}
