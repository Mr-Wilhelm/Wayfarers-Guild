using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_MothProjectile : MonoBehaviour
{
    [SerializeField]
    private float projectileDamage = 5.0f;

    [SerializeField]
    private float projectileSpeed = 25.0f;

    private GameObject ship;

    public void MoveTowardsShip(GameObject moveTarget)
    {
        ship = moveTarget;
    }

    private void Update()
    {
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, ship.transform.position, projectileSpeed * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ship")
        {
            GameObject.FindGameObjectWithTag("ShipHealth").GetComponent<SCR_NetworkedShipHealth>().changeHealth(-projectileDamage);
            Destroy(gameObject);
        }
    }
}
