using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;

public class ShipStatManager : MonoBehaviour
{
    public ShipHealth shipHealthScript;

    public float shipHealth;

    public float shipHealthMax;

    public float shipRepairCost;

    public float shipRepairMultiplier;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        shipHealthScript = GameObject.Find("TEMPShip").GetComponent<ShipHealth>();
    }

    private void Update()
    {
        if( shipHealthScript != null )
        {
            shipHealthMax = shipHealthScript.maxHealth;
            shipRepairMultiplier = 2.0f;

            shipHealth = shipHealthScript.currentHealthNetworked.Value;
            shipRepairCost = Mathf.Abs(shipHealthMax - shipHealth) * shipRepairMultiplier;
        }
    }
}
