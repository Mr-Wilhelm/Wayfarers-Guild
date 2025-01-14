using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class SCR_Portal : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ship")
        {
            NetworkManager.Singleton.SceneManager.LoadScene("CityMenu", LoadSceneMode.Single);
        }
    }
}
