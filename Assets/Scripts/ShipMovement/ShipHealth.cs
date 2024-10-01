using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShipHealth : NetworkBehaviour
{
    private int currentHealth;

    [SerializeField] Image healthBar;


    [SerializeField]
    private int maxHealth;

    [SerializeField]
    private NetworkManager networkManager;

    [SerializeField]
    private SceneManagerScript sceneManager;

    [SerializeField]
    private int damageAmount = 1;


    private void Start()
    {
        currentHealth = maxHealth;
        sceneManager = Object.FindFirstObjectByType<SceneManagerScript>();
        healthBar = GameObject.Find("Canvas/Health").GetComponent<Image>();
        healthBar.fillAmount = currentHealth / 100f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Destroy(collision.gameObject);
            TakeDamage(damageAmount);
        }
    }

    private void TakeDamage(int damageAmount)
    {
        ChangeShipHealthRpc();
    }

    private void Health(int healingAmount)
    {
        currentHealth += healingAmount;
        healthBar.fillAmount = currentHealth / 100f;
    }

    private void Update()
    {
        //THIS NEEDS CHANGING TO WHEN THE SHIP DIES, NOT ONE PLAYER!!!
        if(currentHealth <= 0)
        {
            sceneManager.LoadDefeatScene();

            //this works because of the OnSceneUnloaded() function in the network manager
            //it makes sure that everything is cleanly stopped
            if(NetworkManager.Singleton != null)
            {
                Destroy(NetworkManager.Singleton.gameObject);
            }
        }
    }

    //Executes the same function across all versions of this script (aka across players)
    [Rpc(SendTo.ClientsAndHost)]
    void ChangeShipHealthRpc()
    {
        Debug.Log("Receiving message");
        currentHealth -= damageAmount;
        healthBar.fillAmount = currentHealth / 100f;
    }
}
