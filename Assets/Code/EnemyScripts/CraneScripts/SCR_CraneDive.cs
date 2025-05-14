using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SCR_CraneDive : MonoBehaviour
{
    [SerializeField]
    private float diveSpeed;

    [SerializeField]
    private float timeDiving;

    [SerializeField]
    private float diveDamage;

    [SerializeField]
    private SCR_CraneAnimation craneAnimation;

    private void Start()
    {
        craneAnimation = GetComponent<SCR_CraneAnimation>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ship")
        {
            Debug.Log("Diving");
            craneAnimation.DiveAnim();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Ship")
        {
            craneAnimation.StopDiving();
        }
    }
}
