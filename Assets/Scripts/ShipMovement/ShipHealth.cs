using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipHealth : MonoBehaviour
{
    [SerializeField]
    private float currentHealth;

    [SerializeField]
    private float maxHealth;

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
        if(currentHealth <= 0)
        {
            SceneManager.LoadScene(1);
        }
    }
}
