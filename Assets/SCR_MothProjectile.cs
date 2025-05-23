using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class SCR_MothProjectile : MonoBehaviour
{
    [SerializeField]
    private float projectileDamage = 5.0f;

    [SerializeField]
    private float projectileSpeed = 25.0f;

    private GameObject ship;

    private Vector3 mothPosition;

    [SerializeField] float mothStopZoneRadius = 400.0f;

    public void MoveTowardsShip(GameObject moveTarget, Vector3 mothPos)
    {
        ship = moveTarget;
        mothPosition = mothPos;
        Invoke(nameof(DespawnProjectile), 4f);
    }

    private void Update()
    {
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, ship.transform.position, projectileSpeed * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ship")
        {
            Vector3 projectilePath = (gameObject.transform.position - mothPosition).normalized;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            GameObject.FindGameObjectWithTag("ShipHealth").GetComponent<SCR_NetworkedShipHealth>().changeHealth(-projectileDamage);
            GameObject.Find("PRF_Outer_wilds").GetComponent<SCR_TerrainCollision>().CallScreenShakeRpc();
            RaycastToShip(projectilePath);
            Invoke(nameof(DespawnProjectile), 0.1f);
        }
    }

    private void RaycastToShip(Vector3 direction)
    {
        print("cum cum");
        LayerMask layerMask = LayerMask.GetMask("ShipHull");
        RaycastHit hit;

        //Get point past ship
        Physics.Raycast(transform.position, direction, mothStopZoneRadius);
        //Debug.DrawRay(transform.position, direction * 100.0f, Color.yellow, Mathf.Infinity);
        Vector3 endPoint = transform.position + (direction * mothStopZoneRadius);
        GameObject endPointOBJ = new GameObject("endPoint"); endPointOBJ.transform.position = endPoint;

        if (Physics.Raycast(transform.position, direction, out hit, 9999999, layerMask))
        {
            print("cum cum 3");
            Debug.DrawRay(transform.position, direction * hit.distance, Color.yellow, Mathf.Infinity);
            Debug.Log($"Hit {hit.collider.name}");
        }

        direction = (mothPosition - endPoint).normalized;
        if (Physics.Raycast(endPoint, direction, out hit, 9999999, layerMask))
        {
            print("cum cum 3");
            Debug.DrawRay(endPoint, direction * hit.distance, Color.blue, Mathf.Infinity);
            Debug.Log($"Hit {hit.collider.name}");
        }
    }

    private void DespawnProjectile()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
    }
}
