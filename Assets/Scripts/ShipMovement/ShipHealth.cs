using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipHealth : MonoBehaviour
{
    [SerializeField]
    private float currentHealth;

    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private NetworkManager networkManager;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Destroy(collision.gameObject);
            currentHealth -= 1;
        }
    }

    private void Update()
    {
        //THIS NEEDS CHANGING TO WHEN THE SHIP DIES, NOT ONE PLAYER!!!
        if(currentHealth <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(1);

            //this works because of the OnSceneUnloaded() function in the network manager
            //it makes sure that everything is cleanly stopped
            if(NetworkManager.Singleton != null)
            {
                Destroy(NetworkManager.Singleton.gameObject);
            }
        }
    }
}
