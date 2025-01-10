using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class SCR_EnemyParent : NetworkBehaviour
{
    [SerializeField]
    protected float moveSpeed;

    [SerializeField]
    protected float followRadius;

    [SerializeField]
    public GameObject Ship;

    [SerializeField]
    protected NavMeshAgent navMesh;

    [SerializeField]
    protected SphereCollider followSphere;

    [SerializeField]
    protected float destroyCountdown;

    [SerializeField]
    protected float destroyTimer;

    [SerializeField]
    protected bool isInRadius;

    public bool gameStarted;

    [SerializeField]
    public GameObject enemySpawner;


    private void Awake()
    {
        navMesh = GetComponent<NavMeshAgent>();
        followSphere = GetComponent<SphereCollider>();
        destroyCountdown = destroyTimer;
        //Ship = GameObject.Find("TEMPShip");
        //enemySpawner = GameObject.Find("SCR_EnemySpawner");     <<doesnt work and to qoute joe "uhhhhh idk" 

        if(followSphere.isTrigger)
        {
            followSphere.radius = followRadius;
        }
    }

    private void Update()
    {
        navMesh.destination = Ship.transform.position;

        if (!isInRadius)
        {
            destroyCountdown -= Time.deltaTime;
        }

        if (destroyCountdown <= 0)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                Debug.Log("Calling despawn");
                enemySpawner.GetComponent<SCR_EnemySpawner>().DespawnEnemy(gameObject.GetComponent<SCR_Enemy>());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Entered Radius");
            isInRadius = true;
            destroyCountdown = destroyTimer;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Left radius");
            isInRadius = false;
        }
    }
}
