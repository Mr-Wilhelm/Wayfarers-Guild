using GLTFast.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_ShipControls : MonoBehaviour
{

    public bool onWheel = false;
    private GameObject ship;
    private Rigidbody shipRb;

    [SerializeField] private float shipAcceleration;
    private float shipCurrentSpeed;
    [SerializeField] float shipMaxSpeed;

    // Start is called before the first frame update
    void Start()
    {
        ship = GameObject.Find("PRE-Airship");
        Debug.Log(ship.name);
        shipRb = ship.GetComponent<Rigidbody>();
        Debug.Log(shipRb.gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        if(onWheel == true)
        {
            if(Input.GetKey(KeyCode.W))
            {
                GameObject.Find("PRE-Airship").transform.position += GameObject.Find("PRE-Airship").transform.right * shipAcceleration * Time.deltaTime;
                /*GameObject.Find("PRE-Airship").GetComponent<Rigidbody>().AddForce(GameObject.Find("PRE-Airship").transform.forward * shipAcceleration, ForceMode.Acceleration);*/
            }
        }
        else
        {
            Debug.Log("Not controlling wheel");
        }
        Debug.Log(GameObject.Find("PRE-Airship").transform.position);
    }
}
