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

    public NetworkVariable<int> currentHealthNetworked = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField] Image healthBar;

    [SerializeField]
    private int maxHealth = 100;

    [SerializeField]
    private NetworkManager networkManager;

    [SerializeField]
    private SceneManagerScript sceneManager;

    [SerializeField]
    private int damageAmount = 1;


    private void Awake()
    {
        currentHealthNetworked = new NetworkVariable<int>(maxHealth);
        sceneManager = Object.FindFirstObjectByType<SceneManagerScript>();
        healthBar = GameObject.Find("Health").GetComponent<Image>();
        ChangeShipHealthServerRpc();
    }

    public void TakeDamage()
    {
        HostHealthUpdate();
    }

    private void Health(int healingAmount)
    {
        currentHealth += healingAmount;
        healthBar.fillAmount = currentHealth / 100f;
    }

    private void Update()
    {
        //THIS NEEDS CHANGING TO WHEN THE SHIP DIES, NOT ONE PLAYER!!!
        if(currentHealthNetworked.Value <= 0)
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
    
    void HostHealthUpdate()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            //Host taking damage away and updating health bar
            Debug.Log("Host taking damage: " + healthBar.fillAmount);
            currentHealthNetworked.Value -= damageAmount;
            //healthBar.fillAmount = currentHealhtNetworked.Value / 100f;
            StartCoroutine(DelayUpdateHealth());
        }
    }

    private IEnumerator DelayUpdateHealth()
    {
        yield return new WaitForSeconds(0.1f);
        ChangeShipHealthServerRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void ChangeShipHealthServerRpc()
    {
        //Client updating health bar
        Debug.Log("Updating health: " + healthBar.fillAmount);
        healthBar.fillAmount = currentHealthNetworked.Value / 100f;
    }
}
