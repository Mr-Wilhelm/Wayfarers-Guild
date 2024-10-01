using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShipHealth : MonoBehaviour
{
    [SerializeField]
    private float currentHealth;

    [SerializeField]
    public Image healthBar;

    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private NetworkManager networkManager;

    [SerializeField]
    private SceneManagerScript sceneManager;

    [SerializeField]
    private float damageAmount = 1f;

    private void Start()
    {
        currentHealth = maxHealth;
        sceneManager = Object.FindFirstObjectByType<SceneManagerScript>();
        healthBar = GameObject.Find("Canvas/Health").GetComponent<Image>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Destroy(collision.gameObject);
            TakeDamage(damageAmount);
        }
    }

    private void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        healthBar.fillAmount = currentHealth / 100f;
    }

    private void Health(float healingAmount)
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
}
