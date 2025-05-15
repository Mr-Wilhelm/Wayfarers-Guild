using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class SCR_Portal : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 90.0f;

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ship")
        {
            GameObject.Find("PRE-Airship").GetComponent<NetworkObject>().Despawn();
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                player.GetComponent<SCR_ShipControls>().enabled = false;
            }
            GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SCR_SceneManagerScript>().LoadCityScene();
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
    }
}
